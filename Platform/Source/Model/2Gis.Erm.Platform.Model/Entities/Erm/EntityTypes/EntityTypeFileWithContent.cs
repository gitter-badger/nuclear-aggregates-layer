using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeFileWithContent : EntityTypeBase<EntityTypeFileWithContent>
    {
        public override int Id
        {
            get { return EntityTypeIds.FileWithContent; }
        }

        public override string Description
        {
            get { return "FileWithContent"; }
        }
    }
}