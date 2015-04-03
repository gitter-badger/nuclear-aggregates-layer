using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.Common.PrintFormEngine
{
    public sealed class PrintData : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _values;

        public PrintData()
        {
            _values = new Dictionary<string, object>();
        }

        public static PrintData Concat(params PrintData[] sources)
        {
            var result = new PrintData();
            foreach (var pair in sources.SelectMany(source => source))
            {
                result.Add(pair.Key, pair.Value);
            }

            return result;
        }

        public object GetData(string path)
        {
            var pathItems = path.Split('.');

            if (pathItems.Length == 1)
            {
                object value;
                return _values.TryGetValue(pathItems[0], out value) ? value : null;
            }

            var container = GetData(pathItems[0]) as PrintData;
            return container != null ? container.GetData(string.Join(".", pathItems.Skip(1))) : null;
        }

        public IEnumerable<PrintData> GetTable(string path)
        {
            var data = GetData(path) as IEnumerable<PrintData>;
            return data;
        }

        public PrintData GetObject(string path)
        {
            var data = GetData(path) as PrintData;
            return data;
        }

        public void Add(string path, object value)
        {
            var pathItems = path.Split('.');

            if (pathItems.Length == 1)
            {
                object oldValue;
                if (!_values.TryGetValue(pathItems[0], out oldValue))
                {
                    _values.Add(pathItems[0], value);
                }
                else if (!Equals(oldValue, value))
                {
                    var message = string.Format("Container already contains another value for {0}", path);
                    throw new ArgumentException(message);
                }

                return;
            }

            object container;
            _values.TryGetValue(pathItems[0], out container);
            var printDataContainer = container as PrintData;
            if (container == null)
            {
                printDataContainer = new PrintData();
                _values.Add(pathItems[0], printDataContainer);
            }
            else if (printDataContainer == null) // container exists but not a PrintData
            {
                throw new InvalidOperationException(string.Format("Container already contains field {0} but it is not container", pathItems[0]));
            }

            printDataContainer.Add(string.Join(".", pathItems.Skip(1)), value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            var result = new List<KeyValuePair<string, object>>();
            foreach (var pair in _values)
            {
                var container = pair.Value as PrintData;

                if (container != null)
                {
                    result.AddRange(container.Select(r => new KeyValuePair<string, object>(pair.Key + "." + r.Key, r.Value)));
                }
                else 
                {
                    result.Add(new KeyValuePair<string, object>(pair.Key, pair.Value));
                }
            }

            return result.GetEnumerator();
        }
    }
}
