using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Domains;
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
    /// This represents the HTTP trigger entity that redirects to the original URL.
    /// </summary>
    public class RedirectUrlHttpTrigger
    {
        private readonly AppSettings _settings;
        private readonly IUrl _url;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectUrlHttpTrigger"/> class.
        /// </summary>
        /// <param name="settings"><see cref="AppSettings"/> instance.</param>
        /// <param name="url"><see cref="IUrl"/> instance.</param>
        public RedirectUrlHttpTrigger(AppSettings settings, IUrl url)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._url = url ?? throw new ArgumentNullException(nameof(url));
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> object.</param>
        /// <param name="shortUrl">Short URL value.</param>
        /// <param name="context"><see cref="ExecutionContext"/> instance.</param>
        /// <param name="log"><see cref="ILogger"/> instance.</param>
        /// <returns>Returns the <see cref="ContentResult"/> instance as the <see cref="Task{IActionResult}"/> instance.</returns>
        [FunctionName(nameof(RedirectUrlHttpTrigger.RedirectUrl))]
        public async Task<IActionResult> RedirectUrl(
            [HttpTrigger(AuthorizationLevel.Function, "GET", Route = "b/{shortUrl}")] HttpRequest req,
            string shortUrl,
            ExecutionContext context,
            ILogger log)
        {
            if (this._settings.FilesToBeIgnired.Contains(shortUrl))
            {
                return await Task.FromResult(new OkResult()).ConfigureAwait(false);
            }

            var requestId = (string)req.HttpContext.Items["MS_AzureFunctionsRequestID"];

            log.LogInformation($"{requestId}: {(req.IsHttps ? "https" : "http")}://{req.Host.Value}/{shortUrl} was hit");

            var utcNow = DateTimeOffset.UtcNow;

            try
            {
                await this._url
                          .GetRequestAsync(req, shortUrl)
                          .ExpandAsync()
                          .AddHitCountAsync<ExpanderResponse>()
                          .UpdateRecordAsync<ExpanderResponse>(utcNow)
                          .ConfigureAwait(false);
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

            var html = default(string);
            using (var stream = new FileStream($"{context.FunctionAppDirectory.TrimEnd('/')}/redirect.html", FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                html = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            html = html.Replace("{{REDIRECT_URL}}", this._url.Original.ToString())
                       .Replace("{{GOOGLE_ANALYTICS_CODE}}", this._settings.GoogleAnalyticsCode);

            var result = new ContentResult()
            {
                Content = html,
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK
            };

            return result;
        }
    }
}
