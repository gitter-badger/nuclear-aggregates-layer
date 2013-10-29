namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures
{
    public sealed class HiddenFeature : IPropertyFeature, IDataFieldFeature
    {
        public EntityProperty TargetProperty { get; set; }
    }
}
