using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VisualRecognition.Services
{
    public class LoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine(request.ToString());
            var response = await base.SendAsync(request, cancellationToken);
            var msg = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase);
            Console.WriteLine(msg);
            return response;
        }
    }
}
