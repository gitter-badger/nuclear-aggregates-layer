using DoubleGis.Erm.Platform.Model.Zones.Infrastructure;

namespace DoubleGis.Erm.Platform.Model.Zones
{
    /// <summary>
    /// Декларирует принадлежность сборки, в которой имплементируется этот интерфейс, к зоне <see cref="TZone"/>
    /// </summary>
    public interface IZoneAssembly<TZone> : IZonePart<TZone, AssemblyZonePartScope>
        where TZone : IZone
    {
    }

    /// <summary>
    /// Декларирует принадлежность сборки, в которой имплементируется этот интерфейс, к зоне <see cref="TZone"/> и бизнес-модели <see cref="TBusinessModel"/>
    /// </summary>
    public interface IZoneAssembly<TZone, TBusinessModel> : IZonePart<TZone, AssemblyZonePartScope, TBusinessModel>
        where TZone : IZone
    {
    }
}