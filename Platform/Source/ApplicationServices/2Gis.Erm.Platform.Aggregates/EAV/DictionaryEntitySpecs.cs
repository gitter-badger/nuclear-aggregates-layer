﻿using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.EAV;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Properties;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
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

                 public static FindSpecification<DictionaryEntityInstance> ByPropertyValue(IEntityPropertyIdentity identity, long entityId)
                 {
                     return new FindSpecification<DictionaryEntityInstance>(
                         x => x.DictionaryEntityPropertyInstances.Any(y => y.PropertyId == identity.Id && y.NumericValue == entityId));
                 }
             }

             public static class Select
             {
                public static ISelectSpecification<DictionaryEntityInstance, DynamicEntityInstanceDto<DictionaryEntityInstance, DictionaryEntityPropertyInstance>> DynamicEntityInstanceDto()
                {
                    return new SelectSpecification<DictionaryEntityInstance, DynamicEntityInstanceDto<DictionaryEntityInstance, DictionaryEntityPropertyInstance>>(
                        x => new DynamicEntityInstanceDto<DictionaryEntityInstance, DictionaryEntityPropertyInstance>
                            {
                                EntityInstance = x,
                                PropertyInstances = x.DictionaryEntityPropertyInstances
                            });
                }

                 public static ISelectSpecification<DictionaryEntityInstance, string> Name()
                 {
                     return new SelectSpecification<DictionaryEntityInstance, string>(x => x.DictionaryEntityPropertyInstances
                                                                                            .Where(y => y.PropertyId == NameIdentity.Instance.Id)
                                                                                            .Select(y => y.TextValue)
                                                                                            .FirstOrDefault());
                 }
             }
         }
    }
}