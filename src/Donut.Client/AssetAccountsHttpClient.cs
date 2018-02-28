// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Client
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An HTTP client for creating asset account.
    /// </summary>
    public class AssetAccountsHttpClient : HttpClientBase, IAssetAccountsHttpClient
    {
        private const string ApiPath = "/api/assetAccount";

        public AssetAccountsHttpClient(string authority, HttpMessageHandler innerHandler = null)
            : base(authority, innerHandler)
        {
        }

        /// <summary>
        /// Adds the specified investor asset account.
        /// </summary>
        /// <param name="assetAccount">The investor asset account.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task AddAssetAccountAsync(InvestorAssetAccount assetAccount, CancellationToken cancellationToken = default)
        {
            await this.SendAsync(HttpMethod.Post, this.RelativeUrl(string.Concat(ApiPath, "/investor")), assetAccount, cancellationToken).ConfigureAwait(false);
        }
    }
}
