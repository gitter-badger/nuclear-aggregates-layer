using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    // FIXME {all, 04.06.2014}: после очистки PerformedBusinessOperations от записей со старым форматом контекста (лето 2013 года)  нужно удалить
    [Obsolete("устаревший parser формирует набор фиктивных операций над множествами сущностей, после перехода на транспорт выполненных бизнес операций, удалить")]
    public sealed class OldOperationContextParser : IOldOperationContextParser
    {
        private static readonly Lazy<Dictionary<int, EntitySet>> EntitySetHashes = new Lazy<Dictionary<int, EntitySet>>(
           () =>
           {
               var serviceEntityNames = new IEntityType[] { EntityType.Instance.None(), EntityType.Instance.All() };

               var result = new Dictionary<int, EntitySet>();
               var values = EntityType.Instance.GetTypes().Where(x => !serviceEntityNames.Contains(x)).ToArray();
               foreach (var value in values)
               {
                   result.Add(new[] { value }.EvaluateHash(), new EntitySet(value));
                   result.Add(new[] { value, value }.EvaluateHash(), new EntitySet(value));
               }

               return result;
           });

        private readonly IOperationIdentityRegistry _operationIdentityRegistry;

        public OldOperationContextParser(IOperationIdentityRegistry operationIdentityRegistry)
        {
            _operationIdentityRegistry = operationIdentityRegistry;
        }

        [Obsolete("метод формирует набор фиктивных операций над множествами сущностей, после перехода на транспорт выполненных бизнес операций, удалить")]
        public IReadOnlyDictionary<StrictOperationIdentity, IEnumerable<long>> GetGroupedIdsFromContext(string context, int operation, int descriptor)
        {
            var operationIdentity = _operationIdentityRegistry.GetIdentity(operation);
            return GetGroupedIdsFromContext(context, operationIdentity, descriptor);
        }

        private IReadOnlyDictionary<StrictOperationIdentity, IEnumerable<long>> GetGroupedIdsFromContext(
            string context,
            IOperationIdentity operationIdentity,
            int descriptor)
        {
            var result = new Dictionary<StrictOperationIdentity, IEnumerable<long>>();
            using (var reader = new StringReader(context))
            {
                var element = XElement.Load(reader);
                foreach (var entity in element.Elements("entity"))
                {
                    var id = long.Parse(entity.Attribute("id").Value);

                    var typeAttribute = entity.Attribute("type");
                    var entitySet = typeAttribute == null
                        ? EvaluateEntitySetFromHash(operationIdentity.Id, descriptor)
                        : new EntitySet(EntityType.Instance.Parse(int.Parse(typeAttribute.Value)));

                    var strictOperationIdentity = new StrictOperationIdentity(operationIdentity, entitySet);

                    IEnumerable<long> ids;
                    if (!result.TryGetValue(strictOperationIdentity, out ids))
                    {
                        ids = new List<long>();
                        result.Add(strictOperationIdentity, ids);
                    }

                    ((List<long>)ids).Add(id);
                }
            }

            return result;
        }

        // FIXME {d.ivanov, 02.09.2013}: Код для восстановления EntityName из дескриптора операции. Убрать после того, как можно будет убедиться, что все операции, где type не указан, - обработаны
        private EntitySet EvaluateEntitySetFromHash(int operation, int descriptor)
        {
            var entitiesWithFile = new IEntityType[] { EntityType.Instance.AdvertisementElement(), EntityType.Instance.Theme(), EntityType.Instance.ThemeTemplate() };

            EntitySet entitySet;
            if (EntitySetHashes.Value.TryGetValue(descriptor, out entitySet))
            {
                // FIXME {d.ivanov, 03.09.2013}: Код для подмены EntityName, полученного из операции Upload. Убрать после того, как для всех измененных сущностей, кроме id, будет высталяться type в контексте бизнес-операци
                if (operation == UploadIdentity.Instance.Id &&
                    entitySet.Entities.Count() == 1 &&
                    entitiesWithFile.Contains(entitySet.Entities.First()))
                {
                    return new EntitySet(EntityType.Instance.File());
                }

                return entitySet;
            }

            throw new ArgumentException(string.Format("Entity name cannot be evaluated from operation. Descriptor value is {0}", descriptor));
        }
    }
}
