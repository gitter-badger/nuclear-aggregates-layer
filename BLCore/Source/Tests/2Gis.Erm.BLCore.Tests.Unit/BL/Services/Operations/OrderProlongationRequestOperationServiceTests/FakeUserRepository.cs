using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext.Profile;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using OrganizationUnitDto = DoubleGis.Erm.BLCore.API.Aggregates.Users.Dto.OrganizationUnitDto;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations.OrderProlongationRequestOperationServiceTests
{
    /// <summary>
    /// Приходится фэйкать, т.к. Moq не справляется с таким сложным интерфейсом - кидает эксепшн "Duplicate element: Castle.DynamicProxy.Generators.MetaMethod"
    /// Причнина ошибки - проект Castle.DynamicProxy, котороый интегрирован в нашу сборку Moq, не может обработать случай, когда требуется явная реализация трех и более одноимённых методов
    /// interface IFoo<T> { void Foo(); }
    /// interface IMustBeMocked : IFoo<A>, IFoo<B>, IFoo<C> { }
    /// </summary>
    public class FakeUserRepository : IUserRepository
    {
        public FakeUserRepository()
        {
            Users = new List<User>();
        }

        public List<User> Users { get; private set; }

        int IActivateAggregateRepository<Department>.Activate(long entityId)
        {
            throw new NotImplementedException();
        }

        int IActivateAggregateRepository<OrganizationUnit>.Activate(long entityId)
        {
            throw new NotImplementedException();
        }

        int IDeactivateAggregateRepository<OrganizationUnit>.Deactivate(long entityId)
        {
            throw new NotImplementedException();
        }

        int IDeactivateAggregateRepository<Territory>.Deactivate(long entityId)
        {
            throw new NotImplementedException();
        }

        int IDeactivateAggregateRepository<Department>.Deactivate(long entityId)
        {
            throw new NotImplementedException();
        }

        int IDeleteAggregateRepository<OrganizationUnit>.Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        int IDeleteAggregateRepository<UserOrganizationUnit>.Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        public int Activate(User user)
        {
            throw new NotImplementedException();
        }

        public int Activate(UserProfile userProfile)
        {
            throw new NotImplementedException();
        }

        public int Activate(Department department)
        {
            throw new NotImplementedException();
        }

        public int Activate(OrganizationUnit organizationUnit)
        {
            throw new NotImplementedException();
        }

        public int Deactivate(User user)
        {
            throw new NotImplementedException();
        }

        public int Deactivate(UserProfile userProfile)
        {
            throw new NotImplementedException();
        }

        public int Deactivate(OrganizationUnit organizationUnit)
        {
            throw new NotImplementedException();
        }

        public int Deactivate(Territory territory)
        {
            throw new NotImplementedException();
        }

        public int Deactivate(Department department)
        {
            throw new NotImplementedException();
        }

        public int Delete(OrganizationUnit organizationUnit)
        {
            throw new NotImplementedException();
        }

        public int Delete(UserRole userRole)
        {
            throw new NotImplementedException();
        }

        public int Delete(UserOrganizationUnit userOrganizationUnit)
        {
            throw new NotImplementedException();
        }

        public int Delete(UserTerritory userTerritory)
        {
            throw new NotImplementedException();
        }

        public int DeleteUserRole(long userId, long roleId)
        {
            throw new NotImplementedException();
        }

        public int DeleteUserOrganizationUnit(long userId, long organizationUnitId)
        {
            throw new NotImplementedException();
        }

        public int DeleteUserTerritory(long userId, long territoryId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetUsersByDepartments(IEnumerable<long> departmentIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetUsersByTerritory(long territoryId)
        {
            throw new NotImplementedException();
        }

        public OrganizationUnit GetOrganizationUnit(long orgUnitId)
        {
            throw new NotImplementedException();
        }

        public OrganizationUnitWithUsersDto GetOrganizationUnitDetails(long entityId)
        {
            throw new NotImplementedException();
        }

        public OrganizationUnitDto GetSingleOrDefaultOrganizationUnit(long userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetUsersByOrganizationUnit(long organizationUnitId)
        {
            throw new NotImplementedException();
        }

        public virtual User GetUser(long id)
        {
            return Users.FirstOrDefault(x => x.Id == id);
        }

        public virtual User GetUser(string account)
        {
            throw new NotImplementedException();
        }

        public virtual User FindAnyUserWithPrivelege(IEnumerable<long> organizationUnitId, FunctionalPrivilegeName privelege)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<long> GetUserTerritoryIds(long userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<long> GetUserOrganizationUnitsTerritoryIds(long userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<long> GetAllTerritoryIds()
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdate(User user)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdate(Department department)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdate(UserRole userRole)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdate(UserOrganizationUnit userOrganizationUnit)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdate(UserTerritory userTerritory)
        {
            throw new NotImplementedException();
        }

        public void DeleteUserTerritory(User user, long territoryId)
        {
            throw new NotImplementedException();
        }

        public UserProfile GetProfileForUser(long userCode)
        {
            throw new NotImplementedException();
        }

        public UserProfileDto[] GetAllUserProfiles()
        {
            throw new NotImplementedException();
        }

        public void UpdateUserProfiles(UserProfileDto[] userProfileDtos)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdate(UserProfile profile)
        {
            throw new NotImplementedException();
        }

        public LocaleInfo GetUserLocaleInfo(long userCode)
        {
            throw new NotImplementedException();
        }

        public bool IsUserLinkedWithOrganizationUnit(long userId, long organizationUnitId)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdate(Territory territory)
        {
            throw new NotImplementedException();
        }

        public Territory GetTerritory(long territoryId)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdate(OrganizationUnit organizationUnit)
        {
            throw new NotImplementedException();
        }

        public void AssignUserRelatedEntities(long userId, long newOwnerCode)
        {
            throw new NotImplementedException();
        }

        public bool TryGetSingleUserOrganizationUnit(long userId, out OrganizationUnit organizationUnit)
        {
            throw new NotImplementedException();
        }

        public OrganizationUnit GetFirstUserOrganizationUnit(long userId)
        {
            throw new NotImplementedException();
        }

        public void ChangeUserTerritory(IEnumerable<User> users, long oldTerritoryId, long newTerritoryId)
        {
            throw new NotImplementedException();
        }

        public int AddTerritory(IEnumerable<User> users, long territoryId)
        {
            throw new NotImplementedException();
        }
    }
}
