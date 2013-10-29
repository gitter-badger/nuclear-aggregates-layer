﻿using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace DoubleGis.Erm.Platform.Common.Utils
{
    public enum CompareObjectMode
    {
        Deep = 1,
        Shallow
    }

    public class CompareObjectsHelper
    {
        public static Dictionary<string, Tuple<object, object>> CompareObjects(CompareObjectMode compareObjectMode,
                                                                               object originalObject,
                                                                               object modifiedObject,
                                                                               IEnumerable<string> elementsToIgnore)
        {
            if (elementsToIgnore == null)
            {
                elementsToIgnore = Enumerable.Empty<string>();
            }
            var objectComparer = new CompareObjects
                {
                    CompareChildren = compareObjectMode == CompareObjectMode.Deep,
                    CompareFields = false,
                    ComparePrivateFields = false,
                    CompareReadOnly = false,
                    ComparePrivateProperties = false,
                    CompareProperties = true,
                    ElementsToIgnore = new List<string>(elementsToIgnore.Union(new[] { "Timestamp", "ChangeTracker" })),
                    MaxDifferences = 1000,
                };
            objectComparer.Compare(originalObject, modifiedObject);
            return objectComparer.DifferenceMap;
        }

        public static object CreateObjectDeepClone(object originalObject)
        {
            if (originalObject == null)
            {
                return null;
            }

            var data = JsonConvert.SerializeObject(originalObject);
            return JsonConvert.DeserializeObject(data, originalObject.GetType());
        }
    }
}