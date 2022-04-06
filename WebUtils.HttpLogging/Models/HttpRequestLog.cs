using Microsoft.Extensions.Primitives;

namespace WebUtils.HttpLogging.Models
{
    public class HttpRequestLog
    {
        public string TraceIdentifier { get; set; }

        //HTTP Request Properties
        public string Protocol { get; set; }

        public string Method { get; set; }

        public string Scheme { get; set; }

        public string Host { get; set; }

        public string Path { get; set; }

        public string PathBase { get; set; }

        public string QueryString { get; set; }

        //HTTP Request Headers
        public Dictionary<string,string> Headers { get; set; }

        //HTTP Request Body
        public string Body { get; set; }

    }
}
