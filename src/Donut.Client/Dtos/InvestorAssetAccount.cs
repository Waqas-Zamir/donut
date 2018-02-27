// Copyright 2017 Lykke Corp.
// See LICENSE file in the project root for full license information.

namespace Donut.Client
{
    public class InvestorAssetAccount
    {
        public string AssetAccountId { get; set; }

        public string OwnerId { get; set; }

        public AssetAccountType Type { get; set; }

        public string IntermediaryId { get; set; }

        public string MarginAccount { get; set; }

        public string BankIdentificationMargin { get; set; }

        public string ReferenceAccount { get; set; }

        public string BankIdentificationReference { get; set; }

        public bool WithdrawalAllowed { get; set; }
    }
}
