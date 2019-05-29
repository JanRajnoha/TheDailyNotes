using Framework.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Exceptions
{
    public class Exception : System.Exception
    {
        public Exception()
        {
            LogService.AddLogMessage("General framework exception");
        }

        public Exception(string message) : base(message)
        {
            LogService.AddLogMessage("Framework exception, message: " + message);
        }

        public Exception(string message, System.Exception innerException) : base(message, innerException)
        {
            LogService.AddLogMessage("Framework exception, message: " + message);
        }

        protected Exception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            LogService.AddLogMessage("General framework exception");
        }
    }
}
