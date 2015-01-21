using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeDictionaryEntityInstance : EntityTypeBase<EntityTypeDictionaryEntityInstance>
    {
        public override int Id
        {
            get { return EntityTypeIds.DictionaryEntityInstance; }
        }

        public override string Description
        {
            get { return "DictionaryEntityInstance"; }
        }
    }
}