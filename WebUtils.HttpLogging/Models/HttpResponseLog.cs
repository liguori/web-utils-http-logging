using Microsoft.Extensions.Primitives;

namespace WebUtils.HttpLogging.Models
{
    internal class HttpResponseLog
    {
        //HTTP Response Headers
        public Dictionary<string, StringValues> Headers { get; set; }

        //HTTP Response Body
        public string Body { get; set; }
    }
}
