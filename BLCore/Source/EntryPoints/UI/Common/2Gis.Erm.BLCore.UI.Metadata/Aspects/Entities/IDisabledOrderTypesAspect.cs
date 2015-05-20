using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IDisabledOrderTypesAspect : IAspect
    {
        string DisabledOrderTypes { get; set; }
    }
}
