using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Validators
{
    public interface IMetadataValidator
    {
        bool IsValid(out string report);
    }

    public interface IMetadataValidator<TMetadataKindIdentity> : IMetadataValidator
        where TMetadataKindIdentity : class, IMetadataKindIdentity, new()
    {
    }
}
