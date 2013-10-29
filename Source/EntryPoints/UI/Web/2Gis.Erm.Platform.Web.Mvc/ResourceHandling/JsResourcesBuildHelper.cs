using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

using DoubleGis.Erm.Platform.API.Core.Settings;

namespace DoubleGis.Erm.Platform.Web.Mvc.ResourceHandling
{
    /// <summary>
    /// Вспомогательный класс, используется в t4-преобразовании JsResourcesGeneration
    /// </summary>
    public class JsResourcesBuildHelper
    {
        public const string JsResourceFileNameFormat = "Ext.LocalizedResources.{0}.js";

        private readonly List<Type> _resourceDictionaries;
        private readonly SortedDictionary<string, string> _processedResourceStrings = new SortedDictionary<string, string>();
        private readonly List<string> _notFoundKeys = new List<string>();
        private readonly List<string> _invalidCultureKeys = new List<string>();
        private readonly List<string> _allKeys = new List<string>(200);
        private readonly List<ResourceAssemblyInfo> _assemblyInfos;

        public int ProcessedFilesCount { get; private set; }
        public int ProcessedMatchesCount { get; private set; }

        public List<ResourceAssemblyInfo> AssemblyInfos
        {
            get { return _assemblyInfos; }
        }

        public SortedDictionary<string, string> ProcessedResourceStrings
        {
            get { return _processedResourceStrings; }
        }

        public List<string> NotFoundKeys
        {
            get { return _notFoundKeys; }
        }

        public List<string> InvalidCultureKeys
        {
            get { return _invalidCultureKeys; }
        }

        public List<string> AllKeys
        {
            get { return _allKeys; }
        }

        public JsResourcesBuildHelper(IEnumerable<ResourceAssemblyInfo> resourceAssemblyInfos)
        {
            _assemblyInfos = resourceAssemblyInfos.ToList();

            _resourceDictionaries = new List<Type>();

            foreach (ResourceAssemblyInfo resourceAssemblyInfo in resourceAssemblyInfos)
            {
                Assembly a = Assembly.Load(resourceAssemblyInfo.AssemblyName);

                var dictTypes = (from type in a.ExportedTypes
                                 from resName in resourceAssemblyInfo.ResXFiles
                                 where type.Name == resName
                                 select type).ToList();

                if (dictTypes.Any())
                {
                    dictTypes.ForEach(type =>
                    {
                        if (!_resourceDictionaries.Any(resourceType => resourceType == type))
                        {
                            _resourceDictionaries.Add(type);
                        }
                    });
                }
            }

            foreach (Type type in _resourceDictionaries)
            {
                ResourceManager rm = new ResourceManager(type);
                foreach(CultureInfo culture in LocalizationSettings.SupportedCultures)
                {
                    var resourceSet = rm.GetResourceSet(culture, true, true);
                    IDictionaryEnumerator enumerator = resourceSet.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        _allKeys.Add(enumerator.Key.ToString());
                    }
                }
            }
        }

        public void ProcessResourceManagers(CultureInfo cultureInfo)
        {
            var resourceDicts = _resourceDictionaries.Select(type => new ResourceManager(type)).ToArray();

            var resourceSets = resourceDicts.Select(x => x.GetResourceSet(cultureInfo, true, true)).ToArray();

            foreach(string key in _allKeys)
            {
                foreach(var rs in resourceSets)
                {
                    string value = rs.GetString(key);
                    if(value == null)
                    {
                        value = FindInResourceManagers(resourceDicts, key);
                        if(value != null)
                        {
                            _invalidCultureKeys.Add(key);
                        }
                        else
                        {
                            _notFoundKeys.Add(key);
                        }
                    }

                    if(value != null)
                    {
                        _processedResourceStrings[key] = value;
                    }
                }
            }

            //foreach (Type type in _resourceDictionaries)
            //{
            //    ResourceManager rm = new ResourceManager(type);
            //    ResourceSet resourceSet = rm.GetResourceSet(cultureInfo, true, true);
            //    IDictionaryEnumerator enumerator = resourceSet.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        _processedResourceStrings[enumerator.Key.ToString()] = enumerator.Value.ToString();
            //    }
            //}
        }

        private string FindInResourceManagers(ResourceManager[] managers, string key)
        {
            string result = null;
            for (int i = 0; i < managers.Length; i++)
            {
                result = managers[i].GetString(key);
                if (result != null)
                    break;
            }

            return result;
        }

    }
}
