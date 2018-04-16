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

#pragma warning disable CA1054
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetAccountsHttpClient"/> class.
        /// </summary>
        /// <param name="serviceUrl">The service url.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public AssetAccountsHttpClient(string serviceUrl, HttpMessageHandler innerHandler = null)
            : base(serviceUrl, innerHandler)
        {
        }

        /// <summary>
        /// Adds the specified investor asset account.
        /// </summary>
        /// <param name="assetAccount">The investor asset account.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task AddAsync(InvestorAssetAccount assetAccount, CancellationToken cancellationToken = default) =>
            this.SendAsync(HttpMethod.Post, this.RelativeUrl($"{ApiPath}/investor"), assetAccount, cancellationToken);

        /// <summary>
        /// Updates investor asset account basic info
        /// </summary>
        /// <param name="investorAssetAccountBasicInfo">The basic info of the investor asset account.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task UpdateAsync(InvestorAssetAccountBasicInfo investorAssetAccountBasicInfo, CancellationToken cancellationToken = default) =>
            this.PatchAsync(this.RelativeUrl($"{ApiPath}/investor/{investorAssetAccountBasicInfo.AssetAccountId}"), investorAssetAccountBasicInfo, cancellationToken);

        /// <summary>
        /// Closes an asset account.
        /// </summary>
        /// <param name="assetAccountId">The asset account unique id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task CloseAsync(string assetAccountId, CancellationToken cancellationToken = default) =>
            this.PatchAsync(this.RelativeUrl($"{ApiPath}/{assetAccountId}/close"), cancellationToken);

        /// <summary>
        /// Terminates an asset account.
        /// </summary>
        /// <param name="assetAccountId">The asset account unique id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task TerminateAsync(string assetAccountId, CancellationToken cancellationToken = default) =>
            this.DeleteAsync(this.RelativeUrl($"{ApiPath}/{assetAccountId}"), cancellationToken);

        /// <summary>
        /// Deposits on an asset account
        /// </summary>
        /// <param name="depositAssetAccount">A <see cref="DepositAssetAccount"/> representing deposit request</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task DepositAsync(DepositAssetAccount depositAssetAccount, CancellationToken cancellationToken = default) =>
            this.SendAsync(HttpMethod.Post, this.RelativeUrl($"{ApiPath}/{depositAssetAccount.AssetAccountId}/deposit"), depositAssetAccount, cancellationToken);
    }
}
