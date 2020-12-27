using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace DevRelKr.UrlShortener.WebApp.Components
{
    public partial class Authenticate : ComponentBase
    {
        [Parameter]
        public EventCallback<Dictionary<string, string>> OnClick { get; set; }

        protected string Email { get; set; }

        protected string Password { get; set; }

        protected async Task OnAuthenticateClickedAsync()
        {
            var value = new Dictionary<string, string>() { { this.Email, this.Password } };

            await this.OnClick.InvokeAsync(value).ConfigureAwait(false);
        }
    }
}
