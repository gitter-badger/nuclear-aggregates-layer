﻿using System;
using System.Collections.Generic;
using System.Linq;


using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DI.Interception.PolicyInjection.Handlers;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using Microsoft.Practices.Unity.InterceptionExtension;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging
{
    public sealed class LogWebRequestHandler : LoggingCallHandler
    {
        private readonly IActionLogger _actionLogger;
        private readonly EntityName _entityType;
        private readonly CompareObjectMode _compareObjectMode;
        private readonly IEnumerable<string> _elementsToIgnore;
        private readonly IFinder _finder;

        public LogWebRequestHandler(ICommonLog logger, IActionLogger actionLogger, EntityName entityType, CompareObjectMode compareObjectMode, IEnumerable<string> elementsToIgnore, IFinder finder)
            : base(logger)
        {
            _actionLogger = actionLogger;
            _entityType = entityType;
            _compareObjectMode = compareObjectMode;
            _elementsToIgnore = elementsToIgnore;
            _finder = finder;
        }

        public override IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var viewModel = (IEntityViewModelBase)input.Arguments[0];
            object originalValue = null;

            var entity = GetEntity(viewModel);

            if (entity == null)
            {
                Logger.FatalEx("Критичная ошибка журналирования объекта. Не удалось получить экземпляр объекта до изменения");
            }
            else
            {
                try
                {
                    originalValue = CompareObjectsHelper.CreateObjectDeepClone(entity);
                }
                catch (Exception ex)
                {
                    Logger.FatalEx(ex, "Критичная ошибка создания копии объекта до изменения");
                }
            }

            var result = getNext()(input, getNext);
            if (entity != null && result.Exception == null)
            {
                try
                {
                    var modifiedEntity = GetEntity(viewModel);
                    var differenceMap = CompareObjectsHelper.CompareObjects(_compareObjectMode, originalValue, modifiedEntity, _elementsToIgnore);
                    _actionLogger.LogChanges(ChangesDescriptor.Create(_entityType, entity.Id, differenceMap));
                }
                catch (Exception ex)
                {
                    Logger.FatalEx(ex, "Критичная ошибка журналирования операций");
                }
            }

            return result;
        }

        private IEntityKey GetEntity(IEntityViewModelBase viewModel)
        {
            switch (_entityType)
            {
                case EntityName.Order:
                    return _finder.Find(Specs.Find.ById<Order>(viewModel.Id)).Single();

                case EntityName.Client:
                    return _finder.Find(Specs.Find.ById<Client>(viewModel.Id)).Single();

                case EntityName.LegalPerson:
                    return _finder.Find(Specs.Find.ById<LegalPerson>(viewModel.Id)).Single();

                case EntityName.Deal:
                    return _finder.Find(Specs.Find.ById<Deal>(viewModel.Id)).Single();

                default:
                    throw new ArgumentOutOfRangeException("Не работает журналирование для сущности " + _entityType);
            }
        }
    }
}