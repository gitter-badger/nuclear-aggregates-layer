using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class UserObtainer : IBusinessModelEntityObtainer<User>, IAggregateReadModel<User>
    {
        private readonly IFinder _finder;

        public UserObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public User ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (UserDomainEntityDto)domainEntityDto;

            var user = _finder.Find(Specs.Find.ById<User>(dto.Id)).One() ??
                       new User { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && user.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            user.Account = dto.Account;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.DisplayName = dto.DisplayName;
            user.DepartmentId = dto.DepartmentRef.Id.Value;
            user.ParentId = dto.ParentRef.Id;
            user.IsServiceUser = dto.IsServiceUser;
            user.Timestamp = dto.Timestamp;

            return user;
        }
    }
}