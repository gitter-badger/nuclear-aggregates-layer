namespace NuClear.Storage.UseCases
{
    /// <summary>
    /// Интерфейс хранилища данных, где с типизированными ключами сопоставлены значения
    /// </summary>
    public interface IProcessingContext
    {
        /// <summary>
        /// Получить значение по ключу в контексте
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="key">Instance ключа</param>
        /// <param name="throwIfNotExists">Если <c>true</c>, то бросает exception потому что значение с таким ключои в контексте не зарегистрировано</param>
        T GetValue<T>(IContextKey<T> key, bool throwIfNotExists);

        /// <summary>
        /// Получить значение по ключу в контексте. Exception никогда не бросает, даже если в контексте нет запрошенных данных
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="key">Instance ключа</param>
        T GetValue<T>(IContextKey<T> key);

        /// <summary>
        /// Положить/изменить значение в контексте
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="key">Instance ключа</param>
        void SetValue<T>(IContextKey<T> key, T value);

        /// <summary>
        /// Проверяет зарегистрирован ли данный ключ в контексте
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="key">Instance ключа</param>
        bool ContainsKey<T>(IContextKey<T> key);
    }
}