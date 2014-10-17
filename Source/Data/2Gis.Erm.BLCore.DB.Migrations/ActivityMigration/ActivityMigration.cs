#define PROFILE

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Extensions;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.MW;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Xrm.Client.Data.Services;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration
{
    using CrmUserMetadata = Metadata.Crm.User;
    using ErmEntityName = Metadata.Erm.EntityName;

    public abstract class ActivityMigration<TActivity> : IContextedMigration<IActivityMigrationContext>, INonDefaultDatabaseMigration where TActivity : class
    {
        private const int PageSize = 1000;
        private const int SqlBulkSizeDefault = 10000;
        private const int ProfileSlice = 5000;
        private const long MigrationLimit = long.MaxValue;

        protected ActivityMigration(int sqlBulkSize = SqlBulkSizeDefault)
        {
            SqlBulkSize = sqlBulkSize;
        }

        private int SqlBulkSize { get; set; }

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmConnection; }
        }

        public void Apply(IActivityMigrationContext context)
        {
            Execute(new ActivityMigrationContextExtended(context));
        }

        public void Revert(IActivityMigrationContext context)
        {
            throw new NotSupportedException();
        }

        private void Execute(IActivityMigrationContextExtended context)
        {
            context.Connection.BeginTransaction();
            try
            {
                Migrate(context);
                context.Connection.CommitTransaction();
            }
            catch
            {
                context.Connection.RollBackTransaction();
                throw;
            }
        }

        private void Migrate(IActivityMigrationContextExtended context)
        {
            var taskStack = new Stack<Task>();

            Action<string> executeQuery = sql =>
            {
                try
                {
                    context.Connection.ExecuteNonQuery(sql);
                }
                catch (Exception ex)
                {
                    TraceError("SQL execution failed.");
                    TraceError(sql);
                    TraceError(ex.ToString());
                }
            };
            Action<string> enqueueQuery =
                sql => taskStack.Push(taskStack.Count > 0
                    ? taskStack.Pop().ContinueWith(_ => executeQuery(sql))
                    : Task.Run(() => executeQuery(sql)));

            var entityCount = 0L;
            var sqlCache = new StringBuilder();

#if PROFILE
            var times = new List<TimeSpan>();
            var w = System.Diagnostics.Stopwatch.StartNew();
#endif

            using (var service = context.CrmContext.CreateService())
            {
                var query = CreateQuery();

                foreach (var entity in service.LoadEntities(query, PageSize))
                {
                    ++entityCount;
                    try
                    {
                        var activity = ParseActivity(context, entity);
                        if (activity == null)
                        {
                            var key = entity.Properties.OfType<KeyProperty>().Select(x => x.Value).FirstOrDefault();
                            TraceInfo("{0} '{1}' was skipped.", typeof(TActivity).Name, context.Parse<Guid>(key));
                            continue;
                        }

                        sqlCache.AppendLine(BuildSql(activity));

                        if (entityCount % SqlBulkSize == 0)
                        {
                            enqueueQuery(sqlCache.ToString());
                            sqlCache.Clear();
                        }
                    }
                    catch (Exception e)
                    {
                        var key = entity.Properties.OfType<KeyProperty>().Select(x => x.Value).FirstOrDefault();
                        TraceError("{0} '{1}' cannot be migrated due to the exception.", typeof(TActivity).Name, context.Parse<Guid>(key));
                        TraceError(e.ToString());
                    }

#if PROFILE
                    if (entityCount % ProfileSlice == 0)
                    {
                        times.Add(w.Elapsed);
                        w.Restart();
                        Task.Run(() =>
                        {
                            var avg = new TimeSpan((long)times.Average(x => x.Ticks));
                            var sum = new TimeSpan(times.Sum(x => x.Ticks));
                            var r = string.Format("elapsed time: {0}, time per {2}: {1}", sum, avg, ProfileSlice);
                            TraceInfo("Processed {0} records: {1}", entityCount, r);
                        });
                    }
                    if (entityCount >= MigrationLimit) break;
#endif
                }
            }

            if (sqlCache.Length > 0) ;
            enqueueQuery(sqlCache.ToString());

            Task.WaitAll(taskStack.ToArray());

#if PROFILE
            w.Stop(); times.Add(w.Elapsed);
            var total = new TimeSpan(times.Sum(x => x.Ticks));
            TraceInfo("{0} records in {1}", entityCount, total);
#endif
        }

        internal abstract QueryExpression CreateQuery();
        internal abstract TActivity ParseActivity(IActivityMigrationContextExtended context, DynamicEntity entity);
        internal abstract string BuildSql(TActivity activity);

        internal static string BuildSqlStatement<T>(string template, long activityId, Enum reference, ErmEntityName referencedType, T? referencedId) where T : struct
        {
            return referencedId.HasValue
                ? QueryBuilder.Format(template, activityId, reference, referencedType, referencedId.Value)
                : null;
        }

        #region ActivityMigrationContextExtended

        private class ActivityMigrationContextExtended : IActivityMigrationContextExtended
        {
            private readonly IActivityMigrationContext _context;
            private readonly ReferenceResolver _referenceResolver;

            public ActivityMigrationContextExtended(IActivityMigrationContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                _context = context;
                _referenceResolver = new ReferenceResolver(context);
            }

            #region IMigrationContext

            Database IMigrationContext.Database
            {
                get { return _context.Database; }
            }

            ServerConnection IMigrationContext.Connection
            {
                get { return _context.Connection; }
            }

            TextWriter IMigrationContext.Output
            {
                get { return _context.Output; }
            }

            string IMigrationContext.ErmDatabaseName
            {
                get { return _context.ErmDatabaseName; }
            }

            string IMigrationContext.LoggingDatabaseName
            {
                get { return _context.LoggingDatabaseName; }
            }

            string IMigrationContext.CrmDatabaseName
            {
                get { return _context.CrmDatabaseName; }
            }

            void IDisposable.Dispose()
            {
                _context.Dispose();
            }

            #endregion

            #region ICrmMigrationContext

            public CrmDataContext CrmContext
            {
                get { return _context.CrmContext; }
            }

            #endregion

            public long NewIdentity()
            {
                return _context.NewIdentity();
            }

            public T Parse<T>(object value)
            {
                return Cast<T>(ParseImpl(value));
            }

            #region Parsing Stuff

            /// <summary>
            /// Converts CRM types to .NET ones.
            /// </summary>
            private object ParseImpl(object value)
            {
                if (value == null)
                    return null;

                var valueType = value.GetType();

                // Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, Single and String
                if (valueType.IsPrimitive || valueType == typeof(String))
                    return value;

                // dynamic approach for simple types wrapped in CRM
                if (value is CrmBoolean || value is CrmDouble || value is CrmFloat || value is CrmNumber || value is Picklist || value is Status || value is EntityNameReference)
                {
                    dynamic valueAsDynamic = value;
                    return valueAsDynamic.IsNull
                               ? DBNull.Value
                               : valueAsDynamic.Value;
                }

                var dateTime = value as CrmDateTime;
                if (dateTime != null)
                {
                    if (dateTime.IsNull)
                        return DBNull.Value;
                    return dateTime.UniversalTime;
                }

                var reference = value as CrmReference;
                if (reference != null)
                {
                    if (reference.IsNull)
                        return DBNull.Value;
                    long referenceId;
                    if (_referenceResolver.TryGetReference(reference.type, reference.Value, out referenceId))
                    {
                        return referenceId;
                    }
                    throw new Exception(string.Format("The reference '{2}' with name '{1}' and type '{0}' cannot be resolved.", reference.type, reference.name, reference.Value));
                    //return referenceId;
                }

                var key = value as Key;
                if (key != null)
                {
                    if (ReferenceEquals(key, Key.Null))
                        return DBNull.Value;
                    return key.Value;
                }

                throw new NotSupportedException();
            }

            /// <summary>
            /// Converts the value to the requested type.
            /// </summary>
            private static T Cast<T>(object value)
            {
                if (value == null || ReferenceEquals(value, DBNull.Value))
                    return default(T);

                var valueType = typeof(T);
                var invariantCulture = CultureInfo.InvariantCulture;

                // Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, Single
                if (valueType.IsPrimitive)
                    return (T)Convert.ChangeType(value, valueType, invariantCulture);

                // String, DateTime, Guid
                if (valueType == typeof(DateTime) || valueType == typeof(String) || valueType == typeof(Guid))
                    return (T)Convert.ChangeType(value, valueType, invariantCulture);

                // Enums
                if (valueType.IsEnum)
                    return (T)Enum.Parse(valueType, Convert.ToString(value, invariantCulture));

                // Nullable
                if (Nullable.GetUnderlyingType(valueType) != null)
                    return (T)Convert.ChangeType(value, Nullable.GetUnderlyingType(valueType), invariantCulture);

                throw new NotSupportedException();
            }

            #endregion

            #region ReferenceResolver

            /// <summary>
            /// Contains the maps to convert CRM identities to ERM ones.
            /// </summary>
            private class ReferenceResolver
            {
                private readonly IDictionary<string, Map> _dics = new Dictionary<string, Map>();

                public ReferenceResolver(IActivityMigrationContext context)
                {
                    _dics.Add(EntityName.systemuser.ToString(), new UserMap(context));
                    _dics.Add(EntityName.appointment.ToString(), new BaseMap(context, "[Activity].[AppointmentBase]"));
                    _dics.Add(EntityName.phonecall.ToString(), new BaseMap(context, "[Activity].[PhonecallBase]"));
                    _dics.Add(EntityName.task.ToString(), new BaseMap(context, "[Activity].[TaskBase]"));
                    _dics.Add(EntityName.account.ToString(), new BaseMap(context, "[Billing].[Clients]"));
                    _dics.Add(EntityName.contact.ToString(), new BaseMap(context, "[Billing].[Contacts]"));
                    _dics.Add(EntityName.opportunity.ToString(), new BaseMap(context, "[Billing].[Deals]"));
                    _dics.Add("dg_firm", new BaseMap(context, "[BusinessDirectory].[Firms]"));
                    _dics.Add(EntityName.bulkoperation.ToString(), new StubMap());
                }

                public bool TryGetReference(string type, Guid reference, out long id)
                {
                    Map map;
                    if (_dics.TryGetValue(type, out map))
                    {
                        return map.TryGetReference(reference, out id);
                    }
                    id = 0;
                    return false;
                }

                private abstract class Map
                {
                    public abstract bool TryGetReference(Guid reference, out long id);
                }

                private class UserMap : Map
                {
                    private const string IntegrationAccout = "Integration";

                    private IActivityMigrationContext _context;
                    private IDictionary<Guid, long> _userMap;
                    private long _integrationUserId;

                    public UserMap(IActivityMigrationContext context)
                    {
                        _context = context;
                    }

                    public override bool TryGetReference(Guid reference, out long id)
                    {
                        EnsureInitialized();

                        if (!_userMap.TryGetValue(reference, out id))
                        {
                            id = _integrationUserId;
                        }

                        return true;
                    }

                    private void EnsureInitialized()
                    {
                        if (_userMap == null)
                        {
                            _userMap = LookupUsers(_context, out _integrationUserId);
                            _context = null;
                        }
                    }

                    private static IDictionary<Guid, long> LookupUsers(IActivityMigrationContext context, out long integrationUserId)
                    {
                        integrationUserId = 0;

                        Dictionary<string, Guid> crmUsers;
                        using (var service = context.CrmContext.CreateService())
                        {
                            crmUsers = service.LoadEntities(new QueryExpression
                            {
                                EntityName = EntityName.systemuser.ToString(),
                                ColumnSet = new ColumnSet(new[] { CrmUserMetadata.Id, CrmUserMetadata.DomainName }),
                            })
                                .Select(x => new { Key = (Key)x[CrmUserMetadata.Id], Account = (string)x[CrmUserMetadata.DomainName] })
                                .Where(x => !string.IsNullOrWhiteSpace(x.Account) && x.Account.StartsWith("2GIS\\"))
                                .ToDictionary(x => OmitDomainName(x.Account), x => x.Key.Value);
                        }

                        var users = new Dictionary<Guid, long>();

                        // get users in the order to avoid a case when account name is not unique
                        using (var reader = context.Connection.ExecuteReader(@"SELECT Id,Account FROM [Security].[Users] ORDER BY CreatedOn"))
                            while (reader.Read())
                            {
                                var ermId = reader.GetSqlInt64(0);
                                var account = reader.GetSqlString(1);

                                if (ermId.IsNull || account.IsNull) continue;

                                if (string.CompareOrdinal(IntegrationAccout, account.Value) == 0)
                                {
                                    integrationUserId = ermId.Value;
                                }

                                Guid crmId;
                                if (crmUsers.TryGetValue(account.Value, out crmId))
                                {
                                    // set operation is used to override previous value according to created date order
                                    users[crmId] = ermId.Value;
                                }
                            }

                        return users;
                    }

                    private static string OmitDomainName(string account)
                    {
                        if (account != null && account.Contains("\\"))
                            return account.Split('\\')[1];
                        return account;
                    }
                }

                private abstract class ReplicatedMap : Map
                {
                    private readonly Lazy<IDictionary<Guid, long>> _map;

                    protected ReplicatedMap(Func<IDictionary<Guid, long>> factory)
                    {
                        _map = new Lazy<IDictionary<Guid, long>>(factory);
                    }

                    public override bool TryGetReference(Guid reference, out long id)
                    {
                        return _map.Value.TryGetValue(reference, out id);
                    }

                    protected static IDictionary<Guid, long> LookupEntity(IMigrationContext context, string tableName, string ermIdName = "[Id]", string crmIdName = "[ReplicationCode]")
                    {
                        var map = new Dictionary<Guid, long>();

                        var command = string.Format(@"SELECT {1}, {2} FROM {0}", tableName, ermIdName, crmIdName);
                        const int ErmIdOrdinal = 0;
                        const int CrmIdOrdinal = 1;

                        using (var reader = context.Connection.ExecuteReader(command))
                            while (reader.Read())
                            {
                                var ermId = reader.GetSqlInt64(ErmIdOrdinal);
                                var crmId = reader.GetSqlGuid(CrmIdOrdinal);

                                if (ermId.IsNull || crmId.IsNull) continue;

                                map[crmId.Value] = ermId.Value;
                            }

                        return map;
                    }
                }

                private class BaseMap : ReplicatedMap
                {
                    public BaseMap(IMigrationContext context, string table)
                        : base(() => LookupEntity(context, table))
                    {
                    }
                }

                private class StubMap : Map
                {
                    public override bool TryGetReference(Guid reference, out long id)
                    {
                        id = 0;
                        return true;
                    }
                }
            }

            #endregion
        }

        #endregion

        #region SerializationHelper

        protected static class QueryBuilder
        {
            public static string Format(string template, params object[] arguments)
            {
                return FlattenTemplate(template, arguments);
            }

            private static string FlattenTemplate(string template, params object[] args)
            {
                return string.Format(template, args.Select(SerialzeValue).Cast<object>().ToArray());
            }

            private static string SerialzeValue(object value)
            {
                if (value == null)
                    return "NULL";

                if (value is Guid)
                    return "'" + ((Guid)value).ToString("D") + "'";

                if (value is DateTime)
                    return "'" + ((DateTime)value).ToString("o", Culture) + "'"; // ISO 8601 format

                if (value is Enum)
                    return ((Enum)value).ToString("d"); // as an integer

                if (value is String)
                    return "'" + Urn.EscapeString((string)value) + "'";

                if (value is bool)
                    return ((bool)value) ? "1" : "0";

                if (value is byte[])
                    return SerializeByteArray((byte[])value);

                var formattableValue = value as IFormattable;
                if (formattableValue != null)
                    return formattableValue.ToString(null, Culture);

                return value.ToString();
            }

            private static string SerializeByteArray(byte[] array)
            {
                if (array.Length == 0) return "0x0";

                var hex = new StringBuilder(array.Length * 2);
                hex.Append("0x");
                foreach (var b in array)
                {
                    hex.AppendFormat("{0:x2}", b);
                }
                return hex.ToString();
            }

            private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        }

        #endregion

        #region Tracing

        protected static void TraceError(string msg, params object[] args)
        {
            Console.Error.WriteLineAsync(string.Format(msg, args));
        }

        protected static void TraceInfo(string msg, params object[] args)
        {
            Console.Out.WriteLineAsync(string.Format(msg, args));
        }

        #endregion
    }
}