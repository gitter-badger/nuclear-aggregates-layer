using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary
{
    public static class CategorySpecs
    {
        public static class Categories
        {
            public static class Find
            {
                private static readonly IReadOnlyDictionary<long, IReadOnlyCollection<long>> PlannedProvisionCategories =
                    new Dictionary<long, IReadOnlyCollection<long>>
                        {
                            {
                                6, // Новосибирск:
                                new long[]
                                    {
                                        192, // Кинотеатры
                                        356, // Магазины обувные
                                        390, // Часы
                                        15502, // Товары для творчества и рукоделия
                                        207, // Аптеки
                                        526, // Сумки / кожгалантерея
                                        405, // Автомойки
                                        508, // Ткани
                                        384, // Охотничьи принадлежности / Аксессуары
                                        347, // Книги
                                        205 // Ветеринарные клиники
                                    }
                            },
                            {
                                1, // Самара:
                                new long[]
                                    {
                                        13100, // Детская обувь 
                                        609, // Детская одежда
                                        346, // Игрушки 
                                        345, // Товары для новорожденных 
                                        533, // Заказ пассажирского легкового транспорта
                                        207, // Аптеки 
                                        1203, // Доставка готовых блюд
                                        161, // Кафе
                                        164, // Рестораны
                                        15791, // Суши-бары / рестораны
                                    }
                            },
                            {
                                2, // Екатеринбург:
                                new long[]
                                    {
                                        14426, // Верхняя одежда 
                                        354, // Головные уборы
                                        606, // Женская одежда
                                        355, // Меха/Дублёнки/Кожа
                                        612, // Мужская одежда 
                                        383, // Свадебные товары 
                                        207, // Аптеки
                                    }
                            }
                        };

                public static FindSpecification<Category> ActiveCategoryForSalesModelInOrganizationUnit(SalesModel salesModel, long organizationUnitId)
                {
                    var resultSpec = ForOrganizationUnit(organizationUnitId) && Platform.DAL.Specifications.Specs.Find.ActiveAndNotDeleted<Category>();

                    // TODO {y.baranihin, 15.01.2015}: Использовать IsPlannedProvisionSalesModel() после появления справочника ограничений
                    if (salesModel != SalesModel.PlannedProvision)
                    {
                        return resultSpec;
                    }

                    if (PlannedProvisionCategories.ContainsKey(organizationUnitId))
                    {
                        var restrictions = PlannedProvisionCategories[organizationUnitId];
                        resultSpec &= new FindSpecification<Category>(category => restrictions.Contains(category.Id));
                    }
                    else
                    {
                        resultSpec &= new FindSpecification<Category>(category => false);
                    }

                    return resultSpec;
                }

                public static FindSpecification<Category> ForOrganizationUnit(long organizationUnitId)
                {
                    return new FindSpecification<Category>(x => x.CategoryOrganizationUnits.Any(y => y.IsActive && !y.IsDeleted && y.OrganizationUnitId == organizationUnitId));
                }
            }
        }

        public static class CategoryOrganizationUnits
        {
            public static class Find
            {
                public static FindSpecification<CategoryOrganizationUnit> ForOrganizationUnit(long organizationUnitId)
                {
                    return new FindSpecification<CategoryOrganizationUnit>(x => x.OrganizationUnitId == organizationUnitId);
                }

                public static FindSpecification<CategoryOrganizationUnit> ForCategories(IEnumerable<long> categoryIds)
                {
                    return new FindSpecification<CategoryOrganizationUnit>(x => categoryIds.Contains(x.CategoryId));
                }
            }
        }

        public static class CategoryFirmAddresses
        {
            public static class Find
            {
                public static FindSpecification<CategoryFirmAddress> ByFirmAddresses(IEnumerable<long> addressIds)
                {
                    return new FindSpecification<CategoryFirmAddress>(x => addressIds.Contains(x.FirmAddressId));
                }
            }
        }
    }
}