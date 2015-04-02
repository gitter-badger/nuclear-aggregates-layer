using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.CRM;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Xrm.Client.Data.Services;

namespace DoubleGis.Erm.Platform.Migration.MW
{
    public sealed class ActivityMigrationContext : IActivityMigrationContext
    {
        private readonly IMigrationContext _ermContext;
        private readonly ICrmMigrationContext _crmContext;

        public ActivityMigrationContext(IMigrationContext ermContext, ICrmMigrationContext crmContext)
        {
            _ermContext = ermContext;
            _crmContext = crmContext;
        }

        public Database Database
        {
            get { return _ermContext.Database; }
        }

        public ServerConnection Connection
        {
            get { return _ermContext.Connection; }
        }

        public TextWriter Output
        {
            get { return _ermContext.Output; }
        }

        public string ErmDatabaseName
        {
            get { return _ermContext.ErmDatabaseName; }
        }

        public string LoggingDatabaseName
        {
            get { return _ermContext.LoggingDatabaseName; }
        }

        public string CrmDatabaseName
        {
            get { return _ermContext.CrmDatabaseName; }
        }

        public CrmDataContext CrmContext
        {
            get { return _crmContext.CrmContext; }
        }

        public void Dispose()
        {
            _ermContext.Dispose();
        }

        public long NewIdentity()
        {
            return IdentityGenerator.NewIdentity();
        }

        #region Identity Generator

        private static class IdentityGenerator
        {
            private static readonly BufferedIdentityProviderService IdentityRequest = new BufferedIdentityProviderService(new IdentityProviderService(50));

            public static long NewIdentity()
            {
                return IdentityRequest.GetIdentities(1)[0];
            }

            private sealed class BufferedIdentityProviderService
            {
                // Максимальное количество идентификаторов, которое можно запросить у сервиса генерации идентификаторов
                private const int MaxRequestCount = 32767;

                private readonly IdentityProviderService _service;
                private readonly Queue<long> _buffer = new Queue<long>(MaxRequestCount);
                private int _nextRequestCount = 42;

                public BufferedIdentityProviderService(IdentityProviderService service)
                {
                    _service = service;
                }

                public long[] GetIdentities(int count)
                {
                    EnsureCount(count);

                    var ids = new long[count];
                    for (var i = 0; i < ids.Length; i++)
                    {
                        ids[i] = _buffer.Dequeue();
                    }

                    return ids;
                }

                private void EnsureCount(int count)
                {
                    var bufferedCount = _buffer.Count;
                    if (bufferedCount >= count)
                    {
                        return;
                    }

                    var missingCount = count - bufferedCount;
                    var requestCount = Math.Max(_nextRequestCount, missingCount);

                    foreach (var id in _service.GetIdentities(requestCount))
                    {
                        _buffer.Enqueue(id);
                    }

                    _nextRequestCount = Math.Min(_nextRequestCount * 2, MaxRequestCount);
                }
            }

            /// <summary>
            /// Копипаст реализации из 2GIS.NuClear.IdentityService
            /// </summary>
            private sealed class IdentityProviderService
            {
                private const int TimeMaskLength = 41;
                private const int IdentityProviderMaskLength = 8;
                private const int SequenceMaskLength = 15;

                private const long TimeMask = -1L ^ (-1L << TimeMaskLength);
                private const long SequenceMask = -1L ^ (-1L << SequenceMaskLength);
                private const long IdentityProviderMask = -1L ^ (-1L << IdentityProviderMaskLength);

                private const long ErmEpochStart = 634925952000000000; // Эпоха ЕРМ началась 2013-01-01

                private readonly object _timeCheckSync = new object();

                private readonly int _identityServiceUniqueId;

                private long _previousTimestamp;
                private int _incrementedValue;

                public IdentityProviderService(int identityServiceUniqueId)
                {
                    _identityServiceUniqueId = identityServiceUniqueId;
                }

                public long[] GetIdentities(int count)
                {
                    if ((count & ~SequenceMask) != 0)
                    {
                        throw new ArgumentException(string.Format("Can not generate more than {0} ids at once", SequenceMask));
                    }

                    if ((_identityServiceUniqueId & ~IdentityProviderMask) != 0)
                    {
                        throw new ArgumentException(string.Format("Can not generate with id provider value greater than {0}", IdentityProviderMask));
                    }

                    var timestamp = GetTimestamp();
                    var startIndex = Interlocked.Add(ref _incrementedValue, count) - count;

                    if ((timestamp & ~TimeMask) != 0)
                    {
                        throw new ArgumentException(string.Format("Erm epoch has ended at {0}, you need to refactor entity identities",
                                                                  new DateTime(ErmEpochStart + (TimeMask * 10000))));
                    }

                    return Ids(timestamp, count, startIndex, _identityServiceUniqueId);
                }

                private static long GetCurrentTimestamp()
                {
                    var now = DateTime.UtcNow.Ticks;

                    // There are 10,000 ticks in a millisecond (http://msdn.microsoft.com/en-gb/library/system.datetime.ticks.aspx) 
                    return (now - ErmEpochStart) / 10000;
                }

                private static long[] Ids(long timestamp, int count, int startIndex, int identityProvider)
                {
                    var result = new long[count];

                    for (var i = 0; i < count; i++)
                    {
                        var sequence = (startIndex + i) & SequenceMask;
                        result[i] = timestamp << (SequenceMaskLength + IdentityProviderMaskLength);
                        result[i] |= sequence << IdentityProviderMaskLength;
                        result[i] |= (uint)identityProvider;
                    }

                    return result;
                }

                private static long WaitForTimeAfter(long lastTimestamp)
                {
                    var generatedTimestamp = GetCurrentTimestamp();
                    while (generatedTimestamp <= lastTimestamp)
                    {
                        Thread.Sleep(1);
                        generatedTimestamp = GetCurrentTimestamp();
                    }

                    return generatedTimestamp;
                }

                private long GetTimestamp()
                {
                    lock (_timeCheckSync)
                    {
                        var currentTimestamp = GetCurrentTimestamp();
                        if (currentTimestamp < _previousTimestamp)
                        {
                            throw new ArgumentException(string.Format("Clock value was moved back, id generation stopped for {0} milliseconds",
                                                                      (_previousTimestamp - currentTimestamp)));
                        }

                        if (currentTimestamp == _previousTimestamp)
                        {
                            currentTimestamp = WaitForTimeAfter(currentTimestamp);
                        }

                        _previousTimestamp = currentTimestamp;

                        return currentTimestamp;
                    }
                }
            }
        }

        #endregion
    }
}