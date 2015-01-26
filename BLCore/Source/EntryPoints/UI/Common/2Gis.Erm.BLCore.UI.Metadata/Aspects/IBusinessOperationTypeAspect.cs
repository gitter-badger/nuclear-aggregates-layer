using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects
{
    public interface IBusinessOperationTypeAspect : IAspect
    {
        BusinessOperation Type { get; }
    }
}