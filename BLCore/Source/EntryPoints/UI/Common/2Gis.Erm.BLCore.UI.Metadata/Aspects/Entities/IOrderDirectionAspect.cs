using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IOrderDirectionAspect : IAspect
    {
        long? SourceOrganizationUnitKey { get; }
        long? DestinationOrganizationUnitKey { get; }

        string SourceOrganizationUnitValue { get; }
        string DestinationOrganizationUnitValue { get; }
    }
}
