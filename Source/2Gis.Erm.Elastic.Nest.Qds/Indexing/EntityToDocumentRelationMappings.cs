using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Etl;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Indexing
{
    public static class EntityToDocumentRelationMappings
    {
        public static IEnumerable<DocumentWrapper<ClientGridDoc>> SelectClientGridDoc(IQueryable<Client> query)
        {
            var documents = query.Select(x => new
            {
                x.Id,
                x.Name,
                x.MainAddress,
                x.TerritoryId,
                x.OwnerCode,
                x.IsAdvertisingAgency,
                x.MainFirmId,
                x.MainPhoneNumber,
                x.CreatedOn,
                x.LastQualifyTime,
                x.LastDisqualifyTime,
                x.IsActive,
                x.IsDeleted,
                x.InformationSource,
                x.Timestamp,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<ClientGridDoc>
            {
                Id = x.Id.ToString(),
                TimeStamp = x.Timestamp,
                Document = new ClientGridDoc
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    MainAddress = x.MainAddress,
                    IsAdvertisingAgency = x.IsAdvertisingAgency,
                    MainPhoneNumber = x.MainPhoneNumber,
                    CreatedOn = x.CreatedOn,
                    LastQualifyTime = x.LastQualifyTime,
                    LastDisqualifyTime = x.LastDisqualifyTime,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    InformationSourceEnum = (InformationSource)x.InformationSource,

                    // parts
                    TerritoryId = x.TerritoryId.ToString(),
                    OwnerCode = x.OwnerCode.ToString(),
                    MainFirmId = x.MainFirmId.ToString(),
                },
            });

            return documents;
        }

        public static IEnumerable<DocumentWrapper<FirmGridDoc>> SelectFirmGridDoc(IQueryable<Firm> query)
        {
            var documents = query.Select(x => new
            {
                x.Id,
                x.Name,
                x.PromisingScore,
                x.LastQualifyTime,
                x.LastDisqualifyTime,
                x.IsActive,
                x.IsDeleted,
                x.ClosedForAscertainment,

                x.ClientId,
                x.OwnerCode,
                x.TerritoryId,
                x.OrganizationUnitId,
                x.Timestamp,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<FirmGridDoc>
            {
                Id = x.Id.ToString(),
                TimeStamp = x.Timestamp,
                Document = new FirmGridDoc
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    PromisingScore = x.PromisingScore,
                    LastQualifyTime = x.LastQualifyTime,
                    LastDisqualifyTime = x.LastDisqualifyTime,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    ClosedForAscertainment = x.ClosedForAscertainment,

                    // parts
                    ClientId = x.ClientId.ToString(),
                    OwnerCode = x.OwnerCode.ToString(),
                    TerritoryId = x.TerritoryId.ToString(),
                    OrganizationUnitId = x.OrganizationUnitId.ToString(),
                },
            });

            return documents;
        }

        public static IEnumerable<DocumentWrapper<TerritoryDoc>> SelectTerritoryDoc(IQueryable<Territory> query)
        {
            var documents = query.Select(x => new
            {
                x.Id,
                x.Name,
                x.Timestamp,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<TerritoryDoc>
            {
                Id = x.Id.ToString(),
                TimeStamp = x.Timestamp,
                Document = new TerritoryDoc
                {
                    Id = x.Id,
                    Name = x.Name,
                },
            });

            return documents;
        }
    }

    public sealed class OrderToOrderGridDocRelation : DefaultEntityToDocumentRelation<Order, OrderGridDoc>
    {
        private readonly ILocalizationSettings _localizationSettings;

        public OrderToOrderGridDocRelation(IFinder finder, IElasticApi elasticApi, IDocumentRelationFactory documentRelationFactory, ILocalizationSettings localizationSettings)
            : base(finder, elasticApi, documentRelationFactory)
        {
            _localizationSettings = localizationSettings;

            SelectDocumentsFunc = SelectDocumentsFuncInternal;
        }

        public IEnumerable<DocumentWrapper<OrderGridDoc>> SelectDocumentsFuncInternal(IQueryable<Order> query)
        {
            var documents = query.Select(x => new
            {
                x.Id,
                x.Number,

                x.BeginDistributionDate,
                x.EndDistributionDatePlan,
                x.EndDistributionDateFact,

                x.IsActive,
                x.IsDeleted,
                x.HasDocumentsDebt,

                x.CreatedOn,
                x.ModifiedOn,

                x.PayablePlan,
                WorkflowStepEnum = (OrderState)x.WorkflowStepId,

                x.AmountToWithdraw,
                x.AmountWithdrawn,

                x.Timestamp,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<OrderGridDoc>
            {
                Id = x.Id.ToString(),
                TimeStamp = x.Timestamp,
                Document = new OrderGridDoc
                {
                    Id = x.Id,

                    Number = x.Number,

                    BeginDistributionDate = x.BeginDistributionDate,
                    EndDistributionDatePlan = x.EndDistributionDatePlan,
                    EndDistributionDateFact = x.EndDistributionDateFact,

                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    HasDocumentsDebt = x.HasDocumentsDebt,

                    CreatedOn = x.CreatedOn,
                    ModifiedOn = x.ModifiedOn,

                    PayablePlan = (double)x.PayablePlan,
                    WorkflowStep = x.WorkflowStepEnum.ToStringLocalized(EnumResources.ResourceManager, _localizationSettings.ApplicationCulture),

                    AmountToWithdraw = (double)x.AmountToWithdraw,
                    AmountWithdrawn = (double)x.AmountWithdrawn,
                },
            });

            return documents;
        }
    }

    public sealed class UserToUserDocRelation : DefaultEntityToDocumentRelation<User, UserDoc>
    {
        private readonly IFinder _finder;
        private readonly IEntityToDocumentRelationMetadataContainer _metadataContainer;

        public UserToUserDocRelation(IFinder finder, IElasticApi elasticApi, IDocumentRelationFactory documentRelationFactory, IEntityToDocumentRelationMetadataContainer metadataContainer)
            : base(finder, elasticApi, documentRelationFactory)
        {
            _finder = finder;
            _metadataContainer = metadataContainer;

            SelectDocumentsFunc = SelectDocumentsFuncInternal;
        }

        private IEnumerable<DocumentWrapper<UserDoc>> SelectDocumentsFuncInternal(IQueryable<User> query)
        {
            // индексируем только операции чтения
            const int ReadEntityOperationIdentity = 1;

            var dtos = query.Select(x => new
            {
                x.Id,
                x.DepartmentId,
                x.Department.LeftBorder,
                x.Department.RightBorder,

                x.DisplayName,
                x.Timestamp,

                TerritoryIds = x.UserTerritories.Where(y => !y.IsDeleted).Select(y => y.TerritoryId).Distinct(),
                OrganizationUnitIds = x.UserOrganizationUnits.Select(y => y.OrganizationUnitId).Distinct(),

                Permissions = x.UserRoles.Select(y => y.Role).SelectMany(y => y.RolePrivileges)
                    .Where(y => y.Privilege.Operation == ReadEntityOperationIdentity)
                    .Select(y => new
                    {
                        EntityName = (EntityName)y.Privilege.EntityType.Value,
                        Mask = (EntityPrivilegeDepthState)y.Mask,
                    })
                    .Distinct()
                    .Where(y => y.Mask != EntityPrivilegeDepthState.None),
            });

            var departments = _finder.FindAll<Department>().ToArray();

            var documentWrappers = dtos
                .AsEnumerable()
                .Select(x =>
                {
                    var entityPermissions = x.Permissions.GroupBy(y => y.EntityName, y => y.Mask)
                    .SelectMany(y =>
                    {
                        var entityType = y.Key.AsEntityType();
                        var metadatas = _metadataContainer.GetMetadatasForEntityType(entityType)
                            .Where(metadata => typeof(IAuthorizationDoc).IsAssignableFrom(metadata.DocumentType));

                        return metadatas.SelectMany(metadata =>
                        {
                            var operation = "List/" + metadata.DocumentType.Name;

                            return y.Select(z =>
                            {
                                switch (z)
                                {
                                    case EntityPrivilegeDepthState.User:
                                        return new OperationPermission
                                        {
                                            Operation = operation,
                                            Tags = new[] { UserIdTag(x.Id) }
                                        };

                                    case EntityPrivilegeDepthState.Department:
                                        return new OperationPermission
                                        {
                                            Operation = operation,
                                            Tags = new[] { DepartmentIdTag(x.DepartmentId) }
                                        };

                                    case EntityPrivilegeDepthState.DepartmentAndChilds:
                                        {
                                            var childDepartmentsIds = departments.Where(p => p.LeftBorder >= x.LeftBorder && p.RightBorder <= x.RightBorder).Select(p => DepartmentIdTag(p.Id)).ToArray();

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
                    })
                    .ToArray();

                    return new DocumentWrapper<UserDoc>
                    {
                        Id = x.Id.ToString(),
                        TimeStamp = x.Timestamp,
                        Document = new UserDoc
                        {
                            Id = x.Id,
                            Name = x.DisplayName,
                            TerritoryIds = x.TerritoryIds.Select(y => y.ToString()).ToArray(),
                            OrganizationUnitIds = x.OrganizationUnitIds.Select(y => y.ToString()).ToArray(),
                            Permissions = entityPermissions,
                            Tags = new[]
                            {
                                UserIdTag(x.Id),
                                DepartmentIdTag(x.DepartmentId)
                            }
                        },
                    };
                });

            return documentWrappers;
        }

        private static string UserIdTag(long id)
        {
            return "UserId/" + id;
        }

        private static string DepartmentIdTag(long id)
        {
            return "DepartmentId/" + id;
        }
    }
}
