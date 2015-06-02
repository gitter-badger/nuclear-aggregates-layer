using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowNomenclaturesNomenclatureElementRelation : EntityTypeBase<EntityTypeExportFlowNomenclaturesNomenclatureElementRelation>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowNomenclaturesNomenclatureElementRelation; }
        }

        public override string Description
        {
            get { return "ExportFlowNomenclaturesNomenclatureElementRelation"; }
        }
    }
}