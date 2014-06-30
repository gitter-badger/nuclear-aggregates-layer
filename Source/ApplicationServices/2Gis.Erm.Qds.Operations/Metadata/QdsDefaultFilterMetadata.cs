using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Docs;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static class QdsDefaultFilterMetadata
    {
        private static readonly Dictionary<Tuple<Type, string>, Delegate> FilterMap = new Dictionary<Tuple<Type, string>, Delegate>()
            .RegisterFilter<OrderGridDoc>("DListOrdersFast", x => x.And(x.Term(y => y.IsActive, true), x.Term(y => y.IsDeleted, false)))

            .RegisterFilter<ClientGridDoc>("DListClients", x => x.And(x.Term(y => y.IsActive, true), x.Term(y => y.IsDeleted, false)))
            // Мои клиенты
            .RegisterFilter<ClientGridDoc>("DListMyClients", x => x.And(x.Term(y => y.IsActive, true), x.Term(y => y.IsDeleted, false)))
            // Мои клиенты, созданные сегодня
            .RegisterFilter<ClientGridDoc>("DListMyClientsCreatedToday", x => x.And(x.Term(y => y.IsActive, true), x.Term(y => y.IsDeleted, false)))
            // Клиенты на моей территории
            .RegisterFilter<ClientGridDoc>("DListClientsOnMyTerritory", x => x.And(x.Term(y => y.IsActive, true), x.Term(y => y.IsDeleted, false)))
            // Мои клиенты с дебиторской задолженностью
            .RegisterFilter<ClientGridDoc>("DListMyClientsWithDebt", x => x.And(x.Term(y => y.IsActive, true), x.Term(y => y.IsDeleted, false)))
            // Клиенты в резерве на моей территории
            .RegisterFilter<ClientGridDoc>("DListReservedClientsOnMyTerritory", x => x.And(x.Term(y => y.IsActive, true), x.Term(y => y.IsDeleted, false)))
            // Все клиенты по филиалу
            .RegisterFilter<ClientGridDoc>("DListClientsAtMyBranch", x => x.And(x.Term(y => y.IsActive, true), x.Term(y => y.IsDeleted, false)))

            .RegisterFilter<FirmGridDoc>("DListActiveFirms", x => x.And(x.Term(y => y.IsActive, true), x.Term(y => y.IsDeleted, false), x.Term(y => y.ClosedForAscertainment, false)))
            .RegisterFilter<FirmGridDoc>("DListInactiveFirms", x => x.And(x.Term(y => y.IsActive, true), x.Term(y => y.IsDeleted, false), x.Term(y => y.ClosedForAscertainment, true)))
            // Мои фирмы
            .RegisterFilter<FirmGridDoc>("DListMyFirms", x => x.And(x.Term(y => y.IsActive, true), x.Term(y => y.IsDeleted, false)))
            // Все фирмы по филиалу
            .RegisterFilter<FirmGridDoc>("DListFirmsAtMyBranch", x => x.Term(y => y.IsDeleted, false))
            // Все активные фирмы по филиалу
            .RegisterFilter<FirmGridDoc>("DListActiveFirmsAtMyBranch", x => x.And(x.Term(y => y.IsActive, true), x.Term(y => y.IsDeleted, false)))
            ;

        private static Dictionary<Tuple<Type, string>, Delegate> RegisterFilter<TDocument>(this Dictionary<Tuple<Type, string>, Delegate> map, string filterName, Func<FilterDescriptor<TDocument>, BaseFilter> func)
            where TDocument : class
        {
            var key = Tuple.Create(typeof(TDocument), filterName);
            map.Add(key, func);
            return map;
        }

        public static bool ContainsFilter<TDocument>(string filterName)
        {
            var key = Tuple.Create(typeof(TDocument), filterName);
            return FilterMap.ContainsKey(key);
        }

        public static bool TryGetFilter<TDocument>(string filterName, out Func<FilterDescriptor<TDocument>, BaseFilter> filter)
            where TDocument : class
        {
            // lookup не имеет заполненного filterName
            if (string.IsNullOrEmpty(filterName))
            {
                filter = (Func<FilterDescriptor<TDocument>, BaseFilter>)FilterMap.Where(x => x.Key.Item1 == typeof(TDocument)).Select(x => x.Value).First();
                return true;
            }

            var key = Tuple.Create(typeof(TDocument), filterName);

            Delegate @delegate;
            if (!FilterMap.TryGetValue(key, out @delegate))
            {
                filter = null;
                return false;
            }

            filter = (Func<FilterDescriptor<TDocument>, BaseFilter>)@delegate;
            return true;
        }
    }
}