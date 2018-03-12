// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Tests
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    internal static class HttpRequestExtensions
    {
        internal static async Task<@object> DeserializeBody<@object>(this HttpRequest httpRequest)
        {
            using (var reader = new StreamReader(httpRequest.Body, Encoding.UTF8))
            {
                var content = await reader.ReadToEndAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<@object>(content);
            }
        }
    }
}
