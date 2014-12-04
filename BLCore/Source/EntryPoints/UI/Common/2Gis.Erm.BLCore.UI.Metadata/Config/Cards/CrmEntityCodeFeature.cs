using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public class CrmEntityCodeFeature : ICardFeature
    {
        public CrmEntityCodeFeature(CrmEntity crmEntity)
        {
            CrmEntity = crmEntity;
        }

        public CrmEntity CrmEntity { get; private set; }
    }
}