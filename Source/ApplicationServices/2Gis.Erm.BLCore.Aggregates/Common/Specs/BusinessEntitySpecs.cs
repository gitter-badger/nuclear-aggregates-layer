using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Specs
{
    public static class BusinessEntitySpecs
    {
        public static class BusinessEntity
        {
            public static class Find
            {
                public static FindSpecification<BusinessEntityInstance> ByReferencedEntity(long referencedEntityId)
                {
                    return new FindSpecification<BusinessEntityInstance>(x => x.EntityId == referencedEntityId);
                }

                public static FindSpecification<BusinessEntityInstance> ByBusinessModel(BusinessModel businessModel)
                {
                    return new FindSpecification<BusinessEntityInstance>(
                        x => x.BusinessEntityPropertyInstances.Any(y => y.PropertyId == BusinessModelIdentity.Instance.Id &&
                                                                        y.NumericValue == (decimal)businessModel));
                }
            }

            public static class Select
            {
                public static ISelectSpecification<BusinessEntityInstance, BusinessEntityInstanceDto> BusinessEntityInstanceDto()
                {
                    return new SelectSpecification<BusinessEntityInstance, BusinessEntityInstanceDto>(x => new BusinessEntityInstanceDto
                        {
                            EntityInstance = x,
                            PropertyInstances = x.BusinessEntityPropertyInstances
                        });
                }
            }
        }
    }
}