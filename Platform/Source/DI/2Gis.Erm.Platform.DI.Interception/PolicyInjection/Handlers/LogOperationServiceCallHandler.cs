﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Common.Utils;

using Microsoft.Practices.Unity.InterceptionExtension;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.DI.Interception.PolicyInjection.Handlers
{
    public sealed class LogOperationServiceCallHandler : LoggingCallHandler
    {
        private readonly IEnumerable<string> _elementsToIgnoreByDefault = new[] { "ModifiedOn", "ModifiedBy", "LastQualifyTime", "LastDisqualifyTime" };

        private readonly IActionLogger _actionLogger;
        private readonly IDependentEntityProvider _entityProvider;
        private readonly CompareObjectMode _compareObjectMode;
        private readonly IEnumerable<string> _elementsToIgnore;

        public LogOperationServiceCallHandler(ITracer tracer,
                                              IActionLogger actionLogger,
                                              IDependentEntityProvider entityProvider,
                                              CompareObjectMode compareObjectMode,
                                              IEnumerable<string> elementsToIgnore)
            : base(tracer)
        {
            _actionLogger = actionLogger;
            _entityProvider = entityProvider;
            _compareObjectMode = compareObjectMode;
            _elementsToIgnore = elementsToIgnore;
        }

        public override IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            Type operationInterface = null;
            Type entityType = null;
            long entityId = 0;

            var originalEntities = new IEntityKey[0];
            var entities = new IEntityKey[0];

            try
            {
                operationInterface = input.MethodBase.DeclaringType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && typeof(IEntityOperation).IsAssignableFrom(x));
                if (operationInterface != null)
                {
                    entityType = operationInterface.GetGenericArguments().Single();

                    var domainEntityDto = input.Arguments[0] as IDomainEntityDto;
                    if (domainEntityDto != null)
                    {
                        entityId = domainEntityDto.Id;
                    }
                    else
                    {
                        entityId = (long)input.Arguments[0];
                    }

                    if (entityId != 0)
                    {
                        entities = GetEntities(entityType, entityId);
                        originalEntities = entities.Select(CreateOriginalObjectDeepClone).ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Tracer.Fatal(ex, "Critical error occured during object copying before modification");
            }

            var result = getNext()(input, getNext);
            
            // Wrapping exception to prevent track trace loose
            if (result.Exception != null)
            {
                result.Exception = new TargetInvocationException(result.Exception.Message, result.Exception);
            }

            if (operationInterface != null && entities.Any() && originalEntities.Any() && result.Exception == null)
            {
                try
                {
                    var modifiedEntities = GetEntities(entityType, entityId);
                    foreach (var pair in originalEntities.Join(modifiedEntities, o => o, m => m, (o, m) => new { Original = o, Modified = m }))
                    {
                        var differenceMap = CompareObjectsHelper.CompareObjects(_compareObjectMode,
                                                                                pair.Original,
                                                                                pair.Modified,
                                                                                _elementsToIgnoreByDefault.Union(_elementsToIgnore ?? Enumerable.Empty<string>()));
                        _actionLogger.LogChanges(ChangesDescriptor.Create(pair.Original.GetType(), pair.Original.Id, differenceMap));
                    }
                }
                catch (Exception ex)
                {
                    Tracer.Fatal(ex, "Critical error occured during object's changes evaluating and logging");
                }
            }

            return result;
        }

        private static IEntityKey CreateOriginalObjectDeepClone(object originalObject)
        {
            var entityKey = originalObject as IEntityKey;
            return (entityKey == null) ? null : (IEntityKey)CompareObjectsHelper.CreateObjectDeepClone(originalObject);
        }

        private IEntityKey[] GetEntities(Type entityType, long entityId)
        {
            var parentEntityName = entityType.AsEntityName();

            // Возвращаем все сущности (из списка логгируемых), на которые могла повлиять операция над родительской сущностью
            var entityNames = _entityProvider.GetDependentEntityNames(parentEntityName);
            return entityNames.SelectMany(en => _entityProvider.GetDependentEntities(parentEntityName, en, entityId)).ToArray();
        }
    }
}