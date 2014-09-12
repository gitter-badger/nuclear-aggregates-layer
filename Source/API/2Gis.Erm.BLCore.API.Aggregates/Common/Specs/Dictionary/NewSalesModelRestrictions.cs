using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary
{
    public static class NewSalesModelRestrictions
    {
        private static readonly IReadOnlyDictionary<long, IReadOnlyCollection<long>> SupportedCategoryOrganizationUnits =
            new Dictionary<long, IReadOnlyCollection<long>>
                {
                    {
                        6,          // Новосибирск:
                        new long[]
                            {
                                192,    // Кинотеатры
                                356,    // Магазины обувные
                                390,    // Часы
                                15502,  // Товары для творчества и рукоделия
                                207,    // Аптеки
                                526,    // Сумки / кожгалантерея
                                405,    // Автомойки
                                508,    // Ткани
                                384,    // Охотничьи принадлежности / Аксессуары
                                347,    // Книги
                                205     // Ветеринарные клиники
                            }
                    },
                    {
                        1,          // Самара:
                        new long[]
                            {
                                13100,  // Детская обувь 
                                609,    // Детская одежда
                                346,    // Игрушки 
                                345,    // Товары для новорожденных 
                                533,    // Заказ пассажирского легкового транспорта
                                207,    // Аптеки 
                            }
                    },
                    {
                        2,          // Екатеринбург:
                        new long[]
                            {
                                14426,  // Верхняя одежда 
                                354,    // Головные уборы
                                606,    // Женская одежда
                                355,    // Меха/Дублёнки/Кожа
                                612,    // Мужская одежда 
                                383,    // Свадебные товары 
                                207,    // Аптеки
                            }
                    }
                };

        public static bool IsOrganizationUnitSupported(long destOrganizationUnitId)
        {
            return SupportedCategoryOrganizationUnits.ContainsKey(destOrganizationUnitId);
        }

        public static IReadOnlyCollection<long> GetSupportedCategoryIds(long destOrganizationUnitId)
        {
            return SupportedCategoryOrganizationUnits[destOrganizationUnitId];
        }
    }

}