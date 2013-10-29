namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles
{
    public interface ITitledElement : IConfigElementAspect
    {
        ITitleDescriptor TitleDescriptor { get; }
    }
}
