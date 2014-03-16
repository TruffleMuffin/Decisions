using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

            return output;
        }

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>An <see cref="Expression"/> representing the input.</returns>
        private Expression Parse(string input)
        {
            Expression expression = null;
            var variable = string.Empty;
            var not = false;
            var operation = '&';
            var depth = 0;

            foreach (var c in input)
            {
                switch (c)
                {
                    case '(':
                        depth++;
                        break;

                    case ')':
                        depth--;
                        if (depth == 0)
                        {
                            expression = Combine(expression, Parse(variable), operation, not);
                            variable = string.Empty;
                            not = false;
                        }

                        break;

                    case '!':
                        if (depth == 0)
                        {
                            not = !not;
                        }
                        else
                        {
                            variable += c;
                        }

                        break;

                    case '&':
                    case '|':
                        if (depth == 0)
                        {
                            if (variable.Length > 0)
                            {
                                expression = Combine(expression, Call(variable), operation, not);
                                variable = string.Empty;
                                not = false;
                            }

                            operation = c;
                        }
                        else
                        {
                            variable += c;
                        }

                        break;

                    default:
                        variable += c;
                        break;
                }
            }

            // Ensure we don't drop the last variable
            if (variable.Length > 0)
            {
                expression = Combine(expression, Call(variable), operation, not);
            }

            return expression;
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
