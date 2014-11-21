using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListClientService : ListEntityDtoServiceBase<Client, ListClientDto>
    {
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;
        private readonly IUserRepository _userRepository;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public ListClientService(
            IUserRepository userRepository,
            ISecurityServiceUserIdentifier userIdentifierService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IFinder finder,
            ISecureFinder secureFinder,
            IUserContext userContext,
            FilterHelper filterHelper,
            IDebtProcessingSettings debtProcessingSettings)
        {
            _userContext = userContext;
            _filterHelper = filterHelper;
            _debtProcessingSettings = debtProcessingSettings;
            _userRepository = userRepository;
            _userIdentifierService = userIdentifierService;
            _functionalAccessService = functionalAccessService;
            _finder = finder;
            _secureFinder = secureFinder;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<Client>();

            bool availableForLinking;
            if (querySettings.TryGetExtendedProperty("AvailableForLinking", out availableForLinking))
            {
                return GetClientsAvailableForLinking(querySettings);
            }

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            var myTerritoryFilter = querySettings.CreateForExtendedProperty<Client, bool>("MyTerritory", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.Territory.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);
            });

            var myBranchFilter = querySettings.CreateForExtendedProperty<Client, bool>("MyBranch", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);
            });

            var debtFilter = querySettings.CreateForExtendedProperty<Client, bool>("WithDebt", info =>
            {
                var minDebtAmount = _debtProcessingSettings.MinDebtAmount;
                return x => x.LegalPersons.Where(y => y.IsActive && !y.IsDeleted)
                          .SelectMany(y => y.Accounts).Where(y => y.IsActive && !y.IsDeleted)
                          .Any(y => y.Balance < minDebtAmount);
            });

            var barterOrdersFilter = querySettings.CreateForExtendedProperty<Client, bool>("WithBarterOrders", info =>
            {
                return x => x.LegalPersons.Any(y => !y.IsDeleted && y.IsActive && y.Orders.Any(z => !z.IsDeleted && z.IsActive && (z.OrderType == OrderType.AdsBarter || z.OrderType == OrderType.ProductBarter || z.OrderType == OrderType.ServiceBarter)));
            });

            var noMakingDecisionsFilter = querySettings.CreateForExtendedProperty<Client, bool>("NoMakingDecisions", info =>
            {
                return x => !x.Contacts.Any(y => !y.IsDeleted && y.IsActive && !y.IsFired && y.AccountRole == AccountRole.MakingDecisions);
            });

            var regionalFilter = querySettings.CreateForExtendedProperty<Client, bool>("IsRegional", info =>
            {
                return x => x.LegalPersons.Any(y => !y.IsDeleted && y.IsActive && y.Orders.Any(z => !z.IsDeleted && z.IsActive && z.SourceOrganizationUnitId != z.DestOrganizationUnitId));
            });

            var dealCountFilter = querySettings.CreateForExtendedProperty<Client, int>("MinDealCount", dealCount =>
            {
                return x => x.Deals.Count(y => !y.IsDeleted && y.IsActive) > dealCount;
            });

            query = query.Filter(_filterHelper, myTerritoryFilter, myBranchFilter, debtFilter, barterOrdersFilter, noMakingDecisionsFilter, regionalFilter, dealCountFilter);

            RemoteCollection<ListClientDto> clients;
            if (TryGetClientsRestrictedByUser(query, querySettings, out clients))
            {
                return clients;
            }

            if (TryGetClientsRestrictedByMergeClientPrivilege(query, querySettings, out clients))
            {
                return clients;
            }

	        // FIXME {s.pomadin, 30.07.2014}: there is no relation between client and activities anymore
			Expression<Func<Client, bool>> with1AppointmentFilter = null;
			Expression<Func<Client, bool>> warmClientTaskFilter = null;
			Expression<Func<Client, bool>> outdatedActivityFilter = null;
//            var with1AppointmentFilter = querySettings.CreateForExtendedProperty<Client, bool>(
//                "With1Appointment",
//                with1Appointment =>
//                {
//                    if (!with1Appointment)
//                    {
//                        return null;
//                    }
//
//
//                    return x =>
//                           x.ActivityInstances.Count(
//                               y => y.Type == (int)ActivityType.Appointment && !y.IsDeleted && y.IsActive
//                                    && y.ActivityPropertyInstances.Any(z => (z.PropertyId == StatusIdentity.Instance.Id && z.NumericValue == 2))) == 1;
//                });
//
//            var warmClientTaskFilter = querySettings.CreateForExtendedProperty<Client, bool>(
//                "WarmClientTask",
//                warmClientTask => client =>
//                    client.ActivityInstances.Any(activity => activity.ActivityPropertyInstances.Any(property => property.PropertyId == TaskTypeIdentity.Instance.Id
//                                                                                                  && property.NumericValue == (int)ActivityTaskType.WarmClient)));
//
//            var outdatedActivityFilter = querySettings.CreateForExtendedProperty<Client, bool>(
//                "Outdated",
//                outdated =>
//                {
//                    if (outdated)
//                    {
//                        return client => client.ActivityInstances.Any(activity => activity.ActivityPropertyInstances.Any(
//                            property => property.PropertyId == ScheduledEndIdentity.Instance.Id && property.DateTimeValue < DateTime.Today));
//                    }
//
//                    return null;
//                });

            var contactFilter = querySettings.CreateForExtendedProperty<Client, long>(
                "ContactId",
                contactId => x => x.Contacts.Any(y => y.Id == contactId));
            var dealFilter = querySettings.CreateForExtendedProperty<Client, long>(
                "DealId",
                dealId => x => x.Deals.Any(y => y.Id == dealId));
            var firmFilter = querySettings.CreateForExtendedProperty<Client, long>(
                "FirmId",
                firmId => x => x.Firms.Any(y => y.Id == firmId));

            return SelectClients(
                query
                    .Where(x => !x.IsDeleted)
                    .Filter(_filterHelper
                    , with1AppointmentFilter
                    , warmClientTaskFilter
                    , outdatedActivityFilter
                    , contactFilter
                    , dealFilter
                    , firmFilter)
                    , querySettings);
        }

        private IRemoteCollection GetClientsAvailableForLinking(QuerySettings querySettings)
        {
            var expr = CreateExpToFilterReserveClientsByPermission();
            var excludeClients = GetClientsAlreadyLinked(querySettings);

            var clientsQuery = _secureFinder.Find(expr)
                                            .Where(c => !excludeClients.Contains(c.Id))
                                            .Where(c => !c.IsAdvertisingAgency);

            return SelectClients(clientsQuery, querySettings);
        }

        private Expression<Func<Client, bool>> CreateExpToFilterReserveClientsByPermission()
        {
            var userCode = _userContext.Identity.Code;
            var reserveRights = GetMaxAccessForReserve(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.ReserveAccess, userCode));
            var reserveUserCode = _userIdentifierService.GetReserveUserIdentity().Code;

            switch (reserveRights)
            {
                case ReserveAccess.None:
                    {
                        return c => c.OwnerCode != reserveUserCode;
                    }

                case ReserveAccess.Territory:
                    {
                        return c => c.OwnerCode != reserveUserCode ||
                                    c.Territory.UserTerritoriesOrganizationUnits.Any(u => u.UserId == userCode);
                    }

                case ReserveAccess.OrganizationUnit:
                    {
                        return c => c.OwnerCode != reserveUserCode ||
                                   (c.Territory.UserTerritoriesOrganizationUnits.Any(u => u.UserId == userCode) ||
                                     c.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(u => u.UserId == userCode));
                    }

                case ReserveAccess.Full:
                    {
                        return c => true;
                    }

                default:
                    throw new NotSupportedException(reserveRights.ToString());
            }
        }

        private IEnumerable<long> GetClientsAlreadyLinked(QuerySettings querySettings)
        {
            bool excludeChildClients;
            querySettings.TryGetExtendedProperty("ExcludeChildClients", out excludeChildClients);

            if (excludeChildClients && querySettings.ParentEntityId.HasValue)
            {
                var currentClient = querySettings.ParentEntityId.Value;
                var list = _finder.Find<ClientLink>(cl => cl.MasterClientId == currentClient && !cl.IsDeleted).Select(cl => cl.ChildClientId).ToList();

                list.Add(currentClient);
                return list;
            }

            return new long[0];
        }

        private static ReserveAccess GetMaxAccessForReserve(int[] accesses)
        {
            if (!accesses.Any())
            {
                return ReserveAccess.None;
            }

            var priorities = new[] { ReserveAccess.None, ReserveAccess.Territory, ReserveAccess.OrganizationUnit, ReserveAccess.Full };

            var maxPriority = accesses.Select(x => Array.IndexOf(priorities, (ReserveAccess)x)).Max();
            return priorities[maxPriority];
        }

        private bool TryGetClientsRestrictedByUser(
            IQueryable<Client> query,
            QuerySettings querySettings,
            out RemoteCollection<ListClientDto> clients)
        {
            clients = null;

            var currentUserFilter = querySettings.CreateForExtendedProperty<Client, bool>(
                "filterToCurrentUser",
                filterToCurrentUser =>
                {
                    var currentUserId = _userContext.Identity.Code;
                    return x => x.OwnerCode == currentUserId;
                });

            var userFilter = querySettings.CreateForExtendedProperty<Client, long>(
                "userId",
                userId => x => x.OwnerCode == userId);

            if (userFilter != null || currentUserFilter != null)
            {
                clients = SelectClients(query
                                    .Where(x => !x.IsDeleted)
                                    .Filter(_filterHelper, userFilter, currentUserFilter)
                                    , querySettings);
                return true;
            }

            return false;
        }

        private bool TryGetClientsRestrictedByMergeClientPrivilege(
            IQueryable<Client> query,
            QuerySettings querySettings,
            out RemoteCollection<ListClientDto> clients)
        {
            clients = null;

            var currentIdentity = _userContext.Identity;

            var restrictForMergeIdFilter = querySettings.CreateForExtendedProperty<Client, long>(
                "restrictForMergeId",
                clientId =>
                {
                    var privelegDepth = GetMaxAccess(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.MergeClients, currentIdentity.Code));
                    switch (privelegDepth)
                    {
                        case MergeClientsAccess.None:
                            {
                                throw new NotificationException(string.Format(BLResources.AccesDeniedForMergingClients, Environment.NewLine));
                            }

                        case MergeClientsAccess.User:
                            {
                                return x => x.OwnerCode == currentIdentity.Code && x.Id != clientId;
                            }

                        case MergeClientsAccess.Department:
                        case MergeClientsAccess.DepartmentWithChilds:
                            {
                                bool useDepartmentsWithChilds = privelegDepth == MergeClientsAccess.DepartmentWithChilds;
                                var departments = _userIdentifierService.GetUserDepartments(currentIdentity.Code, useDepartmentsWithChilds);
                                var ownerDepartments = _userRepository.GetUsersByDepartments(departments).Select(u => u.Id);
                                return c => c.Id != clientId && ownerDepartments.Contains(c.OwnerCode);
                            }
                    }

                    return x => x.Id != clientId;
                });

            if (restrictForMergeIdFilter != null)
            {
                clients = SelectClients(query
                                    .Where(x => !x.IsDeleted)
                                    .Filter(_filterHelper, restrictForMergeIdFilter)
                                    , querySettings);

                return true;
            }

            return false;
        }

        private static MergeClientsAccess GetMaxAccess(int[] accesses)
        {
            if (!accesses.Any())
            {
                return MergeClientsAccess.None;
            }

            var priorities = new[]
                {
                    MergeClientsAccess.None, 
                    MergeClientsAccess.User, 
                    MergeClientsAccess.Department, 
                    MergeClientsAccess.DepartmentWithChilds, 
                    MergeClientsAccess.Full
                };

            var maxPriority = accesses.Select(x => Array.IndexOf(priorities, (MergeClientsAccess)x)).Max();
            return priorities[maxPriority];
        }

        private RemoteCollection<ListClientDto> SelectClients(IQueryable<Client> clients, QuerySettings querySettings)
        {
            return clients.Select(x => new ListClientDto
            {
                Id = x.Id,
                ReplicationCode = x.ReplicationCode,
                Name = x.Name,
                MainAddress = x.MainAddress,
                TerritoryId = x.TerritoryId,
                TerritoryName = x.Territory.Name,
                OwnerCode = x.OwnerCode,
                MainFirmId = x.MainFirmId,
                MainFirmName = x.Firm.Name,
                MainPhoneNumber = x.MainPhoneNumber,
                LastQualifyTime = x.LastQualifyTime,
                LastDisqualifyTime = x.LastDisqualifyTime,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                CreatedOn = x.CreatedOn,
                IsAdvertisingAgency = x.IsAdvertisingAgency,
                InformationSourceEnum = x.InformationSource,
                OwnerName = null,
            })
            .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListClientDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}