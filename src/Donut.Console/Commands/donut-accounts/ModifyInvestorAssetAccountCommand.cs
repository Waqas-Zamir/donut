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

            // options
            var optionType = app.Option("-t|--type", string.Concat("The asset account type (", string.Join(" | ", Enum.GetNames(typeof(AssetAccountType))), ")"), CommandOptionType.SingleValue);
            var optionStatus = app.Option("-s|--status", $"The asset account status ({string.Join(" | ", Enum.GetNames(typeof(AssetAccountStatus)))})", CommandOptionType.SingleValue);

            var optionMarginAccount = app.Option("-m|--margin_account <margin_account>", "The margin account id", CommandOptionType.SingleValue);
            var optionReferenceAccount = app.Option("-r|--reference_account <reference_account>", "The reference account id", CommandOptionType.SingleValue);
            var optionBankIdentificationMargin = app.Option("-b|--bank_ident_margin <bank_identification_margin>", "The bank identification margin", CommandOptionType.SingleValue);
            var optionBankIdentificationReference = app.Option("-i|--bank_ident_reference <bank_identification_reference>", "The bank identification reference", CommandOptionType.SingleValue);
            var optionWithdrawalAllowed = app.Option("-a|--withdrawal_allowed", "withdrawal allowed", CommandOptionType.NoValue);
            var optionInteractive = app.Option("-i|--interactive", "Enters interactive mode", CommandOptionType.NoValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    AssetAccountType type = default;
                    AssetAccountStatus status = default;
                    if ((string.IsNullOrWhiteSpace(argumentAssetAccountId.Value)
                        && (string.IsNullOrWhiteSpace(optionType.Value()) || !Enum.TryParse(optionType.Value(), out type))
                        && (string.IsNullOrWhiteSpace(optionStatus.Value()) || !Enum.TryParse(optionStatus.Value(), out status)))
                        && string.IsNullOrWhiteSpace(optionMarginAccount.Value())
                        && string.IsNullOrWhiteSpace(optionReferenceAccount.Value())
                        && string.IsNullOrWhiteSpace(optionBankIdentificationMargin.Value())
                        && string.IsNullOrWhiteSpace(optionBankIdentificationReference.Value())
                        && string.IsNullOrWhiteSpace(optionMarginAccount.Value())
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
                        MarginAccount = optionMarginAccount.Value(),
                        ReferenceAccount = optionReferenceAccount.Value(),
                        BankIdentificationMargin = optionBankIdentificationMargin.Value(),
                        BankIdentificationReference = optionBankIdentificationReference.Value(),
                        WithdrawalAllowed = withdrawalAllowed,
                    };

                    if (Enum.TryParse(optionType.Value(), out type))
                    {
                        account.Type = type;
                    }

                    if (Enum.TryParse(optionStatus.Value(), out status))
                    {
                        account.Status = status;
                    }

                    reporter.Verbose("Prototype account (from command line arguments):");
                    reporter.Verbose(JsonConvert.SerializeObject(account));

                    if (!(helper.IsValid(account) || optionWithdrawalAllowed.HasValue())
                        || optionInteractive.HasValue())
                    {
                        try
                        {
                            account = helper.GetValid(account);

                            if (!helper.IsValid(account))
                            {
                                throw new CommandParsingException(app, "Operation Aborted. please provide atleast one argument to modify.");
                            }
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
                !string.IsNullOrWhiteSpace(account.AssetAccountId)
                && (account.Type.HasValue
                    || account.Status.HasValue
                    || !string.IsNullOrWhiteSpace(account.MarginAccount)
                    || !string.IsNullOrWhiteSpace(account.ReferenceAccount)
                    || !string.IsNullOrWhiteSpace(account.BankIdentificationMargin)
                    || !string.IsNullOrWhiteSpace(account.BankIdentificationReference));

            public InvestorAssetAccountBasicInfo GetValid(InvestorAssetAccountBasicInfo account)
            {
                account.AssetAccountId = Safe(Prompt.GetString("Asset Account Id:", account.AssetAccountId), "Cannot modify account without specifying account id");

                if (Enum.TryParse(Prompt.GetString(string.Concat("Type: (", string.Join(" | ", Enum.GetNames(typeof(AssetAccountType))), ")"), account.Type?.ToString()), out AssetAccountType type))
                {
                    account.Type = type;
                }

                if (Enum.TryParse(Prompt.GetString(string.Concat("Status: (", string.Join(" | ", Enum.GetNames(typeof(AssetAccountStatus))), ")"), account.Status?.ToString()), out AssetAccountStatus status))
                {
                    account.Status = status;
                }

                account.MarginAccount = Prompt.GetString("Margin Account:", account.MarginAccount);
                account.ReferenceAccount = Prompt.GetString("Reference Account:", account.ReferenceAccount);
                account.BankIdentificationMargin = Prompt.GetString("Bank Identification Margin:", account.BankIdentificationMargin);
                account.BankIdentificationReference = Prompt.GetString("Bank Identification Reference:", account.BankIdentificationReference);
                account.WithdrawalAllowed = Prompt.GetYesNo("Withdrawal Allowed:", account.WithdrawalAllowed.HasValue ? account.WithdrawalAllowed.Value : false, ConsoleColor.Red);

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
