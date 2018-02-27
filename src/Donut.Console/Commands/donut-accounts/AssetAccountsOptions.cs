// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Console.Commands
{
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class AssetAccountsOptions
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Create asset accounts";
            app.HelpOption();

            // commands
            app.Command("add", command => AddInvestorAssetAccountCommand.Configure(command, options, console));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }
    }
}