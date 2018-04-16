// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Console.Persistence
{
    public class CommandData
    {
        public string Authority { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

#pragma warning disable CA1056
        public string ServiceUrl { get; set; }
    }
}
