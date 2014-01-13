using System;
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

            IEntityKey entity;
            switch (_entityType)
            {
                case EntityName.Order:
                    {
                        entity = _finder.Find(Specs.Find.ById<Order>(viewModel.Id)).Single();
                        break;
                    }

                case EntityName.Client:
                    {
                        entity = _finder.Find(Specs.Find.ById<Client>(viewModel.Id)).Single();
                        break;
                    }

                case EntityName.LegalPerson:
                    {
                        entity = _finder.Find(Specs.Find.ById<LegalPerson>(viewModel.Id)).Single();
                        break;
                    }

                case EntityName.Deal:
                    {
                        entity = _finder.Find(Specs.Find.ById<Deal>(viewModel.Id)).Single();
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException("Не работает журналирование для сущности " + _entityType);
            }

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
                    var differenceMap = CompareObjectsHelper.CompareObjects(_compareObjectMode, originalValue, entity, _elementsToIgnore);
                    _actionLogger.LogChanges(ChangesDescriptor.Create(_entityType, entity.Id, differenceMap));
                }
                catch (Exception ex)
                {
                    Logger.FatalEx(ex, "Критичная ошибка журналирования операций");
                }
            }

            return result;   
        }
    }
}