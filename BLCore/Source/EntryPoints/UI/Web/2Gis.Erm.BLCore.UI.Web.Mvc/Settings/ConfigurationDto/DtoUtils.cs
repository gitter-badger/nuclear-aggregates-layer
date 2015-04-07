using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;

using Omu.ValueInjecter;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto
{
    public static class DtoUtils
    {
        private sealed class CloneInjection : ConventionInjection
        {
            protected override bool Match(ConventionInfo c)
            {
                return c.SourceProp.Name == c.TargetProp.Name && c.SourceProp.Value != null;
            }

            protected override object SetValue(ConventionInfo c)
            {
                //for value types and string just return the value as is
                if (c.SourceProp.Type.IsValueType || c.SourceProp.Type == typeof(string)
                    || c.TargetProp.Type.IsValueType || c.TargetProp.Type == typeof(string))
                    return c.SourceProp.Value;

                //handle arrays
                if (c.SourceProp.Type.IsArray)
                {
                    var arr = c.SourceProp.Value as Array;
                    var clone = Activator.CreateInstance(c.TargetProp.Type, arr.Length) as Array;

                    for (int index = 0; index < arr.Length; index++)
                    {
                        var a = arr.GetValue(index);
                        if (a.GetType().IsValueType || a.GetType() == typeof(string)) continue;
                        clone.SetValue(Activator.CreateInstance(c.TargetProp.Type.GetElementType()).InjectFrom<CloneInjection>(a), index);
                    }
                    return clone;
                }


                if (c.SourceProp.Type.IsGenericType)
                {
                    //handle IEnumerable<> also ICollection<> IList<> List<>
                    if (c.SourceProp.Type.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
                    {
                        var t = c.TargetProp.Type.GetGenericArguments()[0];
                        if (t.IsValueType || t == typeof(string)) return c.SourceProp.Value;

                        var tlist = typeof(List<>).MakeGenericType(t);
                        var list = Activator.CreateInstance(tlist);

                        var addMethod = tlist.GetMethod("Add");
                        foreach (var o in c.SourceProp.Value as IEnumerable)
                        {
                            var e = Activator.CreateInstance(t).InjectFrom<CloneInjection>(o);
                            addMethod.Invoke(list, new[] { e }); // in 4.0 you can use dynamic and just do list.Add(e);
                        }
                        return list;
                    }

                    //unhandled generic type, you could also return null or throw
                    return c.SourceProp.Value;
                }

                //for simple object types create a new instace and apply the clone injection on it
                return Activator.CreateInstance(c.TargetProp.Type)
                    .InjectFrom<CloneInjection>(c.SourceProp.Value);
            }
        }

        /// <summary>
        /// Временное решение. Выполняет mapping настроек гридов для сущности из метаданных в специфичные структуры для web mvc клиента - необходимо для поддержки jsonignore и т.п. специфики
        /// По факту цель просто ограничить влияние изменений в метаданных, до их стабилизации.
        /// В итоге необходимо откзаться от mapping и использовать, непосредственно, те же типы, что используются в метаданных
        /// </summary>
        public static EntityViewSet ToEntityViewSet(this EntityDataListsContainer dataListsContainer)
        {
            var result = new EntityViewSet();
            result.InjectFrom<CloneInjection>(dataListsContainer);
            return result;
        }
    }
}