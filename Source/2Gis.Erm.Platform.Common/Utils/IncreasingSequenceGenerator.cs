using System.Threading;

namespace DoubleGis.Erm.Platform.Common.Utils
{
    /// <summary>
    /// Потокобезопасно генерирует монотонно возрастающую последовательность уникальных значений
    /// Значения являются уникальными в рамках одного и того же instance AppDomain
    /// </summary>
    public static class IncreasingSequenceGenerator
    {
        private static int _counter;

        /// <summary>
        /// Возвращает следующий уникальный элемент последовательности
        /// </summary>
        public static int Next 
        {
            get
            {
                return Interlocked.Increment(ref _counter);
            }
        }
    }
}
