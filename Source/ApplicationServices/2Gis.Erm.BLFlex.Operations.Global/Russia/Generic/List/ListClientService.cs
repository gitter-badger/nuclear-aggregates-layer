using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Aggregates.Activities.PropertyIdentities;
using DoubleGis.Erm.BL.Aggregates.Users;
using DoubleGis.Erm.BL.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BL.API.Operations.Metadata;
using DoubleGis.Erm.BL.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.List
{
    public class ListClientService : ListEntityDtoServiceBase<Client, ListClientDto>, IRussiaAdapted, ICyprusAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IUserRepository _userRepository;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public ListClientService(
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

        protected override IEnumerable<ListClientDto> GetListData(
            IQueryable<Client> query, 
            QuerySettings querySettings, 
            ListFilterManager filterManager,
            out int count)
        {
            IEnumerable<ListClientDto> clients;
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
            out IEnumerable<ListClientDto> clients, 
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
            out IEnumerable<ListClientDto> clients,
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

        private IEnumerable<ListClientDto> SelectClients(IQueryable<Client> clients)
        {
            return clients
                    .Select(x => new
                                {
                                    x.Id,
                                    x.ReplicationCode,
                                    x.Name,
                                    x.MainAddress,
                                    x.TerritoryId,
                                    TerritoryName = x.Territory.Name,
                                    x.OwnerCode
                                })
                    .AsEnumerable()
                    .Select(x =>
                            new ListClientDto
                                {
                                    Id = x.Id,
                                    ReplicationCode = x.ReplicationCode,
                                    Name = x.Name,
                                    MainAddress = x.MainAddress,
                                    TerritoryId = x.TerritoryId,
                                    TerritoryName = x.TerritoryName,
                                    OwnerCode = x.OwnerCode,
                                    OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName
                                });
        }
    }
}