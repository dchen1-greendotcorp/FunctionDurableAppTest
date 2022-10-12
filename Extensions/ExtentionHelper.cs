using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.Extensions
{
    public static class ExtentionHelper
    {
        public static HttpResponseMessage CreateCustomResponse(this HttpRequestMessage request,
            HttpStatusCode httpStatusCode, string content, IDictionary<string, string> headers = null)
        {
            HttpResponseMessage response = new HttpResponseMessage(httpStatusCode) { Content = new StringContent(content) };
            if (headers != null && headers.Any())
            {
                foreach (KeyValuePair<string, string> kvp in headers)
                {
                    response.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            return response;
        }
    }
}
