using System;
using System.Collections.Generic;
using System.IO;

using DoubleGis.Erm.Platform.Common.Identities;
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
			private static readonly BufferedIdentityProviderService IdentityRequest = new BufferedIdentityProviderService(new IdentityProviderService(new IdentityServiceUniqueIdProvider()));

			public static long NewIdentity()
			{
				return IdentityRequest.GetIdentities(1)[0];
			}

			private class IdentityServiceUniqueIdProvider : IIdentityServiceUniqueIdProvider
			{
				// NOTE: it's reserved especially for Activity migration from CRM to ERM
				private const byte CrmMigrationServiceId = 50;

			    public byte GetUniqueId()
			    {
			        return CrmMigrationServiceId;
			    }
			}

			private sealed class BufferedIdentityProviderService : IIdentityProviderService
			{
				// Максимальное количество идентификаторов, которое можно запросить у сервиса генерации идентификаторов
				private const int MaxRequestCount = 32767;

				private readonly IIdentityProviderService _service;
				private readonly Queue<long> _buffer = new Queue<long>(MaxRequestCount);
				private int _nextRequestCount = 42;

				public BufferedIdentityProviderService(IIdentityProviderService service)
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
					if (bufferedCount >= count) return;

					var missingCount = count - bufferedCount;
					var requestCount = Math.Max(_nextRequestCount, missingCount);

					foreach (var id in _service.GetIdentities(requestCount))
					{
						_buffer.Enqueue(id);
					}

					_nextRequestCount = Math.Min(_nextRequestCount * 2, MaxRequestCount);
				}
			}
		}

		#endregion
	}
}