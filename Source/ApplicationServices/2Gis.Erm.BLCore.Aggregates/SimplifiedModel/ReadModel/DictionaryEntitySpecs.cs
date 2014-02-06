using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel
{
    public static class DictionaryEntitySpecs
    {
         public static class DictionaryEntity
         {
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