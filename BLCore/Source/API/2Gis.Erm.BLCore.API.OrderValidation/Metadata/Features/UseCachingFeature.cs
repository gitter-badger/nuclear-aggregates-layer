using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata.Features
{
    /// <summary>
    /// Используем закешированные результаты пройденных проверок только для конкретных групп, 
    /// для которых определены события сброса признака корректности группы
    /// </summary>
    public sealed class UseCachingFeature : IUniqueMetadataFeature
    {
    }
}