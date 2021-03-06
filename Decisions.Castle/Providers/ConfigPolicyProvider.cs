﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Decisions.Contracts;

namespace Decisions.Castle.Providers
{
    /// <summary>
    /// Implements a <see cref="IPolicyProvider"/> which uses a Config file to load and register <see cref="IPolicy"/>. 
    /// </summary>
    /// <remarks>Uses a <see cref="WindsorContainer"/> to initialise the <see cref="IPolicy"/> classes.</remarks>
    public class ConfigPolicyProvider : IPolicyProvider
    {
        // internal windsor container, so we don't need to worry about scope issues with components being added to the same
        // container as any other code in the system. These policies should not really be available to access by other parts
        // of the application anyway.
        private static readonly IWindsorContainer container = new WindsorContainer();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigPolicyProvider"/> class.
        /// </summary>
        /// <param name="configPath">The config path.</param>
        public ConfigPolicyProvider(string configPath) : this(XElement.Load(Path.GetFullPath(configPath))) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigPolicyProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentException">
        /// All policies must specify a unique 'key'.
        /// or
        /// All policies must specify an IPolicy policy type 'value'.
        /// or
        /// All policies must implement 'Decision.IPolicy'.
        /// </exception>
        public ConfigPolicyProvider(XContainer settings)
        {
            var policies = settings.Element("policies");

            foreach (var item in policies.Elements("item"))
            {
                // Validate that a key is present
                var key = item.Attribute("key");
                if (key == null || string.IsNullOrWhiteSpace((string)key))
                {
                    throw new ArgumentException("All policies must specify a unique 'key'.", item.ToString());
                }

                // Validate that a value is present
                var value = item.Attribute("value");
                if (value == null || string.IsNullOrWhiteSpace((string)value))
                {
                    throw new ArgumentException("All policies must specify an IPolicy policy type 'value'.", key.ToString());
                }

                // Extract type and assembly information specified by the value
                var typeName = (string)value;
                string assembly = null;
                if (typeName.Contains(","))
                {
                    var typeSplit = typeName.Split(',');
                    assembly = typeSplit[1].Trim();
                    typeName = typeSplit[0].Trim();
                }

                // Validate the type is correct
                var type = assembly == null ? Type.GetType(typeName, false) : Assembly.Load(assembly).GetType(typeName, false);
                if (type == null || type.GetInterface(typeof(IPolicy).FullName) == null)
                {
                    throw new ArgumentException("All policies must implement 'Decisions.Contracts.IPolicy'.", typeName);
                }

                container.Register(Component.For(type).Forward<IPolicy>().Named(key.Value).DependsOn(Dependencies(type, item).ToArray()));
            }
        }

        /// <summary>
        /// Gets all the policies this instance of the provider wishes to offer.
        /// </summary>
        /// <returns>
        /// A dictionary of policies, the key should be a identifier that is used to link to the decision.
        /// </returns>
        public IDictionary<string, IPolicy> GetPolicies()
        {
            return container.Kernel.GetHandlers(typeof(IPolicy)).ToDictionary(a => a.ComponentModel.Name, a => container.Resolve<IPolicy>(a.ComponentModel.Name));
        }

        /// <summary>
        /// Loads <see cref="Dependency"/>s for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="item">The item.</param>
        /// <returns>A set of <see cref="Dependency"/>s</returns>
        private static IEnumerable<Dependency> Dependencies(Type type, XElement item)
        {
            // Efficiency block to avoid doing reflection work if there are no more attributes than the bare minimum (key and value)
            var attributes = item.Attributes();
            if (attributes.Count() > 2)
            {
                var constructors = type.GetConstructors();
                var parameters = constructors
                    .SelectMany(x => x.GetParameters())
                    .Where(x => x.ParameterType.IsPrimitive || x.ParameterType.IsEnum || x.ParameterType.FullName == "System.String")
                    .ToArray();

                foreach (var attribute in attributes)
                {
                    // Ignore if key, value, not primitive or simply not a parameter
                    var name = attribute.Name.LocalName;
                    var info = parameters.FirstOrDefault(x => x.Name == name);
                    if (name == "key" || name == "value" || info == null)
                    {
                        continue;
                    }

                    // Convert type
                    if (info.ParameterType.IsEnum)
                    {
                        yield return Dependency.OnValue(name, Enum.Parse(info.ParameterType, attribute.Value));
                    }
                    else
                    {
                        var value = Convert.ChangeType(attribute.Value, info.ParameterType);
                        yield return Dependency.OnValue(name, value);
                    }
                }
            }
        }
    }
}