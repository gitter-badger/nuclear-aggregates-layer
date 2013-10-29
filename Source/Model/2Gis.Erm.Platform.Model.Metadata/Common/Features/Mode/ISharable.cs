namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Mode
{
    public interface ISharable : IConfigElementAspect
    {
        bool IsShared { get; }
    }
}
