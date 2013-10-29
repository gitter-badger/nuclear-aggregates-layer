namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class OnlyValuePropertyFeature<TValue> : IPropertyFeature
    {
        public OnlyValuePropertyFeature(TValue value)
        {
            Value = value;
        }

        public TValue Value
        {
            get;
            private set;
        }

        public EntityProperty TargetProperty { get; set; }
    }
}
