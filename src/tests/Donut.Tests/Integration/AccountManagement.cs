// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Tests.Integration
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Donut.Client;
    using Donut.Tests.Sdk;
    using FluentAssertions;
    using Xunit;

#pragma warning disable CA1001
    public class AccountManagement : IntegrationTest
    {
        private readonly AssetAccountsHttpClient httpClient;

        public AccountManagement(SecurityFixture securityFixture, DonutFixture donutFixture, WebTerminalFixture webTerminalFixture)
            : base(securityFixture, donutFixture, webTerminalFixture)
        {
            this.httpClient = new AssetAccountsHttpClient("http://localhost:5009", this.Handler);
        }

        [Fact]
        public async Task CanAddInvestorAccount()
        {
            // arrange
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
                    }

                        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                });

            // act
            await this.httpClient.AddAssetAccountAsync(expectedAccount).ConfigureAwait(false);

            // assert
            actualAccount.Should().BeEquivalentTo(expectedAccount);
        }

        [Fact]
        public async Task CanModifyInvestorAccount()
        {
            // arrange
            var expectedAccount = new InvestorAssetAccountBasicInfo
            {
                AssetAccountId = "accountId",
                Type = AssetAccountType.Lykke,
                Status = AssetAccountStatus.Active,
                MarginAccount = "marginAccountId",
                BankIdentificationMargin = "bim",
                ReferenceAccount = "ra",
                BankIdentificationReference = "bir",
                WithdrawalAllowed = true,
            };

            var actualAccount = default(InvestorAssetAccountBasicInfo);

            // hook into the context (somehow) and verify
            this.AssignRequestDelegate(
                async httpContext =>
                {
                    if (httpContext.Request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase)
                        && httpContext.Request.Path.Value.Equals($"/api/externalAssetAccount/investor/{expectedAccount.AssetAccountId}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        actualAccount = await httpContext.Request.DeserializeBody<InvestorAssetAccountBasicInfo>().ConfigureAwait(false);

                        if (actualAccount != null)
                        {
                            httpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;

                            return;
                    }
                    }

                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                });

            // act
            await this.httpClient.UpdateAssetAccountAsync(expectedAccount).ConfigureAwait(false);

            // assert
            actualAccount.Should().BeEquivalentTo(expectedAccount);
        }

        [Fact]
        public async Task CanCloseAssetAccount()
        {
            // arrange
            var expectedAssetAccountId = "AA1111";

            this.AssignRequestDelegate(
                httpContext =>
                {
                    // The url and method types should match
                    if (httpContext.Request.Method.Equals("PATCH", StringComparison.InvariantCultureIgnoreCase)
                        && httpContext.Request.Path.Value.Equals($"/api/externalAssetAccount/{expectedAssetAccountId}/close", StringComparison.InvariantCultureIgnoreCase))
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;

                        return Task.CompletedTask;
                    }

                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    return Task.CompletedTask;
                });

            // act
            await this.httpClient.CloseAsync(expectedAssetAccountId).ConfigureAwait(false);
        }

        [Fact]
        public async Task CanTerminateAssetAccount()
        {
            // arrange
            var expectedAssetAccountId = "AA1111";

            this.AssignRequestDelegate(
                httpContext =>
                {
                    // The url and method types should match
                    if (httpContext.Request.Method.Equals("DELETE", StringComparison.InvariantCultureIgnoreCase)
                        && httpContext.Request.Path.Value.Equals($"/api/externalAssetAccount/{expectedAssetAccountId}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;

                        return Task.CompletedTask;
                    }

                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    return Task.CompletedTask;
                });

            // act
            await this.httpClient.TerminateAsync(expectedAssetAccountId).ConfigureAwait(false);
        }

    }
}
