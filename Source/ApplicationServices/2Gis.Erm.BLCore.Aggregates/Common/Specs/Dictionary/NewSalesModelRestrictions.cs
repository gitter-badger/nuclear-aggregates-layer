using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Dictionary
{
    public class NewSalesModelRestrictions
    {
        public static readonly IReadOnlyCollection<long> SupportedCategoryIds =
           new long[]
                {
                    192, // Кинотеатры
                    356, // Магазины обувные
                    390, // Часы
                    15502, // Товары для творчества и рукоделия
                    207, // Аптеки
                    526, // Сумки / кожгалантерея
                };

        public static readonly IReadOnlyCollection<long> SupportedOrganizationUnitIds =
            new long[]
                {
                    6 // Новосибирск
                };
    }
}