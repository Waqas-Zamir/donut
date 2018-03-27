// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Console.Commands
{
    using System;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class CloseAssetAccountCommand : ICommand
    {
        private string assetAccountId;

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Closes an inactive asset account";
            app.ExtendedHelpText = $"{Environment.NewLine}Use 'accounts close -i' to enter interactive mode{Environment.NewLine}";

            // arguments
            var argumentAssetAccountId = app.Argument("account", "The asset account id to close", false);

            // options
            var optionInteractive = app.Option("-i|--interactive", "Enters interactive mode", CommandOptionType.NoValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentAssetAccountId.Value)
                        && !optionInteractive.HasValue())
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    var reporter = new ConsoleReporter(console, options.Verbose.HasValue(), false);
                    var assetAccountId = argumentAssetAccountId.Value;

                    reporter.Verbose("Prototype account Id (from command line arguments):");
                    reporter.Verbose($"assetAccountId: {assetAccountId}");

                    if (string.IsNullOrWhiteSpace(assetAccountId) || optionInteractive.HasValue())
                    {
                        try
                        {
                            assetAccountId = Safe(Prompt.GetString("Asset Account Id:", assetAccountId), "Cannot close account without account id");
                        }
                        catch (NotSupportedException ex)
                        {
                            throw new CommandParsingException(app, $"Operation Aborted. {ex.Message}", ex);
                        }

                        reporter.Verbose("Validated account Id (from interactive console):");
                        reporter.Verbose($"assetAccountId: {assetAccountId}");
                    }

                    options.Command = new CloseAssetAccountCommand { assetAccountId = assetAccountId };
                });
        }

        public Task ExecuteAsync(CommandContext context) => context.AssetAccountsClient.CloseAsync(this.assetAccountId);

        private static string Safe(string value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NotSupportedException(errorMessage);
            }

            return value;
        }
    }
}
