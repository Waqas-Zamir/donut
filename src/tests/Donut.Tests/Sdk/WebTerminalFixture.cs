// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Tests.Sdk
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    public sealed class WebTerminalFixture : IDisposable
    {
        private readonly IWebHost host;

        private RequestDelegate requestDelegate;

        public WebTerminalFixture()
        {
            var config = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            this.requestDelegate = ctx => Task.CompletedTask;

            var url = $"http://*:5000";
            this.host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(url)
                .Configure(this.Configure)
                .Build();

            this.host.Start();
        }

        public void Dispose()
        {
            Task.Run(
                async () =>
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    this.host.Dispose();
                });
        }

        public void AssignRequestDelegate(RequestDelegate requestDelegate) => this.requestDelegate = requestDelegate;

        private void Configure(IApplicationBuilder app)
        {
            app.Run(this.requestDelegate);
        }
    }
}