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

    internal class DepositAssetAccountCommand : ICommand
    {
        private DepositAssetAccount deposit;

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Deposit to an asset account";
            app.ExtendedHelpText = $"{Environment.NewLine}Use 'accounts deposit -i' to enter interactive mode{Environment.NewLine}";

            // arguments
            var argumentAssetAccountId = app.Argument("account", "The asset account id to deposit", false);
            var argumentReferenceId = app.Argument("reference", "The unique reference identifier for this transaction", false);
            var argumentAmount = app.Argument("amount", "The amount to deposit", false);
            var argumentSettlementCurrency = app.Argument("currency", "The ISO currency code for settlement currency", false);

            // options
            var optionInteractive = app.Option("-i|--interactive", "Enters interactive mode", CommandOptionType.NoValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if ((string.IsNullOrWhiteSpace(argumentAssetAccountId.Value)
                        || string.IsNullOrWhiteSpace(argumentReferenceId.Value)
                        || string.IsNullOrWhiteSpace(argumentSettlementCurrency.Value)
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

                    var reporter = new ConsoleReporter(console, options.Verbose.HasValue(), false);
                    var helper = new DepositHelper();

                    var deposit = new DepositAssetAccount
                    {
                        AssetAccountId = argumentAssetAccountId.Value,
                        Amount = amount,
                        SettlementCurrency = argumentSettlementCurrency.Value,
                        Precision = string.IsNullOrEmpty(argumentAmount.Value)
                            ? 0
                            : argumentAmount.Value
                            .SkipWhile(c => c != '.')
                            .Skip(1)
                            .Count()
                };

                    reporter.Verbose("Prototype deposit (from command line arguments):");
                    reporter.Verbose(JsonConvert.SerializeObject(deposit));

                    if (!helper.IsValid(deposit) || optionInteractive.HasValue())
                    {
                        try
                        {
                            deposit = helper.GetValid(deposit);
                        }
                        catch (NotSupportedException ex)
                        {
                            throw new CommandParsingException(app, $"Operation Aborted. {ex.Message}", ex);
                        }

                        reporter.Verbose("Validated deposit (from interactive console):");
                        reporter.Verbose(JsonConvert.SerializeObject(deposit));
                    }

                    options.Command = new DepositAssetAccountCommand { deposit = deposit };
                });
        }

        public Task ExecuteAsync(CommandContext context) => context.AssetAccountsClient.DepositAsync(this.deposit);

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
            public DepositAssetAccount GetPrototype(DepositAssetAccount deposit) => deposit;

            public bool IsValid(DepositAssetAccount deposit) =>
                !string.IsNullOrWhiteSpace(deposit.AssetAccountId)
                && !string.IsNullOrWhiteSpace(deposit.ReferenceId)
                && !string.IsNullOrWhiteSpace(deposit.SettlementCurrency)
                && deposit.Amount > 0;

            public DepositAssetAccount GetValid(DepositAssetAccount deposit)
            {
                deposit.AssetAccountId = Safe(Prompt.GetString("Asset Account Id:", deposit.AssetAccountId), "Cannot deposit without specifying account id");
                deposit.ReferenceId = Safe(Prompt.GetString("Reference Id:", deposit.ReferenceId), "Cannot deposit without specifying reference id");
                var amountString = Safe(Prompt.GetString("Amount:", string.Format(new NumberFormatInfo() { NumberDecimalDigits = deposit.Precision }, "{0:F}", deposit.Amount)), "Cannot deposit without specifying some amount");
                deposit.Amount = Convert.ToDecimal(amountString, CultureInfo.InvariantCulture);
                deposit.SettlementCurrency = Safe(Prompt.GetString("Currency code:", deposit.SettlementCurrency), "Cannot deposit without specifying currency code");

                deposit.Precision = amountString
                    .Trim()
                    .SkipWhile(c => c != '.')
                    .Skip(1)
                    .Count();

                return deposit;
            }
        }
    }
}
