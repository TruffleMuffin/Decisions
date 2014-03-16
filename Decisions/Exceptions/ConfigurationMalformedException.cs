using System.Configuration;

namespace Decisions.Exceptions
{
    /// <summary>
    /// The Decisions configuration file is badly formatted in some way and Decisions is unable to successfully parse it.
    /// </summary>
    public class ConfigurationMalformedException : ConfigurationErrorsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationMalformedException"/> class.
        /// </summary>
        /// <param name="message">A message that describes why this <see cref="ConfigurationMalformedException" /> exception was thrown.</param>
        public ConfigurationMalformedException(string message) : base(message) { }
    }
}
