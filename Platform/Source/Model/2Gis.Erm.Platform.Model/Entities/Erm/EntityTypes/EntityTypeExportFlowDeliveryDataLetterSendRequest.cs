using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowDeliveryDataLetterSendRequest : EntityTypeBase<EntityTypeExportFlowDeliveryDataLetterSendRequest>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowDeliveryDataLetterSendRequest; }
        }

        public override string Description
        {
            get { return "ExportFlowDeliveryDataLetterSendRequest"; }
        }
    }
}