using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.Platform.Model.Aspects
{
    public interface IBusinessOperationTypeAspect : IAspect
    {
        BusinessOperation Type { get; }
    }
}