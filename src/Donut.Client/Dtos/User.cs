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

        public ClientTierDto ClientTier { get; set; }

        public UserRolesDto Role { get; set; }

        public string DefaultAssetAccountId { get; set; }
    }
}
