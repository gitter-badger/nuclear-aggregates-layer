using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Roles;
using DoubleGis.Erm.BLCore.API.Aggregates.Roles.Dto;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.Aggregates.Roles
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IRepository<Role> _roleGenericRepository;
        private readonly IRepository<RolePrivilege> _rolePrivilegeGenericRepository;
        private readonly IRepository<UserRole> _userRolesGenericRepository;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly IIdentityProvider _identityProvider;

        public RoleRepository(
            IFinder finder,
            IUserContext userContext, 
            IIdentityProvider identityProvider,
            IRepository<Role> roleGenericRepository, 
            IRepository<UserRole> userRolesGenericRepository,
            IRepository<RolePrivilege> rolePrivilegeGenericRepository)
        {
            _roleGenericRepository = roleGenericRepository;
            _rolePrivilegeGenericRepository = rolePrivilegeGenericRepository;
            _userRolesGenericRepository = userRolesGenericRepository;
            _finder = finder;
            _userContext = userContext;
            _identityProvider = identityProvider;
        }

        public int Delete(Role role)
        {
            var relatedEntities = _finder.Find(Specs.Find.ById<Role>(role.Id))
                .Select(r => new
                    {
                        r.RolePrivileges,
                        r.UserRoles,
                    })
                .Single();

            //Удалить привязку привилегий к роли
            foreach (var rolePrivilege in relatedEntities.RolePrivileges)
            {
                _rolePrivilegeGenericRepository.Delete(rolePrivilege);
            }
            _rolePrivilegeGenericRepository.Save();

            //Удалить привязку пользователей к роли
            foreach (var userRole in relatedEntities.UserRoles)
            {
                _userRolesGenericRepository.Delete(userRole);
            }
            _userRolesGenericRepository.Save();

            _roleGenericRepository.Delete(role);
            return _roleGenericRepository.Save();
        }

        int IDeleteAggregateRepository<Role>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<Role>(entityId)).Single();
            return Delete(entity);
        }

        public IEnumerable<EntityPrivilegeInfo> GetEntityPrivileges(long roleId)
        {
            var entityPrivilegeInfos = _finder.For<Privilege>()
                                              .Where(x => x.EntityType != null)
                                              .GroupBy(x => x.EntityType)
                                              .Select(x => new
                                                  {
                                                      EntityName = x.Key,
                                                      PrivilegeInfoList = x.Select(p => new PrivilegeDto
                                                          {
                                                              PrivilegeId = p.Id,
                                                              Operation = (EntityAccessTypes)p.Operation,
                                                              PrivilegeDepthMask = (EntityPrivilegeDepthState)p.RolePrivileges
                                                                                                               .Where(rp => rp.RoleId == roleId)
                                                                                                               .Select(rp => rp.Mask)
                                                                                                               .FirstOrDefault(),
                                                          })
                                                  })
                                              .AsEnumerable()
                                              .Select(x => new EntityPrivilegeInfo
                                                  {
                                                      EntityName = EntityType.Instance.Parse(x.EntityName.Value),
                                                      PrivilegeInfoList = x.PrivilegeInfoList
                                                  })
                                              .ToArray();

            foreach (var entityPrivilegeInfo in entityPrivilegeInfos)
            {
                entityPrivilegeInfo.EntityNameLocalized = entityPrivilegeInfo.EntityName.ToStringLocalized(EnumResources.ResourceManager, 
                    _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                foreach (var privilegeInfo in entityPrivilegeInfo.PrivilegeInfoList)
                {
                    privilegeInfo.NameLocalized = privilegeInfo.Operation.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                }
            }

            return entityPrivilegeInfos;
        }

        public IEnumerable<FunctionalPrivilegeInfo> GetFunctionalPrivileges(long roleId)
        {
            return _finder.For<Privilege>()
                    .Where(x => x.EntityType == null)
                    .Select(x => new
                    {
                        PrivilegeId = x.Id,
                        Operation = (FunctionalPrivilegeName)x.Operation,
                        Mask = x.RolePrivileges.Where(y => y.RoleId == roleId).Select(y => y.Mask).FirstOrDefault(),
                        Priority = 0
                    }).ToArray()
                    .Select(x => new FunctionalPrivilegeInfo
                    {
                        PrivilegeId = x.PrivilegeId,
                        NameLocalized = x.Operation.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                        Mask = x.Mask,
                        Priority = 0
                    });
        }

        public void UpdateEntityPrivileges(long roleId, PrivilegeDto[] privilegeInfos)
        {
            foreach (var privilegeInfo in privilegeInfos)
            {
                var privilegeId = privilegeInfo.PrivilegeId;

                var rolePrivilege = _finder.Find<RolePrivilege>(x => x.RoleId == roleId && x.PrivilegeId == privilegeId).SingleOrDefault();
                if (rolePrivilege == null)
                {
                    if (privilegeInfo.PrivilegeDepthMask == EntityPrivilegeDepthState.None)
                    {
                        throw new NotificationException(BLResources.InvalidPrivelegeDepthMask);
                    }

                    rolePrivilege = new RolePrivilege
                    {
                        RoleId = roleId,
                        PrivilegeId = privilegeId,
                        Mask = (int)privilegeInfo.PrivilegeDepthMask,
                        Priority = 0,
                    };

                    _identityProvider.SetFor(rolePrivilege);
                    _rolePrivilegeGenericRepository.Add(rolePrivilege);

                    continue;
                }

                if (privilegeInfo.PrivilegeDepthMask == EntityPrivilegeDepthState.None)
                {
                    _rolePrivilegeGenericRepository.Delete(rolePrivilege);
                }
                else
                {
                    rolePrivilege.Mask = (int)privilegeInfo.PrivilegeDepthMask;
                    rolePrivilege.Priority = 0;

                    _rolePrivilegeGenericRepository.Update(rolePrivilege);
                }
            }

            _rolePrivilegeGenericRepository.Save();
        }

        public void UpdateFunctionalPrivileges(long roleId, FunctionalPrivilegeInfo[] privilegeInfos)
        {
            foreach (var privilegeInfo in privilegeInfos)
            {
                var privilegeId = privilegeInfo.PrivilegeId;

                var rolePrivilege = _finder.Find<RolePrivilege>(x => x.RoleId == roleId && x.PrivilegeId == privilegeId).SingleOrDefault();
                if (rolePrivilege == null)
                {
                    if (privilegeInfo.Mask == 0)
                    {
                        throw new NotificationException(BLResources.InvalidPrivelegeDepthMask);
                    }

                    rolePrivilege = new RolePrivilege
                    {
                        RoleId = roleId,
                        PrivilegeId = privilegeId,
                        Mask = privilegeInfo.Mask,
                        Priority = privilegeInfo.Priority,
                    };

                    _identityProvider.SetFor(rolePrivilege);
                    _rolePrivilegeGenericRepository.Add(rolePrivilege);

                    continue;
                }

                if (privilegeInfo.Mask == 0)
                {
                    _rolePrivilegeGenericRepository.Delete(rolePrivilege);
                }
                else
                {
                    rolePrivilege.Mask = privilegeInfo.Mask;
                    rolePrivilege.Priority = privilegeInfo.Priority;
                    _rolePrivilegeGenericRepository.Update(rolePrivilege);
                }
            }

            _rolePrivilegeGenericRepository.Save();
        }

        public void CreateOrUpdate(Role role)
        {
            var roleExists = _finder.Find<Role>(x => x.Name == role.Name && x.Id != role.Id).Any();
            if (roleExists)
            {
                throw new NotificationException(BLResources.RecordAlreadyExists);
            }

            if (role.IsNew())
            {
                _roleGenericRepository.Add(role);
            }
            else
            {
                _roleGenericRepository.Update(role);
            }

            _roleGenericRepository.Save();
        }

        public bool HasUsers(long roleId)
        {
            var hasUsers = _finder.Find<Role>(x => x.Id == roleId).SelectMany(x => x.UserRoles).Select(x => x.User).Distinct().Any(x => x.IsActive && !x.IsDeleted);
            return hasUsers;
        }

        public IEnumerable<FunctionalPrivilegeInfo> FindAllFunctionalPriveleges()
        {
            return _finder.For<FunctionalPrivilegeDepth>()
                          .Select(x => new FunctionalPrivilegeInfo
                                           {
                                               PrivilegeId = x.PrivilegeId,
                                               NameLocalized = x.LocalResourceName,
                                               Mask = x.Mask,
                                               Priority = x.Priority
                                           }).AsEnumerable();
        }
    }
}
