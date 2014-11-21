using DoubleGis.Erm.Platform.Model.Zones.Infrastructure;

namespace DoubleGis.Erm.Platform.Model.Zones
{
    /// <summary>
    /// Декларирует принадлежность пространства имен, в котором имплементируется этот интерфейс, к зоне <see cref="TZone"/>
    /// </summary>
    public interface IZoneNamespace<TZone> : IZonePart<TZone, NamespaceZonePartScope>
        where TZone : IZone
    {
    }

    /// <summary>
    /// Декларирует принадлежность пространства имен, в котором имплементируется этот интерфейс, к зоне <see cref="TZone"/> и бизнес-модели <see cref="TBusinessModel"/>
    /// </summary>
    public interface IZoneNamespace<TZone, TBusinessModel> : IZonePart<TZone, NamespaceZonePartScope, TBusinessModel>
        where TZone : IZone
    {
    }
}