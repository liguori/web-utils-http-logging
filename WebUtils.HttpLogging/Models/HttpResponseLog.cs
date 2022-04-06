using Microsoft.Extensions.Primitives;

namespace WebUtils.HttpLogging.Models
{
    internal class HttpResponseLog
    {
        public string TraceIdentifier { get; set; }

        //HTTP Response Properties
        public int StatusCode { get; set; }

        //HTTP Response Headers
        public Dictionary<string, string> Headers { get; set; }

        //HTTP Response Body
        public string Body { get; set; }
    }
}
