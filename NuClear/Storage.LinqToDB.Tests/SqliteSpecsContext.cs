using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Expressions;
using LinqToDB.Mapping;
using LinqToDB.SqlQuery;

using Machine.Specifications;

namespace Storage.LinqToDB.Tests
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedMember.Local
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
    abstract class SqliteSpecsContext
    {
        protected static DataConnection DataConnection { get; private set; }

        Establish context = () => DataConnection = CreateConnection("SqliteSpecsContext", new MappingSchema());

        Cleanup after = () =>
        {
            foreach (var connection in Connections.Values.Select(x => x.Connection))
            {
                connection.Close();
            }

            Connections.Clear();
        };

        static readonly ConcurrentDictionary<ConnectionStringSettings, DataConnection> Connections = new ConcurrentDictionary<ConnectionStringSettings, DataConnection>();
        
        static DataConnection CreateConnection(string connectionStringName, MappingSchema schema)
        {
            var connectionSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionSettings == null)
            {
                throw new ArgumentException("The connection settings was not found.", "connectionStringName");
            }
             
            return Connections.GetOrAdd(
                connectionSettings,
                settings =>
                {
                    var provider = DataConnection.GetDataProvider(settings.Name);

                    var connection = provider.CreateConnection(settings.ConnectionString);
                    connection.Open();

                    var dataConnection = new DataConnection(provider, connection).AddMappingSchema(schema);
                    
                    return TuneConnectionIfSqlite(dataConnection);
                });
        }

        static DataConnection TuneConnectionIfSqlite(DataConnection dataConnection)
        {
            if (dataConnection.DataProvider.Name == ProviderName.SQLite)
            {
                using (new NoSqlTrace())
                {
                    var schema = dataConnection.MappingSchema;
                    foreach (var table in Tables.Value)
                    {
                        var attributes = schema.GetAttributes<TableAttribute>(table);
                        if (attributes != null && attributes.Length > 0)
                        {
                            // SQLite does not support schemas
                            Array.ForEach(attributes, attr => attr.Schema = null);

                            // create empty table
                            CreateTableMethodInfo.MakeGenericMethod(table).Invoke(null, new object[] { dataConnection, null, null, null, null, null, DefaulNullable.None });
                        }
                    }

                    // SQLite does not support schemas
                    Tables.Value.SelectMany(table => dataConnection.MappingSchema.GetAttributes<TableAttribute>(table)).ToList().ForEach(x => x.Schema = null);
                }
            }

            return dataConnection;
        }

        static readonly MethodInfo CreateTableMethodInfo =
            MemberHelper.MethodOf(() => default(IDataContext).CreateTable<object>(default(string),
                                                                                  default(string),
                                                                                  default(string),
                                                                                  default(string),
                                                                                  default(string),
                                                                                  DefaulNullable.None))
                        .GetGenericMethodDefinition();

        static readonly Lazy<Type[]> Tables = new Lazy<Type[]>(
            () =>
            {
                var accessor = typeof(Entity);
                return accessor.Assembly.GetTypes()
                               .Where(t => t.IsClass && (t.Namespace ?? string.Empty).Contains(accessor.Namespace ?? string.Empty))
                               .ToArray();
            });

        sealed class NoSqlTrace : IDisposable
        {
            private readonly TraceLevel _level;

            public NoSqlTrace(TraceLevel level = TraceLevel.Off)
            {
                _level = DataConnection.TraceSwitch.Level;
                DataConnection.TurnTraceSwitchOn(level);
            }

            public void Dispose()
            {
                DataConnection.TurnTraceSwitchOn(_level);
            }
        }
    }
}