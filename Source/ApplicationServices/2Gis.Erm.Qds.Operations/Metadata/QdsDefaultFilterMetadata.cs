using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations.Docs;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static class QdsDefaultFilterMetadata
    {
        private static readonly Dictionary<Tuple<Type, string>, Delegate> FilterMap = new Dictionary<Tuple<Type, string>, Delegate>()
            // быстрый поиск заказов
            .RegisterFilter<OrderGridDoc>("DListOrdersFast", x => x.And(x.Term(t => t.IsDeleted, false)))

            .RegisterFilter<ClientGridDoc>("DListClients", x => x.And(x.Term(t => t.IsActive, true), x.Term(t => t.IsDeleted, false)))
            // Мои клиенты
            .RegisterFilter<ClientGridDoc>("DListMyClients", x => x.And(x.Term(t => t.IsActive, true), x.Term(t => t.IsDeleted, false)))
            // Мои клиенты, созданные сегодня
            .RegisterFilter<ClientGridDoc>("DListMyClientsCreatedToday", x => x.And(x.Term(t => t.IsActive, true), x.Term(t => t.IsDeleted, false)))
            // Клиенты на моей территории
            .RegisterFilter<ClientGridDoc>("DListClientsOnMyTerritory", x => x.And(x.Term(t => t.IsActive, true), x.Term(t => t.IsDeleted, false)))
            // Мои клиенты с дебиторской задолженностью
            .RegisterFilter<ClientGridDoc>("DListMyClientsWithDebt", x => x.And(x.Term(t => t.IsActive, true), x.Term(t => t.IsDeleted, false)))
            // Клиенты в резерве на моей территории
            .RegisterFilter<ClientGridDoc>("DListReservedClientsOnMyTerritory", x => x.And(x.Term(t => t.IsActive, true), x.Term(t => t.IsDeleted, false)))
            // Все клиенты по филиалу
            .RegisterFilter<ClientGridDoc>("DListClientsAtMyBranch", x => x.And(x.Term(t => t.IsActive, true), x.Term(t => t.IsDeleted, false)))

            .RegisterFilter<FirmGridDoc>("DListActiveFirms", x => x.And(x.Term(t => t.IsActive, true), x.Term(t => t.IsDeleted, false), x.Term(t => t.ClosedForAscertainment, false)))
            .RegisterFilter<FirmGridDoc>("DListInactiveFirms", x => x.And(x.Term(t => t.IsActive, true), x.Term(t => t.IsDeleted, false), x.Term(t => t.ClosedForAscertainment, true)))
            // Мои фирмы
            .RegisterFilter<FirmGridDoc>("DListMyFirms", x => x.And(x.Term(t => t.IsActive, true), x.Term(t => t.IsDeleted, false)))
            // Все фирмы по филиалу
            .RegisterFilter<FirmGridDoc>("DListFirmsAtMyBranch", x => x.Term(t => t.IsDeleted, false))
            // Все активные фирмы по филиалу
            .RegisterFilter<FirmGridDoc>("DListActiveFirmsAtMyBranch", x => x.And(x.Term(t => t.IsActive, true), x.Term(t => t.IsDeleted, false)))

            .RegisterFilter<DepartmentGridDoc>("DListDepartment", x => x.And(x.Term(t => t.IsActive, true), x.Term(t => t.IsDeleted, false)))
            .RegisterFilter<DepartmentGridDoc>("DListInactiveDepartment", x => x.And(x.Term(t => t.IsActive, false), x.Term(t => t.IsDeleted, false)))
            ;

        private static Dictionary<Tuple<Type, string>, Delegate> RegisterFilter<TDocument>(this Dictionary<Tuple<Type, string>, Delegate> map, string filterName, Func<FilterDescriptor<TDocument>, FilterContainer> func)
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

        public static bool TryGetFilter<TDocument>(string filterName, out Func<FilterDescriptor<TDocument>, FilterContainer> filter)
            where TDocument : class
        {
            // lookup не имеет заполненного filterName
            if (string.IsNullOrEmpty(filterName))
            {
                filter = (Func<FilterDescriptor<TDocument>, FilterContainer>)FilterMap.Where(x => x.Key.Item1 == typeof(TDocument)).Select(x => x.Value).First();
                return true;
            }

            var key = Tuple.Create(typeof(TDocument), filterName);

            Delegate @delegate;
            if (!FilterMap.TryGetValue(key, out @delegate))
            {
                filter = null;
                return false;
            }

            filter = (Func<FilterDescriptor<TDocument>, FilterContainer>)@delegate;
            return true;
        }
    }
}