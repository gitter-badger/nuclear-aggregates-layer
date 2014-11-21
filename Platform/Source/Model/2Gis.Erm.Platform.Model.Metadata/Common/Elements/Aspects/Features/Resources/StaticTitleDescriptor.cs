namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources
{
    public sealed class StaticResourceDescriptor : IResourceDescriptor
    {
        public StaticResourceDescriptor(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}