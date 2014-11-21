using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Zones;
using DoubleGis.Erm.Platform.Model.Zones.Infrastructure;

namespace DoubleGis.Erm.Platform.DI.Zones
{
    public static class ZoneUtils
    {
        public static bool IsZonePartMarker(this Type type, Type zoneType)
        {
            var zones = type.GetInterfaces()
                            .Where(IsZonePart)
                            .Select(i => i.GetGenericArguments()[0])
                            .Distinct()
                            .ToArray();

            return zones.Length == 1 && zones[0] == zoneType;
        }

        public static ZonePartInfo GetZonePartInfo(this Type type)
        {
            var interfaces = type.GetInterfaces();
            return interfaces.Where(i => i.IsGenericType)
                             .Select(i => new { Interface = i, Definition = i.GetGenericTypeDefinition(), GenericArguments = i.GetGenericArguments() })
                             .Where(x => x.Definition == typeof(IZonePart<,>) || x.Definition == typeof(IZonePart<,,>))
                             .Select(x => new ZonePartInfo
                                 {
                                     MarkerType = type,
                                     ZoneScope = x.GenericArguments[1],
                                     BusinessModel = x.GenericArguments.Length == 3 ? x.GenericArguments[2] : null,
                                     ContainedTypes = interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IContainsType<>))
                                                                .Select(i => i.GetGenericArguments()[0])
                                 })
                             .SingleOrDefault();
        }

        private static bool IsZonePart(Type interfaceType)
        {
            if (!interfaceType.IsGenericType)
            {
                return false;
            }

            var genericTypeDefinition = interfaceType.GetGenericTypeDefinition();
            return genericTypeDefinition == typeof(IZonePart<,>) || interfaceType.GetGenericTypeDefinition() == typeof(IZonePart<,,>);
        }
    }
}