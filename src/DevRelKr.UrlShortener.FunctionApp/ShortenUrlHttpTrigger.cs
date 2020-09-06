using System;
using System.Net;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Domains;
using DevRelKr.UrlShortener.Domains.Exceptions;
using DevRelKr.UrlShortener.Domains.Extensions;
using DevRelKr.UrlShortener.Models.Configurations;
using DevRelKr.UrlShortener.Models.Responses;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DevRelKr.UrlShortener.FunctionApp
{
    /// <summary>
    /// This represents the HTTP trigger entity that shortens URLs.
    /// </summary>
    public class ShortenUrlHttpTrigger
    {
        private readonly AppSettings _settings;
        private readonly IUrl _url;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortenUrlHttpTrigger"/> class.
        /// </summary>
        /// <param name="settings"><see cref="AppSettings"/> instance.</param>
        /// <param name="url"><see cref="IUrl"/> instance.</param>
        public ShortenUrlHttpTrigger(AppSettings settings, IUrl url)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._url = url ?? throw new ArgumentNullException(nameof(url));
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> object.</param>
        /// <param name="log"><see cref="ILogger"/> instance.</param>
        /// <returns>Returns the <see cref="OkObjectResult"/> instance as the <see cref="Task{IActionResult}"/> instance.</returns>
        [FunctionName(nameof(ShortenUrlHttpTrigger.ShortenUrl))]
        public async Task<IActionResult> ShortenUrl(
            [HttpTrigger(AuthorizationLevel.Function, "GET", "POST", Route = "api/shorten")] HttpRequest req,
            ILogger log)
        {
            var utcNow = DateTimeOffset.UtcNow;
            var entityId = Guid.NewGuid();

            try
            {
                await this._url.GetRequestAsync(req)
                               .ValidateAsync()
                               .ShortenAsync()
                               .CreateRecordAsync(utcNow, entityId)
                               .ConfigureAwait(false);
            }
            catch (UrlExistsException ex)
            {
                var error = new { message = ex.Message, shortUrl = ex.ShortUrl };

                return new BadRequestObjectResult(error);
            }
            catch (Exception ex)
            {
                var error = new ExceptionResponse() { Message = ex.Message };
                if (this._settings.IsDevelopment)
                {
                    error.StackTrace = ex.StackTrace;
                }

                return new ObjectResult(error) { StatusCode = (int) HttpStatusCode.InternalServerError };
            }

            log.LogInformation($"Processed: {this._url.Original} => {this._url.Shortened}");

            var result = this._url.ShortenerResponse;

            return new OkObjectResult(result);
        }
    }
}
