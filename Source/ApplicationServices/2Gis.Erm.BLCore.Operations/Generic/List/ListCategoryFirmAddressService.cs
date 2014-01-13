using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
{
    public class ListCategoryFirmAddressService : ListEntityDtoServiceBase<CategoryFirmAddress, ListCategoryFirmAddressDto>
    {
        private readonly IFinder _finder;

        public ListCategoryFirmAddressService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _finder = finder;
        }

        protected override IEnumerable<ListCategoryFirmAddressDto> GetListData(IQueryable<CategoryFirmAddress> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            var defaultCategoryRate = string.Format(UserContext.Profile.UserLocaleInfo.UserCultureInfo, "{0:p0}", 1);
            long? categoryRateOrganizationUnitId = null;

            if (filterManager.ParentEntityName == EntityName.Firm)
            {
                categoryRateOrganizationUnitId = _finder.Find(Specs.Find.ById<Firm>(filterManager.ParentEntityId))
                                                        .Select(firm => firm.OrganizationUnitId)
                                                        .Single();

                Expression<Func<CategoryFirmAddress, bool>> firmFilterExpression;
                if (!querySettings.DefaultFilter.Contains("!IsActive"))
                {
                    firmFilterExpression = x => !x.Category.IsDeleted &&
                                                x.Category.IsActive &&
                                                x.IsActive && !x.IsDeleted && x.FirmAddress.IsActive &&
                                                !x.FirmAddress.IsDeleted && !x.FirmAddress.ClosedForAscertainment &&
                                                x.FirmAddress.FirmId == filterManager.ParentEntityId;
                }
                else
                {
                    firmFilterExpression = x => x.Category.IsActive &&
                                                !x.IsActive &&
                                                x.FirmAddress.IsActive &&
                                                x.FirmAddress.FirmId == filterManager.ParentEntityId;
                }

                // Для фирмы агрегируем по рубрикам адресов
                return query
                    .Where(x => !x.Category.IsDeleted && !x.FirmAddress.IsDeleted && !x.IsDeleted)
                    .ApplyFilter(firmFilterExpression)
                    .GroupBy(x => x.Category)
                    .Select(x => new ListCategoryFirmAddressDto
                        {
                            CategoryGroup = categoryRateOrganizationUnitId.HasValue
                                ? x.Key.CategoryOrganizationUnits.Where(unit => unit.OrganizationUnitId == categoryRateOrganizationUnitId.Value)
                                            .Select(unit => unit.CategoryGroup.CategoryGroupName).FirstOrDefault() ?? defaultCategoryRate
                                : string.Empty,
                            CategoryId = x.Key.Id,
                            Name = x.Key.Name,
                            ParentId = x.Key.ParentId,
                            ParentName = x.Key.ParentCategory != null ? x.FirstOrDefault().Category.ParentCategory.Name : null,
                            Level = x.Key.Level,
                            SortingPosition = x.Min(y => y.SortingPosition),
                            IsPrimary = x.Any(y => y.IsPrimary),
                            IsActive = x.Any(y => y.IsActive)
                        })
                    .ApplyQuerySettings(querySettings, out count)
                    .ToArray();
            }

            if (filterManager.ParentEntityName == EntityName.FirmAddress)
            {
                categoryRateOrganizationUnitId = _finder.Find(Specs.Find.ById<FirmAddress>(filterManager.ParentEntityId))
                                                        .Select(address => address.Firm.OrganizationUnitId)
                                                        .Single();
            }

            return query
                .Where(x => !x.Category.IsDeleted && !x.FirmAddress.IsDeleted && !x.IsDeleted)
                .Select(x => new ListCategoryFirmAddressDto
                    {
                        CategoryGroup = categoryRateOrganizationUnitId.HasValue
                            ? x.Category.CategoryOrganizationUnits.Where(unit => unit.OrganizationUnitId == categoryRateOrganizationUnitId.Value)
                                        .Select(unit => unit.CategoryGroup.CategoryGroupName).FirstOrDefault() ?? defaultCategoryRate
                            : string.Empty,
                        CategoryId = x.Category.Id,
                        FirmAddressId = x.FirmAddressId,
                        SortingPosition = x.SortingPosition,
                        Name = x.Category.Name,
                        ParentId = x.Category.ParentId,
                        ParentName = x.Category.ParentCategory != null ? x.Category.ParentCategory.Name : null,
                        Level = x.Category.Level,
                        IsPrimary = x.IsPrimary,
                        IsActive = x.IsActive
                    })
                .ApplyQuerySettings(querySettings, out count)
                .ToArray();
        }
    }
}
