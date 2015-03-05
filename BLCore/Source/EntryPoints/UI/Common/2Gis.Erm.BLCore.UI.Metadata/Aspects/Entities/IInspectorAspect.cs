using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IInspectorAspect : IAspect
    {
        long? InspectorKey { get; }
        string InspectorValue { get; set; }
    }
}
