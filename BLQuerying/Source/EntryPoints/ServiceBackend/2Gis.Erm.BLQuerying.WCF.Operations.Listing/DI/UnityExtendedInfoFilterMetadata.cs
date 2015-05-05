using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.WCF.Operations.Listing.DI
{
    public sealed class UnityExtendedInfoFilterMetadata : IExtendedInfoFilterMetadata
    {
        private readonly IUnityContainer _unityContainer;
        private readonly Dictionary<Tuple<Type, string>, Delegate> _filtersMap = new Dictionary<Tuple<Type, string>, Delegate>();

        public UnityExtendedInfoFilterMetadata(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            RegisterAll();
        }

        private void RegisterAll()
        {
            RegisterExtendedInfoFilter<ListAccountDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListAccountDto, bool>("NegativeBalance", value => x => x.Balance < 0);
            RegisterExtendedInfoFilter<ListAccountDto, bool>("ForMe", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });

            RegisterExtendedInfoFilter<ListAccountDetailDto, bool>("Deleted", value => x => x.IsDeleted == value);


            RegisterExtendedInfoFilter<ListActivityDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListActivityDto, bool>("NotDeleted", value => x => !x.IsDeleted);
            RegisterExtendedInfoFilter<ListActivityDto, bool>("NotActiveBusinessMeaning", value => x => x.IsDeleted || !x.IsActive || x.StatusEnum == ActivityStatus.Completed || x.StatusEnum == ActivityStatus.Canceled);
            RegisterExtendedInfoFilter<ListActivityDto, bool>("InProgress", value => x => x.StatusEnum == ActivityStatus.InProgress);
            RegisterExtendedInfoFilter<ListActivityDto, bool>("Completed", value => x => x.StatusEnum == ActivityStatus.Completed);
            RegisterExtendedInfoFilter<ListActivityDto, bool>("WarmClient", value => x => x.TaskType == TaskType.WarmClient);

            RegisterExtendedInfoFilter<ListActivityDto, bool>("ForToday", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();

                var userDateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTodayUtc = TimeZoneInfo.ConvertTimeToUtc(userDateTimeNow.Date, userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTomorrowUtc = userDateTimeTodayUtc.AddDays(1);

                return
                    x =>
                    userDateTimeTodayUtc <= x.ScheduledStart && x.ScheduledStart < userDateTimeTomorrowUtc ||
                    userDateTimeTodayUtc <= x.ScheduledEnd && x.ScheduledEnd < userDateTimeTomorrowUtc;
            });
            RegisterExtendedInfoFilter<ListActivityDto, bool>("ForMe", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });
            RegisterExtendedInfoFilter<ListActivityDto, bool>("Expired", value =>
            {
                const int HotClientTaskThreshold = 60; // in hours
                var userContext = _unityContainer.Resolve<IUserContext>();

                var startOfTheDay = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow.AddHours(-HotClientTaskThreshold), 
                    userContext.Profile.UserLocaleInfo.UserTimeZoneInfo
                    ).Date;
                var thresholdDate = TimeZoneInfo.ConvertTimeToUtc(startOfTheDay, userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);

                if (value)
                {
                    return x => x.ScheduledStart < thresholdDate;
                }

                return x => true;
            });

            RegisterExtendedInfoFilter<ListAdsTemplatesAdsElementTemplateDto, bool>("NotDeleted", value => x => !x.IsDeleted);

            RegisterExtendedInfoFilter<ListAdvertisementElementDenialReasonsDto, bool>("ActiveOrChecked", value => x => x.IsActive || x.Checked);

            RegisterExtendedInfoFilter<ListDenialReasonDto, bool>("Active", value => x => x.IsActive == value);

            RegisterExtendedInfoFilter<ListAdvertisementTemplateDto, bool>("Deleted", value => x => x.IsDeleted == value);
            RegisterExtendedInfoFilter<ListAdvertisementTemplateDto, bool>("isPublished", value => x => x.IsPublished);
            RegisterExtendedInfoFilter<ListAdvertisementTemplateDto, bool>("isUnpublished", value => x => !x.IsPublished);

            RegisterExtendedInfoFilter<ListAdvertisementElementTemplateDto, bool>("Deleted", value => x => x.IsDeleted == value);

            RegisterExtendedInfoFilter<ListAdvertisementDto, bool>("Deleted", value => x => x.IsDeleted == value);
            RegisterExtendedInfoFilter<ListAdvertisementDto, long>("FirmId", value =>  x => x.FirmId == value);
            RegisterExtendedInfoFilter<ListAdvertisementDto, long>("AdvertisementTemplateId", value => x => x.AdvertisementTemplateId == value);
            RegisterExtendedInfoFilter<ListAdvertisementDto, bool>("isAllowedToWhiteList", value => x => x.IsAllowedToWhiteList);

            RegisterExtendedInfoFilter<ListAssociatedPositionDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListAssociatedPositionDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListAssociatedPositionsGroupDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListAssociatedPositionsGroupDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListBargainDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListBargainDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListBargainDto, bool>("ForMe", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });
            RegisterExtendedInfoFilter<ListBargainDto, long>("legalPersonId", value => x => x.LegalPersonId == value);
            RegisterExtendedInfoFilter<ListBargainDto, long>("branchOfficeOrganizationUnitId", value => x => x.ExecutorBranchOfficeId == value);

            RegisterExtendedInfoFilter<ListBargainFileDto, bool>("NotDeleted", value => x => !x.IsDeleted);

            RegisterExtendedInfoFilter<ListBargainTypeDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListBargainTypeDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListBillDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListBranchOfficeDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListBranchOfficeDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListBranchOfficeOrganizationUnitDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListBranchOfficeOrganizationUnitDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListBranchOfficeOrganizationUnitDto, bool>("ParentsNotDeleted", value => x => !x.OrganizationUnitIsDeleted && !x.BranchOfficeIsDeleted);
            RegisterExtendedInfoFilter<ListBranchOfficeOrganizationUnitDto, bool>("Primary", value => x => x.IsPrimary);

            RegisterExtendedInfoFilter<ListCategoryDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListCategoryDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListCategoryDto, bool>("Active", value => x => x.IsActive == value);
            RegisterExtendedInfoFilter<ListCategoryDto, int>("minLevel", value => x => x.Level > value);
            RegisterExtendedInfoFilter<ListCategoryDto, int>("Level", value => x => x.Level == value);

            RegisterExtendedInfoFilter<ListCategoryFirmAddressDto, bool>("ActiveBusinessMeaning", value => x => x.IsActive && !x.IsDeleted && x.CategoryIsActive && !x.CategoryIsDeleted && x.CategoryOrganizationUnitIsActive && !x.CategoryOrganizationUnitIsDeleted);
            RegisterExtendedInfoFilter<ListCategoryFirmAddressDto, bool>("InactiveBusinessMeaning", value => x => !x.IsActive && !x.IsDeleted || !x.CategoryIsActive && !x.CategoryIsDeleted || !x.CategoryOrganizationUnitIsActive && !x.CategoryOrganizationUnitIsDeleted);
            
            RegisterExtendedInfoFilter<ListCategoryGroupDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListCategoryGroupDto, bool>("NotDeleted", value => x => !x.IsDeleted);

            RegisterExtendedInfoFilter<ListClientLinkDto, bool>("IsDeleted", value => x => x.IsDeleted == value);

            RegisterExtendedInfoFilter<ListClientDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListClientDto, bool>("Warm", value => x => x.InformationSourceEnum == InformationSource.WarmClient);
            RegisterExtendedInfoFilter<ListClientDto, bool>("ForReserve", value =>
            {
                var userIdentifierService = _unityContainer.Resolve<ISecurityServiceUserIdentifier>();
                var reserveId = userIdentifierService.GetReserveUserIdentity().Code;
                if (value)
                {
                    return x => x.OwnerCode == reserveId;
                }
                return x => x.OwnerCode != reserveId;
            });
            RegisterExtendedInfoFilter<ListClientDto, bool>("ForMe", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });
            RegisterExtendedInfoFilter<ListClientDto, bool>("ForToday", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userDateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTodayUtc = TimeZoneInfo.ConvertTimeToUtc(userDateTimeNow.Date, userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTomorrowUtc = userDateTimeTodayUtc.AddDays(1);

                return x => userDateTimeTodayUtc <= x.CreatedOn && x.CreatedOn < userDateTimeTomorrowUtc;
            });

            RegisterExtendedInfoFilter<ListContactDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListContactDto, bool>("Fired", value => x => x.IsFired == value);
            RegisterExtendedInfoFilter<ListContactDto, bool>("ForMe", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });

            RegisterExtendedInfoFilter<ListCategoryOrganizationUnitDto, bool>("NotDeletedAndParentsNotDeleted", value => x => !x.IsDeleted && !x.CategoryIsDeleted);

            RegisterExtendedInfoFilter<ListContributionTypeDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListContributionTypeDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListCountryDto, bool>("NotDeleted", value => x => !x.IsDeleted);

            RegisterExtendedInfoFilter<ListCurrencyDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListCurrencyDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListDealDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListDealDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListDealDto, bool>("ForMe", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });

            RegisterExtendedInfoFilter<ListDeniedPositionDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListDeniedPositionDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListDeniedPositionDto, long>("PositionId", value => x => x.PositionId == value);
            RegisterExtendedInfoFilter<ListDeniedPositionDto, long>("PriceId", value => x => x.PriceId == value);

            RegisterExtendedInfoFilter<ListDepartmentDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListDepartmentDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListDepartmentDto, long>("excludeId", value => x => x.Id != value);

            RegisterExtendedInfoFilter<ListFirmAddressDto, long>("FirmId", value => x => x.FirmId == value);

            RegisterExtendedInfoFilter<ListFirmDto, bool>("NotDeleted", value => x => !x.IsDeleted);
            RegisterExtendedInfoFilter<ListFirmDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListFirmDto, bool>("ActiveBusinessMeaning", value => x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment);
            RegisterExtendedInfoFilter<ListFirmDto, bool>("InactiveBusinessMeaning", value => x => !x.IsDeleted && (!x.IsActive || x.ClosedForAscertainment));
            RegisterExtendedInfoFilter<ListFirmDto, bool>("QualifyTimeLastYear", value => x => x.LastDisqualifyTime == null || ((DateTime.Now.Month - x.LastDisqualifyTime.Value.Month) + 12 * (DateTime.Now.Year - x.LastDisqualifyTime.Value.Year)) > 2);
            RegisterExtendedInfoFilter<ListFirmDto, long>("organizationUnitId", value => x => x.OrganizationUnitId == value);
            RegisterExtendedInfoFilter<ListFirmDto, bool>("CreatedInCurrentMonth", value =>
            {
                if (!value)
                {
                    return null;
                }

                var nextMonth = DateTime.Now.AddMonths(1);
                nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);

                var currentMonthLastDate = nextMonth.AddSeconds(-1);
                var currentMonthFirstDate = new DateTime(currentMonthLastDate.Year, currentMonthLastDate.Month, 1);

                return x => x.CreatedOn >= currentMonthFirstDate && x.CreatedOn <= currentMonthLastDate;
            });
            RegisterExtendedInfoFilter<ListFirmDto, bool>("ForReserve", value =>
            {
                var userIdentifierService = _unityContainer.Resolve<ISecurityServiceUserIdentifier>();
                var reserveId = userIdentifierService.GetReserveUserIdentity().Code;
                if (value)
                {
                    return x => x.OwnerCode == reserveId;
                }
                return x => x.OwnerCode != reserveId;
            });
            RegisterExtendedInfoFilter<ListFirmDto, bool>("ForMe", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });

            RegisterExtendedInfoFilter<ListFirmAddressDto, bool>("ActiveBusinessMeaning", value => x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment);
            RegisterExtendedInfoFilter<ListFirmAddressDto, bool>("InactiveBusinessMeaning", value => x => !x.IsDeleted && (!x.IsActive || x.ClosedForAscertainment));

            RegisterExtendedInfoFilter<ListFirmDealDto, bool>("Deleted", value => x => x.IsDeleted == value);

            RegisterExtendedInfoFilter<ListLegalPersonDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListLegalPersonDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListLegalPersonProfileDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListLegalPersonDealDto, bool>("NotDeleted", value => x => !x.IsDeleted);

            RegisterExtendedInfoFilter<ListLockDto, bool>("Active", value => x => x.IsActive == value);
            RegisterExtendedInfoFilter<ListLockDetailDto, bool>("Active", value => x => x.IsActive == value);

            RegisterExtendedInfoFilter<ListLimitDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListLimitDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListLimitDto, bool>("Opened", value => x => x.StatusEnum == LimitStatus.Opened);
            RegisterExtendedInfoFilter<ListLimitDto, bool>("Approved", value => x => x.StatusEnum == LimitStatus.Approved);
            RegisterExtendedInfoFilter<ListLimitDto, bool>("Rejected", value => x => x.StatusEnum == LimitStatus.Rejected);
            RegisterExtendedInfoFilter<ListLimitDto, bool>("useNextMonthForStartPeriodDate", value =>
            {
                if (!value)
                {
                    return null;
                }

                var nextMonth = DateTime.Now.AddMonths(1);
                nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);
                return x => x.StartPeriodDate == nextMonth;
            });
            RegisterExtendedInfoFilter<ListLimitDto, bool>("ForMe", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });
            RegisterExtendedInfoFilter<ListLimitDto, bool>("MyInspection", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.InspectorCode == userId;
            });

            RegisterExtendedInfoFilter<ListLocalMessageDto, bool>("ActiveBusinessMeaning", value => x => x.StatusEnum == LocalMessageStatus.NotProcessed || x.StatusEnum == LocalMessageStatus.WaitForProcess || x.StatusEnum == LocalMessageStatus.Processing);
            RegisterExtendedInfoFilter<ListLocalMessageDto, bool>("Processed", value => x => x.StatusEnum == LocalMessageStatus.Processed);
            RegisterExtendedInfoFilter<ListLocalMessageDto, bool>("Failed", value => x => x.StatusEnum == LocalMessageStatus.Failed);
            RegisterExtendedInfoFilter<ListLocalMessageDto, bool>("ToErm", value => x => x.ReceiverSystemEnum == IntegrationSystem.Erm);
            RegisterExtendedInfoFilter<ListLocalMessageDto, bool>("FromErm", value => x => x.SenderSystemEnum == IntegrationSystem.Erm);

            RegisterExtendedInfoFilter<ListOrderProcessingRequestDto, bool>("NotDeleted", value => x => !x.IsDeleted);

            RegisterExtendedInfoFilter<ListOrderPositionDto, bool>("NotDeleted", value => x => !x.IsDeleted);

            RegisterExtendedInfoFilter<ListOrderFileDto, bool>("NotDeleted", value => x => !x.IsDeleted);

            RegisterExtendedInfoFilter<ListOperationTypeDto, string>("excludeSyncCode", value => x => x.SyncCode1C != value);


            RegisterExtendedInfoFilter<ListOrganizationUnitDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListOrganizationUnitDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListOrganizationUnitDto, bool>("UseErm", value => x => x.ErmLaunchDate != null);
            RegisterExtendedInfoFilter<ListOrganizationUnitDto, bool>("UseIR", value => x => x.InfoRussiaLaunchDate != null);
            RegisterExtendedInfoFilter<ListOrganizationUnitDto, long>("currencyId", value => x => x.CurrencyId == value);
            RegisterExtendedInfoFilter<ListOrganizationUnitDto, bool>("filterByMovedToErm", value => x => x.IsActive && !x.IsDeleted && x.ErmLaunchDate != null);

            RegisterExtendedInfoFilter<ListPositionDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListPositionDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListPositionDto, bool>("isSupportedByExport", value => x => x.IsSupportedByExport == value);
            RegisterExtendedInfoFilter<ListPositionDto, bool>("composite", value => x => x.IsComposite == value);

            RegisterExtendedInfoFilter<ListPositionChildrenDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListPositionCategoryDto, bool>("NotDeleted", value => x => !x.IsDeleted);

            RegisterExtendedInfoFilter<ListPrintFormTemplateDto, bool>("NotDeleted", value => x => !x.IsDeleted);
            RegisterExtendedInfoFilter<ListProjectDto, bool>("Active", value => x => x.IsActive);

            RegisterExtendedInfoFilter<ListPriceDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListPriceDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListPriceDto, long>("excludeId", value => x => x.Id != value);

            RegisterExtendedInfoFilter<ListPricePositionDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListPricePositionDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListReleaseInfoDto, bool>("Beta", value => x => x.IsBeta == value);
            RegisterExtendedInfoFilter<ListReleaseInfoDto, bool>("InProgress", value => x => x.StatusEnum == ReleaseStatus.InProgressInternalProcessingStarted || x.StatusEnum == ReleaseStatus.InProgressWaitingExternalProcessing);
            RegisterExtendedInfoFilter<ListReleaseInfoDto, bool>("Success", value => x => x.StatusEnum == ReleaseStatus.Success);
            RegisterExtendedInfoFilter<ListReleaseInfoDto, bool>("Error", value => x => x.StatusEnum == ReleaseStatus.Error);

            RegisterExtendedInfoFilter<ListUserDto, bool>("hideReserveUser", value =>
            {
                var userIdentifierService = _unityContainer.Resolve<ISecurityServiceUserIdentifier>();
                var reserveUserId = userIdentifierService.GetReserveUserIdentity().Code;
                return x => x.Id != reserveUserId;
            });
            RegisterExtendedInfoFilter<ListUserDto, long>("excludeId", value => x => x.Id != value);

            RegisterExtendedInfoFilter<ListTerritoryDto, bool>("Active", value => x => x.IsActive == value);

            RegisterExtendedInfoFilter<ListUserDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListUserDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListUserTerritoryDto, bool>("TerritoryActiveAndNotDeleted", value => x => x.TerritoryIsActive && !x.IsDeleted);

            RegisterExtendedInfoFilter<ListUserOrganizationUnitDto, bool>("ParentsActiveAndNotDeleted", value => x => x.UserIsActive && !x.UserIsDeleted);

            RegisterExtendedInfoFilter<ListThemeDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListThemeTemplateDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListThemeOrganizationUnitDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            RegisterExtendedInfoFilter<ListThemeCategoryDto, bool>("NotDeleted", value => x => !x.IsDeleted);
        }

        public void RegisterExtendedInfoFilter<TDocument, TInfoType>(string filterName, Func<TInfoType, Expression<Func<TDocument, bool>>> func)
        {
            var key = Tuple.Create(typeof(TDocument), filterName.ToLowerInvariant());
            Func<string, Expression<Func<TDocument, bool>>> value = x =>
            {
                var argument = (TInfoType)Convert.ChangeType(x, typeof(TInfoType), CultureInfo.InvariantCulture);
                return func(argument);
            };
            _filtersMap.Add(key, value);
        }

        public IReadOnlyCollection<Expression<Func<TDocument, bool>>> GetExtendedInfoFilters<TDocument>(IReadOnlyDictionary<string, string> extendedInfoMap)
        {
            var filters = extendedInfoMap.Select(x =>
            {
                var key = Tuple.Create(typeof(TDocument), x.Key);

                Delegate func;
                if (!_filtersMap.TryGetValue(key, out func))
                {
                    // TODO {m.pashuk, 05.09.2014}: включить после перевода всех extendedInfo
                    //throw new ArgumentException(string.Format("Для типа '{0}' не найден фильтр '{1}'", typeof(TDocument), x.Key));
                    return null;
                }

                var filter = ((Func<string, Expression<Func<TDocument, bool>>>)func)(x.Value);
                return filter;
            })
            .Where(x => x != null)
            .ToList();

            return filters;
        }
    }
}