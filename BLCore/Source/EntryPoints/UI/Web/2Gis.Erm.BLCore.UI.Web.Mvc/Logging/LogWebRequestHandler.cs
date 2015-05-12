using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DI.Interception.PolicyInjection.Handlers;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Microsoft.Practices.Unity.InterceptionExtension;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging
{
    public sealed class LogWebRequestHandler : LoggingCallHandler
    {
        private readonly IActionLogger _actionLogger;
        private readonly IEntityType _entityType;
        private readonly CompareObjectMode _compareObjectMode;
        private readonly IEnumerable<string> _elementsToIgnore;
        private readonly IFinder _finder;

        public LogWebRequestHandler(ITracer tracer,
                                    IActionLogger actionLogger,
                                    IEntityType entityType,
                                    CompareObjectMode compareObjectMode,
                                    IEnumerable<string> elementsToIgnore,
                                    IFinder finder)
            : base(tracer)
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
                Tracer.Fatal("Критичная ошибка журналирования объекта. Не удалось получить экземпляр объекта до изменения");
            }
            else
            {
                try
                {
                    originalValue = CompareObjectsHelper.CreateObjectDeepClone(entity);
                }
                catch (Exception ex)
                {
                    Tracer.Fatal(ex, "Критичная ошибка создания копии объекта до изменения");
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
                    Tracer.Fatal(ex, "Критичная ошибка журналирования операций");
                }
            }

            return result;
        }

        private IEntityKey GetEntity(IEntityViewModelBase viewModel)
        {
            // FIXME {a.rechkalov, 21.05.2014}: Давай будем пользоваться FindOne, если требуесть получить одну сущность, вне зависимости от ее типа. 
            // Текущая реализация вызывает вопросы "почему для Order используется Find, а для LegalPerson - FindOne"
            if (_entityType.Equals(EntityType.Instance.Order()))
            {
                    return _finder.Find(Specs.Find.ById<Order>(viewModel.Id)).Single();
            }

            if (_entityType.Equals(EntityType.Instance.Client()))
            {
                    return _finder.Find(Specs.Find.ById<Client>(viewModel.Id)).Single();
            }

            if (_entityType.Equals(EntityType.Instance.LegalPerson()))
            {
                    return _finder.FindOne(Specs.Find.ById<LegalPerson>(viewModel.Id));
            }

            if (_entityType.Equals(EntityType.Instance.Deal()))
            {
                    return _finder.Find(Specs.Find.ById<Deal>(viewModel.Id)).Single();
            }

                    throw new ArgumentOutOfRangeException("Не работает журналирование для сущности " + _entityType);
            }
        }
    }
