using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace DoubleGis.Erm.Platform.Common.Utils
{
    public enum CompareObjectMode
    {
        Deep = 1,
        Shallow = 2
    }

    public static class CompareObjectsHelper
    {
        public static IReadOnlyDictionary<string, PropertyChangeDescriptor> CompareObjects(
            CompareObjectMode compareObjectMode,
            object originalObject,
            object modifiedObject,
            IEnumerable<string> elementsToIgnore)
        {
            var defaultIgnoredElements = new[] { "Timestamp", "ChangeTracker" };

            var objectComparer = new CompareObjects
                {
                    CompareChildren = compareObjectMode == CompareObjectMode.Deep,
                    CompareEnumChildren = true,
                    CompareFields = false,
                    ComparePrivateFields = false,
                    CompareReadOnly = false,
                    ComparePrivateProperties = false,
                    CompareProperties = true,
                    ElementsToIgnore = new List<string>(elementsToIgnore == null 
                                            ? defaultIgnoredElements 
                                            : elementsToIgnore.Union(defaultIgnoredElements)),
                    MaxDifferences = 1000,
                };

            objectComparer.Compare(originalObject, modifiedObject);
            return objectComparer.DifferenceMap;
        }

        public static TObject CreateObjectDeepClone<TObject>(TObject originalObject) where TObject : class
        {
            if (originalObject == null)
            {
                return null;
            }

            // Важно, чтобы указывались имена только для типов, но не для коллекций, чтобы не было попыток инстанцировать каой-нибудь итератор вместо массива.
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            var data = JsonConvert.SerializeObject(originalObject, settings);
            return (TObject)JsonConvert.DeserializeObject(data, originalObject.GetType(), settings);
        }
    }
}