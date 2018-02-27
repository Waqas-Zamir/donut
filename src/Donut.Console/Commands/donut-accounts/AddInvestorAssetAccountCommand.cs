// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Console.Commands
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Donut.Client;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class AddInvestorAssetAccountCommand : ICommand
    {
        private InvestorAssetAccount account;

        private AddInvestorAssetAccountCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Creates a new asset account";
            app.ExtendedHelpText = $"{Environment.NewLine}Use 'accounts add -i' to enter interactive mode{Environment.NewLine}";

            // arguments
            var argumentAssetAccountId = app.Argument("account", "The asset account id", false);
            var argumentOwnerId = app.Argument("id", "The user subject identifier of the owner user", false);
            var argumentClientTier = app.Argument("tier", "The user client tier", false);

            // options
            var optionDefaultAssetAccountId = app.Option("-a|--default_account <default_account>", "The default asset account id for the user", CommandOptionType.SingleValue);
            var optionInteractive = app.Option("-i|--interactive", "Enters interactive mode", CommandOptionType.NoValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentOwnerId.Value) && !optionInteractive.HasValue())
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    var reporter = new ConsoleReporter(console, options.Verbose.HasValue(), false);
                    var helper = new AccountHelper();

                    var account = new InvestorAssetAccount
                    {
                        AssetAccountId = argumentAssetAccountId.Value,
                        OwnerId = argumentOwnerId.Value,
                    };

                    reporter.Verbose("Prototype user (from command line arguments):");
                    reporter.Verbose(JsonConvert.SerializeObject(account));

                    if (!helper.IsValid(account) || optionInteractive.HasValue())
                    {
                        try
                        {
                            account = helper.GetValid(account);
                        }
                        catch (NotSupportedException ex)
                        {
                            throw new CommandParsingException(app, $"Operation Aborted. {ex.Message}", ex);
                        }

                        reporter.Verbose("Validated user (from interactive console):");
                        reporter.Verbose(JsonConvert.SerializeObject(account));
                    }

                    options.Command = new AddInvestorAssetAccountCommand { account = account };
                });
        }

        public async Task ExecuteAsync(CommandContext context) => await context.AssetAccountsClient.AddAssetAccountAsync(this.account).ConfigureAwait(false);

        private static string Safe(string value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NotSupportedException(errorMessage);
            }

            return value;
        }

        private class AccountHelper
        {
            public InvestorAssetAccount GetPrototype(InvestorAssetAccount account) => account;

            public bool IsValid(InvestorAssetAccount account) =>
                !string.IsNullOrEmpty(account.AssetAccountId)
                && !string.IsNullOrEmpty(account.OwnerId);

            public InvestorAssetAccount GetValid(InvestorAssetAccount account)
            {
                account.AssetAccountId = Safe(Prompt.GetString("Asset Account Id:", account.AssetAccountId), "Cannot create account without account id");
                account.OwnerId = Safe(Prompt.GetString("User Id:", account.OwnerId), "Cannot create an account without owner user id.");
                account.IntermediaryId = Safe(Prompt.GetString("Intermediary Account:", account.IntermediaryId), "Cannot create an account without intermediary asset account id");
                account.Type = account.Type == default(AssetAccountType) ? Enum.Parse<AssetAccountType>(Safe(Prompt.GetString("Type:", account.Type.ToString()), "Cannot create account type without type"), true) : account.Type;

                account.MarginAccount = Prompt.GetString("Margin Account (optional):", account.MarginAccount);
                account.ReferenceAccount = Prompt.GetString("Reference Account (optional):", account.ReferenceAccount);
                account.BankIdentificationMargin = Prompt.GetString("Bank Identification Margin (optional):", account.BankIdentificationMargin);
                account.BankIdentificationReference = Prompt.GetString("Bank Identification Reference (optional):", account.BankIdentificationReference);
                account.WithdrawalAllowed = Prompt.GetYesNo("Withdrawal Allowed default (yes):", true, ConsoleColor.Red, ConsoleColor.DarkRed);

                // defaults
                account.MarginAccount = string.IsNullOrWhiteSpace(account.MarginAccount) ? null : account.MarginAccount;

                return account;
            }
        }
    }
}
