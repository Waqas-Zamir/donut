// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Client
{
#pragma warning disable CA1717
    /// <summary>
    /// Represents Asset Account available status
    /// </summary>
    public enum AssetAccountStatus
    {
        /// <summary>
        /// Represents an asset account is created but not active for trading
        /// </summary>
        Open,

        /// <summary>
        /// Represents an asset account is ready for trading
        /// </summary>
        Active,

        /// <summary>
        /// Represents an asset account is disabled for trading
        /// </summary>
        Inactive,

        /// <summary>
        /// Represents an asset account is closed
        /// </summary>
        Closed,

        /// <summary>
        /// Represents an asset account is no longer available
        /// </summary>
        Terminated,

        /// <summary>
        /// Represents an asset account balance has reached to margin call
        /// </summary>
        MarginCallReached,

        /// <summary>
        /// Represents liquidation is in progress for the asset account
        /// </summary>
        LiquidationInProgress,

        /// <summary>
        /// Represents an asset account has been liquidated
        /// </summary>
        Liquidated,
    }
}
