using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
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
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public ListClientService(
            IUserRepository userRepository,
            ISecurityServiceUserIdentifier userIdentifierService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IFinder finder,
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
        }

        protected override IEnumerable<ListClientDto> List(QuerySettings querySettings,
            out int count)
        {
            var query = _finder.FindAll<Client>();

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
                return x => x.LegalPersons.Any(y => !y.IsDeleted && y.IsActive && y.Accounts.Any(z => !z.IsDeleted && z.IsActive && z.Balance < minDebtAmount));
            });

            var barterOrdersFilter = querySettings.CreateForExtendedProperty<Client, bool>("WithBarterOrders", info =>
            {
                return x => x.LegalPersons.Any(y => !y.IsDeleted && y.IsActive && y.Orders.Any(z => !z.IsDeleted && z.IsActive && (z.OrderType == (int)OrderType.AdsBarter || z.OrderType == (int)OrderType.ProductBarter || z.OrderType == (int)OrderType.ServiceBarter)));
            });

            var noMakingDecisionsFilter = querySettings.CreateForExtendedProperty<Client, bool>("NoMakingDecisions", info =>
            {
                return x => !x.Contacts.Any(y => !y.IsDeleted && y.IsActive && !y.IsFired && y.AccountRole == (int)AccountRole.MakingDecisions);
            });

            var regionalFilter = querySettings.CreateForExtendedProperty<Client, bool>("IsRegional", info =>
            {
                return x => x.LegalPersons.Any(y => !y.IsDeleted && y.IsActive && y.Orders.Any(z => !z.IsDeleted && z.IsActive && z.SourceOrganizationUnitId != z.DestOrganizationUnitId));
            });

            var dealCountFilter = querySettings.CreateForExtendedProperty<Client, int>("MinDealCount", dealCount =>
            {
                return x => x.Deals.Count(y => !y.IsDeleted && y.IsActive) > dealCount;
            });

            var reserveFilter = querySettings.CreateForExtendedProperty<Client, bool>("ForReserve", info =>
            {
                var reserveId = _userIdentifierService.GetReserveUserIdentity().Code;
                return x => x.OwnerCode == reserveId;
            });

            var myFilter = querySettings.CreateForExtendedProperty<Client, bool>("ForMe", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });

            var todayFilter = querySettings.CreateForExtendedProperty<Client, bool>("ForToday", info =>
            {
                var userDateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTodayUtc = TimeZoneInfo.ConvertTimeToUtc(userDateTimeNow.Date, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTomorrowUtc = userDateTimeTodayUtc.AddDays(1);

                return x => userDateTimeTodayUtc <= x.CreatedOn && x.CreatedOn < userDateTimeTomorrowUtc;
            });

            query = query.Filter(_filterHelper, myTerritoryFilter, myBranchFilter, debtFilter, barterOrdersFilter, noMakingDecisionsFilter, regionalFilter, dealCountFilter, reserveFilter, myFilter);

            IEnumerable<ListClientDto> clients;
            if (TryGetClientsRestrictedByUser(query, querySettings, out clients, out count))
            {
                return clients;
            }

            if (TryGetClientsRestrictedByMergeClientPrivilege(query, querySettings, out clients, out count))
            {
                return clients;
            }

            var with1AppointmentFilter = querySettings.CreateForExtendedProperty<Client, bool>(
                "With1Appointment",
                with1Appointment =>
                {
                    if (!with1Appointment)
                    {
                        return null;
                    }


                    return x =>
                           x.ActivityInstances.Count(
                               y => y.Type == (int)ActivityType.Appointment && !y.IsDeleted && y.IsActive
                                    && y.ActivityPropertyInstances.Any(z => (z.PropertyId == StatusIdentity.Instance.Id && z.NumericValue == 2))) == 1;
                });

            var warmClientTaskFilter = querySettings.CreateForExtendedProperty<Client, bool>(
                "WarmClientTask",
                warmClientTask => client =>
                    client.ActivityInstances.Any(activity => activity.ActivityPropertyInstances.Any(property => property.PropertyId == TaskTypeIdentity.Instance.Id
                                                                                                  && property.NumericValue == (int)ActivityTaskType.WarmClient)));

            var outdatedActivityFilter = querySettings.CreateForExtendedProperty<Client, bool>(
                "Outdated",
                outdated =>
                {
                    if (outdated)
                    {
                        return client => client.ActivityInstances.Any(activity => activity.ActivityPropertyInstances.Any(
                            property => property.PropertyId == ScheduledEndIdentity.Instance.Id && property.DateTimeValue < DateTime.Today));
                    }

                    return null;
                });

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
                    , firmFilter
                    , todayFilter)
                    , querySettings, out count);
        }

        private bool TryGetClientsRestrictedByUser(
            IQueryable<Client> query,
            QuerySettings querySettings,
            out IEnumerable<ListClientDto> clients,
            out int count)
        {
            clients = null;
            count = 0;

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
                                    , querySettings, out count);
                return true;
            }

            return false;
        }

        private bool TryGetClientsRestrictedByMergeClientPrivilege(
            IQueryable<Client> query,
            QuerySettings querySettings,
            out IEnumerable<ListClientDto> clients,
            out int count)
        {
            clients = null;
            count = 0;

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
                                    , querySettings, out count);

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

        private IEnumerable<ListClientDto> SelectClients(IQueryable<Client> clients, QuerySettings querySettings, out int count)
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
                InformationSource = (InformationSource)x.InformationSource,
                IsAdvertisingAgency = x.IsAdvertisingAgency,
                OwnerName = null,
            })
            .QuerySettings(_filterHelper, querySettings, out count)
            .Select(x =>
            {
                x.OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName;
                return x;
            });
        }
    }
}