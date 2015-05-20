using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowNomenclaturesNomenclatureElement : EntityTypeBase<EntityTypeExportFlowNomenclaturesNomenclatureElement>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowNomenclaturesNomenclatureElement; }
        }

        public override string Description
        {
            get { return "ExportFlowNomenclaturesNomenclatureElement"; }
        }
    }
}