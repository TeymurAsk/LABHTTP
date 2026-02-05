using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LABHTTP.TEST.Auth
{
    public class CookieDelegatingHandler : DelegatingHandler
    {
        // Make this property mutable so tests can reset cookies
        public CookieContainer Cookies { get; private set; } = new CookieContainer();

        // Call this to reset cookies between users
        public void ResetCookies()
        {
            Cookies = new CookieContainer();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Attach cookies
            var cookieHeader = Cookies.GetCookieHeader(request.RequestUri!);
            if (!string.IsNullOrEmpty(cookieHeader))
                request.Headers.Add("Cookie", cookieHeader);

            var response = await base.SendAsync(request, cancellationToken);

            // Capture Set-Cookie headers
            if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
            {
                foreach (var cookie in setCookies)
                {
                    Cookies.SetCookies(request.RequestUri!, cookie);
                }
            }

            return response;
        }
    }
}
