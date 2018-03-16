// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Console.Commands
{
    using System;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Donut.Console.Persistence;
    using IdentityModel.Client;
    using IdentityModel.OidcClient;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class LoginCommand : ICommand
    {
        public const string DefaultAuthority = "https://auth.lykkecloud.com";
        public const string DefaultService = "http://localhost:5009";

        private Api api;

        private LoginCommand()
        {
        }

        public string Authority { get; private set; }

        public string ServiceUrl { get; private set; }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Log in to an authorization server";

            // arguments
            var argumentServiceUrl = app.Argument("serviceUrl", "The service URL to send requests to");

            // options
            var optionAuthority = app.Option("-a|--authority", "The URL for the authorization server to log in to", CommandOptionType.SingleValue);
            var optionTest = app.Option("-t|--test", "Uses the Lykke TEST authorization server", CommandOptionType.NoValue);
            var optionReset = app.Option("-r|--reset", "Resets the authorization context", CommandOptionType.NoValue);
            app.HelpOption();

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (!string.IsNullOrEmpty(optionReset.Value()) && string.IsNullOrEmpty(optionAuthority.Value()) && string.IsNullOrEmpty(optionTest.Value()))
                    {
                        // only --reset was specified
                        options.Command = new Reset();
                        return;
                    }

                    // service URL
                    var service = argumentServiceUrl.Value;
                    if (string.IsNullOrEmpty(service))
                    {
                        service = DefaultService;
                    }

                    // validate
                    if (!Uri.TryCreate(service, UriKind.Absolute, out var serviceUri))
                    {
                        console.Error.WriteLine($"Invalid service URL specified: {service}.");
                        return;
                    }

                    // TODO (Cameron): Perform an API check against the donut service.

                    // authority
                    var authority = optionAuthority.Value();
                    if (string.IsNullOrEmpty(authority))
                    {
                        authority = string.IsNullOrEmpty(optionTest.Value()) ? DefaultAuthority : "https://auth-test.lykkecloud.com";
                    }
                    else if (!string.IsNullOrEmpty(optionTest.Value()))
                    {
                        ////console.WriteLine("Ignoring test option as authority was specified.");
                    }

                    // validate
                    if (!Uri.TryCreate(authority, UriKind.Absolute, out var authorityUri))
                    {
                        console.Error.WriteLine($"Invalid authority URL specified: {authority}.");
                        return;
                    }

                    var api = default(Api);
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            using (var response = client.GetAsync(new Uri(authority + "/api")).GetAwaiter().GetResult())
                            {
                                api = JsonConvert.DeserializeObject<Api>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                            }
                        }
                        catch (HttpRequestException)
                        {
                            console.Error.WriteLine($"Unable to connect to: {authority}.");
                            return;
                        }

                        if (api == null)
                        {
                            console.Error.WriteLine($"Invalid response from: {authority}.");
                            return;
                        }

                        try
                        {
                            using (var response = client.GetAsync(new Uri($"{service}/platform")).GetAwaiter().GetResult())
                            {
                                response.EnsureSuccessStatusCode();
                                api = JsonConvert.DeserializeObject<Api>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                            }
                        }
                        catch (HttpRequestException)
                        {
                            console.Error.WriteLine($"Unable to connect to: {service}.");
                            return;
                        }

                        if (api == null)
                        {
                            console.Error.WriteLine($"Invalid response from: {service}.");
                            return;
                        }
                    }

                    options.Command = new LoginCommand { Authority = authority, api = api, ServiceUrl = service };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            context.Console.WriteLine($"Saving Server Url: {this.ServiceUrl}");
            context.Console.WriteLine($"Logging in to {this.Authority} ({this.api.Title} v{this.api.Version} running on {this.api.OS})...");

            var data = context.Repository.GetCommandData();
            if (data != null && data.Authority == this.Authority)
            {
                // already logged in?
                var discoveryResponse = default(DiscoveryResponse);
                using (var discoveryClient = new DiscoveryClient(this.Authority))
                {
                    discoveryResponse = await discoveryClient.GetAsync().ConfigureAwait(false);
                    if (!discoveryResponse.IsError)
                    {
                        using (var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, "donut_console"))
                        using (var refreshTokenHandler = new RefreshTokenHandler(tokenClient, data.RefreshToken, data.AccessToken))
                        using (var userInfoClient = new UserInfoClient(discoveryResponse.UserInfoEndpoint, refreshTokenHandler))
                        {
                            var response = await userInfoClient.GetAsync(data.AccessToken).ConfigureAwait(false);
                            if (!response.IsError)
                            {
                                var claimsIdentity = new ClaimsIdentity(response.Claims, "idSvr", "name", "role");
                                context.Console.WriteLine($"Logged in as {claimsIdentity.Name}.");
                                return;
                            }
                        }
                    }
                }
            }

            var browser = new SystemBrowser();
            var options = new OidcClientOptions
            {
                Authority = this.Authority,
                ClientId = "donut_console",
                RedirectUri = $"http://127.0.0.1:{browser.Port}",
                Scope = "openid profile users_api accounts_api offline_access",
                FilterClaims = false,
                Browser = browser
            };

            var oidcClient = new OidcClient(options);
            var result = await oidcClient.LoginAsync(new LoginRequest()).ConfigureAwait(false);
            if (result.IsError)
            {
                context.Console.Error.WriteLine($"Error attempting to log in:{Environment.NewLine}{result.Error}");
                return;
            }

            context.Repository.SetCommandData(
                new CommandData
                {
                    Authority = this.Authority,
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    ServiceUrl = this.ServiceUrl,
                });

            context.Console.WriteLine($"Logged in as {result.User.Identity.Name}.");
        }

        public class Reset : ICommand
        {
            public Task ExecuteAsync(CommandContext context)
            {
                context.Repository.SetCommandData(null);
                return Task.CompletedTask;
            }
        }

#pragma warning disable CA1812
        private class Api
        {
            public string Title { get; set; }

            public string Version { get; set; }

            public string OS { get; set; }
        }
    }
}
