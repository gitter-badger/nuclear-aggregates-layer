namespace NuClear.Storage.Specifications
{
    public interface IProjectSpecification<in TInput, out TOutput>
    {
        TOutput Project(TInput input);
    }
}