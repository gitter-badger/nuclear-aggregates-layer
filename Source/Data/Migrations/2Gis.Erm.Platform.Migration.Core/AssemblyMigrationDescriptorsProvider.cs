using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DoubleGis.Erm.Platform.Migration.Core
{
    /// <summary>
    /// Класс для загрузки миграций из сборки.
    /// </summary>
    public sealed class AssemblyMigrationDescriptorsProvider : IMigrationDescriptorsProvider
    {
        private readonly string _namespace;
        private readonly bool _loadNestedNamespaces;
        private readonly List<Assembly> _assemblies = new List<Assembly>();

// Используется в DI контейнере.
// ReSharper disable UnusedMember.Global
        public AssemblyMigrationDescriptorsProvider(IEnumerable<string> assemblyNames)
// ReSharper restore UnusedMember.Global
            : this(assemblyNames, null, true)
        {
        }

        public AssemblyMigrationDescriptorsProvider(IEnumerable<string> assemblyNames, string @namespace, bool loadNestedNamespaces)
        {
            foreach (var assemblyName in assemblyNames)
            {
                _assemblies.Add(Assembly.Load(assemblyName));
            }
            
            _namespace = @namespace;
            _loadNestedNamespaces = loadNestedNamespaces;

            Initialize();
        }

        public AssemblyMigrationDescriptorsProvider(IEnumerable<Assembly> assemblies, string @namespace, bool loadNestedNamespaces)
        {
            _assemblies.AddRange(assemblies);
            _namespace = @namespace;
            _loadNestedNamespaces = loadNestedNamespaces;

            Initialize();
        }

        public List<MigrationDescriptor> MigrationDescriptors { get; private set; }

        private void Initialize()
        {
            MigrationDescriptors = new List<MigrationDescriptor>();

            IEnumerable<MigrationDescriptor> migrationList = FindMigrations();

            foreach (var migrationMetadata in migrationList)
            {
                if (MigrationDescriptors.FirstOrDefault(x => x.Version == migrationMetadata.Version) != null)
                {
                    throw new Exception(string.Format("Duplicate migration version {0}.", migrationMetadata.Version));
                }

                MigrationDescriptors.Add(migrationMetadata);
            }

            MigrationDescriptors.Sort((x, y) => x.Version.CompareTo(y.Version));
        }

        private IEnumerable<MigrationDescriptor> FindMigrations()
        {
            var matchedTypes = _assemblies.SelectMany(x => x.ExportedTypes.Where(TypeIsMigration));

            if (!string.IsNullOrEmpty(_namespace))
            {
                Func<Type, bool> shouldInclude = t => t.Namespace == _namespace;
                if (_loadNestedNamespaces)
                {
                    var matchNested = _namespace + ".";
                    shouldInclude = t => t.Namespace == _namespace || (t.Namespace != null && t.Namespace.StartsWith(matchNested));
                }

                matchedTypes = matchedTypes.Where(shouldInclude);
            }

            return matchedTypes.Select(GetDescriptorForMigration);
        }

        private static bool TypeIsMigration(Type type)
        {
            return typeof(IMigration).IsAssignableFrom(type) && type.HasAttribute<MigrationAttribute>();
        }

        private static MigrationDescriptor GetDescriptorForMigration(Type type)
        {
            var migrationAttribute = type.GetOneAttribute<MigrationAttribute>();
            var migrationVersion = migrationAttribute.Version;
            var migrationDescription = migrationAttribute.Description;
            var migrationAuthor = migrationAttribute.Author;

            return new MigrationDescriptor
            {
                Type = type,
                Version = migrationVersion,
                Description = migrationDescription,
                Author = migrationAuthor,
            };
        }
    }
}