namespace NuClear.Storage.UseCases
{
    /// <summary>
    /// Интерфейс ключа для processing контекста - маркерный интерфейс, для реализующих его классов
    /// Типовое использование - класс реализует данный интерфейс, и при этом имеет статическое публичное поле Instance - т.е. являеться singleton
    /// </summary>
    public interface IContextKey<T>
    {
    }
}
