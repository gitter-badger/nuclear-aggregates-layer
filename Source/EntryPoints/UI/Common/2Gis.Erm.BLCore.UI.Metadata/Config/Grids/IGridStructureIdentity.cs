using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public interface IGridStructureIdentity : IConfigElementIdentity
    {
         EntityName EntityName { get; }
    }
}