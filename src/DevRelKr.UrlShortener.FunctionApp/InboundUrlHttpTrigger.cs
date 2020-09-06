using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DevRelKr.UrlShortener.FunctionApp
{
    /// <summary>
    /// This represents the HTTP trigger entity for the inbound short URLs.
    /// </summary>
    public class InboundUrlHttpTrigger
    {
        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> object.</param>
        /// <param name="shortUrl">Short URL value.</param>
        /// <param name="log"><see cref="ILogger"/> instance.</param>
        /// <returns>Returns the <see cref="RedirectResult"/> instance as the <see cref="Task{IActionResult}"/> instance.</returns>
        [FunctionName(nameof(InboundUrlHttpTrigger.BounceUrl))]
        public async Task<IActionResult> BounceUrl(
            [HttpTrigger(AuthorizationLevel.Function, "GET", Route = "{shortUrl}")] HttpRequest req,
            string shortUrl,
            ILogger log)
        {
            log.LogInformation($"{(req.IsHttps ? "https" : "http")}://{req.Host.Value}/{shortUrl} was hit");

            var result = new RedirectResult($"{(req.IsHttps ? "https" : "http")}://{req.Host.Value}/b/{shortUrl}", permanent: true);

            return await Task.FromResult(result).ConfigureAwait(false);
        }
    }
}
