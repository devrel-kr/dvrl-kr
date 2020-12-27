using System.Collections.Generic;

using DevRelKr.UrlShortener.Models.Responses;

using Microsoft.AspNetCore.Components;

namespace DevRelKr.UrlShortener.WebApp.Components
{
    public partial class ShortenedUrlList : ComponentBase
    {
        [Parameter]
        public List<ShortenerResponse> DataSource { get; set; } = new List<ShortenerResponse>();
    }
}
