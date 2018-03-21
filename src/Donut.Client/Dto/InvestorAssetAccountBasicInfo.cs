// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Client
{
    /// <summary>
    /// Represents Update-able Investor Asset Account Basic Info
    /// </summary>
    public class InvestorAssetAccountBasicInfo
    {
        /// <summary>
        /// Gets or sets asset account id to update
        /// </summary>
        public string AssetAccountId { get; set; }

        /// <summary>
        /// Gets or sets asset account type
        /// </summary>
        public AssetAccountType Type { get; set; }

        /// <summary>
        /// Gets or sets asset account status
        /// </summary>
        public AssetAccountStatus Status { get; set; }

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
        /// Gets or sets a value indicating whether withdrawal allowed for this asset account
        /// </summary>
        public bool WithdrawalAllowed { get; set; }
    }
}
