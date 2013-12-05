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

            var data = JsonConvert.SerializeObject(originalObject);
            return JsonConvert.DeserializeObject<TObject>(data);
        }
    }
}