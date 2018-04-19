// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Client
{
    using System;

    /// <summary>
    /// Represents Deposit request on asset account
    /// </summary>
    public class WithdrawAssetAccount
    {
        /// <summary>
        /// Gets or sets the asset account id on which the deposit will be made
        /// </summary>
        public string AssetAccountId { get; set; }

        /// <summary>
        /// Gets or sets the amount to be deposited
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the reference account id
        /// </summary>
        public string ReferenceAccountId { get; set; }

        /// <summary>
        /// Gets or sets the transaction timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
