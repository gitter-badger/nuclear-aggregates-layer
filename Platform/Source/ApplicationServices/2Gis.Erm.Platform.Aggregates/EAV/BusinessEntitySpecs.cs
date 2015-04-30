using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.EAV;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public static class BusinessEntitySpecs
    {
        public static class BusinessEntity
        {
            public static class Find
            {
                public static IFindSpecification<BusinessEntityInstance> ByReferencedEntities(IEnumerable<long> entityIds)
                {
                    return new FindSpecification<BusinessEntityInstance>(x => entityIds.Contains(x.EntityId.Value));
                }

                public static FindSpecification<BusinessEntityInstance> ByBusinessModel(BusinessModel businessModel)
                {
                    return new FindSpecification<BusinessEntityInstance>(
                        x => x.BusinessEntityPropertyInstances.Any(y => y.PropertyId == BusinessModelIdentity.Instance.Id &&
                                                                        y.NumericValue == (decimal)businessModel));
                }

                public static FindSpecification<BusinessEntityInstance> ByProperty(int propertyId, string propertyValue)
                {
                    return new FindSpecification<BusinessEntityInstance>(
                        x => x.BusinessEntityPropertyInstances.Any(y => y.PropertyId == propertyId &&
                                                                        y.TextValue == propertyValue));
                }
            }

            public static class Select
            {
                public static ISelectSpecification<BusinessEntityInstance, DynamicEntityInstanceDto<BusinessEntityInstance, BusinessEntityPropertyInstance>> DynamicEntityInstanceDto()
                {
                    return new SelectSpecification<BusinessEntityInstance, DynamicEntityInstanceDto<BusinessEntityInstance, BusinessEntityPropertyInstance>>(
                        x => new DynamicEntityInstanceDto<BusinessEntityInstance, BusinessEntityPropertyInstance>
                            {
                                EntityInstance = x,
                                PropertyInstances = x.BusinessEntityPropertyInstances
                            });
                }
            }
        }
    }
}