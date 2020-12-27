using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.Responses;

using Microsoft.AspNetCore.Components;

namespace DevRelKr.UrlShortener.WebApp.Pages
{
    public partial class Index : ComponentBase
    {
        protected List<ShortenerResponse> ItemCollection { get; set; } = new List<ShortenerResponse>();

        protected async Task DisplayShortenedUrlsAsync(Dictionary<string, string> auth)
        {
            this.ItemCollection.Add(new ShortenerResponse() { Original = new Uri("https://www.microsoft.com"), Shortened = new Uri("https://dvrl.kr/msft") });
            this.ItemCollection.Add(new ShortenerResponse() { Original = new Uri("https://docs.microsoft.com"), Shortened = new Uri("https://dvrl.kr/docs") });
            this.ItemCollection.Add(new ShortenerResponse() { Original = new Uri("https://docs.microsoft.com/learn"), Shortened = new Uri("https://dvrl.kr/learn") });
            this.ItemCollection.Add(new ShortenerResponse() { Original = new Uri("https://azure.microsoft.com"), Shortened = new Uri("https://dvrl.kr/azure") });
        }
    }
}
