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

    public class UserManagement : IntegrationTest
    {
        public UserManagement(SecurityFixture securityFixture, DonutFixture donutFixture, WebTerminalFixture webTerminalFixture)
            : base(securityFixture, donutFixture, webTerminalFixture)
        {
        }

        [Fact]
        public async Task CanAddUser()
        {
            // arrange
            var httpClient = new UsersHttpClient("http://localhost:5009", this.Handler);
            var expectedUser = new User
            {
                UserId = "sub",
                ClientTier = ClientTierDto.TIER_2,
                DefaultAssetAccountId = "account",
            };

            var actualUser = default(User);

            // hook into the context (somehow) and verify
            this.AssignRequestDelegate(
                async httpContext =>
                {
                    if (httpContext.Request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase)
                        && httpContext.Request.Path.Value.Equals("/api/user", StringComparison.InvariantCultureIgnoreCase))
                    {
                        actualUser = await httpContext.Request.DeserializeBody<User>().ConfigureAwait(false);

                        if (actualUser != null)
                        {
                            httpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;

                            return;
                        }
                    }

                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                });

            // act
            await httpClient.AddUserAsync(expectedUser).ConfigureAwait(false);

            // assert
            Assert.Equal(expectedUser.UserId, actualUser.UserId);
            Assert.Equal(expectedUser.ClientTier, actualUser.ClientTier);
            Assert.Equal(expectedUser.DefaultAssetAccountId, actualUser.DefaultAssetAccountId);
        }
    }
}
