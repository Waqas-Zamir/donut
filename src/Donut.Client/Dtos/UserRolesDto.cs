// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Client
{
    /// <summary>
    /// Represents User Roles
    /// </summary>
    public enum UserRolesDto
    {
        // Note: (Waqas) copied from web terminal
#pragma warning disable CA1707 // Identifiers should not contain underscores
        /// <summary>
        /// SuperUser role for snow project
        /// </summary>
        Snow_SuperUser = 1,

        /// <summary>
        /// Market maker role for snow project
        /// </summary>
        Snow_MarketMaker = 2,

        /// <summary>
        /// Intermediary role for snow project
        /// </summary>
        Snow_Intermediary = 3,

        /// <summary>
        /// Investor role for snow project
        /// </summary>
        Snow_Investor = 4,

        /// <summary>
        /// Customer care role for snow project
        /// </summary>
        Snow_CustomerCare = 5,

        /// <summary>
        /// Credit role for snow project
        /// </summary>
        Snow_Credit = 6,

        /// <summary>
        /// Back office trading role for snow project
        /// </summary>
        Snow_BackOfficeTrading = 7,

        /// <summary>
        /// Back office administration role for snow project
        /// </summary>
        Snow_BackOfficeAdmin = 8,
#pragma warning restore CA1707 // Identifiers should not contain underscores
    }
}
