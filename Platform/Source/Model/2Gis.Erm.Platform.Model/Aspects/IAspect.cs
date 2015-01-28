namespace DoubleGis.Erm.Platform.Model.Aspects
{
    /// <summary>
    /// Маркерный интерфейс аспекта. Предполагается, что данный интерфейс будет расширяться конкретными сторонами объекта, например: INameAspect, IDeactivatableAspect.
    /// Расширения данного интерфейсы могут реализовывать, например: сущности или Dto доменной области, view-model'и.
    /// </summary>
    public interface IAspect 
    {
    }
}
