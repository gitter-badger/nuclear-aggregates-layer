﻿using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model;

[assembly: ContainedTypes(
    typeof(IOperation),
    typeof(IOrderValidationRule),
    typeof(IRequestHandler))]