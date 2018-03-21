// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Console.Commands
{
    using System;
    using System.Threading.Tasks;
    using Donut.Client;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class ModifyInvestorAssetAccountCommand : ICommand
    {
        private InvestorAssetAccountBasicInfo accountBasicInfo;

        private ModifyInvestorAssetAccountCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Modifies existing asset account";
            app.ExtendedHelpText = $"{Environment.NewLine}Use 'accounts modify -i' to enter interactive mode{Environment.NewLine}";

            // arguments
            var argumentAssetAccountId = app.Argument("account", "The asset account id", false);
            var argumentType = app.Argument("type", string.Concat("The asset account type (", string.Join(" | ", Enum.GetNames(typeof(AssetAccountType))), ")"), false);
            var argumentStatus = app.Argument("status", $"The asset account status ({string.Join(" | ", Enum.GetNames(typeof(AssetAccountStatus)))})", false);

            // options
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
                    AssetAccountStatus status = default;
                    if ((string.IsNullOrEmpty(argumentAssetAccountId.Value)
                        || string.IsNullOrEmpty(argumentType.Value)
                        || !Enum.TryParse(argumentType.Value, out type)
                        || string.IsNullOrEmpty(argumentStatus.Value)
                        || !Enum.TryParse(argumentStatus.Value, out status))
                        && !optionInteractive.HasValue())
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    var withdrawalAllowed = optionWithdrawalAllowed.HasValue();

                    var reporter = new ConsoleReporter(console, options.Verbose.HasValue(), false);
                    var helper = new AccountHelper();

                    var account = new InvestorAssetAccountBasicInfo
                    {
                        AssetAccountId = argumentAssetAccountId.Value,
                        Type = type,
                        Status = status,
                        MarginAccount = optionMarginAccount.Value(),
                        ReferenceAccount = optionReferenceAccount.Value(),
                        BankIdentificationMargin = optionBankIdentificationMargin.Value(),
                        BankIdentificationReference = optionBankIdentificationReference.Value(),
                        WithdrawalAllowed = withdrawalAllowed,
                    };

                    reporter.Verbose("Prototype account (from command line arguments):");
                    reporter.Verbose(JsonConvert.SerializeObject(account));

                    if (!helper.IsValid(account)
                        || string.IsNullOrEmpty(argumentType.Value)
                        || string.IsNullOrEmpty(argumentStatus.Value)
                        || optionInteractive.HasValue())
                    {
                        try
                        {
                            account = helper.GetValid(account);
                        }
                        catch (NotSupportedException ex)
                        {
                            throw new CommandParsingException(app, $"Operation Aborted. {ex.Message}", ex);
                        }

                        reporter.Verbose("Validated account (from interactive console):");
                        reporter.Verbose(JsonConvert.SerializeObject(account));
                    }

                    options.Command = new ModifyInvestorAssetAccountCommand { accountBasicInfo = account };
                });
        }

        public async Task ExecuteAsync(CommandContext context) => await context.AssetAccountsClient.UpdateAssetAccountAsync(this.accountBasicInfo).ConfigureAwait(false);

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
            public InvestorAssetAccountBasicInfo GetPrototype(InvestorAssetAccountBasicInfo account) => account;

            public bool IsValid(InvestorAssetAccountBasicInfo account) =>
                !string.IsNullOrEmpty(account.AssetAccountId);

            public InvestorAssetAccountBasicInfo GetValid(InvestorAssetAccountBasicInfo account)
            {
                account.AssetAccountId = Safe(Prompt.GetString("Asset Account Id:", account.AssetAccountId), "Cannot modify account without specifying account id");
                account.Type = account.Type == default
                    ? Enum.Parse<AssetAccountType>(Safe(Prompt.GetString(string.Concat("Type: (", string.Join(" | ", Enum.GetNames(typeof(AssetAccountType))), ")"), account.Type.ToString()), "Cannot modify account without specifying type"), true) : account.Type;
                account.Status = account.Status == default
                    ? Enum.Parse<AssetAccountStatus>(Safe(Prompt.GetString(string.Concat("Status: (", string.Join(" | ", Enum.GetNames(typeof(AssetAccountStatus))), ")"), account.Status.ToString()), "Cannot modify account without specifying status"), true) : account.Status;

                account.MarginAccount = Prompt.GetString("Margin Account (optional):", account.MarginAccount);
                account.ReferenceAccount = Prompt.GetString("Reference Account (optional):", account.ReferenceAccount);
                account.BankIdentificationMargin = Prompt.GetString("Bank Identification Margin (optional):", account.BankIdentificationMargin);
                account.BankIdentificationReference = Prompt.GetString("Bank Identification Reference (optional):", account.BankIdentificationReference);
                account.WithdrawalAllowed = Prompt.GetYesNo("Withdrawal Allowed default (yes):", true, ConsoleColor.Red, ConsoleColor.DarkRed);

                // defaults
                account.MarginAccount = string.IsNullOrWhiteSpace(account.MarginAccount) ? null : account.MarginAccount;
                account.ReferenceAccount = string.IsNullOrWhiteSpace(account.ReferenceAccount) ? null : account.ReferenceAccount;
                account.BankIdentificationMargin = string.IsNullOrWhiteSpace(account.BankIdentificationMargin) ? null : account.BankIdentificationMargin;
                account.BankIdentificationReference = string.IsNullOrWhiteSpace(account.BankIdentificationReference) ? null : account.BankIdentificationReference;

                return account;
            }
        }
    }
}
