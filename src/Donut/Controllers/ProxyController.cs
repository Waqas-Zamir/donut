// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Donut.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    [Authorize]
    public class ProxyController : ControllerBase
    {
        private readonly DonutSettings settings;

        public ProxyController(IOptions<DonutSettings> settings)
        {
            this.settings = settings.Value;
        }

        [Route("api/assetAccount/{*url}")]
        public async Task AssetAccountHandlerAsync()
        {
            var uri = new Uri(
                string.Concat(
                    this.settings.TerminalMatchingEngineUrl,
                    this.Request.Path.Value.Replace("/api/assetAccount", "/api/externalAssetAccount", StringComparison.InvariantCultureIgnoreCase)));

            await this.HttpContext.ProxyRequest(uri);
        }

        [Route("api/user/{*url}")]
        public async Task UsersHandlerAsync()
        {
            var uri = new Uri(string.Concat(this.settings.TerminalMatchingEngineUrl, this.Request.Path.Value));

            await this.HttpContext.ProxyRequest(uri);
        }
    }
}
