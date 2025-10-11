using System.Net;
using component.template.api.domain.Interfaces.Common;
using Newtonsoft.Json;

namespace component.template.api.domain.Exceptions
{
    [System.Serializable]
    public class ConfigurationNotFoundExceptionException : System.Exception, ICustomException
    {
        private const string customMessage = "Invalid values.";
        public string DetailsMessage
        {
            get
            {
                return customMessage;
            }
        }

        public string FaultCode
        {
            get
            {
                return "InternalServerError";
            }
        }

        public System.Diagnostics.TraceLevel Level
        {
            get
            {
                return System.Diagnostics.TraceLevel.Error;
            }
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public ConfigurationNotFoundExceptionException() : base(customMessage) { }
        public ConfigurationNotFoundExceptionException(string message) : base(message) { }
        public ConfigurationNotFoundExceptionException(string message, System.Exception inner) : base(message, inner) { }        
        public ConfigurationNotFoundExceptionException(System.Exception inner) : base(customMessage, inner) { }
        public ConfigurationNotFoundExceptionException(object message) :base($"{customMessage } --> {JsonConvert.SerializeObject(message)}") { }
    }
}