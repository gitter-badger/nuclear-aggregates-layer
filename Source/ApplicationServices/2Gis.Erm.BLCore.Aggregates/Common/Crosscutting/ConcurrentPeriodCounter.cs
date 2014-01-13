using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    public sealed class ConcurrentPeriodCounter : IConcurrentPeriodCounter
    {
        public int Count(IEnumerable<TimePeriod> periods, DateTime fullPeriodStart, DateTime fullPeriodEnd)
        {
            // Разбиваем период по месяцам и считаем,
            // сколько тематик размещалось в каждом отрезке времени
            // todo: можно отптимизировать и обобщить, 
            // проведя линии разбивки не по месяцам, а по граничным датам периодов 
            // это не увеличит (возможно, уменьшит) число периодов.
            var periodSize = fullPeriodStart.MonthDifference(fullPeriodEnd) + 1;
            var activeThemeCount = new int[periodSize];
            foreach (var period in periods)
            {
                // Поскольку возможна очень избыточная десериализация объектов, если в запрос забыть добавить фильтр по датам,
                // поэтому выдаём сообзение об ошибке при передаче лишних (не входящих в запрашиваемый интервал) периодов
                if (period.End < fullPeriodStart || period.Start > fullPeriodEnd)
                {
                    throw new ArgumentException(BLResources.InvalidPeriodsArgument, "periods");
                }

                // Номер месяца, когда начала действовать тематика
                var themeStart = period.Start > fullPeriodStart
                                     ? fullPeriodStart.MonthDifference(period.Start)
                                     : 0;

                // И когда она закончилась
                var themeEnd = period.End < fullPeriodEnd
                                   ? fullPeriodStart.MonthDifference(period.End)
                                   : periodSize - 1;

                // Запоминаем период действия тематики
                for (var i = themeStart; i <= themeEnd; i++)
                {
                    activeThemeCount[i]++;
                }
            }

            // Из наложенных друг на друга периодов выбираем, когда их было больше всего.
            var maxThemeCount = activeThemeCount.Max();

            return maxThemeCount;
        }
    }
}
