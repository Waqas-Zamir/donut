// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Controllers
{
    using System.Diagnostics;
    using System.Reflection;
    using Donut;
    using Microsoft.AspNetCore.Mvc;

    [Route("platform")]
    public class RootController : Controller
    {
        private static readonly object Version =
            new
            {
                Title = typeof(Program).Assembly.Attribute<AssemblyTitleAttribute>(attribute => attribute.Title),
                Version = typeof(Program).Assembly.Attribute<AssemblyInformationalVersionAttribute>(attribute => attribute.InformationalVersion),
                OS = System.Runtime.InteropServices.RuntimeInformation.OSDescription.TrimEnd(),
                ProcessId = Process.GetCurrentProcess().Id,
            };

        [HttpGet]
        public IActionResult Get() => this.Ok(Version);
    }
}
