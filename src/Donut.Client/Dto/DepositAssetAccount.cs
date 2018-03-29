// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Client
{
    /// <summary>
    /// Represents Deposit request on asset account
    /// </summary>
    public class DepositAssetAccount
    {
        /// <summary>
        /// Gets or sets the asset account id on which the deposit will be made
        /// </summary>
        public string AssetAccountId { get; set; }

        /// <summary>
        /// Gets or sets ISO currency code for this deposit
        /// </summary>
        public string SettlementCurrency { get; set; }

        /// <summary>
        /// Gets or sets decimal precision for this deposit request
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// Gets or sets the amount to be deposited
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the unique reference identifier for this deposit transaction
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets information about this transaction
        /// </summary>
        public string ReferenceText { get; set; }
    }
}
