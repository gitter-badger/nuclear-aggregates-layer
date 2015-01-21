using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePrintFormTemplate : EntityTypeBase<EntityTypePrintFormTemplate>
    {
        public override int Id
        {
            get { return EntityTypeIds.PrintFormTemplate; }
        }

        public override string Description
        {
            get { return "PrintFormTemplate"; }
        }
    }
}