// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Client
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Exposes the public members of the assetAccounts client.
    /// </summary>
    public interface IAssetAccountsHttpClient
    {
        /// <summary>
        /// Adds the specified investor asset account.
        /// </summary>
        /// <param name="assetAccount">The investor asset account.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddAssetAccountAsync(InvestorAssetAccount assetAccount, CancellationToken cancellationToken = default);
    }
}
