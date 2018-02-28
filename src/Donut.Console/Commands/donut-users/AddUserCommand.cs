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

    internal class AddUserCommand : ICommand
    {
        private User user;

        private AddUserCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Creates a new user";
            app.ExtendedHelpText = $"{Environment.NewLine}Use 'users add -i' to enter interactive mode{Environment.NewLine}";

            // arguments
            var argumentUserId = app.Argument("id", "The user subject identifier", false);
            var argumentClientTier = app.Argument("tier", string.Concat("The user client tier (", string.Join(" | ", Enum.GetNames(typeof(ClientTierDto))), ")"), false);

            // options
            var optionDefaultAssetAccountId = app.Option("-a|--default_account <default_account>", "The default asset account id for the user", CommandOptionType.SingleValue);
            var optionInteractive = app.Option("-i|--interactive", "Enters interactive mode", CommandOptionType.NoValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    ClientTierDto tier = default(ClientTierDto);

                    if ((string.IsNullOrEmpty(argumentUserId.Value) || !Enum.TryParse(argumentClientTier.Value, true, out tier)) && !optionInteractive.HasValue())
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    var reporter = new ConsoleReporter(console, options.Verbose.HasValue(), false);
                    var helper = new UserHelper();

                    var user = new User
                    {
                        UserId = argumentUserId.Value,
                        ClientTier = tier,
                        Role = UserRolesDto.Snow_Investor,
                        DefaultAssetAccountId = optionDefaultAssetAccountId.Value(),
                    };

                    reporter.Verbose("Prototype user (from command line arguments):");
                    reporter.Verbose(JsonConvert.SerializeObject(user));

                    if (!helper.IsValid(user) || optionInteractive.HasValue())
                    {
                        try
                        {
                            user = helper.GetValid(user);
                        }
                        catch (NotSupportedException ex)
                        {
                            throw new CommandParsingException(app, $"Operation Aborted. {ex.Message}", ex);
                        }

                        reporter.Verbose("Validated user (from interactive console):");
                        reporter.Verbose(JsonConvert.SerializeObject(user));
                    }

                    options.Command = new AddUserCommand { user = user };
                });
        }

        public async Task ExecuteAsync(CommandContext context) => await context.UsersClient.AddUserAsync(this.user).ConfigureAwait(false);

        private static string Safe(string value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NotSupportedException(errorMessage);
            }

            return value;
        }

        private class UserHelper
        {
            public User GetPrototype(User user) => user;

            public bool IsValid(User user) => !string.IsNullOrEmpty(user.UserId);

            public User GetValid(User user)
            {
                user.UserId = Safe(Prompt.GetString("User Id:", user.UserId), "Cannot create a user without a user id.");
                user.ClientTier = user.ClientTier == 0 ? Enum.Parse<ClientTierDto>(Safe(Prompt.GetString("Client Tier:", user.ClientTier.ToString()), "Cannot create a user without Client Tier")) : user.ClientTier;
                user.DefaultAssetAccountId = Prompt.GetString("Default Asset Account Id for the user [optional]:", user.DefaultAssetAccountId);

                // defaults
                user.DefaultAssetAccountId = string.IsNullOrWhiteSpace(user.DefaultAssetAccountId) ? null : user.DefaultAssetAccountId;

                return user;
            }
        }
    }
}
