using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Xrm.Client.Data.Services;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Security.API.UserContext.Profile;
using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

using OrganizationUnitDto = DoubleGis.Erm.BLCore.API.Aggregates.Users.Dto.OrganizationUnitDto;
using TimeZone = DoubleGis.Erm.Platform.Model.Entities.Security.TimeZone;

namespace DoubleGis.Erm.BLCore.Aggregates.Users
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IUserPersistenceService _userPersistenceService;

        private readonly IFinder _finder;

        private readonly IQuery _query;
        private readonly IRepository<User> _userGenericRepository;
        private readonly IRepository<UserRole> _userRoleGenericRepository;
        private readonly IRepository<UserOrganizationUnit> _userOrganizationUnitGenericRepository;
        private readonly IRepository<UserTerritory> _userTerritoryGenericRepository;
        private readonly IRepository<Department> _departmentGenericRepository;
        private readonly IRepository<UserProfile> _userProfileGenericRepository;

        private readonly IRepository<OrganizationUnit> _organizationUnitGenericRepository;
        private readonly IRepository<Territory> _territoryGenericRepository;

        private readonly IRepository<Client> _clientGenericRepository;
        private readonly IRepository<Firm> _firmGenericRepository;
        private readonly IRepository<Deal> _dealGenericRepository;
        private readonly IRepository<LegalPerson> _legalPersonGenericRepository;
        private readonly IRepository<LegalPersonProfile> _legalPersonProfileGenericRepository;
        private readonly IRepository<Bargain> _bargainGenericRepository;
        private readonly IRepository<Contact> _contactGenericRepository;
        private readonly IRepository<Order> _orderGenericRepository;
        private readonly IRepository<OrderPosition> _orderPositionGenericRepository;
        private readonly IRepository<Account> _accountGenericRepository;
        private readonly IRepository<Limit> _limitGenericRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public UserRepository(
            IQuery query,
            IRepository<User> userGenericRepository,
            IRepository<UserRole> userRoleGenericRepository,
            IRepository<OrganizationUnit> organizationUnitGenericRepository,
            IRepository<LegalPersonProfile> legalPersonProfileGenericRepository,
            IRepository<UserOrganizationUnit> userOrganizationUnitGenericRepository,
            IRepository<Territory> territoryGenericRepository,
            IRepository<Department> departmentGenericRepository,
            IRepository<UserTerritory> userTerritoryGenericRepository,
            IMsCrmSettings msCrmSettings,
            IRepository<UserProfile> userProfileGenericRepository,
            IFinder finder,
            IRepository<Client> clientGenericRepository,
            IRepository<Firm> firmGenericRepository,
            IRepository<Deal> dealGenericRepository,
            IRepository<LegalPerson> legalPersonGenericRepository,
            IRepository<Bargain> bargainGenericRepository,
            IRepository<Contact> contactGenericRepository,
            IRepository<Order> orderGenericRepository,
            IRepository<OrderPosition> orderPositionGenericRepository,
            IRepository<Account> accountGenericRepository,
            IRepository<Limit> limitGenericRepository,
            IUserPersistenceService userPersistenceService,
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory)
        {
            _query = query;
            _userGenericRepository = userGenericRepository;
            _userRoleGenericRepository = userRoleGenericRepository;
            _organizationUnitGenericRepository = organizationUnitGenericRepository;
            _territoryGenericRepository = territoryGenericRepository;
            _userOrganizationUnitGenericRepository = userOrganizationUnitGenericRepository;
            _departmentGenericRepository = departmentGenericRepository;
            _userTerritoryGenericRepository = userTerritoryGenericRepository;
            _msCrmSettings = msCrmSettings;
            _userProfileGenericRepository = userProfileGenericRepository;
            _finder = finder;
            _clientGenericRepository = clientGenericRepository;
            _firmGenericRepository = firmGenericRepository;
            _dealGenericRepository = dealGenericRepository;
            _legalPersonGenericRepository = legalPersonGenericRepository;
            _bargainGenericRepository = bargainGenericRepository;
            _contactGenericRepository = contactGenericRepository;
            _orderGenericRepository = orderGenericRepository;
            _orderPositionGenericRepository = orderPositionGenericRepository;
            _accountGenericRepository = accountGenericRepository;
            _limitGenericRepository = limitGenericRepository;
            _userPersistenceService = userPersistenceService;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
            _legalPersonProfileGenericRepository = legalPersonProfileGenericRepository;
        }

        public int Activate(Department department)
        {
            var childDepartments =
                _finder
                    .Find(Specs.Find.ActiveAndNotDeleted<Department>() 
                                && UserSpecs.Departments.Find.ChildrensOf(department))
                    .Many();

            var departmentIds = childDepartments.Select(x => x.Id).ToList();
            departmentIds.Add(department.Id);

            var userInfos = _finder
                .Find(Specs.Find.InactiveAndNotDeletedEntities<User>() && UserSpecs.Users.Find.ByDepartments(departmentIds))
                .Many();

            var count = 0;

            // Активировать неактивных пользователей
            using (var scope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, User>())
            {
                foreach (var user in userInfos)
                {
                    user.IsActive = true;
                    _userGenericRepository.Update(user);
                    scope.Updated<User>(user.Id);
                }

                count += _userGenericRepository.Save();
                scope.Complete();
            }

            // Активировать неактивные дочерние подразделения
            using (var scope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, Department>())
            {
                foreach (var childDepartment in childDepartments)
                {
                    childDepartment.IsActive = true;
                    _departmentGenericRepository.Update(childDepartment);
                    scope.Updated<Department>(childDepartment.Id);
                }

                // Активировать подразделение
                department.IsActive = true;
                _departmentGenericRepository.Update(department);
                scope.Updated<Department>(department.Id);

                count += _departmentGenericRepository.Save();
                scope.Complete();
            }

            return count;
        }

        public int Activate(OrganizationUnit organizationUnit)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, OrganizationUnit>())
            {
                organizationUnit.IsActive = true;
                _organizationUnitGenericRepository.Update(organizationUnit);
                var cnt = _organizationUnitGenericRepository.Save();

                scope.Updated(organizationUnit)
                     .Complete();

                return cnt;
        }
        }

        // FIXME {all, 23.12.2014}: при конвертации в OperationService + набор AggregateService, учесть наличие ещё и связанных действий см. метод   
        public void AssignUserRelatedEntities(long userId, long newOwnerCode)
        {
            var clients = _finder.Find(Specs.Find.Owned<Client>(userId)).Many();
            var firms = _finder.Find(Specs.Find.Owned<Firm>(userId)).Many();
            var deals = _finder.Find(Specs.Find.Owned<Deal>(userId) && Specs.Find.ActiveAndNotDeleted<Deal>()).Many();
            var legalPersons = _finder.Find(Specs.Find.Owned<LegalPerson>(userId)).Many();
            var legalPersonProfiles = _finder.Find(Specs.Find.Owned<LegalPersonProfile>(userId)).Many();
            var accounts = _finder.Find(Specs.Find.Owned<Account>(userId)).Many();
            var limits = _finder.Find(Specs.Find.Owned<Limit>(userId) && Specs.Find.ActiveAndNotDeleted<Limit>()).Many();
            var bargains = _finder.Find(Specs.Find.Owned<Bargain>(userId)).Many();
            var contacts = _finder.Find(Specs.Find.Owned<Contact>(userId)).Many();

            // Критерии поиска заказов учитываются при экспорте. Если меняешь - меняй и там.
            var ordersWithPositions = _finder.Find(Specs.Find.Owned<Order>(userId) &&
                                                   Specs.Find.ActiveAndNotDeleted<Order>() &&
                                                   OrderSpecs.Orders.Find.NotInArchive())
                                             .Map(q => q.Select(o => new { Order = o, o.OrderPositions }))
                                             .Many();

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Client>())
            {
                foreach (var client in clients)
                {
                    client.OwnerCode = newOwnerCode;
                    _clientGenericRepository.Update(client);
                    operationScope.Updated<Client>(client.Id);
                }

                _clientGenericRepository.Save();
                operationScope.Complete();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Firm>())
            {
                foreach (var firm in firms)
                {
                    firm.OwnerCode = newOwnerCode;
                    _firmGenericRepository.Update(firm);
                    operationScope.Updated<Firm>(firm.Id);
                }

                _firmGenericRepository.Save();
                operationScope.Complete();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Deal>())
            {
                foreach (var deal in deals)
                {
                    deal.OwnerCode = newOwnerCode;
                    _dealGenericRepository.Update(deal);
                    operationScope.Updated<Deal>(deal.Id);
                }

                _dealGenericRepository.Save();
                operationScope.Complete();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, LegalPerson>())
            {
                foreach (var legalPerson in legalPersons)
                {
                    legalPerson.OwnerCode = newOwnerCode;
                    _legalPersonGenericRepository.Update(legalPerson);
                    operationScope.Updated<LegalPerson>(legalPerson.Id);
                }

                _legalPersonGenericRepository.Save();
                operationScope.Complete();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, LegalPersonProfile>())
            {
                foreach (var legalPersonProfile in legalPersonProfiles)
                {
                    legalPersonProfile.OwnerCode = newOwnerCode;
                    _legalPersonProfileGenericRepository.Update(legalPersonProfile);
                    operationScope.Updated<LegalPersonProfile>(legalPersonProfile.Id);
                }

                _legalPersonProfileGenericRepository.Save();
                operationScope.Complete();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Account>())
            {
                foreach (var account in accounts)
                {
                    account.OwnerCode = newOwnerCode;
                    _accountGenericRepository.Update(account);
                    operationScope.Updated<Account>(account.Id);
                }

                _accountGenericRepository.Save();
                operationScope.Complete();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Limit>())
            {
                foreach (var limit in limits)
                {
                    limit.OwnerCode = newOwnerCode;
                    _limitGenericRepository.Update(limit);
                    operationScope.Updated<Limit>(limit.Id);
                }

                _limitGenericRepository.Save();
                operationScope.Complete();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Bargain>())
            {
                foreach (var bargain in bargains)
                {
                    bargain.OwnerCode = newOwnerCode;
                    _bargainGenericRepository.Update(bargain);
                    operationScope.Updated<Bargain>(bargain.Id);
                }

                _bargainGenericRepository.Save();
                operationScope.Complete();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Contact>())
            {
                foreach (var contact in contacts)
                {
                    contact.OwnerCode = newOwnerCode;
                    _contactGenericRepository.Update(contact);
                    operationScope.Updated<Contact>(contact.Id);
                }

                _contactGenericRepository.Save();
                operationScope.Complete();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Order>())
            {
                foreach (var orderWithPositions in ordersWithPositions)
                {
                    foreach (var orderPosition in orderWithPositions.OrderPositions)
                    {
                        orderPosition.OwnerCode = newOwnerCode;
                        _orderPositionGenericRepository.Update(orderPosition);
                        operationScope.Updated<OrderPosition>(orderPosition.Id);
                    }

                    orderWithPositions.Order.OwnerCode = newOwnerCode;
                    _orderGenericRepository.Update(orderWithPositions.Order);
                    operationScope.Updated<Order>(orderWithPositions.Order.Id);
                }

                _orderGenericRepository.Save();
                _orderPositionGenericRepository.Save();
                operationScope.Complete();
            }
        }

        public bool TryGetSingleUserOrganizationUnit(long userId, out OrganizationUnit organizationUnit)
        {
            organizationUnit = _finder.Find(new FindSpecification<UserTerritoriesOrganizationUnits>(x => x.UserId == userId)).Fold(q => q.Select(x => x.OrganizationUnitId).Distinct().Count()) == 1
                                   ? _finder.FindObsolete(new FindSpecification<UserTerritoriesOrganizationUnits>(x => x.UserId == userId)).Select(x => x.OrganizationUnit).First()
                                   : null;

            return organizationUnit != null;
        }

        public OrganizationUnit GetFirstUserOrganizationUnit(long userId)
        {
            var organizationUnits = _finder.Find(new FindSpecification<UserOrganizationUnit>(unit => unit.UserId == userId))
                                           .Map(q => q.Select(unit => unit.OrganizationUnitId))
                                           .Many();

            return _finder.Find(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() &&
                                new FindSpecification<OrganizationUnit>(unit => unit.ErmLaunchDate != null && organizationUnits.Contains(unit.Id)))
                          .Top();
        }

        public int Deactivate(OrganizationUnit organizationUnit)
        {
            var isLinkedWithUsers = _finder.Find(new FindSpecification<UserOrganizationUnit>(x => x.OrganizationUnitId == organizationUnit.Id)).Any();
            if (isLinkedWithUsers)
            {
                throw new ArgumentException(BLResources.OrgUnitLinkedWithActiveUsers);
            }

            using (var scope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, OrganizationUnit>())
            {
                organizationUnit.IsActive = false;
                _organizationUnitGenericRepository.Update(organizationUnit);
                var cnt = _organizationUnitGenericRepository.Save();

                scope.Updated(organizationUnit)
                     .Complete();

                return cnt;
            }

        }

        public int Deactivate(Territory territory)
        {
            var isLinkedWithUsers = _finder.Find(new FindSpecification<UserTerritoriesOrganizationUnits>(x => x.TerritoryId == territory.Id)).Any();
            if (isLinkedWithUsers)
            {
                throw new ArgumentException(BLResources.TerritoryLinkedWithActiveUser);
            }

            var count = 0;

            using (var scope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, Territory>())
            {
                territory.IsActive = false;
                _territoryGenericRepository.Update(territory);
                scope.Updated<Territory>(territory.Id);
                count += _territoryGenericRepository.Save();

                scope.Complete();
            }

            return count;
        }

        public int Deactivate(Department department)
        {
            var hasChildDepartments = _finder.Find(Specs.Find.ActiveAndNotDeleted<Department>() && UserSpecs.Departments.Find.ChildrensOf(department)).Any();
            if (hasChildDepartments)
            {
                throw new ArgumentException(BLResources.CannotDeactivateOrgUnitWithChildren);
            }

            var userInfos = _finder.Find(Specs.Find.ActiveAndNotDeleted<User>() && UserSpecs.Users.Find.ByDepartment(department.Id))
                                   .Map(q => q.Select(x => new
                                       {
                                           User = x,
                                           x.UserRoles
                                       }))
                                   .Many();

            var count = 0;
            // Удалить привязку ролей у пользователей
            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, UserRole>())
            {
                foreach (var userRole in userInfos.SelectMany(x => x.UserRoles))
                {
                    _userRoleGenericRepository.Delete(userRole);
                    scope.Deleted(userRole);
                }

                count += _userRoleGenericRepository.Save();
                scope.Complete();
            }

            // Деактивировать активных пользователей
            using (var scope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, User>())
            {
                foreach (var userInfo in userInfos)
                {
                    var user = userInfo.User;

                    user.IsActive = false;
                    _userGenericRepository.Update(user);
                    scope.Updated(user);
                }

                count += _userGenericRepository.Save();
                scope.Complete();
            }

            // Деактивировать подразделение
            using (var scope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, Department>())
            {
                department.IsActive = false;
                _departmentGenericRepository.Update(department);
                scope.Updated(department);

                count += _departmentGenericRepository.Save();

                scope.Complete();

            return count;
        }
        }

        public int Delete(OrganizationUnit organizationUnit)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, OrganizationUnit>())
            {
                _organizationUnitGenericRepository.Delete(organizationUnit);
                var cnt = _organizationUnitGenericRepository.Save();

                scope.Deleted(organizationUnit)
                     .Complete();

                return cnt;
            }
        }

        public int Delete(UserOrganizationUnit userOrganizationUnit)
        {
            _userOrganizationUnitGenericRepository.Delete(userOrganizationUnit);
            return _userOrganizationUnitGenericRepository.Save();
        }

        public int DeleteUserRole(long userId, long roleId)
        {
            var userRole = _finder.Find(new FindSpecification<UserRole>(x => x.UserId == userId && x.RoleId == roleId)).One();
            if (userRole == null)
            {
                throw new NotificationException(BLResources.UserRoleNotFound);
            }

            return Delete(userRole);
        }

        public int DeleteUserOrganizationUnit(long userId, long organizationUnitId)
        {
            var userOrganizationUnit =
                _finder.Find(new FindSpecification<UserOrganizationUnit>(x => x.UserId == userId && x.OrganizationUnitId == organizationUnitId)).One();
            if (userOrganizationUnit == null)
            {
                throw new NotificationException(BLResources.UserOrgUnitNotFound);
            }

            return Delete(userOrganizationUnit);
        }

        public int Delete(UserRole userRole)
        {
            _userRoleGenericRepository.Delete(userRole);

            var isServiceUser = _finder.FindObsolete(new FindSpecification<User>(x => x.Id == userRole.UserId)).Select(x => x.IsServiceUser).Single();
            if (!isServiceUser)
            {
                DeleteRoleFromCrm(userRole);
            }

            return _userRoleGenericRepository.Save();
        }

        public OrganizationUnit GetOrganizationUnit(long orgUnitId)
        {
            return _finder.Find(Specs.Find.ById<OrganizationUnit>(orgUnitId)).One();
        }

        public OrganizationUnitWithUsersDto GetOrganizationUnitDetails(long entityId)
        {
            return _finder.Find(Specs.Find.ById<OrganizationUnit>(entityId))
                          .Map(q => q.Select(unit => new OrganizationUnitWithUsersDto
                              {
                                  Unit = unit,
                                  HasLinkedUsers = unit.UserTerritoriesOrganizationUnits.Any()
                              }))
                          .One();
        }

        public void CreateOrUpdate(UserOrganizationUnit userOrganizationUnit)
        {
            var organizationUnitExist = _finder.Find(new FindSpecification<UserOrganizationUnit>(
                                                         x => x.UserId == userOrganizationUnit.UserId &&
                                                              x.OrganizationUnitId == userOrganizationUnit.OrganizationUnitId))
                                               .Any();
            if (organizationUnitExist)
            {
                throw new ArgumentException(BLResources.EditUserOrganizationUnitHandler_WarningOrganizationUnitAlreadyExists);
            }

            _identityProvider.SetFor(userOrganizationUnit);
            _userOrganizationUnitGenericRepository.Add(userOrganizationUnit);
            _userOrganizationUnitGenericRepository.Save();
        }

        public void CreateOrUpdate(UserTerritory userTerritory)
        {
            var territoryExist = _finder.Find(new FindSpecification<UserTerritory>(x => x.UserId == userTerritory.UserId &&
                                                                                        x.TerritoryId == userTerritory.TerritoryId && !x.IsDeleted))
                                        .Any();
            if (territoryExist)
            {
                throw new ArgumentException(BLResources.EditUserTerritoryHandler_WarningUserTerritoryAlreadyExists);
            }

            var userTerritoryBelongsToUserOrganizationUnits = _finder.FindObsolete(Specs.Find.ById<User>(userTerritory.UserId))
                .SelectMany(x => x.UserOrganizationUnits)
                .Select(x => x.OrganizationUnitDto)
                .SelectMany(x => x.Territories)
                .Select(x => x.Id)
                .Distinct()
                .Contains(userTerritory.TerritoryId);

            if (!userTerritoryBelongsToUserOrganizationUnits)
            {
                throw new ArgumentException(BLResources.EditUserTerritoryHandler_WarningWrongTerritory);
            }

            _identityProvider.SetFor(userTerritory);

            using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, UserTerritory>())
            {
            _userTerritoryGenericRepository.Add(userTerritory);
            _userTerritoryGenericRepository.Save();
                scope.Added(userTerritory)
                     .Complete();
            }
        }

        public void CreateOrUpdate(User user)
        {
            var userExist = _finder.Find(new FindSpecification<User>(x => x.Account == user.Account && x.Id != user.Id)).Any();
            if (userExist)
            {
                throw new ArgumentException(BLResources.UserAlreadyExists);
            }

            using (var scope = _operationScopeFactory.CreateOrUpdateOperationFor(user))
            {
                if (user.IsNew())
                {
                    _userGenericRepository.Add(user);
                    scope.Added<User>(user.Id);
                }
                else
                {
                    // проверка руководителя
                    if (user.ParentId != null)
                    {
                        var recursion = _userPersistenceService.CheckUserParentnessRecursion(user.Id, user.ParentId.Value);
                        if (recursion > 0)
                        {
                            throw new ArgumentException(BLResources.UserParentnessRecursionError);
                        }
                    }

                    _userGenericRepository.Update(user);
                    scope.Updated<User>(user.Id);
                }

                scope.Complete();
            }

            _userGenericRepository.Save();
        }

        public void CreateOrUpdate(Department department)
        {
            var departmentExist = _finder.Find(new FindSpecification<Department>(x => x.Name == department.Name && x.Id != department.Id && x.IsActive && !x.IsDeleted)).Any();
            if (departmentExist)
            {
                throw new ArgumentException(BLResources.RecordAlreadyExists);
            }

            if (department.ParentId == null)
            {
                var departmentRootExist = _finder.Find(new FindSpecification<Department>(x => x.ParentId == null && x.Id != department.Id && x.IsActive && !x.IsDeleted)).Any();
                if (departmentRootExist)
                {
                    throw new ArgumentException(BLResources.EditDepartmentSingleParentlessError);
                }
            }

            if (department.IsNew())
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Department>())
                {
                    _departmentGenericRepository.Add(department);
                    scope.Added<Department>(department.Id);
                    _departmentGenericRepository.Save();

                    scope.Complete();
                }
            }
            else
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Department>())
                {
                    _departmentGenericRepository.Update(department);
                    scope.Updated<Department>(department.Id);
                    _departmentGenericRepository.Save();

                    scope.Complete();
                }
            }
        }

        public UserProfile GetProfileForUser(long userCode)
        {
            return 
                _finder
                    .Find(Specs.Find.ActiveAndNotDeleted<UserProfile>() 
                                    && UserSpecs.UserProfiles.Find.ForUser(userCode))
                    .Top();
        }

        public IReadOnlyCollection<UserProfileDto> GetAllUserProfiles()
        {
            return _finder.Find(new FindSpecification<User>(x => true))
                          .Map(q => q.OrderBy(x => x.ModifiedOn)
                                     .Select(x => new UserProfileDto
                                         {
                                             UserAccountName = x.Account,
                                             UserProfile = x.UserProfiles.FirstOrDefault(),
                                         })
                                     .Where(x => x.UserProfile != null))
                          .Many();
        }

        // TODO: {all, 14.02.2013}: вынести методы для работы с профилями пользователей в SimplifiedModel service (см. например ContributionTypeService)
        public void UpdateUserProfiles(IReadOnlyCollection<UserProfileDto> userProfileDtos)
        {
            foreach (var userProfileDto in userProfileDtos)
            {
                _userProfileGenericRepository.Update(userProfileDto.UserProfile);
            }

            _userProfileGenericRepository.Save();
        }

        public void CreateOrUpdate(UserProfile userProfile)
        {
            if (userProfile.IsNew())
            {
                _userProfileGenericRepository.Add(userProfile);
            }
            else
            {
                _userProfileGenericRepository.Update(userProfile);
            }

            _userProfileGenericRepository.Save();
        }

        public LocaleInfo GetUserLocaleInfo(long userCode)
        {
            var userProfileDto = _finder.Find(new FindSpecification<User>(x => x.Id == userCode))
                                        .Map(q => q.SelectMany(x => x.UserProfiles)
                                                   .Select(x => new
                                                       {
                                                           x.CultureInfoLCID,
                                                           x.TimeZone.TimeZoneId,
                                                       }))
                                        .One();

            LocaleInfo localeInfo;
            if (userProfileDto == null)
            {
                var timeZoneId = GetUserTimeZoneHeuristic(userCode);
                localeInfo = (timeZoneId != null)
                                 ? new LocaleInfo(timeZoneId, LocaleInfo.Default.UserCultureInfo.LCID)
                                 : LocaleInfo.Default;
            }
            else
            {
                localeInfo = new LocaleInfo(userProfileDto.TimeZoneId, userProfileDto.CultureInfoLCID);
            }

            return localeInfo;
        }

        public bool IsUserLinkedWithOrganizationUnit(long userId, long organizationUnitId)
        {
            return _finder.Find(new FindSpecification<UserTerritoriesOrganizationUnits>(x => x.UserId == userId && x.OrganizationUnitId == organizationUnitId)).Any();
        }

        public OrganizationUnitDto GetSingleOrDefaultOrganizationUnit(long userId)
        {
            // Запрос идет по линии раздела edmx, поэтому после удаления ErmSecurity.edmx можно будет обойтись одним.
            var singleOrganizationUnitIds = _finder.Find(Specs.Find.ById<User>(userId))
                                                   .Map(q => q.SelectMany(x => x.UserOrganizationUnits)
                                                              .Select(x => x.OrganizationUnitId)
                                                              .Take(2))
                                                   .Many();

            if (singleOrganizationUnitIds.Count == 0 || singleOrganizationUnitIds.Count > 1)
            {
                return null;
            }

            var organizationUnitDto = _finder.FindObsolete(Specs.Find.ById<OrganizationUnit>(singleOrganizationUnitIds.Single()))
            .Select(x => new OrganizationUnitDto
            {
                Id = x.Id,
                Name = x.Name,

                CurrencyId = x.Country.Currency.Id,
                CurrencyName = x.Country.Currency.Name,

                ProjectExists = x.Projects.Any(),
            })
            .Single();

            return organizationUnitDto;
        }

        int IActivateAggregateRepository<Department>.Activate(long entityId)
        {
            var entity = _finder.FindObsolete(Specs.Find.ById<Department>(entityId)).Single();
            return Activate(entity);
        }

        int IActivateAggregateRepository<OrganizationUnit>.Activate(long entityId)
        {
            var entity = _finder.FindObsolete(Specs.Find.ById<OrganizationUnit>(entityId)).Single();
            return Activate(entity);
        }

        public IEnumerable<User> GetUsersByDepartments(IEnumerable<long> departmentIds)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<User>() && UserSpecs.Users.Find.ByDepartments(departmentIds))
                          .Many();
        }

        int IDeactivateAggregateRepository<OrganizationUnit>.Deactivate(long entityId)
        {
            var entity = _finder.FindObsolete(Specs.Find.ById<OrganizationUnit>(entityId)).Single();
            return Deactivate(entity);
        }

        int IDeactivateAggregateRepository<Territory>.Deactivate(long entityId)
        {
            var entity = _finder.FindObsolete(Specs.Find.ById<Territory>(entityId)).Single();
            return Deactivate(entity);
        }

        int IDeactivateAggregateRepository<Department>.Deactivate(long entityId)
        {
            var entity = _finder.FindObsolete(Specs.Find.ById<Department>(entityId)).Single();
            return Deactivate(entity);
        }

        int IDeleteAggregateRepository<OrganizationUnit>.Delete(long entityId)
        {
            var unit = _finder.FindObsolete(Specs.Find.ById<OrganizationUnit>(entityId)).Single();
            return Delete(unit);
        }

        int IDeleteAggregateRepository<UserOrganizationUnit>.Delete(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, UserOrganizationUnit>())
            {
                var userOrganizationUnit = _finder.FindObsolete(Specs.Find.ById<UserOrganizationUnit>(entityId)).Single();

                var territoryIds = _finder.FindObsolete(TerritoryDtoSpecifications.TerritoriesFromOrganizationUnit(userOrganizationUnit.OrganizationUnitId)).Select(x => x.Id);

                // Удаление территорий пользователя данной территории организации
                var userTerritories = _finder
                    .Find(new FindSpecification<UserTerritory>(x => x.UserId == userOrganizationUnit.UserId && territoryIds.Contains(x.TerritoryId) && !x.IsDeleted))
                    .Many();

                foreach (var userTerritory in userTerritories)
                {
                    _userTerritoryGenericRepository.Delete(userTerritory);
                    scope.Deleted(userTerritory);
                }

                _userTerritoryGenericRepository.Save();

                _userOrganizationUnitGenericRepository.Delete(userOrganizationUnit);
                scope.Deleted(userOrganizationUnit);

                var result = _userOrganizationUnitGenericRepository.Save();
                
                scope.Complete();

                return result;
            }
        }

        public void CreateOrUpdate(Territory territory)
        {
            using (var scope = _operationScopeFactory.CreateOrUpdateOperationFor(territory))
            {
                if (territory.IsNew())
                {
                    _territoryGenericRepository.Add(territory);
                    scope.Added<Territory>(territory.Id);
                }
                else
                {
                    _territoryGenericRepository.Update(territory);
                    scope.Updated<Territory>(territory.Id);
                }
                
                _territoryGenericRepository.Save();

                scope.Complete();
            }
        }

        public Territory GetTerritory(long territoryId)
        {
            return _finder.Find(new FindSpecification<Territory>(x => territoryId == x.Id)).One();
        }

        public IEnumerable<User> GetUsersByTerritory(long territoryId)
        {
            return _finder.Find(new FindSpecification<User>(x => x.UserTerritories.Any(y => y.TerritoryId == territoryId))).Many();
        }

        public IEnumerable<User> GetUsersByOrganizationUnit(long organizationUnitId)
        {
            return _finder.Find(new FindSpecification<User>(
                                    x => x.IsActive && x.UserOrganizationUnits.Any(y => y.OrganizationUnitId == organizationUnitId)))
                          .Many();
        }

        public IEnumerable<long> GetUserTerritoryIds(long userId)
        {
            // Представление vwTerritories уже включает в себя фильтр по неактивным
            return _finder.Find(Specs.Find.ById<User>(userId))
                          .Map(q => q.SelectMany(user => user.UserTerritories)
                                     .Select(territory => territory.TerritoryDto)
                                     .Select(dto => dto.Id))
                          .Many();
        }

        public IEnumerable<long> GetUserOrganizationUnitsTerritoryIds(long userId)
        {
            // Представления vwTerritories, vwOrganizationUnits на которых основаны dto-сущности не содержат удаленных или неактивых записей.
            return _finder.Find(Specs.Find.ById<User>(userId))
                          .Map(q => q.SelectMany(user => user.UserOrganizationUnits)
                                     .Select(unit => unit.OrganizationUnitDto)
                                     .SelectMany(unitDto => unitDto.Territories)
                                     .Select(territoryDto => territoryDto.Id))
                          .Many();
        }

        public IEnumerable<long> GetAllTerritoryIds()
        {
            return _finder.Find(new FindSpecification<Territory>(territory => territory.IsActive))
                          .Map(q => q.Select(territory => territory.Id))
                          .Many();
        }

        public void ChangeUserTerritory(IEnumerable<User> users, long oldTerritoryId, long newTerritoryId)
        {
            var userIds = users.Select(user => user.Id);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ChangeTerritoryIdentity, User>())
            {
                // Все существующие ссылки на старую территорию удаляем
                var linksToRemove = _finder.Find(new FindSpecification<UserTerritory>(link => userIds.Contains(link.UserId) && link.TerritoryId == oldTerritoryId)).Many();
                foreach (var userTerritory in linksToRemove)
                {
                    _userTerritoryGenericRepository.Delete(userTerritory);
                    operationScope.Deleted<UserTerritory>(userTerritory.Id);
                }

                // Новые ссылки добавляем только тем, у кого этоё территории не было
                var usersHavingNewTerritory = _finder.Find(new FindSpecification<UserTerritory>(link => userIds.Contains(link.UserId) && link.TerritoryId == newTerritoryId))
                                                     .Map(q => q.Select(territory => territory.UserId))
                                                     .Many();
                var usersNeedNewTerritory = userIds.Except(usersHavingNewTerritory).ToArray();
                foreach (var userId in usersNeedNewTerritory)
                {
                    var link = new UserTerritory
                    {
                        UserId = userId,
                        TerritoryId = newTerritoryId,
                    };

                    _identityProvider.SetFor(link);
                    _userTerritoryGenericRepository.Add(link);
                    operationScope.Added<UserTerritory>(link.Id);
                }

                _userTerritoryGenericRepository.Save();
                operationScope.Complete();
            }
        }

        public void CreateOrUpdate(OrganizationUnit organizationUnit)
        {
            var dgppIdNotUnique = _finder.Find(new FindSpecification<OrganizationUnit>(x => x.DgppId == organizationUnit.DgppId && x.Id != organizationUnit.Id)).Any();
            if (dgppIdNotUnique)
            {
                throw new NotificationException(string.Format(CultureInfo.CurrentCulture, BLResources.OrganizationUnitNonUniqueDgppId, organizationUnit.DgppId));
            }

            var notUniqueSyncCode1C = _finder.Find(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() &&
                                                   new FindSpecification<OrganizationUnit>(x => x.SyncCode1C == organizationUnit.SyncCode1C && x.Id != organizationUnit.Id))
                                             .Any();

            if (!organizationUnit.IsNew())
            {
                var organizationUnitDatesInfo = _finder.FindObsolete(Specs.Find.ById<OrganizationUnit>(organizationUnit.Id)).Select(x => new
                {
                    x.FirstEmitDate,
                    x.ErmLaunchDate,
                    x.InfoRussiaLaunchDate
                }).Single();

                if (organizationUnitDatesInfo.FirstEmitDate.Date != organizationUnit.FirstEmitDate.Date &&
                    (organizationUnitDatesInfo.ErmLaunchDate.HasValue || organizationUnit.InfoRussiaLaunchDate.HasValue))
                {
                    throw new BusinessLogicException(BLResources.CanNotChangeFirstEmitDateSinceOrganizationUnitIsLaunchedOnErmOrInfoRussia);
                }
            }

            if (notUniqueSyncCode1C)
            {
                throw new NotificationException(string.Format(
                    CultureInfo.CurrentCulture,
                    BLResources.OrganizationUnitNonUniqueSyncCode1C,
                    organizationUnit.SyncCode1C));
            }


            if (organizationUnit.IsNew())
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, OrganizationUnit>())
                {
                    _organizationUnitGenericRepository.Add(organizationUnit);
                    scope.Added<OrganizationUnit>(organizationUnit.Id);
                    _organizationUnitGenericRepository.Save();

                    scope.Complete();
                }
                
            }
            else
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, OrganizationUnit>())
                {
                    _organizationUnitGenericRepository.Update(organizationUnit);
                    scope.Updated<OrganizationUnit>(organizationUnit.Id);
                    _organizationUnitGenericRepository.Save();

                    scope.Complete();
                }
            }

        }

        private static Guid GetCrmRoleId(CrmDataContext crmDataContext, string crmRoleName, CrmDataContextExtensions.CrmUserInfo crmUserInfo)
        {
            Guid crmRoleId;
            if (!crmDataContext.TryGetCrmRoleId(crmUserInfo.BusinessUnitId, crmRoleName, out crmRoleId))
            {
                throw new ArgumentException(BLResources.RoleNotExistInCRM);
            }

            return crmRoleId;
        }

        private string GetCrmRoleName(UserRole userRole)
        {
            var crmRoleName = _finder.Find(new FindSpecification<Role>(x => x.Id == userRole.RoleId)).Map(q => q.Select(x => x.Name)).Top();
            if (crmRoleName == null)
            {
                throw new ArgumentException(BLResources.RoleNotMappedToCRMRole);
            }

            return crmRoleName;
        }

        private void DeleteRoleFromCrm(UserRole userRole)
        {
            if (!_msCrmSettings.IntegrationMode.HasFlag(MsCrmIntegrationMode.Sdk))
            {
                return;
            }

            try
            {
                var crmRoleName = GetCrmRoleName(userRole);

                var crmDataContext = _msCrmSettings.CreateDataContext();
                var crmUserInfo = GetCrmUserInfo(crmDataContext, userRole.UserId);
                var crmRoleId = GetCrmRoleId(crmDataContext, crmRoleName, crmUserInfo);

                crmDataContext.UsingService(x => x.Execute(new RemoveUserRolesRoleRequest { UserId = crmUserInfo.UserId, RoleIds = new[] { crmRoleId } }));
            }
            catch (WebException ex)
            {
                throw new NotificationException(BLResources.Errors_DynamicsCrmConectionFailed, ex);
            }
        }

        private CrmDataContextExtensions.CrmUserInfo GetCrmUserInfo(ICrmDataContext crmDataContext, long userId)
        {
            var userAccount = _finder.FindObsolete(Specs.Find.ById<User>(userId)).Select(x => x.Account).Single();
            var userInfo = crmDataContext.GetSystemUserByDomainName(userAccount, true);
            return userInfo;
        }

        private string GetUserTimeZoneHeuristic(long userCode)
        {
            var organizationUnits = _finder.Find(new FindSpecification<OrganizationUnit>(x => x.IsActive && !x.IsDeleted))
                                           .Map(q => q.Select(x => new { x.Name, x.TimeZoneId }))
                                           .Many();

            var timezones = _query.For<TimeZone>().ToArray();
            var organizationUnitimeZones = organizationUnits.Join(
                timezones,
                x => x.TimeZoneId,
                x => x.Id,
                (x, y) => new
                {
                    x.Name,
                    y.TimeZoneId,
                }).ToArray();

            // TODO: убрать это УГ
            var userDepartments = _finder.Find(Specs.Find.ById<User>(userCode))
                                         .Map(q => q.Select(x => new
                                             {
                                                 UserId = x.Id,
                                                 DepartmentName = (x.Department.Name == "2ГИС")
                                                                      ? "Новосибирск"
                                                                      : (x.Department.Name == "Алтай")
                                                                            ? "Барнаул"
                                                                            : x.Department.Name,
                                             })).Many();

            var timeZoneId = userDepartments.SelectMany(x => organizationUnitimeZones.DefaultIfEmpty(),
                                                        (x, y) => new
                                                            {
                                                                x.DepartmentName,
                                                                y.Name,

                                                                y.TimeZoneId,
                                                            })
                                            .Where(x => x.DepartmentName.Contains(x.Name))
                                            .Select(x => x.TimeZoneId)
                                            .FirstOrDefault();

            return timeZoneId;
        }
    }
}
