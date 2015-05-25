using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class DepartmentObtainer : IBusinessModelEntityObtainer<Department>, IAggregateReadModel<User>
    {
        private readonly IFinder _finder;

        public DepartmentObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Department ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (DepartmentDomainEntityDto)domainEntityDto;

            var department = _finder.Find(Specs.Find.ById<Department>(dto.Id)).SingleOrDefault() ??
                             new Department { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && department.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            department.Name = dto.Name;
            department.ParentId = dto.ParentRef.Id;
            department.Timestamp = dto.Timestamp;

            return department;
        }
    }
}