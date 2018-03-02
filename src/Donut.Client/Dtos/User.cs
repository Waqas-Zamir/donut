// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Client
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the subject identifier for this user.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets client tier for the user
        /// </summary>
        public ClientTierDto ClientTier { get; set; }

        /// <summary>
        /// Gets or sets default asset account in case if user has multiple asset accounts
        /// </summary>
        public string DefaultAssetAccountId { get; set; }
    }
}
