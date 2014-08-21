using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary
{
    public static class NewSalesModelRestrictions
    {
        public static readonly IReadOnlyCollection<long> SupportedCategoryIds =
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
                    13100   // Детская обувь
                };

        public static readonly IReadOnlyCollection<long> SupportedOrganizationUnitIds =
            new long[]
                {
                    6       // Новосибирск
                };
    }
}