// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Tests.Integration
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Donut.Client;
    using Donut.Tests.Sdk;
    using Xunit;

    public class AccountManagement : IntegrationTest
    {
        public AccountManagement(SecurityFixture securityFixture, DonutFixture donutFixture, WebTerminalFixture webTerminalFixture)
            : base(securityFixture, donutFixture, webTerminalFixture)
        {
        }

        [Fact]
        public async Task CanAddInvestorAccount()
        {
            // arrange
            var httpClient = new AssetAccountsHttpClient("http://localhost:5009", this.Handler);
            var expectedAccount = new InvestorAssetAccount
            {
                AssetAccountId = "accountId",
                OwnerId = "sub",
                Type = AssetAccountType.Lykke,
                IntermediaryId = "intermediaryAccountId",
                MarginAccount = "marginAccountId",
                BankIdentificationMargin = "bim",
                ReferenceAccount = "ra",
                BankIdentificationReference = "bir",
                WithdrawalAllowed = true,
            };

            var actualAccount = default(InvestorAssetAccount);

            // hook into the context (somehow) and verify
            this.AssignRequestDelegate(
                async httpContext =>
                {
                    if (httpContext.Request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase)
                        && httpContext.Request.Path.Value.Equals("/api/externalAssetAccount/investor", StringComparison.InvariantCultureIgnoreCase))
                    {
                        actualAccount = await httpContext.Request.DeserializeBody<InvestorAssetAccount>().ConfigureAwait(false);

                        if (actualAccount != null)
                        {
                            httpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;

                            return;
                        }

                        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                });

            // act
            await httpClient.AddAssetAccountAsync(expectedAccount).ConfigureAwait(false);

            // assert
            Assert.Equal(expectedAccount.AssetAccountId, actualAccount.AssetAccountId);
            Assert.Equal(expectedAccount.OwnerId, actualAccount.OwnerId);
            Assert.Equal(expectedAccount.Type, actualAccount.Type);
            Assert.Equal(expectedAccount.IntermediaryId, actualAccount.IntermediaryId);
            Assert.Equal(expectedAccount.MarginAccount, actualAccount.MarginAccount);
            Assert.Equal(expectedAccount.ReferenceAccount, actualAccount.ReferenceAccount);
            Assert.Equal(expectedAccount.BankIdentificationReference, actualAccount.BankIdentificationReference);
            Assert.Equal(expectedAccount.WithdrawalAllowed, actualAccount.WithdrawalAllowed);
        }
    }
}
