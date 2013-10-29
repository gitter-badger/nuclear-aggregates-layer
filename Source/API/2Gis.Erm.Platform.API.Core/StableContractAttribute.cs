using System;

namespace DoubleGis.Erm.Platform.API.Core
{
    /// <summary>
    /// Маркерный атрибут, показывает, что элемент который им помечен, является частью контракта ERM и доступен через какой-то сервис сторонним клиентам, при этом,
    /// т.к. клиенты сторонние то они не должны напрямую зависеть от AssemblyQualifiedNames типов ERM (которые могут меняться, с каждой версией и даже билдом).
    /// AssemblyQualifiedNames используются в метаданных контрактов сервисов ERM, т.к. используется shared type resolver => клиенты должны передавать названия типов, таким образом,
    /// чтобы в Erm было понятно, что это за типы .NET.
    /// Чтобы в метаданных контрактов ERM типы имели правильные (AssemblyQualifiedNames) name/namespaces - используется используется механизм расширения генерируемых метаданных и он опирается 
    /// на AssemblyQualifiedNames.
    /// Вышеуказанное ограничение касается, клиентов которые используют статические генерируемые классы helper для доступа к сервисам ERM (например, .NET клиенты через add service reference), 
    /// если же при каждом старте клиента происходит актуализация метаданных (например, как в PHP), то проблема не стабильных name/namespaces особо не мешает.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public sealed class StableContractAttribute : Attribute
    {
    }
}