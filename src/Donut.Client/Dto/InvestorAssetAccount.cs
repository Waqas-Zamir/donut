// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Client
{
    /// <summary>
    /// Represents Investor Asset Account
    /// </summary>
    public class InvestorAssetAccount
    {
        /// <summary>
        /// Gets or sets unique identifier for the asset account
        /// </summary>
        public string AssetAccountId { get; set; }

        /// <summary>
        /// Gets or sets subject identifier of the owner user
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Gets or sets type of the asset account
        /// </summary>
        public AssetAccountType Type { get; set; }

        /// <summary>
        /// Gets or sets intermediary Asset Account Id
        /// </summary>
        public string IntermediaryId { get; set; }

        /// <summary>
        /// Gets or sets Margin Account Id
        /// </summary>
        public string MarginAccount { get; set; }

        /// <summary>
        /// Gets or sets bank identification margin
        /// </summary>
        public string BankIdentificationMargin { get; set; }

        /// <summary>
        /// Gets or sets reference account
        /// </summary>
        public string ReferenceAccount { get; set; }

        /// <summary>
        /// Gets or sets bank identification reference
        /// </summary>
        public string BankIdentificationReference { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets withdrawal allowed for this asset account
        /// </summary>
        public bool WithdrawalAllowed { get; set; }
    }
}
