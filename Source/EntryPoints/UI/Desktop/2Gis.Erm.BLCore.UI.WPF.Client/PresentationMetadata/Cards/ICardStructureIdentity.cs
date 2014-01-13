using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public interface ICardStructureIdentity : IConfigElementIdentity
    {
        EntityName EntityName { get; }
    }
}