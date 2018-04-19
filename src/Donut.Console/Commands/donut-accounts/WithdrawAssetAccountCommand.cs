// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Console.Commands
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Donut.Client;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class WithdrawAssetAccountCommand : ICommand
    {
        private WithdrawAssetAccount withdraw;

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Withdraws from an asset account";
            app.ExtendedHelpText = $"{Environment.NewLine}Use `accounts withdraw -i` to enter interactive mode{Environment.NewLine}";

            // arguments
            var argumentAssetAccountId = app.Argument("account", "The asset account id to withdraw", false);
            var argumentAmount = app.Argument("amount", "The amount to withdraw", false);
            var argumentReferenceAccount = app.Argument("reference", "The reference account id", false);

            // options
            var optionInteractive = app.Option("-i|--interactive", "Enters interactive mode", CommandOptionType.NoValue);
            var optionTimestamp = app.Option("-t|--timestamp", "The transaction timestamp MM/dd/yyyy HH:mm", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if ((string.IsNullOrWhiteSpace(argumentAssetAccountId.Value)
                        || string.IsNullOrWhiteSpace(argumentReferenceAccount.Value)
                        || string.IsNullOrWhiteSpace(argumentAmount.Value))
                            && !optionInteractive.HasValue())
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    if (!decimal.TryParse(argumentAmount.Value, out decimal amount)
                        && !optionInteractive.HasValue())
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    var timestamp = DateTime.UtcNow;

                    if (!string.IsNullOrWhiteSpace(optionTimestamp.Value())
                        && !DateTime.TryParse(optionTimestamp.Value(), out timestamp))
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    var reporter = new ConsoleReporter(console, options.Verbose.HasValue(), false);
                    var helper = new DepositHelper();

                    var withdraw = new WithdrawAssetAccount
                    {
                        AssetAccountId = argumentAssetAccountId.Value,
                        ReferenceAccountId = argumentReferenceAccount.Value,
                        Amount = amount,
                        Timestamp = timestamp
                    };

                    reporter.Verbose("Prototype withdraw (from command line arguments):");
                    reporter.Verbose(JsonConvert.SerializeObject(withdraw));

                    if (!helper.IsValid(withdraw) || optionInteractive.HasValue())
                    {
                        try
                        {
                            withdraw = helper.GetValid(withdraw);
                        }
                        catch (NotSupportedException ex)
                        {
                            throw new CommandParsingException(app, $"Operation Aborted. {ex.Message}", ex);
                        }

                        reporter.Verbose("Validated withdraw (from interactive console):");
                        reporter.Verbose(JsonConvert.SerializeObject(withdraw));
                    }

                    options.Command = new WithdrawAssetAccountCommand { withdraw = withdraw };
                });
        }

        public Task ExecuteAsync(CommandContext context) => context.AssetAccountsClient.WithdrawAsync(this.withdraw);

        private static string Safe(string value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NotSupportedException(errorMessage);
            }

            return value;
        }

        private class DepositHelper
        {
            public WithdrawAssetAccount GetPrototype(WithdrawAssetAccount withdraw) => withdraw;

            public bool IsValid(WithdrawAssetAccount withdraw) =>
                !string.IsNullOrWhiteSpace(withdraw.AssetAccountId)
                && !string.IsNullOrWhiteSpace(withdraw.ReferenceAccountId)
                && withdraw.Amount > 0;

            public WithdrawAssetAccount GetValid(WithdrawAssetAccount withdraw)
            {
                withdraw.AssetAccountId = Safe(Prompt.GetString("Asset Account Id:", withdraw.AssetAccountId), "Cannot withdraw without specifying account id");
                withdraw.ReferenceAccountId = Safe(Prompt.GetString("Reference Account:", withdraw.ReferenceAccountId), "Cannot withdraw without specifying reference account id");
                var amountString = Safe(Prompt.GetString("Amount:", string.Format(new NumberFormatInfo() { NumberDecimalDigits = 2 }, "{0:F}", withdraw.Amount)), "Cannot withdraw without specifying some amount");
                withdraw.Amount = Convert.ToDecimal(amountString, CultureInfo.InvariantCulture);

                if (this.GetPrecision(amountString.TrimEnd('0')) > 0)
                {
                    throw new NotSupportedException("Maximum two decimal places are allowed in withdrawal amount");
                }

                if (DateTime.TryParse(Safe(Prompt.GetString("Transaction Timestamp:", withdraw.Timestamp.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)), "Cannot withdraw without specifying account id"), out DateTime timestamp))
                {
                    withdraw.Timestamp = timestamp;
                }
                else
                {
                    throw new NotSupportedException("Specify a valid Date Time for timestamp");
                }

                return withdraw;
            }

            private int GetPrecision(string amount)
            {
                return amount
                    .Trim()
                    .SkipWhile(c => c != '.')
                    .Skip(1)
                    .Count();
            }
        }
    }
}
