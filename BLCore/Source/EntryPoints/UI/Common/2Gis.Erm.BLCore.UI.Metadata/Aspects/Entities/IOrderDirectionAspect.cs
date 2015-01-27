using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IOrderDirectionAspect : IAspect
    {
        LookupField SourceOrganizationUnit { get; set; }
        LookupField DestinationOrganizationUnit { get; set; }
    }
}
