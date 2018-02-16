// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut
{
    /// <summary>
    /// App settings
    /// </summary>
    public class DonutSettings
    {
        /// <summary>
        /// Gets or sets Host address to proxy asset account requests
        /// http[s]://hostname[:port]
        /// </summary>
        public string AssetAccountServiceHost { get; set; }
    }
}
