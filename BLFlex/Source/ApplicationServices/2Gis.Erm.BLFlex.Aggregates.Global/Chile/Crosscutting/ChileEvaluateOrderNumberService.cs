﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Shared;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Crosscutting
{
    public class ChileEvaluateOrderNumberService : IEvaluateOrderNumberService, IChileAdapted
    {
        public const string OrderNumberTemplate = "ORD_{0}-{1}-{2}";
        private readonly IEnumerable<IOrderNumberGenerationStrategy> _strategies;

        public ChileEvaluateOrderNumberService(IEnumerable<IOrderNumberGenerationStrategy> strategies)
        {
            _strategies = strategies;
        }

        public string Evaluate(string currentOrderNumber,
                               string sourceOrganizationUnitSyncCode1C,
                               string destinationOrganizationUnitSyncCode1C,
                               long? generatedOrderIndex,
                               OrderType orderType)
        {
            return _strategies.Execute(OrderNumberTemplate,
                                       currentOrderNumber,
                                       sourceOrganizationUnitSyncCode1C,
                                       destinationOrganizationUnitSyncCode1C,
                                       generatedOrderIndex);
        }

        public string EvaluateRegional(string currentOrderNumber,
                                       string sourceOrganizationUnitSyncCode1C,
                                       string destinationOrganizationUnitSyncCode1C,
                                       long? generatedOrderIndex)
        {
            throw new NotSupportedException(BLResources.RegionalOrdersAreNotSupported);
        }
    }
}