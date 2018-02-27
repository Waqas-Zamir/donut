// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Console.Commands
{
    using Donut.Client;
    using Donut.Console.Persistence;
    using McMaster.Extensions.CommandLineUtils;

    public class CommandContext
    {
        public CommandContext(
            IConsole console,
            IReporter reporter,
            IUsersClient usersClient,
            IAssetAccountsHttpClient assetAccountsClient,
            ICommandDataRepository repository)
        {
            this.Console = console;
            this.Reporter = reporter;
            this.UsersClient = usersClient;
            this.AssetAccountsClient = assetAccountsClient;
            this.Repository = repository;
        }

        public IConsole Console { get; }

        public IReporter Reporter { get; }

        public IUsersClient UsersClient { get; }

        public IAssetAccountsHttpClient AssetAccountsClient { get; set; }

        public ICommandDataRepository Repository { get; }
    }
}