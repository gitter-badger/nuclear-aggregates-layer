﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Deals
{
    // FIXME {all, 13.01.2014}: нужна конвертация в readmodel (видимо разные методы в разные readmodel) с перемещением в нужную сборку 
    public class ClientDealSelectionService : IClientDealSelectionService
    {
        private readonly IFinder _finder;

        public ClientDealSelectionService(IFinder finder)
        {
            _finder = finder;
        }

        public IEnumerable<Deal> GetOpenedDeals(long clientId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Deal>() && DealSpecs.Deals.Find.ForClient(clientId))
                          .Where(x => x.CloseDate == null);
        }

        public Deal GetDealForOrder(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(x => x.Deal)
                          .FirstOrDefault();
        }

        public long? GetOrderClientId(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(x => x.Firm.ClientId)
                          .Single();
        }

        public long? GetMainFirmId(long clientId)
        {
            return _finder.Find(Specs.Find.ById<Client>(clientId))
                          .Select(x => x.MainFirmId)
                          .Single();
        }
    }
}
