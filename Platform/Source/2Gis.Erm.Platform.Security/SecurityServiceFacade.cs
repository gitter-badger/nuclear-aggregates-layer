using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.ServiceModel.Security;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Resources.Server;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Security
{
    public sealed class SecurityServiceFacade : ISecurityServiceUserIdentifier,
                                                ISecurityServiceEntityAccessInternal,
                                                ISecurityServiceFunctionalAccess,
                                                ISecurityServiceSharings
    {
        private const string CacheKeyMask = "security:{0}:{1}:{2}";

        private static readonly IEnumerable<EntityAccessTypes> AtomicAccessTypes =
            ((EntityAccessTypes[])typeof(EntityAccessTypes).GetEnumValues()).Where(x => x != EntityAccessTypes.None && x != EntityAccessTypes.All);

        // FIXME {all, 31.07.2014}: непонятно как данный routing будет работать c EAV сущностями, т.к. в их случае в query будет фигурировать не domain entity, а внутренние сущности хранилища EAV
        // Для каждой пары в словаре проверка привилегий для первой сущности заменяется  
        // проверкой привилегий для второй сущности.
        // TODO : не проверено мирное сосуществование рутингов и Access sharing'а.
        private static readonly Dictionary<IEntityType, IEntityType> EntityPrivilegesRoutings =
            new Dictionary<IEntityType, IEntityType>
                {
                    { EntityType.Instance.Appointment(), EntityType.Instance.Activity() },
                    { EntityType.Instance.Phonecall(), EntityType.Instance.Activity() },
                    { EntityType.Instance.Task(), EntityType.Instance.Activity() },
                    { EntityType.Instance.Letter(), EntityType.Instance.Activity() },
                };

        private readonly IFinder _finder;
        private readonly IUserEntityService _userEntityService;
        private readonly ICommonLog _logger;
        private readonly ICacheAdapter _cacheAdapter;
        private readonly TimeSpan _cacheSlidingSpan = TimeSpan.FromSeconds(60);

        public SecurityServiceFacade(IFinder finder,
            IUserEntityService userEntityService,
            ICacheAdapter cacheAdapter,
            ICommonLog commonLog)
        {
            _finder = finder;
            _userEntityService = userEntityService;
            _cacheAdapter = cacheAdapter;
            _logger = commonLog;
        }

        private DateTime CacheAbsoluteSpan
        {
            get { return DateTime.UtcNow.Add(_cacheSlidingSpan); }
        }

        #region ISecurityServiceUserIdentifier

        IUserInfo ISecurityServiceUserIdentifier.GetReserveUserIdentity()
        {
            const long ReserveUserId = 27;
            const string ReserveUserAccount = "reserve";

            var cacheKey = string.Format(CacheKeyMask, "UserInfo", "ReserveUser", string.Empty);

            var result = _cacheAdapter.Get<UserInfo>(cacheKey);
            if (result != null)
            {
                return result;
            }

            var userInfo = _finder.Find<User>(x => !x.IsDeleted && x.Account == ReserveUserAccount)
                .Select(x => new
                {
                    UserCode = x.Id,
                    x.Account,
                    x.DisplayName
                })
                .SingleOrDefault();

            if (userInfo == null)
            {
                _logger.FatalFormat("Пользователь 'Резерв района' не найден по учетной записи [{0}]", ReserveUserAccount);
                throw new SecurityException("Пользователь 'Резерв района' не найден");
            }

            if (userInfo.UserCode != ReserveUserId)
            {   // небольшое УГ, проверяем, чтобы Id пользователя 'Резерв района' был допустимый, он должен быть стабильным в любой инсталляции системы
                // т.к. на конкретное значение завязаны миграции и т.п.
                _logger.FatalFormat("Пользователь 'Резерв района' имеет Id отличный от допустимого [{0}]", ReserveUserId);
                throw new SecurityException("Пользователь 'Резерв района' имеет недопустимый Id");
            }

            result = new UserInfo(userInfo.UserCode, userInfo.Account, userInfo.DisplayName);
            _cacheAdapter.Add(cacheKey, result, CacheAbsoluteSpan);
            return result;
        }

        IUserInfo ISecurityServiceUserIdentifier.GetUserInfo(long? userCode)
        {
            if (userCode == null || userCode == 0)
            {
                return UserInfo.Empty;
            }

            var key = string.Format(CacheKeyMask, "UserInfo", userCode, string.Empty);

            _logger.DebugFormat("Попытка получить данные пользователя по коду: {0}", userCode);
            var result = _cacheAdapter.Get<UserInfo>(key);
            if (result != null)
            {
                _logger.DebugFormat("Получен пользователь через кэш. Код:{0}. Учетная запись:{1}. Имя:{2}.", result.Code, result.Account, result.DisplayName);
                return result;
            }

            var userInfo = _finder.Find<User>(x => x.Id == userCode.Value)
                                .Select(x => new { UserCode = x.Id, x.Account, x.DisplayName })
                                .FirstOrDefault();

            result = userInfo != null ? new UserInfo(userInfo.UserCode, userInfo.Account, userInfo.DisplayName) : UserInfo.Empty;

            _cacheAdapter.Add(key, result, CacheAbsoluteSpan);

            _logger.DebugFormat("Получен пользователь через БД. Код:{0}. Учетная запись:{1}. Имя:{2}.", result.Code, result.Account, result.DisplayName);
            return result;
        }

        IUserInfo ISecurityServiceUserIdentifier.GetUserInfo(string userAccount)
        {
            if (string.IsNullOrWhiteSpace(userAccount))
            {
                return UserInfo.Empty;
            }

            var key = string.Format(CacheKeyMask, "UserInfo", userAccount, string.Empty);

            _logger.DebugFormat("Попытка получить данные пользователя по учетной записи: {0}", userAccount);

            var result = _cacheAdapter.Get<UserInfo>(key);
            if (result != null)
            {
                _logger.DebugFormat("Получен пользователь через кэш. Код:{0}. Учетная запись:{1}. Имя:{2}.", result.Code, result.Account, result.DisplayName);
                return result;
            }

            var userInfo = _finder.Find<User>(x => !x.IsDeleted && (x.Account == userAccount))
                                .Select(x => new { UserCode = x.Id, x.Account, x.DisplayName })
                                .FirstOrDefault();

            result = userInfo != null ? new UserInfo(userInfo.UserCode, userInfo.Account, userInfo.DisplayName) : UserInfo.Empty;

            _cacheAdapter.Add(key, result, CacheAbsoluteSpan);

            _logger.DebugFormat("Получен пользователь. Код:{0}. Учетная запись:{1}. Имя:{2}.", result.Code, result.Account, result.DisplayName);
            return result;
        }

        IList<long> ISecurityServiceUserIdentifier.GetUserDepartments(long? userCode, bool withChild)
        {
            List<long> result;
            if (withChild)
            {
                var userDepartment = _finder.Find<User>(x => !x.IsDeleted && x.IsActive && x.Id == userCode)
                    .Select(x => new { x.Department.LeftBorder, x.Department.RightBorder })
                    .First();
                result = _finder.Find<User>(u => !u.IsDeleted && u.IsActive)
                    .Select(u => u.Department)
                    .Where(d => d.LeftBorder >= userDepartment.LeftBorder && d.RightBorder <= userDepartment.RightBorder)
                    .Select(d => d.Id)
                    .ToList();
            }
            else
            {
                result = _finder.Find<User>(u => u.Id == userCode && !u.IsDeleted && u.IsActive).Select(d => d.Department.Id).ToList();
            }

            return result;
        }

        bool ISecurityServiceUserIdentifier.UsersInSameDepartment(long firstUserCode, long secondUserCode)
        {
            return (firstUserCode == secondUserCode) || _finder.Find<User>(x => x.Id == firstUserCode && !x.IsDeleted && x.IsActive)
                                                                    .Select(x => x.Department)
                                                                    .SelectMany(x => x.Users)
                                                                    .Any(x => x.Id == secondUserCode && !x.IsDeleted && x.IsActive);
        }

        bool ISecurityServiceUserIdentifier.UsersInSameDepartmentTree(long firstUserCode, long secondUserCode)
        {
            if (firstUserCode == secondUserCode)
            {
                return true;
            }

            var firstUserDepartment = _finder.Find<User>(x => x.Id == firstUserCode && !x.IsDeleted && x.IsActive)
                    .Select(x => new { x.Department.LeftBorder, x.Department.RightBorder })
                    .Single();
            return _finder.Find<User>(x => x.Id == secondUserCode && !x.IsDeleted && x.IsActive)
                    .Select(x => x.Department)
                    .Any(x => x.LeftBorder >= firstUserDepartment.LeftBorder && x.RightBorder <= firstUserDepartment.RightBorder);
        }

        #endregion

        #region ISecurityServiceEntityAccessInternal

        IQueryable ISecurityServiceEntityAccessInternal.RestrictQuery(IQueryable query, IEntityType entityName, long userCode)
        {
            IEntityType entityToCheck;
            if (!EntityPrivilegesRoutings.TryGetValue(entityName, out entityToCheck))
            {
                entityToCheck = entityName;
            }

            var privilegeDepth = GetEntityPrivilegeDepth(userCode, entityToCheck, EntityAccessTypes.Read);
            var restrictUserCodes = GetOwnerCodeRestriction(privilegeDepth, userCode, entityName);

            // shared entities
            var entityTypeId = entityName.Id;
            IEnumerable<int> sharedEntityIds = _finder.Find<User>(x => x.Id == userCode)
                    .SelectMany(x => x.UserEntities)
                    .Where(x => x.Privilege.EntityType == entityTypeId && x.Privilege.Operation == (int)EntityAccessTypes.Read)
                    .Select(x => x.EntityId)
                    .ToArray();

            return QueryableHelper.CreateRestrictedQuery(query, entityName.AsEntityType(), restrictUserCodes, sharedEntityIds);
        }

        private IQueryable<long> GetOwnerCodeRestriction(EntityPrivilegeDepthState privilegeDepth, long userCode, IEntityType entityName)
        {
            switch (privilegeDepth)
            {
                case EntityPrivilegeDepthState.None:
                    throw new SecurityAccessDeniedException(string.Format(ResPlatform.UserIdDoesNotHaveTheRightToOperationOnEntity, userCode, EntityAccessTypes.Read, entityName));

                case EntityPrivilegeDepthState.User:
                    return _finder.Find<SecurityAccelerator>(accelerator => accelerator.UserId == userCode).Select(accelerator => accelerator.UserId);

                case EntityPrivilegeDepthState.Department:
                    var department = _finder.Find(Specs.Find.ById<User>(userCode)).Select(user => user.DepartmentId).SingleOrDefault();
                    return _finder.Find<SecurityAccelerator>(accelerator => accelerator.DepartmentId == department).Select(accelerator => accelerator.UserId);

                case EntityPrivilegeDepthState.DepartmentAndChilds:
                        var departmentBounds = _finder.Find<User>(x => x.Id == userCode)
                                                .Select(x => new
                                                {
                                                    x.Department.LeftBorder,
                                                    x.Department.RightBorder,
                                                  })
                                                  .Single();
                    return _finder.Find<SecurityAccelerator>(accelerator => departmentBounds.LeftBorder <= accelerator.DepartmentLeftBorder && accelerator.DepartmentRightBorder <= departmentBounds.RightBorder).Select(accelerator => accelerator.UserId);

                case EntityPrivilegeDepthState.Organization:
                    return Enumerable.Empty<long>().AsQueryable();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        EntityAccessTypes ISecurityServiceEntityAccessInternal.GetCommonEntityAccessForMetadata(IEntityType entityName, long userCode)
        {
            try
            {
                var allOperationsList = (EntityAccessTypes[])typeof(EntityAccessTypes).GetEnumValues();

                var result = (from operation in allOperationsList
                              let privilegeDepth = GetEntityPrivilegeDepth(userCode, entityName, operation)
                              where (privilegeDepth != EntityPrivilegeDepthState.None)
                              select operation).Aggregate((EntityAccessTypes)0, (current, operation) => current | operation);

                return result;
            }
            catch (SecurityAccessDeniedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SecurityAccessDeniedException(string.Format("Ошибка SecurityService: Разрешение на список. Код пользователя: [{0}].", userCode), ex);
            }
        }

        bool ISecurityServiceEntityAccess.IsSecureEntity(IEntityType entityName)
        {
            IEntityType entityToCheck;
            if (!EntityPrivilegesRoutings.TryGetValue(entityName, out entityToCheck))
            {
                entityToCheck = entityName;
            }

            var key = string.Format(CacheKeyMask, "IsSecureEntity", entityToCheck, string.Empty);
            var isSecure = _cacheAdapter.Get<bool?>(key);
            if (isSecure.HasValue)
            {
                return isSecure.Value;
        }

            var entityToCheckTypeId = entityToCheck.Id;
            var result = _finder.FindAll<Privilege>().Any(x => x.EntityType == entityToCheckTypeId);
            _cacheAdapter.Add(key, result, CacheAbsoluteSpan);
            return result;
        }

        bool ISecurityServiceEntityAccess.HasEntityAccess(EntityAccessTypes accessTypes, IEntityType entityName, long userCode, long? entityId, long ownerCode, long? oldOwnerCode)
        {
            try
            {
                var self = (ISecurityServiceEntityAccess)this;
                var restrictedAccessTypes = self.RestrictEntityAccess(entityName, accessTypes, userCode, entityId, ownerCode, oldOwnerCode);
                return restrictedAccessTypes == accessTypes;
            }
            catch (Exception ex)
            {
                throw new SecurityAccessDeniedException(string.Format("Ошибка SecurityService: Доступ изменения сущности. Код пользователя: [{0}].", userCode), ex);
            }
        }

        EntityAccessTypes ISecurityServiceEntityAccess.RestrictEntityAccess(IEntityType entityName, EntityAccessTypes accessTypes, long userCode, long? entityId, long ownerCode, long? ownerOldCode)
        {
            var restrictedAccessTypes = EntityAccessTypes.None;
            var entityTypeId = entityName.Id;

            // разбиваем accessTypes на атомарные привилегии
            foreach (var atomicAccessType in AtomicAccessTypes.Where(x => accessTypes.HasFlag(x)))
            {
                var compareCode = GetCompareCode(atomicAccessType, userCode, ownerCode, ownerOldCode);

                bool hasAccess;
                var entityPrivilegeDepth = GetEntityPrivilegeDepth(userCode, entityName, atomicAccessType);
                switch (entityPrivilegeDepth)
                {
                    case EntityPrivilegeDepthState.None:
                        {
                            hasAccess = false;
                        }

                        break;

                    case EntityPrivilegeDepthState.User:
                        {
                            hasAccess = userCode == compareCode;
                        }

                        break;

                    case EntityPrivilegeDepthState.Department:
                        {
                            hasAccess = _finder.Find<User>(x => x.Id == userCode)
                                                    .Select(x => x.Department)
                                                    .SelectMany(x => x.Users)
                                                    .Any(x => x.Id == compareCode);
                        }

                        break;

                    case EntityPrivilegeDepthState.DepartmentAndChilds:
                        {
                            var departmentBounds = _finder.Find<User>(x => x.Id == userCode)
                                .Select(x => new { x.Department.LeftBorder, x.Department.RightBorder })
                                .First();

                            hasAccess = _finder.FindAll<Department>()
                                .Where(x => x.LeftBorder >= departmentBounds.LeftBorder && x.RightBorder <= departmentBounds.RightBorder)
                                .SelectMany(x => x.Users)
                                .Distinct()
                                .Any(x => x.Id == compareCode);
                        }

                        break;

                    case EntityPrivilegeDepthState.Organization:
                        {
                            hasAccess = true;
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // check access sharing
                if (!hasAccess && entityId != null)
                {
                    hasAccess = _finder.Find<User>(x => x.Id == userCode)
                                    .SelectMany(x => x.UserEntities)
                                    .Where(x => x.Privilege.EntityType == entityTypeId && x.Privilege.Operation == (int)atomicAccessType)
                                    .Any(x => x.EntityId == entityId);
                }

                if (hasAccess)
                {
                    restrictedAccessTypes |= atomicAccessType;
                }
            }

            return restrictedAccessTypes;
        }

        private long GetCompareCode(EntityAccessTypes atomicAccessType, long userCode, long ownerCode, long? ownerOldCode)
        {
            var compareCode = ownerCode;

            switch (atomicAccessType)
            {
                case EntityAccessTypes.Assign:
                case EntityAccessTypes.Update:
                    {
                        if (ownerOldCode == null || ownerOldCode.Value < 1)
                        {
                            compareCode = ownerCode;
                            break;
                        }

                        compareCode = ownerOldCode.Value;
                    }

                    break;
            }

            var self = (ISecurityServiceUserIdentifier)this;
            var reserverCode = self.GetReserveUserIdentity().Code;
            if (compareCode == reserverCode)
            {
                compareCode = userCode;
            }

            return compareCode;
        }

        private EntityPrivilegeDepthState GetEntityPrivilegeDepth(long userCode, IEntityType entityName, EntityAccessTypes entityAccessType)
        {
            IEntityType entityToCheck;
            if (!EntityPrivilegesRoutings.TryGetValue(entityName, out entityToCheck))
            {
                entityToCheck = entityName;
            }

            var entityToCheckTypeId = entityToCheck.Id;
            var key = string.Format(CacheKeyMask, "EntityPriveleges", entityToCheck, userCode);

            var entityPrivileges = _cacheAdapter.Get<Dictionary<EntityAccessTypes, EntityPrivilegeDepthState>>(key);
            if (entityPrivileges == null)
            {
                entityPrivileges = _finder.Find<User>(x => x.Id == userCode)
                .SelectMany(x => x.UserRoles).Select(x => x.Role)
                .SelectMany(x => x.RolePrivileges)
                                          .Where(x => x.Privilege.EntityType == entityToCheckTypeId)
                .Select(x => new
                {
                    AccessType = (EntityAccessTypes)x.Privilege.Operation,
                    PrivilegeDepth = x.Mask,
                })
                .GroupBy(x => x.AccessType, x => x.PrivilegeDepth)
                .ToDictionary(x => x.Key, x => (EntityPrivilegeDepthState)x.Max());

                _cacheAdapter.Add(key, entityPrivileges, CacheAbsoluteSpan);
            }

            EntityPrivilegeDepthState entityPrivilegeDepthState;
            if (!entityPrivileges.TryGetValue(entityAccessType, out entityPrivilegeDepthState))
            {
                return EntityPrivilegeDepthState.None;
            }

            return entityPrivilegeDepthState;
        }

        #endregion

        #region ISecurityServiceSharings

        IEnumerable<SharingDescriptor> ISecurityServiceSharings.GetAccessSharingsForEntity(IEntityType entityName, long entityId)
        {
            try
            {
                return GetAccessSharingsForEntity(entityName, entityId);
            }
            catch (Exception ex)
            {
                throw new SecurityAccessDeniedException("Ошибка SecurityService", ex);
            }
        }

        void ISecurityServiceSharings.UpdateAccessSharings(IEntityType entityName, long entityId, long entityOwnerCode, IEnumerable<SharingDescriptor> accessSharings, long userCode)
        {
            var entityTypeId = entityName.Id;
            try
            {
                ValidateAccessSharings(userCode, entityName, entityId, entityOwnerCode, accessSharings);

                // delete existing sharings associated with entity type
                _userEntityService.DeleteSharings(entityName, entityId);

                // add new sharings
                foreach (var sharingToAdd in accessSharings)
                {
                    foreach (var entityAccessType in AtomicAccessTypes)
                    {
                        if (!sharingToAdd.AccessTypes.HasFlag(entityAccessType))
                        {
                            continue;
                        }

                        // get entity prvilege by entityAccessType
                        var entityAccessTypeClosure = entityAccessType;

                        var entityPrivilege = _finder.Find<Privilege>(x => x.EntityType == entityTypeId && x.Operation == (int)entityAccessTypeClosure).Single();

                        var userEntity = new UserEntity
                        {
                            EntityId = (int)entityId, // fixme: {a.rechkalov, 2013-03-18}: тут надо пофиксить миграцией колонку
                            UserId = sharingToAdd.UserInfo.Code,
                            PrivilegeId = entityPrivilege.Id,
                        };
                        _userEntityService.Add(userEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SecurityAccessDeniedException("Ошибка SecurityService", ex);
            }
        }

        private static EntityAccessTypes ToEntityAccessTypes(IEnumerable<int?> accessRightsMasks)
        {
            EntityAccessTypes result = 0;

            foreach (var accessRightsMask in accessRightsMasks)
            {
                if (accessRightsMask == null)
                {
                    continue;
                }

                result |= (EntityAccessTypes)accessRightsMask.Value;
            }

            return result;
        }

        private IEnumerable<SharingDescriptor> GetAccessSharingsForEntity(IEntityType entityName, long entityId)
        {
            var entityTypeId = entityName.Id;
            var entityPrivileges = _finder.FindAll<Privilege>()
                    .Where(x => x.EntityType == entityTypeId)
                    .Select(x => x.Id)
                    .ToArray();

            if (entityPrivileges == null)
            {
                throw new Exception("Не найдены привилегии на сущность: " + entityName);
            }

            var accessSharings = _finder.Find<User>(x => !x.IsDeleted && x.IsActive)
                .Select(x => new
            {
                x.Id,
                x.Account,
                x.FirstName,
                x.LastName,
                AccessRightsMasks = x.UserEntities
                         .Where(y => y.EntityId == entityId && entityPrivileges.Contains(y.PrivilegeId))
                         .Select(y => (int?)y.Privilege.Operation)
            })
            .Where(x => x.AccessRightsMasks.Any(y => y != null))
            .AsEnumerable()
            .Select(x => new SharingDescriptor
            {
                UserInfo = new UserInfo(x.Id, x.Account, x.LastName + ' ' + x.FirstName),
                AccessTypes = ToEntityAccessTypes(x.AccessRightsMasks),
            });

            return accessSharings;
        }

        private void ValidateAccessSharings(long userId, IEntityType entityName, long entityId, long entityOwnerId, IEnumerable<SharingDescriptor> accessSharings)
        {
            var self = (ISecurityServiceEntityAccess)this;

            var restrictedAccessTypes = self.RestrictEntityAccess(entityName, EntityAccessTypes.Share, userId, entityId, entityOwnerId, -1);
            if (!restrictedAccessTypes.HasFlag(EntityAccessTypes.Share))
            {
                throw new SecurityAccessDeniedException(string.Format(CultureInfo.CurrentCulture, "Пользователь {0} не может выдавать права на экземпляр {1} сущности {2}, поскольку у него нет права Share.", userId, entityId, entityName));
            }

            var existingAccessSharings = GetAccessSharingsForEntity(entityName, entityId);

            // создаём список вновь выданных разрешений.
            // так же проверяем, что у принимающего пользователя присутствуют необходимые разрешения на тип принимаемой сущности.
            foreach (var accessSharing in accessSharings)
            {
                var givenToUser = (EntityAccessTypes)0;

                foreach (EntityAccessTypes entityAccessType in typeof(EntityAccessTypes).GetEnumValues())
                {
                    if (!accessSharing.AccessTypes.HasFlag(entityAccessType))
                    {
                        continue;
                    }

                    var existingAccessSharing = existingAccessSharings.FirstOrDefault(x => x.UserInfo.Code == accessSharing.UserInfo.Code);
                    if (existingAccessSharing != null && existingAccessSharing.AccessTypes.HasFlag(entityAccessType))
                    {
                        continue;
                    }

                    givenToUser |= entityAccessType;
                }

                //todo: не разобрался до конца, но идея вроде такая (заюзал GetPermissionsForEntityInstance)
                //var userPermissionMask = GetPermissionsForEntityInstance(item.Code, "Erm", entityTypeName, null, -1);
                //if ((userPermissionMask | givenToUser) != userPermissionMask)
                //{
                //    var missingRights = givenToUser ^ (userPermissionMask & givenToUser);
                //    var formattedMessage = string.Format("Пользователю {0} не могут быть выданы права [{1}] на экземпляр сущности {2}, поскольку у него нет такого права на данный тип сущности.", userId, missingRights, entityTypeName);
                //    throw new SecurityAccessDeniedException(formattedMessage);
                //}

                //givenPermissions |= givenToUser;
            }

            //Проверяем, что каждое из вновь выданных разрешений присутствует у выдающего пользователя
            //foreach (EntityAccessTypes entityAccessType in typeof(EntityAccessTypes).GetEnumValues())
            //{
            //    if (!givenPermissions.HasFlag(entityAccessType))
            //        continue;

            //    var hasPermission = HasEntityAccess(userId, entityTypeName, entityAccessType, entityId, entityOwnerId, -1);
            //    if (!hasPermission)
            //    {
            //        var formattedMessage =
            //            string.Format(
            //                "Пользователь {0} не может выдать право {1} на экземпляр {2} сущности {3}, поскольку у него самого нет такого права.",
            //                userId, entityAccessType, entityId, entityTypeName);
            //        throw new SecurityAccessDeniedException(formattedMessage);
            //    }
            //}
        }

        #endregion

        #region ISecurityServiceFunctionalAccess

        bool ISecurityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName privilege, long userCode)
        {
            var userAccesses = GetUserFunctionalPrivilegeMasks(privilege, userCode);

            return userAccesses.Any();
        }

        int[] ISecurityServiceFunctionalAccess.GetFunctionalPrivilege(FunctionalPrivilegeName privilege, long userCode)
        {
            var userAccesses = GetUserFunctionalPrivilegeMasks(privilege, userCode);

            // пока игнорируем priority и каждый сам для себя определяет priority
            return userAccesses.Select(x => x.Mask).ToArray();
        }

        private PriorityMaskDto[] GetUserFunctionalPrivilegeMasks(FunctionalPrivilegeName privilege, long userCode)
        {
            var result = _finder.Find<User>(x => x.Id == userCode)
            .SelectMany(x => x.UserRoles).Select(x => x.Role)
            .SelectMany(x => x.RolePrivileges)
            .Where(x => x.Privilege.Operation == (int)privilege)
            .Distinct()
            .Select(x => new PriorityMaskDto
            {
                Priority = x.Priority,
                Mask = x.Mask,
            })
            .ToArray();

            return result;
        }

        private sealed class PriorityMaskDto
        {
            public byte Priority { get; set; }
            public int Mask { get; set; }
        }

        // TODO: перенести в соответствующий handler
        bool ISecurityServiceFunctionalAccess.HasOrderChangeDocumentsDebtAccess(long organizationUnitId, long userCode)
        {
            var accesses = GetUserFunctionalPrivilegeMasks(FunctionalPrivilegeName.OrderChangeDocumentsDebt, userCode);
            if (!accesses.Any())
            {
                return false;
            }

            var maxAccess = accesses.OrderByDescending(x => x.Priority).Select(x => (OrderChangeDocumentsDebtAccess)x.Mask).First();
            switch (maxAccess)
            {
                case OrderChangeDocumentsDebtAccess.OrganizationUnit:
                    return _finder.FindAll<UserOrganizationUnit>()
                        .Any(x => x.UserId == userCode && x.OrganizationUnitId == organizationUnitId);

                case OrderChangeDocumentsDebtAccess.Full:
                    return true;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        private static class QueryableHelper
        {
            // TODO: криво беру, надо по умней брать
            private static readonly MethodInfo WhereMethod = typeof(Queryable).GetMethods().Where(x => x.Name == "Where").ToArray()[0];
            private static readonly MethodInfo ContainsInt32Method = typeof(Enumerable).GetMethods().Where(x => x.Name == "Contains").ToArray()[0].MakeGenericMethod(typeof(int));
            private static readonly MethodInfo ContainsInt64Method = typeof(Enumerable).GetMethods().Where(x => x.Name == "Contains").ToArray()[0].MakeGenericMethod(typeof(long));

            public static IQueryable CreateRestrictedQuery(IQueryable query, Type entityType, IQueryable<long> ownerCodes, IEnumerable<int> sharedIds)
            {
                var xParameter = Expression.Parameter(entityType, "x");

                Expression ownerCodesContainsX = null;
                if (ownerCodes.Any())
                {
                    var ownerCodeProperty = entityType.GetProperty("OwnerCode");
                    if (ownerCodeProperty == null)
                    {
                        throw new ArgumentException("В типе не определёно свойство OwnerCode");
                    }

                    // ownerCodes.Contains(x.OwnerCode)
                    var xOwnerCodeProperty = Expression.Property(xParameter, ownerCodeProperty);
                    var ownerCodesConstant = Expression.Constant(ownerCodes);
                    ownerCodesContainsX = Expression.Call(ContainsInt64Method, ownerCodesConstant, xOwnerCodeProperty);
                }

                Expression sharedIdsContainsX = null;
                if (sharedIds.Any())
                {
                    var idProperty = entityType.GetProperty("Id");
                    if (idProperty == null)
                    {
                        throw new ArgumentException("В типе не определёно свойство Id");
                    }

                    // sharedIds.Contains(x.Id)
                    var xIdProperty = Expression.Property(xParameter, idProperty);
                    var sharedIdsConstant = Expression.Constant(sharedIds);
                    sharedIdsContainsX = Expression.Call(ContainsInt32Method, sharedIdsConstant, xIdProperty);
                }

                // true, ownerCodes.Contains(x.OwnerCode), sharedIds.Contains(x.Id) или (ownerCodes.Contains(x.OwnerCode) || sharedIds.Contains(x.Id))
                Expression orExpression;
                if (ownerCodesContainsX == null)
                {
                    orExpression = sharedIdsContainsX ?? Expression.Constant(true);
                }
                else
                {
                    orExpression = sharedIdsContainsX == null ? ownerCodesContainsX : Expression.OrElse(ownerCodesContainsX, sharedIdsContainsX);
                }
               
                // query.Where(x => ...)
                var lambdaExpression = Expression.Lambda(orExpression, xParameter);
                var whereMethod = WhereMethod.MakeGenericMethod(entityType);
                var whereExpression = Expression.Call(whereMethod, query.Expression, lambdaExpression);

                return query.Provider.CreateQuery(whereExpression);
            }
        }
    }
}
