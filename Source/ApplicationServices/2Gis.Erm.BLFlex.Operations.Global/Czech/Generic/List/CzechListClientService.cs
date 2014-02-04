using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Czech.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.List
{
    public sealed class CzechListClientService : ListEntityDtoServiceBase<Client, CzechListClientDto>, ICzechAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IUserRepository _userRepository;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public CzechListClientService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            IUserRepository userRepository,
            ISecurityServiceUserIdentifier userIdentifierService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userContext = userContext;
            _userRepository = userRepository;
            _userIdentifierService = userIdentifierService;
            _functionalAccessService = functionalAccessService;
        }

        protected override IEnumerable<CzechListClientDto> GetListData(
            IQueryable<Client> query,
            QuerySettings querySettings,
            ListFilterManager filterManager,
            out int count)
        {
            IEnumerable<CzechListClientDto> clients;
            if (TryGetClientsRestrictedByUser(query, querySettings, filterManager, out clients, out count))
            {
                return clients;
            }

            if (TryGetClientsRestrictedByMergeClientPrivilege(query, querySettings, filterManager, out clients, out count))
            {
                return clients;
            }

            var with1AppointmentFilter = filterManager.CreateForExtendedProperty<Client, bool>(
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

            var warmClientTaskFilter = filterManager.CreateForExtendedProperty<Client, bool>(
                "WarmClientTask",
                warmClientTask => client =>
                    client.ActivityInstances.Any(activity => activity.ActivityPropertyInstances.Any(property => property.PropertyId == TaskTypeIdentity.Instance.Id
                                                                                                  && property.NumericValue == (int)ActivityTaskType.WarmClient)));

            var outdatedActivityFilter = filterManager.CreateForExtendedProperty<Client, bool>(
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

            return SelectClients(
                query
                    .Where(x => !x.IsDeleted)
                    .ApplyFilter(with1AppointmentFilter)
                    .ApplyFilter(warmClientTaskFilter)
                    .ApplyFilter(outdatedActivityFilter)
                    .ApplyQuerySettings(querySettings, out count));
        }

        private bool TryGetClientsRestrictedByUser(
            IQueryable<Client> query,
            QuerySettings querySettings,
            ListFilterManager filterManager,
            out IEnumerable<CzechListClientDto> clients,
            out int count)
        {
            clients = null;
            count = 0;

            var currentIdentity = _userContext.Identity;

            var userFilter = filterManager.CreateForExtendedProperty<Client, long>(
                "userId", userId => x => x.OwnerCode == userId, x => x.OwnerCode == currentIdentity.Code);
            if (userFilter != null)
            {
                clients = SelectClients(query
                                    .Where(x => !x.IsDeleted)
                                    .ApplyFilter(userFilter)
                                    .ApplyQuerySettings(querySettings, out count));
                return true;
            }

            return false;
        }

        private bool TryGetClientsRestrictedByMergeClientPrivilege(
            IQueryable<Client> query,
            QuerySettings querySettings,
            ListFilterManager filterManager,
            out IEnumerable<CzechListClientDto> clients,
            out int count)
        {
            clients = null;
            count = 0;

            var currentIdentity = _userContext.Identity;

            var restrictForMergeIdFilter = filterManager.CreateForExtendedProperty<Client, long?>(
                "restrictForMergeId",
                clientId =>
                {
                    if (!clientId.HasValue)
                    {
                        return c => true;
                    }

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
                                    .ApplyFilter(restrictForMergeIdFilter)
                                    .ApplyQuerySettings(querySettings, out count));

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

        private IEnumerable<CzechListClientDto> SelectClients(IQueryable<Client> clients)
        {
            return clients.Select(x => new
            {
                Id = x.Id,
                ReplicationCode = x.ReplicationCode,
                Name = x.Name,
                MainFirmId = (long?)x.Firm.Id,
                MainFirmName = x.Firm.Name,
                TerritoryId = x.TerritoryId,
                TerritoryName = x.Territory.Name,
                MainPhoneNumber = x.MainPhoneNumber,
                MainAddress = x.MainAddress,
                LastQualifyTime = x.LastQualifyTime,
                LastDisqualifyTime = x.LastDisqualifyTime,
                OwnerCode = x.OwnerCode,
            })
                          .AsEnumerable()
                          .Select(x => new CzechListClientDto
                          {
                              Id = x.Id,
                              ReplicationCode = x.ReplicationCode,
                              Name = x.Name,
                              MainFirmId = x.MainFirmId,
                              MainFirmName = x.MainFirmName,
                              TerritoryId = x.TerritoryId,
                              TerritoryName = x.TerritoryName,
                              MainPhoneNumber = x.MainPhoneNumber,
                              MainAddress = x.MainAddress,
                              LastQualifyTime = x.LastQualifyTime,
                              LastDisqualifyTime = x.LastDisqualifyTime,
                              OwnerCode = x.OwnerCode,
                              OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                          });
        }
    }
}