using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Decisions.Contracts;
using Decisions.Exceptions;
using Decisions.Services;

namespace Decisions.Providers
{
    /// <summary>
    /// Describes an Expression inflation provider for string based input.
    /// </summary>
    internal class ExpressionProvider
    {
        private static readonly ParameterExpression PARAMETER;
        private static readonly MethodInfo METHOD_INFO;

        private readonly ConcurrentDictionary<string, Predicate<DecisionContext>> compiled = new ConcurrentDictionary<string, Predicate<DecisionContext>>();
        private readonly IDictionary<string, string> expressions;
        private readonly PolicyService provider;

        /// <summary>
        /// Initializes the <see cref="ExpressionProvider"/> class.
        /// </summary>
        static ExpressionProvider()
        {
            METHOD_INFO = typeof(AbstractPolicy).GetMethod("Decide", new[] { typeof(DecisionContext) });
            PARAMETER = Expression.Parameter(typeof(DecisionContext), "context");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionProvider" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="provider">The provider.</param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">All expressions must specify a unique 'key'.
        /// or
        /// All expressions must specify a 'value'.</exception>
        /// <exception cref="ConfigurationErrorsException">All expressions must specify a unique 'key'.
        /// or
        /// All expressions must specify a 'value'.</exception>
        public ExpressionProvider(XElement settings, PolicyService provider)
        {
            expressions = new Dictionary<string, string>();
            this.provider = provider;

            // Validate the XML Formatting to prevent bad errors being thrown which have poor debugging information due to their nature
            if (settings == null || settings.Name.LocalName.Equals("decisions") == false)
            {
                throw new ConfigurationMalformedException("No decisions elements could be found.");
            }

            if (settings.Elements("item").Any() == false)
            {
                throw new ConfigurationMalformedException("No item elements could be found.");
            }

            foreach (var item in settings.Elements("item"))
            {
                if (item.HasAttributes == false || item.Attributes().Any(a => a.Name == "key") == false)
                {
                    throw new ConfigurationMalformedException("One of the item elements does not have a key attribute.");
                }

                if (item.Attributes().Any(a => a.Name == "value") == false)
                {
                    throw new ConfigurationMalformedException("One of the item elements does not have a value attribute.");
                }

                var key = item.Attribute("key");
                if (key == null || string.IsNullOrWhiteSpace((string)key))
                {
                    throw new ArgumentException("All expressions must specify a unique 'key'.", key.ToString());
                }

                var value = item.Attribute("value");
                if (value == null || string.IsNullOrWhiteSpace((string)value))
                {
                    throw new ArgumentException("All expressions must specify a 'value'.", value.ToString());
                }

                expressions[(string)key] = Reduce((string)value);
            }
        }

        /// <summary>
        /// Inflates the specified context into an expression that can be executed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A delegate that takes a <see cref="DecisionContext"/> as a parameter to execute</returns>
        public Predicate<DecisionContext> Inflate(DecisionContext context)
        {
            if (compiled.ContainsKey(context.Role) == false)
            {
                var expression = Expression.Lambda<Predicate<DecisionContext>>(Parse(expressions[context.Role]), PARAMETER);
                var compiledExpression = expression.Compile();
                compiled.AddOrUpdate(context.Role, compiledExpression, (key, oldValue) => compiledExpression);
            }

            return compiled[context.Role];
        }

        /// <summary>
        /// Reduces the specified input, removing all extraneous or non-required text.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A string based on the input</returns>
        private static string Reduce(string input)
        {
            var output = input;

            output = Regex.Replace(output, @"(?<=\W)AND(?=\W)", "&", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<=\W)OR(?=\W)", "|", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"\.", "&");
            output = Regex.Replace(output, @"\+", "|");
            output = Regex.Replace(output, @"\s", "");

            return output.StartsWith("(") && output.EndsWith(")") ? output : string.Format("({0})", output);
        }

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>An <see cref="Expression"/> representing the input.</returns>
        private Expression Parse(string input)
        {
            var expression = Parse(new StringReader(input));

            if (expression == null)
            {
                throw new ConfigurationMalformedException("The expression '" + input + "' is malformed and cannot be used. Please check that a set of brackets only includes at most two policies and an operator. For example, (A AND B) is valid, where as (A AND B OR C) is not.");
            }

            return expression;
        }

        /// <summary>
        /// Parses the specified reader, creating an <see cref="Expression"/> from its contents.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>An <see cref="Expression"/> representing the input.</returns>
        private Expression Parse(StringReader reader)
        {
            // Determine if the character is a not
            var not = IsCharThenConsume(reader, '!');

            // If we are starting a bracketed input
            if (IsCharThenConsume(reader, '('))
            {
                // Get the left hand side
                var leftExpression = Parse(reader);

                // If thsi is a single policy within a bracket the finish
                if (IsCharThenConsume(reader, ')'))
                {
                    return leftExpression;
                }

                // Otherwise get the operator and right handside
                var op = GetOperator(reader);
                var rightExpression = Parse(reader);

                // Verify this is the end of the bracketed part
                if (IsCharThenConsume(reader, ')'))
                {
                    return Combine(leftExpression, rightExpression, op, not);
                }

                // If it is not, this isn't supported by our syntax
                return null;
            }

            // This is not a bracketed policy, simply get it and create an expression from the call
            var exp = Call(GetVariable(reader));
            return not ? Expression.Not(exp) : exp;
        }

        /// <summary>
        /// Calls an <see cref="AbstractPolicy"/> registered in the <see cref="PolicyService"/> with the specified alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>An <see cref="Expression"/> representing the result</returns>
        private Expression Call(string alias)
        {
            bool boolean;
            if (bool.TryParse(alias, out boolean))
            {
                return Expression.Constant(boolean);
            }

            return Expression.Call(Expression.Constant(provider.Get(alias)), METHOD_INFO, PARAMETER);
        }

        /// <summary>
        /// Determines whether the next character in the <see cref="StringReader"/> matches the specified character. If it does, it executes
        /// a Read() on the <see cref="StringReader"/>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="c">The c.</param>
        /// <returns>
        ///   <c>true</c> if the next character in the <see cref="StringReader"/> matches the specified character; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCharThenConsume(StringReader reader, char c)
        {
            if (reader.Peek() == c)
            {
                reader.Read();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the next character in the <see cref="StringReader"/> matches the operators '&' or '|'.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        ///   <c>true</c> if the next character in the <see cref="StringReader"/> matches the operators '&' or '|'; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsOperator(StringReader reader)
        {
            var next = reader.Peek();
            return next == '&' || next == '|';
        }

        /// <summary>
        /// Determines whether the next character in the <see cref="StringReader"/> matches the end of the string.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        ///   <c>true</c> if the specified reader is at the end; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsEnd(StringReader reader)
        {
            var next = reader.Peek();
            return next == -1 || next == ')';
        }

        /// <summary>
        /// Gets the next character as an operator.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>A <see cref="char"/></returns>
        private static char GetOperator(StringReader reader)
        {
            return (char)reader.Read();
        }

        /// <summary>
        /// Gets the next variable in the <see cref="StringReader"/>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>A string variable</returns>
        private static string GetVariable(StringReader reader)
        {
            var str = new StringBuilder();
            while (IsOperator(reader) == false && IsEnd(reader) == false)
            {
                str.Append((char)reader.Read());
            }
            return str.ToString();
        }

        /// <summary>
        /// Combines the specified LHS and RHS with the provided operators.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="not">if set to <c>true</c> [not].</param>
        /// <returns>An <see cref="Expression"/> representing the result of the operation</returns>
        /// <exception cref="System.InvalidOperationException">When the operation is not recognised.</exception>
        private static Expression Combine(Expression lhs, Expression rhs, char operation, bool not)
        {
            if (not)
            {
                rhs = Expression.Not(rhs);
            }

            if (lhs == null)
            {
                return rhs;
            }

            switch (operation)
            {
                case '&':
                    return Expression.AndAlso(lhs, rhs);

                case '|':
                    return Expression.OrElse(lhs, rhs);

                default:
                    throw new InvalidOperationException(string.Format("The operation '{0}' is not recognized", operation));
            }
        }

    }
}
