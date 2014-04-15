namespace DoubleGis.Erm.Platform.Model.Zones.Infrastructure
{
    public interface IZonePart<TZone, TScope>
        where TZone : IZone
        where TScope : IZonePartScope
    {
    }

    public interface IZonePart<TZone, TScope, TBusinessModel>
        where TZone : IZone
        where TScope : IZonePartScope
    {
    }
}