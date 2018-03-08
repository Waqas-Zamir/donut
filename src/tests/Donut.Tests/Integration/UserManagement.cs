// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Tests.Integration
{
    using System.Threading.Tasks;
    using Donut.Client;
    using Donut.Tests.Sdk;
    using Xunit;

    public class UserManagement : IntegrationTest
    {
        public UserManagement(SecurityFixture securityFixture, DonutFixture donutFixture, WebTerminalFixture webTerminalFixture)
            : base(securityFixture, donutFixture, webTerminalFixture)
        {
        }

        [Fact]
        public async Task CanAddUserMinimum()
        {
            // arrange
            var httpClient = new UsersHttpClient("https://auth-test.lykkecloud.com", this.Handler);
            var expectedUser = new User
            {
                UserId = "sub",
                ClientTier = ClientTierDto.TIER_2,
                DefaultAssetAccountId = "account",
            };

            await httpClient.AddUserAsync(expectedUser).ConfigureAwait(false);

            // hook into the context (somehow) and verify
            this.AssignRequestDelegate(
                httpContext =>
                {
                    httpContext.Response.StatusCode = 404;
                    return Task.CompletedTask;
                });

            // act
            await httpClient.AddUserAsync(expectedUser).ConfigureAwait(false);

            // assert
        }
    }
}
