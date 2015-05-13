using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Replication.Metadata.Features;
using DoubleGis.Erm.Qds.Operations.Indexing;

using FastMember;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static partial class ProjectionSpecs
    {
        public static class Users
        {
            public static ISelectSpecification<User, object> Select()
            {
                return new SelectSpecification<User, object>(
                    x => new
                             {
                                 x.Id,
                                 x.Account,
                                 x.FirstName,
                                 x.LastName,
                                 x.DisplayName,
                                 x.DepartmentId,
                                 x.ParentId,
                                 x.IsActive,
                                 x.IsDeleted
                             });
            }

            public static IProjectSpecification<ObjectAccessor, IndexedDocumentWrapper<UserGridDoc>> Project()
            {
                return new ProjectSpecification<ObjectAccessor, IndexedDocumentWrapper<UserGridDoc>>(
                    x =>
                        {
                            var accessor = x.BasedOn<User>();
                            return new IndexedDocumentWrapper<UserGridDoc>
                                       {
                                           Id = accessor.Get(c => c.Id).ToString(),
                                           Document = new UserGridDoc
                                                          {
                                                              Id = accessor.Get(c => c.Id),
                                                              Account = accessor.Get(c => c.Account),
                                                              FirstName = accessor.Get(c => c.FirstName),
                                                              LastName = accessor.Get(c => c.LastName),
                                                              DisplayName = accessor.Get(c => c.DisplayName),
                                                              IsActive = accessor.Get(c => c.IsActive),
                                                              IsDeleted = accessor.Get(c => c.IsDeleted),

                                                              // relations
                                                              ParentId = GetRelatedId(accessor.Get(c => c.ParentId)),
                                                              DepartmentId = GetRelatedId(accessor.Get(c => c.DepartmentId)),
                                                          }
                                       };
                        });
            }

            public static ISelectSpecification<User, object> SelectUserPrivilegesContainer()
            {
                // индексируем только операции чтения
                const int ReadEntityOperationIdentity = 1;

                return new SelectSpecification<User, object>(
                    x => new UserPrivilegesContainer
                             {
                                 UserId = x.Id,
                                 DepartmentId = x.DepartmentId,
                                 LeftBorder = x.Department.LeftBorder,
                                 RightBorder = x.Department.RightBorder,

                                 Permissions = x.UserRoles
                                                .Select(y => y.Role)
                                                .SelectMany(y => y.RolePrivileges)
                                                .Where(y => y.Privilege.Operation == ReadEntityOperationIdentity)
                                                .Select(y => new UserPermissions
                                                                 {
                                                                     EntityName = EntityType.Instance.Parse(y.Privilege.EntityType.Value),
                                                                     Mask = (EntityPrivilegeDepthState)y.Mask,
                                                                 })
                                                .Distinct()
                                                .Where(y => y.Mask != EntityPrivilegeDepthState.None),
                             });
            }

            public static IProjectSpecification<ObjectAccessor, IndexedDocumentWrapper<UserAuthorizationDoc>> ProjectToUserAuthorizationDoc(
                IEnumerable<Department> departments,
                IEnumerable<IEntityRelationFeature> relationFeatures)
            {
                return new ProjectSpecification<ObjectAccessor, IndexedDocumentWrapper<UserAuthorizationDoc>>(
                    x =>
                        {
                            var container = (UserPrivilegesContainer)x.Target;

                            var userId = container.UserId;
                            var departmentId = container.DepartmentId;
                            var leftBorder = container.LeftBorder;
                            var rightBorder = container.RightBorder;

                            return new IndexedDocumentWrapper<UserAuthorizationDoc>
                                       {
                                           Id = container.UserId.ToString(),
                                           Document = new UserAuthorizationDoc
                                                          {
                                                              Permissions =
                                                                  container.Permissions
                                                                           .GroupBy(y => y.EntityName, y => y.Mask)
                                                                           .SelectMany(states =>
                                                                                           {
                                                                                               var entityType = states.Key.AsEntityType();
                                                                                               var documentTypes = relationFeatures
                                                                                                   .Where(feature => feature.EntityType == entityType &&
                                                                                                                     typeof(IAuthorizationDoc).IsAssignableFrom(feature.DocumentType))
                                                                                                   .Select(feature => feature.DocumentType);

                                                                                               return GetOperationPermissions(documentTypes,
                                                                                                                              departments,
                                                                                                                              states,
                                                                                                                              userId,
                                                                                                                              departmentId,
                                                                                                                              leftBorder,
                                                                                                                              rightBorder);
                                                                                           })
                                                                           .ToArray(),
                                                              Tags = new[]
                                                                         {
                                                                             UserIdTag(container.UserId),
                                                                             DepartmentIdTag(container.DepartmentId)
                                                                         }
                                                          }
                                       };
                        });
            }

            private static string UserIdTag(dynamic id)
            {
                return "UserId/" + id;
            }

            private static string DepartmentIdTag(dynamic id)
            {
                return "DepartmentId/" + id;
            }

            private static IEnumerable<OperationPermission> GetOperationPermissions(
                IEnumerable<Type> types,
                IEnumerable<Department> departments,
                IEnumerable<EntityPrivilegeDepthState> entityPrivilegeDepthStates,
                long userId,
                long departmentId,
                int? leftBorder,
                int? rightBorder)
            {
                return types
                    .SelectMany(type =>
                                    {
                                        var operation = "List/" + type.Name;
                                        return entityPrivilegeDepthStates
                                            .Select(state =>
                                                        {
                                                            switch (state)
                                                            {
                                                                case EntityPrivilegeDepthState.User:
                                                                    return new OperationPermission
                                                                               {
                                                                                   Operation = operation,
                                                                                   Tags = new[] { UserIdTag(userId) }
                                                                               };

                                                                case EntityPrivilegeDepthState.Department:
                                                                    return new OperationPermission
                                                                               {
                                                                                   Operation = operation,
                                                                                   Tags = new[] { DepartmentIdTag(departmentId) }
                                                                               };

                                                                case EntityPrivilegeDepthState.DepartmentAndChilds:
                                                                {
                                                                    var childDepartmentsIds = departments.Where(p => p.LeftBorder >= leftBorder &&
                                                                                                                     p.RightBorder <= rightBorder)
                                                                                                         .Select(p => DepartmentIdTag(p.Id))
                                                                                                         .ToArray();

                                                                    return new OperationPermission
                                                                               {
                                                                                   Operation = operation,
                                                                                   Tags = childDepartmentsIds,
                                                                               };
                                                                }
                                                                case EntityPrivilegeDepthState.Organization:
                                                                    return new OperationPermission
                                                                               {
                                                                                   Operation = operation,
                                                                                   Tags = new string[0],
                                                                               };
                                                                default:
                                                                    throw new ArgumentException();
                                                            }
                                                        });
                                    });
            }

            public class UserPermissions
            {
                public IEntityType EntityName { get; set; }
                public EntityPrivilegeDepthState Mask { get; set; }
            }

            public class UserPrivilegesContainer
            {
                public long UserId { get; set; }
                public long DepartmentId { get; set; }
                public int? LeftBorder { get; set; }
                public int? RightBorder { get; set; }
                public IEnumerable<UserPermissions> Permissions { get; set; }
            }
        }
    }
}