using System;
using System.Collections.Generic;

namespace NuClear.Storage.UseCases
{
    /// <summary>
    /// Хранилище данных, где с типизированными ключами сопоставлены значения
    /// Чтобы поместить что-то в контекст или получить что-то оттуда, нужно знать instance класса, который реализует маркерный интерфейс <see cref="NuClear.Storage.UseCases.IContextKey{T}"/>
    /// </summary>
    public sealed class ProcessingContext : IProcessingContext
    {
        private readonly IDictionary<Type, object> _values = new Dictionary<Type, object>();
        
        /// <summary>
        /// Объект для синхронизации
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// Получить значение по ключу в контексте
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="key">Instance ключа</param>
        /// <param name="throwIfNotExists">Если <c>true</c>, то бросает exception, потому что значение с таким ключом в контексте не зарегистрировано</param>
        /// <returns>Значение</returns>
        public T GetValue<T>(IContextKey<T> key, bool throwIfNotExists)
        {
            lock (_lockObject)
            {
                object result = GetValueInternal(key.GetType(), throwIfNotExists);
                return result != null ? (T)result : default(T);
            }
        }

        public T GetValue<T>(IContextKey<T> key)
        {
            return GetValue(key, false);
        }

        /// <summary>
        /// Положить/изменить значение в контексте
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="key">Instance ключа</param>
        /// <param name="value">Значение</param>
        public void SetValue<T>(IContextKey<T> key, T value)
        {
            lock (_lockObject)
            {
                SetValueInternal(key.GetType(), value);
            }
        }
        
        /// <summary>
        /// Проверяет зарегистрирован ли данный ключ в контексте
        /// </summary>
        public bool ContainsKey<T>(IContextKey<T> key)
        {
            lock (_lockObject)
            {
                return _values.ContainsKey(key.GetType());
            }
        }

        private void SetValueInternal(Type key, object value)
        {
            if (_values.ContainsKey(key))
            {
                _values[key] = value;
            }
            else
            {
                _values.Add(key, value);
            }
        }

        private object GetValueInternal(Type key, bool throwIfNotExists)
        {
            object result;
            if (!_values.TryGetValue(key, out result))
            {
                if (throwIfNotExists)
                {
                    throw new InvalidOperationException(string.Format("Value with key {0} not exists in context", key));
                }
            }

            return result;
        }
    }
}
