// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

#pragma warning disable CA1812

namespace Donut.Console
{
    using System;
    using System.Threading.Tasks;
    using Donut.Client;
    using Donut.Console.Commands;
    using Donut.Console.Persistence;
    using IdentityModel.Client;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    internal class Program
    {
        private readonly IConsole console;
        private readonly IDataProtectionProvider provider;

        public Program(IConsole console, IDataProtectionProvider provider)
        {
            this.console = console;
            this.provider = provider;
        }

        public static Task<int> Main(string[] args)
        {
            Sdk.DebugHelper.HandleDebugSwitch(ref args);

            JsonConvert.DefaultSettings =
                () =>
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() }
                };

            // LINK (Cameron): https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/using-data-protection
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();
            serviceCollection.AddSingleton(PhysicalConsole.Singleton);
            var services = serviceCollection.BuildServiceProvider();

            var instance = ActivatorUtilities.CreateInstance<Program>(services);
            return instance.TryRunAsync(args);
        }

        public async Task<int> TryRunAsync(string[] args)
        {
            CommandLineOptions options;
            try
            {
                options = CommandLineOptions.Parse(args, this.console);
            }
            catch (CommandParsingException ex)
            {
                new ConsoleReporter(this.console).Warn(ex.Message);
                return 1;
            }

            if (options == null)
            {
                return 1;
            }

            if (options.Help.HasValue())
            {
                return 2;
            }

            if (options.Command == null)
            {
                return 3;
            }

            var repository = new CommandDataRepository(this.provider);

            if (options.Command is LoginCommand.Reset)
            {
                await options.Command.ExecuteAsync(new CommandContext(this.console, null, null, null, repository)).ConfigureAwait(false);
                return 0;
            }

            var data = repository.GetCommandData() ??
                new CommandData
                {
                    Authority = LoginCommand.DefaultAuthority,
                    Service = LoginCommand.DefaultService,
                };

            var authority = data.Authority;
            var service = data.Service;

            if (options.Command is LoginCommand loginCommand)
            {
                authority = loginCommand.Authority;
                service = loginCommand.Service;
            }
            else
            {
                this.console.WriteLine("Executing command against ");
                this.console.ForegroundColor = ConsoleColor.White;
                this.console.WriteLine($"Authority: {authority}");
                this.console.WriteLine($"Service: {service}");
                this.console.ResetColor();
                this.console.WriteLine("...");
            }

            var discoveryResponse = default(DiscoveryResponse);
            using (var discoveryClient = new DiscoveryClient(authority))
            {
                discoveryResponse = await discoveryClient.GetAsync().ConfigureAwait(false);
                if (discoveryResponse.IsError)
                {
                    await this.console.Error.WriteLineAsync(discoveryResponse.Error).ConfigureAwait(false);
                    return 500;
                }
            }

            using (var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, "donut_console"))
            using (var refreshTokenHandler = new RefreshTokenHandler(tokenClient, data.RefreshToken, data.AccessToken))
            using (var usersClient = new UsersHttpClient(service))
            using (var assetAccountsClient = new AssetAccountsHttpClient(service))
            {
                refreshTokenHandler.TokenRefreshed += (sender, e) =>
                {
                    repository.SetCommandData(
                        new CommandData
                        {
                            Authority = authority,
                            AccessToken = e.AccessToken,
                            RefreshToken = e.RefreshToken,
                            Service = service,
                        });
                };

                var reporter = new ConsoleReporter(this.console, options.Verbose.HasValue(), false);
                var context = new CommandContext(this.console, reporter, usersClient, assetAccountsClient, repository);

                try
                {
                    await options.Command.ExecuteAsync(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    reporter.Error(ex.Message);
                    return 500;
                }
                finally
                {
                    this.console.ResetColor();
                }

                return 0;
            }
        }
    }
}
