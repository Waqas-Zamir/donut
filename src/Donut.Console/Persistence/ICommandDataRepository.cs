// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Console.Persistence
{
    public interface ICommandDataRepository
    {
        CommandData GetCommandData();

        void SetCommandData(CommandData commandData);
    }
}
