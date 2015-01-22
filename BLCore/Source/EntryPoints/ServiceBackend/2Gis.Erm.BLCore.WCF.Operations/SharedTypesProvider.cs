using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    public class SharedTypesProvider
    {
        private static readonly IEnumerable<string> CoreAssemblyNameMarkers = new[] { "Common", "Core", "Infrastructure", "Model", "DTO" };

        private static readonly Lazy<IEnumerable<Type>> LazySharedTypes = new Lazy<IEnumerable<Type>>(GetSharedTypes);
        private static readonly Lazy<IDictionary<string, string>> LazyNamespacesByAssemblies = new Lazy<IDictionary<string, string>>(
            () => SharedTypes
                .Where(x => !string.IsNullOrEmpty(x.Namespace))
                .GroupBy(x => x.Namespace)
                .ToDictionary(x => x.Key, x => x.Select(y => y.Assembly.FullName).Distinct().FirstOrDefault()));

        private SharedTypesProvider()
        {
        }

        public static IEnumerable<Type> SharedTypes
        {
            get
            {
                return LazySharedTypes.Value;
            }
        }

        public static IDictionary<string, string> NamespacesByAssemblies
        {
            get
            {
                return LazyNamespacesByAssemblies.Value;
            }
        }

        // TODO {all, 30.07.2013}: Ограничить список типов экспортируемых в wsdl для каждого конкретного контракта wcf сервиса, только типами реально приходящими/уходящими из этого сервиса
        // Пока список экспортируемых типов одинаковый для для всех сервисов использующих одну и ту же точку входа и зависит 
        // от кол-ва реализаций базовых интерфейсов/подклассов (т.е. зависит от точки входа - какие сборки загружены и т.д.)
        // ЗАМЕЧАНИЕ - данная фабрика используется только при обработке WS*HTTP типов binding, 
        // т.е. для rest endpoint экспорт расширенной метаинформации для типов пока не выполняется
        // КАК УЛУЧШИТЬ - нужно параметризовать экспортер набором целевых типов, метаданные о которых нужно экспортировать в WSDL, 
        // для каждого servicecontract, набор должен быть свой
        // Т.о. для каждого endpoint с поддерживаемым типом binding будет добавлен свой экспорт behavior (параметризованный нужным для данного servicecontract списком типов) => и WSDL будет содержать знания только о типах, необходимых для использования конкретного servicecontract
        // КОГДА ПЕРЕДЕЛАТЬ - когда избыточные метаданные станут реальной проблемой. Пока всегда экспортируем метаданные всех типов, т.к. влияет это только на не .net клиентов SOAP части API ERM (не использующих shared type подход)
        // и такой клиент только 1 - команда ЛК, для ЛК избыточные метаданные проблемы не составляют. 
        private static IEnumerable<Type> GetSharedTypes()
        {
            return Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                           .Where(CommonBootstrapper.IsErmAssembly)
                           .Where(x => CoreAssemblyNameMarkers.Any(y => x.Name.ToUpper().Contains(y.ToUpper())))
                           .Select(Assembly.Load)
                           .SelectMany(x => x.ExportedTypes)
                           .Where(x => !x.GetCustomAttributes<StableContractAttribute>().Any() &&
                                        (x.IsEnum ||
                                        typeof(IEntity).IsAssignableFrom(x) ||
                                        typeof(IDomainEntityDto).IsAssignableFrom(x) ||
                                        typeof(IOperationSpecificEntityDto).IsAssignableFrom(x) ||
                                        typeof(IDataListResult).IsAssignableFrom(x) ||
                                        x == typeof(ListResult) || x.IsSubclassOf(typeof(ListResult)) ||
                                        x == typeof(EntityReference) ||
                                        x == typeof(AdvertisementDescriptor)) &&
                                       !x.IsGenericTypeDefinition)
                           .ToArray();
        }
    }
}