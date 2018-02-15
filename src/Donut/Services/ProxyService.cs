// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Services
{
    using System.Net.Http;

    public class ProxyService
    {
        public ProxyService()
        {
            this.Client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false });
        }

        internal HttpClient Client { get; private set; }
    }
}
