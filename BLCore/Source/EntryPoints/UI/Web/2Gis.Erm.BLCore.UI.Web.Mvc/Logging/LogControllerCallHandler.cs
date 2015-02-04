using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DI.Interception.PolicyInjection.Handlers;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using Microsoft.Practices.Unity.InterceptionExtension;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging
{
    public sealed class LogControllerCallHandler : LoggingCallHandler
    {
        private readonly IActionLogger _actionLogger;
        private readonly IDependentEntityProvider _entityProvider;

        public LogControllerCallHandler(ICommonLog logger, IActionLogger actionLogger, IDependentEntityProvider entityProvider)
            : base(logger)
        {
            _actionLogger = actionLogger;
            _entityProvider = entityProvider;
        }

        public override IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var originalEntities = new IEntityKey[0];
            var entities = new IEntityKey[0];

            try
            {
                entities = GetEntities(input);
                originalEntities = entities.Select(CreateOriginalObjectDeepClone).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Критичная ошибка создания копии объекта до изменения");
            }

            var result = getNext()(input, getNext);

            if (entities.Any() && originalEntities.Any() && result.Exception == null)
            {
                try
                {
                    var modifiedEntities = GetEntities(input);
                    foreach (var pair in originalEntities.Zip(modifiedEntities, (o, m) => new { Original = o, Modified = m }))
                    {
                        var differenceMap = CompareObjectsHelper.CompareObjects(CompareObjectMode.Shallow,
                                                                                pair.Original,
                                                                                pair.Modified,
                                                                                new[] { "ModifiedOn", "ModifiedBy", "LastQualifyTime", "LastDisqualifyTime" });
                        _actionLogger.LogChanges(ChangesDescriptor.Create(pair.Original.GetType(), pair.Original.Id, differenceMap));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Fatal(ex, "Критичная ошибка журналирования операций");
                }
            }

            return result;
        }

        private static IEntityKey CreateOriginalObjectDeepClone(object originalObject)
        {
            var entityKey = originalObject as IEntityKey;
            return (entityKey == null) ? null : (IEntityKey)CompareObjectsHelper.CreateObjectDeepClone(originalObject);
        }

        private IEntityKey[] GetEntities(IMethodInvocation input)
        {
            IEnumerable<EntityName> entityNames;

            if (input.MethodBase.Name == "Merge")
            {
                var viewModel = input.Arguments[0];
                if (viewModel == null || !viewModel.GetType().Name.Contains("ClientViewModel"))
                {
                    return new IEntityKey[] { };
                }

                var id = (long)viewModel.GetPropertyValue("AppendedClient");
                entityNames = _entityProvider.GetDependentEntityNames(EntityName.Client);

                return entityNames.SelectMany(en => _entityProvider.GetDependentEntities(EntityName.Client, en, id, true)).ToArray();
            }

            var parentEntityName = (EntityName)input.Arguments[0];
            var parentId = (long)input.Arguments[1];
            entityNames = _entityProvider.GetDependentEntityNames(parentEntityName);

            // Возвращаем все сущности (из списка логгируемых), на которые могла повлиять операция над родительской сущностью
            return entityNames.SelectMany(en => _entityProvider.GetDependentEntities(parentEntityName, en, parentId)).ToArray();
        }
    }
}