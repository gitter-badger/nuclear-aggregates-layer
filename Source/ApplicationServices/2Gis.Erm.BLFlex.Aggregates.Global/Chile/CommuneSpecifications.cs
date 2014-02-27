using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile
{
    public static class CommuneSpecifications
    {
        public static IFindSpecification<DictionaryEntityInstance> FindOnlyCommunes()
        {
            return new FindSpecification<DictionaryEntityInstance>(entity => entity.DictionaryEntityPropertyInstances.Any(property =>
                property.PropertyId == EntityTypeNameIdentity.Instance.Id && property.NumericValue == (int)EntityName.Commune));
        }

        public static ISelectSpecification<DictionaryEntityInstance, Commune> Select()
        {
            return new SelectSpecification<DictionaryEntityInstance, Commune>(
                entity => new Commune
                {
                    Id = entity.Id,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = entity.CreatedOn,
                    ModifiedBy = entity.ModifiedBy,
                    ModifiedOn = entity.ModifiedOn,
                    IsActive = entity.IsActive,
                    IsDeleted = entity.IsDeleted,
                    Timestamp = entity.Timestamp,
                    Name = entity.DictionaryEntityPropertyInstances.FirstOrDefault(instance => instance.PropertyId == NameIdentity.Instance.Id).TextValue,
                });
        }
    }
}
