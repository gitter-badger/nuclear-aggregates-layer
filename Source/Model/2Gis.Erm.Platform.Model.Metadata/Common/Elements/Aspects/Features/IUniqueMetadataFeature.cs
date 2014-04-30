namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features
{
    /// <summary>
    /// конкретные тип feature, реалузующий данный интерфейс, является уникальным, эксклюзиным для элемента - т.е. у элемента может быть только одна фича такого типа
    /// </summary>
    public interface IUniqueMetadataFeature : IMetadataFeature
    {
    }
}