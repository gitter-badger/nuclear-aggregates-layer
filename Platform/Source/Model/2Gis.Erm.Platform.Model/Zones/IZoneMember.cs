using DoubleGis.Erm.Platform.Model.Zones.Infrastructure;

namespace DoubleGis.Erm.Platform.Model.Zones
{
    /// <summary>
    /// Декларирует принадлежность типа, имплементирующего этот интерфейс, к зоне <see cref="TZone"/>
    /// </summary>
    public interface IZoneMember<TZone> : IZonePart<TZone, MemberZonePartScope>
        where TZone : IZone
    {
    }

    /// <summary>
    /// Декларирует принадлежность типа, имплементирующего этот интерфейс, к зоне <see cref="TZone"/> и бизнес-модели <see cref="TBusinessModel"/>
    /// </summary>
    public interface IZoneMember<TZone, TBusinessModel> : IZonePart<TZone, MemberZonePartScope, TBusinessModel>
        where TZone : IZone
    {
    }
}