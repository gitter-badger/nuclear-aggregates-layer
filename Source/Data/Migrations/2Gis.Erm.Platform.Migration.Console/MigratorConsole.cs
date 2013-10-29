using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Runner;

using NDesk.Options;

namespace DoubleGis.Erm.Platform.Migration.Console
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
                    System.Console.WriteLine("FluentMigrator.Console:");
                    System.Console.WriteLine(e.Message);
                    System.Console.WriteLine("Try 'migrate --help' for more information.");
                    return;
                }

                if (_arguments.ShowHelp)
                {
                    DisplayHelp(optionSet);
                    return;
                }

                if (string.IsNullOrEmpty(_arguments.ErmOnlyConnectionString) &&
                    string.IsNullOrEmpty(_arguments.ApplicationRoot) &&
                    string.IsNullOrEmpty(_arguments.ConnectionStringConfigPath) &&
                    !_arguments.ListMigrations)
                {
                    DisplayHelp(optionSet);
                    Environment.ExitCode = 1;
                    return;
                }

                if (_arguments.TargetAssembly == null)
                {
                    var currentPathUri = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                    var currentPath = new Uri(currentPathUri).LocalPath;

                    _arguments.TargetAssembly = Path.Combine(currentPath, "2Gis.Erm.BL.DB.Migrations.dll");
                }

                string remoteHost = null;
                if (_arguments.ErmOnlyConnectionString != null)
                {
                    _calculatedArguments.ConnectionStrings = new ConnectionStringSettingsCollection
                        {
                            new ConnectionStringSettings("Erm", _arguments.ErmOnlyConnectionString)
                        };
                    _calculatedArguments.UseCrmConnection = false;
                }
                else if (_arguments.ApplicationRoot != null)
                {
                    _arguments.ApplicationRoot = Path.GetFullPath(_arguments.ApplicationRoot);

                    var uri = new Uri(_arguments.ApplicationRoot);
                    if (uri.Host.Length > 0)
                    {
                        remoteHost = uri.Host;
                    }

                    if (!Directory.Exists(_arguments.ApplicationRoot))
                    {
                        System.Console.WriteLine("Can't access directory {0}", _arguments.ApplicationRoot);
                        Environment.ExitCode = 1;
                        return;
                    }

                    _arguments.TargetAssembly = Path.Combine(_arguments.ApplicationRoot, "bin", "2Gis.Erm.BL.DB.Migrations.dll");
                    _arguments.ConnectionStringConfigPath = Path.Combine(_arguments.ApplicationRoot, "Web.config");
                }
                
                _arguments.TargetAssembly = Path.GetFullPath(_arguments.TargetAssembly);
                if (!File.Exists(_arguments.TargetAssembly))
                {
                    System.Console.WriteLine("Can't access file {0}", _arguments.TargetAssembly);
                    Environment.ExitCode = 1;
                    return;
                }

                if (_arguments.ConnectionStringConfigPath != null)
                {
                    _arguments.ConnectionStringConfigPath = Path.GetFullPath(_arguments.ConnectionStringConfigPath);
                    if (!File.Exists(_arguments.ConnectionStringConfigPath))
                    {
                        System.Console.WriteLine("Can't access file {0}", _arguments.ConnectionStringConfigPath);
                        Environment.ExitCode = 1;
                        return;
                    }

                    var configuration = ConfigurationHelper.LoadFromFile(_arguments.ConnectionStringConfigPath);

                    _calculatedArguments.ConnectionStrings = configuration.ConnectionStrings.ConnectionStrings;

                    // Если утилита вызвана для удаленного хоста, надо проапдейтить строки подключения к БД (в них обычно указаны локальные адреса).
                    if (remoteHost != null)
                    {
                        _calculatedArguments.ConnectionStrings = UpdateConnectionStrings(_calculatedArguments.ConnectionStrings, remoteHost);
                    }

                    _calculatedArguments.UseCrmConnection = configuration.AppSettings.Settings["EnableReplication"].Value.ToLower() == "true";
                }

                Execute(System.Console.Out, _arguments, _calculatedArguments);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("!! An error has occurred.  The error is:");
                System.Console.WriteLine(ex.ToString());
                
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
            System.Console.WriteLine("2GIS (R) DB Migration Console Utility ver. {0}",
                typeof(MigrationConsole).Assembly.GetName().Version);
            System.Console.WriteLine("Copyright (C) 2GIS. All rights reserved.");
            System.Console.WriteLine();
        }

        private static OptionSet GetConsoleOptionSet(MigrationConsoleArguments arguments)
        {
            return new OptionSet
                {
                    {
                        "root=",
                        "REQUIRED. The ERM application root directory.",
                        v => { arguments.ApplicationRoot = v; }
                    },
                    {
                        "ermonly=",
                        "The ERM connection string.",
                        v => { arguments.ErmOnlyConnectionString = v; }
                    },
                    {
                        "assembly=|a=|target=",
                        "The assembly containing the migrations you want to execute.",
                        v => { arguments.TargetAssembly = v; }
                    },
                    {
                        "connectionStringConfigPath=|configPath=|config=",
                        "The path of the .config file where the connection strings are stored.",
                        v => { arguments.ConnectionStringConfigPath = v; }
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
                        v => { arguments.OutputSQL = true; }
                    },
                    //{
                    //    "timeout=",
                    //    "Overrides the default SqlCommand timeout of 30 seconds.",
                    //    v => { Timeout = int.Parse(v); }
                    //},
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
            System.Console.WriteLine(Hr);
            System.Console.WriteLine("Example:");
            System.Console.WriteLine("  migrate -a bin\\debug\\MyMigrations.dll -config=..\\web.config -c=Erm -u");
            System.Console.WriteLine(Hr);
            System.Console.WriteLine("Example:");
            System.Console.WriteLine("  migrate -root=\"\\\\uk-erm-test\\c$\\inetpub\\wwwroot\\Erm99\" -u");
            System.Console.WriteLine(" DoubleGis.Databases.Erm.Migrations.Impl.dll is assumed to be in \\bin\\ subfolder of root folder");
            System.Console.WriteLine(" Web.config is assumed to be in root folder");
            System.Console.WriteLine(Hr);
            System.Console.WriteLine("Example:");
            System.Console.WriteLine("  migrate -ermonly=\"Data Source=.;Initial Catalog=Erm;Integrated Security=True;Application Name=ErmWeb\" -u");
            System.Console.WriteLine(" Only ERM DB migrations are applied.");
            System.Console.WriteLine(" DoubleGis.Databases.Erm.Migrations.Impl.dll is assumed to be in current folder.");
            System.Console.WriteLine(Hr);
            System.Console.WriteLine("Either 'root' either 'ermonly' either both 'a' and 'config' parameters should be specifed");
            System.Console.WriteLine("Options:");
            p.WriteOptionDescriptions(System.Console.Out);
        }

        private static void Execute(TextWriter output, MigrationConsoleArguments args, MigrationConsoleCalculatedArguments calculatedArguments)
        {
            var dbConnectionRequired = args.ErmOnlyConnectionString != null || args.ListAppliedMigrations || args.Direction != MigrationDirection.None;

            if (!dbConnectionRequired && !args.ListMigrations)
            {
                output.WriteLine("The actions to perform aren't specifed.");
                return;
            }

            var a = Assembly.LoadFrom(args.TargetAssembly);

            if (args.ListMigrations)
            {
                OutputMigrations(output, a, args.Namespace);
            }

            if (!dbConnectionRequired || calculatedArguments.ConnectionStrings == null)
            {
                return;
            }

            var connectionStringsKnower = new ConnectionStringsKnower(calculatedArguments.ConnectionStrings, calculatedArguments.UseCrmConnection);

            if (args.ListAppliedMigrations)
            {
                ListAppliedMigrations(args, connectionStringsKnower, a, output);
                return;
            }

            var contextManager = new MigrationContextManager(connectionStringsKnower.GetConnectionString(), output, args.OutputSQL);
            var appliedVersionsManager = new SmoAppliedVersionsManager(contextManager);

            var migrationDescriptorsProvider = new AssemblyMigrationDescriptorsProvider(a, args.Namespace);
            using (var runner = new MigrationRunner(output, args.OutputSQL, connectionStringsKnower, appliedVersionsManager, calculatedArguments.UseCrmConnection))
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
            var contextManager = new MigrationContextManager(connectionStringsKnower.GetConnectionString(), output);

            var appliedVersionsManager = new SmoAppliedVersionsManager(contextManager);
            var migrationDescriptorsProvider = new AssemblyMigrationDescriptorsProvider(a, args.Namespace);

            OutputAppliedMigrations(output, appliedVersionsManager);
            if (args.ListMigrations)
            {
                output.WriteLine();
                var pendingMigrations = GetPendingMigrations(appliedVersionsManager, migrationDescriptorsProvider).Select(m => m.Version);

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

        private static ConnectionStringSettingsCollection UpdateConnectionStrings(ConnectionStringSettingsCollection connStrings, string hostName)
        {
            var result = new ConnectionStringSettingsCollection();
            
            var newDataSource = string.Format("Data Source={0}", hostName);

            for (var i = 0; i < connStrings.Count; i++)
            {
                var processedConnectionString = Regex.Replace(connStrings[i].ConnectionString, "Data Source=\\.", newDataSource, RegexOptions.IgnoreCase);

                result.Add(new ConnectionStringSettings(connStrings[i].Name, processedConnectionString, connStrings[i].ProviderName));
            }

            return result;
        }

        public class MigrationConsoleArguments
        {
            public MigrationConsoleArguments()
            {
                Direction = MigrationDirection.None;
            }

            public string Namespace { get; set; }
            public string TargetAssembly { get; set; }

            public string ErmOnlyConnectionString { get; set; }
            public int Timeout { get; set; }
            public bool ShowHelp { get; set; }
            public string ConnectionStringConfigPath { get; set; }

            /// <summary>
            /// Отобразить миграции, найденные в сборке.
            /// </summary>
            public bool ListMigrations { get; set; }

            /// <summary>
            /// Отобразить миграции, примененные к БД.
            /// </summary>
            public bool ListAppliedMigrations { get; set; }

            /// <summary>
            /// Вывод SQL кода.
            /// </summary>
            public bool OutputSQL { get; set; }

            public MigrationDirection Direction { get; set; }

            public long TargetMigrationVersion { get; set; }
            public string ApplicationRoot { get; set; }
        }

        public class MigrationConsoleCalculatedArguments
        {
            public ConnectionStringSettingsCollection ConnectionStrings { get; set; }
            public bool UseCrmConnection { get; set; }
        }
    }
}