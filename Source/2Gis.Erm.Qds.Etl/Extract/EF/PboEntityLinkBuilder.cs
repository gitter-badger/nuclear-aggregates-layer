using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    /// <summary>
    /// Преобразует записи лога операций (<see cref="PerformedBusinessOperation"/>) ERM в ссылки на сущности.
    /// </summary>
    public class PboEntityLinkBuilder : IEntityLinkBuilder
    {
        private readonly IOperationContextParser _contextParser;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="contextParser">Парсер контекста операции.</param>
        public PboEntityLinkBuilder(IOperationContextParser contextParser)
        {
            if (contextParser == null)
            {
                throw new ArgumentNullException("contextParser");
            }

            _contextParser = contextParser;
        }

        public IEnumerable<EntityLink> CreateEntityLinks(IChangeDescriptor changeDescriptor)
        {
            if (changeDescriptor == null)
            {
                throw new ArgumentNullException("changeDescriptor");
            }

            var pboDescr = changeDescriptor as PboChangeDescriptor;
            if (pboDescr == null)
            {
                throw new NotSupportedException(changeDescriptor.GetType().FullName);
            }

            return ExtractLinks(pboDescr.Operation);
        }

        private IEnumerable<EntityLink> ExtractLinks(PerformedBusinessOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            return from strictOperation in _contextParser.GetGroupedIdsFromContext(operation.Context, operation.Operation, operation.Descriptor)
                   from id in strictOperation.Value
                   select CreateLink(strictOperation.Key, id);
        }

        private static EntityLink CreateLink(StrictOperationIdentity identity, long id)
        {
            return new EntityLink(identity.Entities.Single(), id);
        }
    }
}