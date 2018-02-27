// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Client
{
    public enum UserRolesDto
    {
        // Note: (Waqas) copied from web terminal
#pragma warning disable CA1707 // Identifiers should not contain underscores
        Snow_SuperUser = 1,
        Snow_MarketMaker = 2,
        Snow_Intermediary = 3,
        Snow_Investor = 4,
        Snow_CustomerCare = 5,
        Snow_Credit = 6,
        Snow_BackOfficeTrading = 7,
        Snow_BackOfficeAdmin = 8,
#pragma warning restore CA1707 // Identifiers should not contain underscores
    }
}
