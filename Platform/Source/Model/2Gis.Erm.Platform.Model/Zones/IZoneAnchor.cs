namespace DoubleGis.Erm.Platform.Model.Zones
{
    /// <summary>
    /// Декларирует наличие в сборке, в которой имплементируется этот интерфейс, какого-либо функционала, связанного с зоной <see cref="TZone"/>
    /// Используется на стороне конкретных composition root'ов для явного указания сборок, которые должны быть в AppDomain
    /// </summary>
    public interface IZoneAnchor<TZone>
        where TZone : IZone
    {
    }
}