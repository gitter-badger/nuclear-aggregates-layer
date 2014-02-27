using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DoubleGis.Erm.BL.Reports
{
    public class ProtectedDictionary : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _collection = null;

        /// <summary>
        /// Создает экземпляр защищенного словаря.
        /// Защищенный словарь не предоставляет методов для добавления/удаления элементов.
        /// Только изменение значений существующих элементов
        /// </summary>
        /// <param name="dictionary">Словарь из которого зоздается защищенный экземпляр</param>
        public ProtectedDictionary(Dictionary<string, object> dictionary)
        {
            _collection = new Dictionary<string, object>(dictionary);
        }

        public ProtectedDictionary()
        {
            _collection = new Dictionary<string, object>();
        }

        public object this[string key]
        {
            get { return _collection[key]; }

            set { _collection[key] = value; }
        }

        public object this[int index]
        {
            get { return _collection.Values.ToArray()[index]; }

            set { _collection.Values.ToArray()[index] = value; }
        }

        public int Count
        {
            get { return _collection.Count; }
        }

        public bool ContainsKey(string key)
        {
            return _collection.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Join("; ", _collection.Select(item => item.ToString()).ToArray());
        }

        public void AddCommandParameters(IDbCommand command)
        {
            foreach (var parameter in this)
            {
                var commandParameter = command.CreateParameter();
                commandParameter.ParameterName = parameter.Key;
                commandParameter.Value = parameter.Value ?? DBNull.Value;
                command.Parameters.Add(commandParameter);
            }
        }

        public void SetCommandParameters(IDbCommand command)
        {
            command.Parameters.Clear();
            AddCommandParameters(command);
        }

        internal void Add(string key, object value)
        {
            _collection.Add(key, value);
        }

        //internal void Clear()
        //{
        //    _collection.Clear();
        //}

        public static ProtectedDictionary Empty
        {
            get
            {
                return new ProtectedDictionary(new Dictionary<string, object>());
            }
        }

        public ProtectedDictionary Clone()
        {
            return new ProtectedDictionary(_collection);
        }
    }
}
