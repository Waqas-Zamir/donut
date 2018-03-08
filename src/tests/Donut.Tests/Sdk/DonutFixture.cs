// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Tests.Sdk
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public sealed class DonutFixture : IDisposable
    {
        private readonly Process donutProcess;

        public DonutFixture()
        {
            this.BaseUri = "http://localhost:5009";
            this.donutProcess = this.StartDonut();
        }

#pragma warning disable CA1056
        public string BaseUri { get; }

        public void Dispose()
        {
            try
            {
                this.donutProcess.Kill();
            }
            catch (InvalidOperationException)
            {
            }

            this.donutProcess.Dispose();
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                NullValueHandling = NullValueHandling.Ignore,
            };

            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }

        [DebuggerStepThrough]
        private Process StartDonut()
        {
            var path = string.Format(
                CultureInfo.InvariantCulture,
                "..{0}..{0}..{0}..{0}..{0}Donut{0}Donut.csproj",
                Path.DirectorySeparatorChar);

            Process.Start(
                new ProcessStartInfo("dotnet", $"run -p {path} --terminalMatchingEngineUrl http://localhost:5000 --terminalWebserviceUrl http://localhost:5000'")
                {
                    UseShellExecute = true,
                });

            var processId = default(int);
            using (var client = new HttpClient())
            {
                var attempt = 0;
                while (true)
                {
                    Thread.Sleep(500);
                    try
                    {
                        using (var response = client.GetAsync(new Uri(this.BaseUri + "/api")).GetAwaiter().GetResult())
                        {
                            var api = JsonConvert.DeserializeObject<Api>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult(), GetJsonSerializerSettings());
                            processId = int.Parse(api.ProcessId, CultureInfo.InvariantCulture);
                        }

                        break;
                    }
                    catch (HttpRequestException)
                    {
                        if (++attempt >= 20)
                        {
                            throw;
                        }
                    }
                }
            }

            return Process.GetProcessById(processId);
        }

#pragma warning disable CA1812
        private class Api
        {
            public string ProcessId { get; set; }
        }
    }
}