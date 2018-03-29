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
        Task AddAsync(InvestorAssetAccount assetAccount, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates investor asset account basic info
        /// </summary>
        /// <param name="investorAssetAccountBasicInfo">The basic info of the investor asset account.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UpdateAsync(InvestorAssetAccountBasicInfo investorAssetAccountBasicInfo, CancellationToken cancellationToken = default);

        /// <summary>
        /// Closes an asset account.
        /// </summary>
        /// <param name="assetAccountId">The asset account unique id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CloseAsync(string assetAccountId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Terminates an asset account.
        /// </summary>
        /// <param name="assetAccountId">The asset account unique id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task TerminateAsync(string assetAccountId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deposits on an asset account
        /// </summary>
        /// <param name="depositAssetAccount">A <see cref="DepositAssetAccount"/> representing deposit request</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DepositAsync(DepositAssetAccount depositAssetAccount, CancellationToken cancellationToken = default);
    }
}
