using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel
{
    public static class DictionaryEntitySpecs
    {
         public static class DictionaryEntity
         {
             public static class Find
             {
                 public static FindSpecification<DictionaryEntityInstance> ByEntityName(EntityName entityName)
                 {
                     return new FindSpecification<DictionaryEntityInstance>(
                         x => x.DictionaryEntityPropertyInstances.Any(y => y.PropertyId == EntityTypeNameIdentity.Instance.Id &&
                                                                           y.NumericValue == (decimal)entityName));
                 }
             }

             public static class Select
             {
                 public static ISelectSpecification<DictionaryEntityInstance, DictionaryEntityInstanceDto> DictionaryEntityInstanceDto()
                 {
                     return new SelectSpecification<DictionaryEntityInstance, DictionaryEntityInstanceDto>(x => new DictionaryEntityInstanceDto
                         {
                             DictionaryEntityInstance = x,
                             DictionaryEntityPropertyInstances = x.DictionaryEntityPropertyInstances
                         });
                 }
             }
         }
    }
}