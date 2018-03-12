// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Client
{
    /// <summary>
    /// Represents Client Tiers
    /// </summary>
    public enum ClientTierDto
    {
        // Note: (Waqas) copied from web terminal
#pragma warning disable CA1707 // Identifiers should not contain underscores
        /// <summary>
        /// Client Tier 1
        /// </summary>
        TIER_1 = 0,

        /// <summary>
        /// Client Tier 2
        /// </summary>
        TIER_2,

        /// <summary>
        /// Client Tier 3
        /// </summary>
        TIER_3,

        /// <summary>
        /// Client Tier 4
        /// </summary>
        TIER_4,

        /// <summary>
        /// Client Tier 5
        /// </summary>
        TIER_5
#pragma warning restore CA1707 // Identifiers should not contain underscores
    }
}
