using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NuClear.Model.Common.Entities.Aspects;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs.Infrastructure
{
    public sealed class DomainEntityDtoRegistry : IDomainEntityDtoRegistry
    {
        private static readonly IReadOnlyDictionary<IEntityType, Type> DtoRegistry;

        static DomainEntityDtoRegistry()
        {
            var registry = new Dictionary<IEntityType, Type>();
            var dtoTypes = Assembly.GetExecutingAssembly().ExportedTypes
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IDomainEntityDto).IsAssignableFrom(t) && !t.IsPersistenceOnly());

            var domainEntityDtoGenericIndicator = typeof(IDomainEntityDto<>);

            foreach (var dtoType in dtoTypes)
            {
                var entityType = dtoType.GetInterfaces()
                       .Where(t => t.IsGenericType && domainEntityDtoGenericIndicator == t.GetGenericTypeDefinition())
                       .Select(t => t.GetGenericArguments().Single())
                       .Single();
                if (entityType.IsPersistenceOnly())
                {
                    continue;
                }

                registry.Add(entityType.AsEntityName(), dtoType);
            }
            
            DtoRegistry = registry;
        }

        public bool TryGetDomainEntityDto(IEntityType entityName, out Type domainEntityDtoType)
        {
            return DtoRegistry.TryGetValue(entityName, out domainEntityDtoType);
        }
    }
}
