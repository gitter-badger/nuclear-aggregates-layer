using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Runner;

using NDesk.Options;

namespace DoubleGis.Erm.Migrator
{
    public class MigrationConsole
    {
        private readonly MigrationConsoleArguments _arguments = new MigrationConsoleArguments();
        private readonly MigrationConsoleCalculatedArguments _calculatedArguments = new MigrationConsoleCalculatedArguments();

        public MigrationConsole(params string[] args)
        {
            OutputHeader();
            try
            {
                var optionSet = GetConsoleOptionSet(_arguments);

                try
                {
                    optionSet.Parse(args);
                }
                catch (OptionException e)
                {
                    Console.WriteLine("FluentMigrator.Console:");
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Try 'migrate --help' for more information.");
                    return;
                }

                if (_arguments.ShowHelp)
                {
                    DisplayHelp(optionSet);
                    return;
                }

                if (string.IsNullOrEmpty(_arguments.TargetEnvironment) &&
                    !_arguments.ListMigrations)
                {
                    DisplayHelp(optionSet);
                    Environment.ExitCode = 1;
                    return;
                }

                // FIXME {all, 29.10.2013}: ����� ����������� ��������� ��������� �������� �� ���������� ������, �.�. ���������� ERM ������ ��������������� ����������, �� � �����-�� ����� �������� ���� �� �����  
                if (_arguments.TargetAssembly == null)
                {
                    var currentPathUri = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                    var currentPath = new Uri(currentPathUri).LocalPath;

                    _arguments.TargetAssembly = Path.Combine(currentPath, "2Gis.Erm.BL.DB.Migrations.dll");
                }
                
                _arguments.TargetAssembly = Path.GetFullPath(_arguments.TargetAssembly);
                if (!File.Exists(_arguments.TargetAssembly))
                {
                    Console.WriteLine("Can't access file {0}", _arguments.TargetAssembly);
                    Environment.ExitCode = 1;
                    return;
                }

                var environmentsSettings = ErmEnvironmentsSettingsLoader.Load(
                        ErmEnvironmentsSettingsLoader.DefaultEnvironmentsConfigFullPath,
                        _arguments.TargetEnvironment,
                        "Migrator");

                _calculatedArguments.ConnectionStrings = _calculatedArguments.ConnectionStrings ?? new ConnectionStringSettingsCollection();
                foreach (var pair in environmentsSettings.ConnectionStrings)
                {
                    _calculatedArguments.ConnectionStrings.Add(new ConnectionStringSettings(pair.Key.ToString(), pair.Value));
                }

                Execute(Console.Out, _arguments, _calculatedArguments);
            }
            catch (Exception ex)
            {
                Console.WriteLine("!! An error has occurred.  The error is:");
                Console.WriteLine(ex.ToString());
                
                Environment.ExitCode = 1;
            }
        }

        public enum MigrationDirection
        {
            None,
            Forward,
            Revert
        }

        private static void OutputHeader()
        {
            Console.WriteLine("2GIS (R) DB Migration Console Utility ver. {0}",
                typeof(MigrationConsole).Assembly.GetName().Version);
            Console.WriteLine("Copyright (C) 2GIS. All rights reserved.");
            Console.WriteLine();
        }

        private static OptionSet GetConsoleOptionSet(MigrationConsoleArguments arguments)
        {
            return new OptionSet
                {
                    {
                        "environment=|env=|e=",
                        "Optional. Target ERM environment",
                        v => { arguments.TargetEnvironment = v; }
                    },
                    {
                        "assembly=|a=|target=",
                        "The assembly containing the migrations you want to execute.",
                        v => { arguments.TargetAssembly = v; }
                    },
                    {
                        "namespace=|ns=",
                        "The namespace contains the migrations you want to run. Default is all migrations found within the Target Assembly will be run.",
                        v => { arguments.Namespace = v; }
                    },
                    {
                        "list|l",
                        "Lists all migrations in Target Assembly",
                        v => arguments.ListMigrations = true
                    },
                    {
                        "applied|ap",
                        "Lists all migrations applied to DB",
                        v => arguments.ListAppliedMigrations = true
                    },

                    {
                        "updateto=|ut=",
                        "Apply all migrations up to migration with specified version.",
                        v =>
                            {
                                arguments.Direction = MigrationDirection.Forward;
                                arguments.TargetMigrationVersion = long.Parse(v);
                            }
                    },
                    {
                        "update|u",
                        "Apply all migrations up to the most recent migration.",
                        v =>
                            {
                                arguments.Direction = MigrationDirection.Forward;
                                arguments.TargetMigrationVersion = long.MaxValue;
                            }
                    },
                    {
                        "revertto=|r=",
                        "Revert migration to the migration with specified number.",
                        v =>
                            {
                                arguments.Direction = MigrationDirection.Revert;
                                arguments.TargetMigrationVersion = long.Parse(v);
                            }
                    },
                    {
                        "output|out",
                        "Output SQL statements.",
                        v => { arguments.OutputSql = true; }
                    },
                    {
                        "help|h|?",
                        "Display this help.",
                        v => { arguments.ShowHelp = true; }
                    }
                };
        }

        private static void DisplayHelp(OptionSet p)
        {
            const string Hr = "-------------------------------------------------------------------------------";
            Console.WriteLine(Hr);
            Console.WriteLine("Example:");
            Console.WriteLine("  migrate -a bin\\debug\\MyMigrations.dll -config=..\\web.config -c=Erm -u");
            Console.WriteLine(Hr);
            Console.WriteLine("Example:");
            Console.WriteLine("  migrate -root=\"\\\\uk-erm-test\\c$\\inetpub\\wwwroot\\Erm99\" -u");
            Console.WriteLine(" DoubleGis.Databases.Erm.Migrations.Impl.dll is assumed to be in \\bin\\ subfolder of root folder");
            Console.WriteLine(" Web.config is assumed to be in root folder");
            Console.WriteLine(Hr);
            Console.WriteLine("Either 'root' either both 'a' and 'config' parameters should be specifed");
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        private static void Execute(TextWriter output, MigrationConsoleArguments args, MigrationConsoleCalculatedArguments calculatedArguments)
        {
            var connectionRequired = args.ListAppliedMigrations || args.Direction != MigrationDirection.None;

            if (!connectionRequired && !args.ListMigrations)
            {
                output.WriteLine("The actions to perform aren't specifed.");
                return;
            }

            var a = Assembly.LoadFrom(args.TargetAssembly);

            if (args.ListMigrations)
            {
                OutputMigrations(output, a, args.Namespace);
            }

            if (!connectionRequired || calculatedArguments.ConnectionStrings == null)
            {
                return;
            }

            var connectionStringsKnower = new ConnectionStringsKnower(calculatedArguments.ConnectionStrings);

            if (args.ListAppliedMigrations)
            {
                ListAppliedMigrations(args, connectionStringsKnower, a, output);
                return;
            }

            var contextManager = new MigrationContextManager(connectionStringsKnower, output, args.OutputSql);
            var appliedVersionsManager = new SmoAppliedVersionsManager(contextManager);

            var migrationDescriptorsProvider = new AssemblyMigrationDescriptorsProvider(a, args.Namespace);
            using (var runner = new MigrationRunner(output, args.OutputSql, connectionStringsKnower, appliedVersionsManager))
            {
                switch (args.Direction)
                {
                    case MigrationDirection.Forward:
                        var pendingMigrations = GetPendingMigrations(appliedVersionsManager, migrationDescriptorsProvider)
                            .Where(m => m.Version <= args.TargetMigrationVersion);

                        foreach (var pendingMigration in pendingMigrations)
                        {
                            runner.ApplyMigration(pendingMigration);
                        }

                        break;
                    case MigrationDirection.Revert:
                        long currentVersion = appliedVersionsManager.AppliedVersionsInfo.Latest();
                        var revetringMigrations = GetPendingMigrations(appliedVersionsManager, migrationDescriptorsProvider)
                            .Where(m => m.Version > args.TargetMigrationVersion && m.Version <= currentVersion)
                            .OrderByDescending(m => m.Version);

                        foreach (var revetringMigration in revetringMigrations)
                        {
                            runner.RevertMigration(revetringMigration);
                        }

                        break;
                }
            }
        }

        private static void ListAppliedMigrations(MigrationConsoleArguments args, ConnectionStringsKnower connectionStringsKnower, Assembly a, TextWriter output)
        {
            var contextManager = new MigrationContextManager(connectionStringsKnower, output);

            var appliedVersionsManager = new SmoAppliedVersionsManager(contextManager);
            var migrationDescriptorsProvider = new AssemblyMigrationDescriptorsProvider(a, args.Namespace);

            OutputAppliedMigrations(output, appliedVersionsManager);
            if (args.ListMigrations)
            {
                output.WriteLine();
                var pendingMigrations = GetPendingMigrations(appliedVersionsManager, migrationDescriptorsProvider).Select(m => m.Version).ToArray();

                output.WriteLine("Pending migrations: {0}", pendingMigrations.Any() ? string.Join(", ", pendingMigrations) : "None");
                output.WriteLine();
            }
        }

        private static IEnumerable<MigrationDescriptor> GetPendingMigrations(IAppliedVersionsReader appliedVersionsManager,
                                                                             IMigrationDescriptorsProvider migrationDescriptorsProvider)
        {
            appliedVersionsManager.LoadVersionInfo();
            var alreadyAppliedMigrations = appliedVersionsManager.AppliedVersionsInfo.GetAppliedMigrations();

            return migrationDescriptorsProvider.MigrationDescriptors
                                               .Where(x => !alreadyAppliedMigrations.Contains(x.Version))
                                               .OrderBy(x => x.Version)
                                               .ToArray();
        }

        private static void OutputAppliedMigrations(TextWriter output, IAppliedVersionsManager versionsManager)
        {
            var count = 0;

            output.WriteLine();

            versionsManager.LoadVersionInfo();
            foreach (long v in versionsManager.AppliedVersionsInfo.GetAppliedMigrations())
            {
                if (count == 0)
                {
                    output.WriteLine("Migrations applied to DB: ");
                    output.WriteLine("==========================");
                    output.WriteLine("Version");
                }

                output.WriteLine("   {0}", v);
                count++;
            }

            if (count == 0)
            {
                output.WriteLine("No migrations applied");
            }

            output.WriteLine();
        }

        private static void OutputMigrations(TextWriter output, Assembly a, string @namespace)
        {
            output.WriteLine();
            var migrationDescriptorsProvider = new AssemblyMigrationDescriptorsProvider(a, @namespace);
            var migrationsProvider = new AssemblyMigrationsProvider();

            output.WriteLine("Migrations in assembly {0} :", a.Location);
            if (migrationDescriptorsProvider.MigrationDescriptors.Count > 0)
            {
                output.WriteLine("========================================================");
                output.WriteLine(" Version               Description");
                output.WriteLine("========================================================");

                foreach (var descriptor in migrationDescriptorsProvider.MigrationDescriptors)
                {
                    var databaseName = string.Empty;
                    var implementation = migrationsProvider.GetMigrationImplementation(descriptor);
                    if (implementation is INonDefaultDatabaseMigration)
                    {
                        databaseName = string.Format("[DB:{0}]  ", (implementation as INonDefaultDatabaseMigration).ConnectionStringKey);
                    }

                    output.WriteLine("  {0}    {2}{1}", descriptor.Version, descriptor.Description, databaseName);
                }
            }
            else
            {
                output.WriteLine();
                output.WriteLine("No migrations found");
            }

            output.WriteLine();
        }

        public class MigrationConsoleArguments
        {
            public MigrationConsoleArguments()
            {
                Direction = MigrationDirection.None;
            }

            public string Namespace { get; set; }
            public string TargetAssembly { get; set; }

            public int Timeout { get; set; }
            public bool ShowHelp { get; set; }

            /// <summary>
            /// ���������� ��������, ��������� � ������.
            /// </summary>
            public bool ListMigrations { get; set; }

            /// <summary>
            /// ���������� ��������, ����������� � ��.
            /// </summary>
            public bool ListAppliedMigrations { get; set; }

            /// <summary>
            /// ����� SQL ����.
            /// </summary>
            public bool OutputSql { get; set; }

            public MigrationDirection Direction { get; set; }

            public long TargetMigrationVersion { get; set; }
            public string TargetEnvironment { get; set; }
        }

        public class MigrationConsoleCalculatedArguments
        {
            public ConnectionStringSettingsCollection ConnectionStrings { get; set; }
        }
    }
}