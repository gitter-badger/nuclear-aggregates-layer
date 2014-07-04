using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public static class NestSettingsAspectUtils
    {
        public static ICollection<ISettingsAspect> UseElasticClientNestSettingsAspect(this ICollection<ISettingsAspect> aspects)
        {
            var connectionStringsAspect = aspects.OfType<ConnectionStringsSettingsAspect>().Single();
            var connectionString = connectionStringsAspect.GetConnectionString(ConnectionStringName.ErmSearch);
            aspects.Add(new NestSettingsAspect(connectionString));

            return aspects;
        }
    }
}