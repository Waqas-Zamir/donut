// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Tests.Sdk
{
    using System.Net.Http;
    using Microsoft.AspNetCore.Http;
    using Xunit;

    [Collection("Donut")]
    public class IntegrationTest
    {
        private readonly SecurityFixture securityFixture;
        private readonly DonutFixture donutFixture;
        private readonly WebTerminalFixture webTerminalFixture;

        public IntegrationTest(SecurityFixture securityFixture, DonutFixture donutFixture, WebTerminalFixture webTerminalFixture)
        {
            this.securityFixture = securityFixture;
            this.donutFixture = donutFixture;
            this.webTerminalFixture = webTerminalFixture;
        }

        protected string Authority => this.securityFixture.Authority;

        protected HttpMessageHandler Handler => this.securityFixture.Handler;

        protected void AssignRequestDelegate(RequestDelegate requestDelegate) => this.webTerminalFixture.AssignRequestDelegate(requestDelegate);
    }
}
