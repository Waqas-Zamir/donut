﻿// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Console
{
    using System.Reflection;
    using McMaster.Extensions.CommandLineUtils;

    internal static class CommandLineApplicationExtensions
    {
        public static void ShowVersionAndHelp(this CommandLineApplication app)
        {
            app.Out.WriteLine($"Web Terminal (CFD Platform) Management Console [{GetInformationalVersion(typeof(Program).Assembly)}]");
            app.Out.WriteLine();

            app.ShowHelp();
        }

        private static string GetInformationalVersion(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            var versionAttribute = attribute == null
                ? assembly.GetName().Version.ToString()
                : attribute.InformationalVersion;

            return versionAttribute;
        }
    }
}
