using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.Model.Zones.Infrastructure;

namespace DoubleGis.Erm.Platform.DI.Zones
{
    public static class ZonePartTypesExtractor
    {
        private static readonly IReadOnlyDictionary<Type, Func<Type, IEnumerable<Type>>> Extractors = new Dictionary<Type, Func<Type, IEnumerable<Type>>>
            {
                { typeof(AssemblyZonePartScope), AssemblyScope },
                { typeof(NamespaceZonePartScope), NamespaceScope },
                { typeof(MemberZonePartScope), MemberScope }
            };

        public static IDictionary<Type, IEnumerable<Type>> ExtractTypesMap(this ZonePartInfo zonePartInfo, Type businessModelIndicator)
        {
            var types = Extractors[zonePartInfo.ZoneScope](zonePartInfo.MarkerType);

            return zonePartInfo.ContainedTypes
                               .Select(x => new
                                   {
                                       AssignableType = x,
                                       Types = types.Where(t => x.IsAssignableFrom(t) && (!t.IsAdaptedType() || businessModelIndicator.IsAssignableFrom(t)))
                                   })
                               .GroupBy(x => x.AssignableType)
                               .ToDictionary(x => x.Key, x => x.SelectMany(y => y.Types));
        }

        #region Extractors

        private static IEnumerable<Type> MemberScope(Type memberType)
        {
            return new[] { memberType };
        }

        private static IEnumerable<Type> NamespaceScope(Type memberType)
        {
            return memberType.Assembly.ExportedTypes.Where(x => x.Namespace.StartsWith(memberType.Namespace));
        }

        private static IEnumerable<Type> AssemblyScope(Type memberType)
        {
            return memberType.Assembly.ExportedTypes;
        }

        #endregion
    }
}