using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel.Communes
{
    // FIXME {all, 10.04.2014}: при рефакторинге EAV постяраться обобщить такие fake спецификации (BankSpecs, CommuneSpecs) - используя либо обобщенный, либо вообще метаданные в "finder" и обходясь без таких спецификаций
    public static class CommuneSpecs
    {
        public static class Find
        {
            public static IFindSpecification<DictionaryEntityInstance> OnlyCommunes
            {
                get
                {
                    var communeTypeId = EntityType.Instance.Commune().Id;
                    return new FindSpecification<DictionaryEntityInstance>(
                        entity => entity.DictionaryEntityPropertyInstances.Any(property =>
                                                                               property.PropertyId == IdentityBase<EntityTypeNameIdentity>.Instance.Id &&
                                                                               property.NumericValue == communeTypeId));
                }
            }
        }

        public static class Select 
        {
            public static ISelectSpecification<DictionaryEntityInstance, Commune> Communes
            {
                get
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
                                Name = entity.DictionaryEntityPropertyInstances
                                             .FirstOrDefault(instance => instance.PropertyId == NameIdentity.Instance.Id).TextValue,
                            });
                }
            }
        }
    }
}
