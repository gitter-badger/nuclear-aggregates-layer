namespace DoubleGis.Erm.Platform.Model.Aspects
{
    public interface ISetReadOnlyAspect : IAspect
    {
        // TODO {all, 24.01.2015}: есть мнение, что проверки, стоящие за этим свойством лучше вынести в кастомизцию
        bool SetReadonly { get; }
    }
}