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
            var argumentIntermediaryId = app.Argument("intermediary", "The intermediary asset account id", false);
            var argumentType = app.Argument("type", string.Concat("The asset account type (", string.Join(" | ", Enum.GetNames(typeof(AssetAccountType))), ")"), false);

            // options
            var optionSettlementCurrency = app.Option("-c|--currency <currency_code>", "The ISO currency code for settlement currency", CommandOptionType.SingleValue);
            var optionMarginAccount = app.Option("--margin_account <margin_account>", "The margin account id", CommandOptionType.SingleValue);
            var optionReferenceAccount = app.Option("--reference_account <reference_account>", "The reference account id", CommandOptionType.SingleValue);
            var optionBankIdentificationMargin = app.Option("--bank_ident_margin <bank_identification_margin>", "The bank identification margin", CommandOptionType.SingleValue);
            var optionBankIdentificationReference = app.Option("--bank_ident_reference <bank_identification_reference>", "The bank identification reference", CommandOptionType.SingleValue);
            var optionWithdrawalAllowed = app.Option("--withdrawal_allowed", "withdrawal allowed", CommandOptionType.NoValue);
            var optionInteractive = app.Option("-i|--interactive", "Enters interactive mode", CommandOptionType.NoValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    AssetAccountType type = default;
                    if ((string.IsNullOrEmpty(argumentAssetAccountId.Value)
                        || string.IsNullOrEmpty(argumentOwnerId.Value)
                        || string.IsNullOrEmpty(argumentIntermediaryId.Value)
                        || !Enum.TryParse(argumentType.Value, out type))
                        && !optionInteractive.HasValue())
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    var withdrawalAllowed = optionWithdrawalAllowed.HasValue();

                    var reporter = new ConsoleReporter(console, options.Verbose.HasValue(), false);
                    var helper = new AccountHelper();

                    var account = new InvestorAssetAccount
                    {
                        AssetAccountId = argumentAssetAccountId.Value,
                        OwnerId = argumentOwnerId.Value,
                        IntermediaryId = argumentIntermediaryId.Value,
                        Type = type,
                        MarginAccount = optionMarginAccount.Value(),
                        ReferenceAccount = optionReferenceAccount.Value(),
                        BankIdentificationMargin = optionBankIdentificationMargin.Value(),
                        BankIdentificationReference = optionBankIdentificationReference.Value(),
                        WithdrawalAllowed = withdrawalAllowed,
                        SettlementCurrency = optionSettlementCurrency.Value(),
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
                account.Type = account.Type == default
                    ? Enum.Parse<AssetAccountType>(Safe(Prompt.GetString(string.Concat("Type: (", string.Join(" | ", Enum.GetNames(typeof(AssetAccountType))), ")"), account.Type.ToString()), "Cannot create account type without type"), true) : account.Type;

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
