using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Dictionary
{
    public class NewSalesModelRestrictions
    {
        public static readonly IReadOnlyCollection<long> SupportedCategoryIds =
           new long[]
                {
                    356, // Магазины обувные
                    15502, // Товары для творчества и рукоделия
                    390, // Часы 
                    192, // Кинотеатры 
                    462, // Электронные компоненты
                    170, // Боулинг 
                    15075, // Копировальные услуги
                    165 // Кафе / рестораны быстрого питания
                };

        public static readonly IReadOnlyCollection<long> SupportedOrganizationUnitIds =
            new long[]
                {
                    6 // Новосибирск
                };
    }
}