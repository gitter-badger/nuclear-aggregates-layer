using System;
using System.Collections.Generic;
using System.Globalization;
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
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Operations.Indexing;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static class EntityToDocumentRelationMappings
    {
        public static IEnumerable<DocumentWrapper<ClientGridDoc>> SelectClientGridDoc(IQueryable<Client> query, CultureInfo cultureInfo)
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
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<ClientGridDoc>
            {
                Id = x.Id.ToString(),
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

                    // relations
                    TerritoryId = GetRelatedId(x.TerritoryId),
                    OwnerCode = GetRelatedId(x.OwnerCode),
                    MainFirmId = GetRelatedId(x.MainFirmId),
                },
            });

            return documents;
        }

        public static IEnumerable<DocumentWrapper<FirmGridDoc>> SelectFirmGridDoc(IQueryable<Firm> query, CultureInfo cultureInfo)
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
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<FirmGridDoc>
            {
                Id = x.Id.ToString(),
                Document = new FirmGridDoc
                {
                    Id = x.Id,
                    Name = x.Name,
                    PromisingScore = x.PromisingScore,
                    LastQualifyTime = x.LastQualifyTime,
                    LastDisqualifyTime = x.LastDisqualifyTime,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    ClosedForAscertainment = x.ClosedForAscertainment,

                    // relations
                    //ClientId = GetRelatedId(x.ClientId),
                    OwnerCode = GetRelatedId(x.OwnerCode),
                    //TerritoryId = GetRelatedId(x.TerritoryId),
                    OrganizationUnitId = GetRelatedId(x.OrganizationUnitId),
                },
            });

            return documents;
        }

        public static IEnumerable<DocumentWrapper<UserGridDoc>> SelectUserGridDoc(IQueryable<User> query, CultureInfo cultureInfo)
        {
            var documents = query.Select(x => new
            {
                x.Id,
                x.Account,
                x.FirstName,
                x.LastName,
                x.DisplayName,
                x.DepartmentId,
                x.ParentId,
                x.IsActive,
                x.IsDeleted,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<UserGridDoc>
            {
                Id = x.Id.ToString(),
                Document = new UserGridDoc
                {
                    Id = x.Id,
                    Account = x.Account,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    DisplayName = x.DisplayName,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,

                    // relations
                    ParentId = GetRelatedId(x.ParentId),
                    DepartmentId = GetRelatedId(x.DepartmentId),
                },
            });

            return documents;
        }

        public static IEnumerable<DocumentWrapper<DepartmentGridDoc>> SelectDepartmentGridDoc(IQueryable<Department> query, CultureInfo cultureInfo)
        {
            var documents = query.Select(x => new
            {
                x.Id,
                x.Name,
                x.ParentId,
                x.IsActive,
                x.IsDeleted,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<DepartmentGridDoc>
            {
                Id = x.Id.ToString(),
                Document = new DepartmentGridDoc
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,

                    // relations
                    ParentId = GetRelatedId(x.ParentId),
                },
            });

            return documents;
        }

        public static IEnumerable<DocumentWrapper<CurrencyGridDoc>> SelectCurrencyGridDoc(IQueryable<Currency> query, CultureInfo cultureInfo)
        {
            var documents = query.Select(x => new
            {
                x.Id,
                x.Name,
                x.ISOCode,
                x.Symbol,
                x.IsBase,
                x.IsActive,
                x.IsDeleted,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<CurrencyGridDoc>
            {
                Id = x.Id.ToString(),
                Document = new CurrencyGridDoc
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsoCode = x.ISOCode,
                    Symbol = x.Symbol,
                    IsBase = x.IsBase,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                },
            });

            return documents;
        }

        public static IEnumerable<DocumentWrapper<CountryGridDoc>> SelectCountryGridDoc(IQueryable<Country> query, CultureInfo cultureInfo)
        {
            var documents = query.Select(x => new
            {
                x.Id,
                x.Name,
                x.IsoCode,
                x.IsDeleted,
                x.CurrencyId,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<CountryGridDoc>
            {
                Id = x.Id.ToString(),
                Document = new CountryGridDoc
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsoCode = long.Parse(x.IsoCode),
                    IsDeleted = x.IsDeleted,

                    // relations
                    CurrencyId = GetRelatedId(x.CurrencyId)
                },
            });

            return documents;
        }

        public static IEnumerable<DocumentWrapper<OrgUnitGridDoc>> SelectOrgUnitGridDoc(IQueryable<OrganizationUnit> query, CultureInfo cultureInfo)
        {
            var documents = query.Select(x => new
            {
                x.Id,
                x.DgppId,
                x.ReplicationCode,
                x.Name,
                x.FirstEmitDate,
                x.ErmLaunchDate,
                x.InfoRussiaLaunchDate,
                ErmLaunched = x.ErmLaunchDate != null,
                x.IsActive,
                x.IsDeleted,

                x.CountryId,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<OrgUnitGridDoc>
            {
                Id = x.Id.ToString(),
                Document = new OrgUnitGridDoc
                {
                    Id = x.Id,
                    DgppId = x.DgppId,
                    ReplicationCode = x.ReplicationCode.ToString(),
                    Name = x.Name,

                    FirstEmitDate = x.FirstEmitDate,
                    ErmLaunchDate = x.ErmLaunchDate,
                    InfoRussiaLaunchDate = x.InfoRussiaLaunchDate,

                    ErmLaunched = x.ErmLaunched,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,

                    // relations
                    CountryId = GetRelatedId(x.CountryId)
                },
            });

            return documents;
        }

        public static IEnumerable<DocumentWrapper<LegalPersonGridDoc>> SelectLegalPersonGridDoc(IQueryable<LegalPerson> query, CultureInfo cultureInfo)
        {
            var documents = query.Select(x => new
            {
                x.Id,
                x.LegalName,
                x.ShortName,
                x.Inn,
                x.Kpp,
                x.LegalAddress,
                x.PassportNumber,
                x.CreatedOn,
                x.IsActive,
                x.IsDeleted,

                x.OwnerCode,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<LegalPersonGridDoc>
            {
                Id = x.Id.ToString(),
                Document = new LegalPersonGridDoc
                {
                    Id = x.Id,
                    LegalName = x.LegalName,
                    ShortName = x.ShortName,
                    Inn = x.Inn,
                    Kpp = x.Kpp,
                    LegalAddress = x.LegalAddress,
                    PassportNumber = x.PassportNumber,
                    CreatedOn = x.CreatedOn,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,

                    // relations
                    OwnerCode = GetRelatedId(x.OwnerCode)
                },
            });

            return documents;
        }

        public static IEnumerable<DocumentWrapper<BargainGridDoc>> SelectBargainGridDoc(IQueryable<Bargain> query, CultureInfo cultureInfo)
        {
            var documents = query.Select(x => new
            {
                x.Id,
                x.Number,
                x.CreatedOn,
                x.BargainEndDate,
                BargainKind = (BargainKind)x.BargainKind,
                x.IsActive,
                x.IsDeleted,

                x.CustomerLegalPersonId,
                x.OwnerCode,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<BargainGridDoc>
            {
                Id = x.Id.ToString(),
                Document = new BargainGridDoc
                {
                    Id = x.Id,
                    Number = x.Number,
                    BargainKindEnum = x.BargainKind,
                    BargainKind = x.BargainKind.ToStringLocalized(EnumResources.ResourceManager, cultureInfo),
                    BargainEndDate = x.BargainEndDate,
                    CreatedOn = x.CreatedOn,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,

                    // relations
                    OwnerCode = GetRelatedId(x.OwnerCode),
                    LegalPersonId = GetRelatedId(x.CustomerLegalPersonId),
                },
            });

            return documents;
        }

        public static IEnumerable<DocumentWrapper<OrderGridDoc>> SelectOrderGridDoc(IQueryable<Order> query, CultureInfo cultureInfo)
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

                x.FirmId,
                x.OwnerCode,
                x.SourceOrganizationUnitId,
                x.DestOrganizationUnitId,
                x.LegalPersonId,
                x.BargainId,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<OrderGridDoc>
            {
                Id = x.Id.ToString(),
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
                    WorkflowStep = x.WorkflowStepEnum.ToStringLocalized(EnumResources.ResourceManager, cultureInfo),

                    AmountToWithdraw = (double)x.AmountToWithdraw,
                    AmountWithdrawn = (double)x.AmountWithdrawn,

                    // relations
                    FirmId = GetRelatedId(x.FirmId),
                    OwnerCode = GetRelatedId(x.OwnerCode),
                    SourceOrganizationUnitId = GetRelatedId(x.SourceOrganizationUnitId),
                    DestOrganizationUnitId = GetRelatedId(x.DestOrganizationUnitId),
                    LegalPersonId = GetRelatedId(x.LegalPersonId),
                    BargainId = GetRelatedId(x.BargainId),
                },
            });

            return documents;
        }

        public static IEnumerable<DocumentWrapper<TerritoryDoc>> SelectTerritoryDoc(IQueryable<Territory> query, CultureInfo cultureInfo)
        {
            var documents = query.Select(x => new
            {
                x.Id,
                x.Name,
            })
            .AsEnumerable()
            .Select(x => new DocumentWrapper<TerritoryDoc>
            {
                Id = x.Id.ToString(),
                Document = new TerritoryDoc
                {
                    Id = x.Id,
                    Name = x.Name,
                },
            });

            return documents;
        }

        private static string GetRelatedId(long? relatedId)
        {
            return (relatedId == null) ? null : relatedId.Value.ToString();
        }
    }

    public sealed class UserToUserAuthorizationDocRelation : DefaultEntityToDocumentRelation<User, UserAuthorizationDoc>
    {
        private readonly IFinder _finder;
        private readonly IEntityToDocumentRelationMetadataContainer _metadataContainer;

        public UserToUserAuthorizationDocRelation(IFinder finder, IEntityToDocumentRelationMetadataContainer metadataContainer, ILocalizationSettings localizationSettings)
            : base(finder, localizationSettings)
        {
            _finder = finder;
            _metadataContainer = metadataContainer;

            SelectDocumentsFunc = SelectDocumentsFuncInternal;
        }

        private IEnumerable<DocumentWrapper<UserAuthorizationDoc>> SelectDocumentsFuncInternal(IQueryable<User> query, CultureInfo cultureInfo)
        {
            // индексируем только операции чтения
            const int ReadEntityOperationIdentity = 1;

            var dtos = query.Select(x => new
            {
                x.Id,
                x.DepartmentId,
                x.Department.LeftBorder,
                x.Department.RightBorder,

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

                    return new DocumentWrapper<UserAuthorizationDoc>
                    {
                        Id = x.Id.ToString(),
                        Document = new UserAuthorizationDoc
                        {
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