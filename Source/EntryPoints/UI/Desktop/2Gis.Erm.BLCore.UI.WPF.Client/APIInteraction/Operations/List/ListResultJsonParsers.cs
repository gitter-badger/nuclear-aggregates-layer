using System;
using System.Collections;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.List
{
    public static class ListResultJsonParsers
    {
        private class ListResultDescriptor
        {
            public string MainAttribute { get; set; }
            public int RowCount { get; set; }
            public ListResultType ResultType { get; set; }
            public string DataRawValue { get; set; }
            public JObject ListResultJObject { get; set; }
        }

        public static ListResult ParseListResult(string listResultJson)
        {
            var listResultDescriptor = GetListResultDescriptionFromJson(listResultJson);
            ListResult listResult = null;

            var serializer = JsonSerializer.Create(new JsonSerializerSettings
                                                       {
                                                           Converters = new JsonConverterCollection { new DotNetDateTimeConverter(), new StringEnumConverter() },
                                                           DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
                                                       });
            switch (listResultDescriptor.ResultType)
            {
                case ListResultType.Dto:
                {
                    var dtoTypeString = listResultDescriptor.ListResultJObject[PropertyNames.EntityDtosListDtoType].Value<string>();
                    if (string.IsNullOrEmpty(dtoTypeString))
                    {
                        throw new InvalidOperationException("Can't get required dto type from list result");
                    }

                    // TODO : ����� �������, ��� � ���������� �������� ������ ����� ������������ ������������ ������ ������ � ������� ��������� DTO
                    // �������� ��������� runitme ���������, � ���������� ��� ���, ������ - �������� �� ����� ������� �� ���� �������
                    var dtoType = Type.GetType(
                        dtoTypeString, 
                        AssemblyResolver,
                        (assem, name, ignore) => assem == null ? Type.GetType(name, false, ignore) : assem.GetType(name, false, ignore));
                    if (dtoType == null)
                    {
                        throw new InvalidOperationException("Can't resolve dto type from string value: " + dtoTypeString);
                    }

                    var realResultType = typeof(EntityDtoListResult);
                    var dataProperty = realResultType.GetProperty(PropertyNames.Data);
                    if (dataProperty == null)
                    {
                        throw new InvalidOperationException("Can't get Data property for type: " + realResultType);
                    }

                    var result = Activator.CreateInstance(realResultType);
                    var collectionResult = serializer.Deserialize(new StringReader(listResultDescriptor.DataRawValue), dtoType.MakeArrayType());
                    dataProperty.SetValue(result, collectionResult);
                    listResult = (ListResult)result;
                    break;
                }
            }

            if (listResult == null)
            {
                throw new InvalidOperationException("Unsupported type of list result: " + listResultDescriptor.ResultType);
            }

            listResult.MainAttribute = listResultDescriptor.MainAttribute;
            listResult.RowCount = listResultDescriptor.RowCount;

            return listResult;
        }

        private static Assembly AssemblyResolver(AssemblyName assemblyName)
        {
            Assembly mostlyAppropriateResult = null;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var checkingName = assembly.GetName();
                if (string.Compare(checkingName.FullName, assemblyName.FullName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return assembly;
                }

                if (AssemblyName.ReferenceMatchesDefinition(checkingName, assemblyName)
                    && checkingName.CultureInfo.Equals(assemblyName.CultureInfo))
                {
                    bool isKeysEquals = true;

                    var sourceSequance = checkingName.GetPublicKeyToken();
                    var targetSequance = assemblyName.GetPublicKeyToken();

                    if (sourceSequance == null || targetSequance == null)
                    {
                        continue;
                    }

                    if (sourceSequance.Length != targetSequance.Length)
                    {
                        continue;
                    }

                    for (int i = 0; i < sourceSequance.Length; i++)
                    {
                        if (sourceSequance[i] != targetSequance[i])
                        {
                            isKeysEquals = false;
                            break;
                        }
                    }

                    if (isKeysEquals)
                    {
                        mostlyAppropriateResult = assembly;
                    }
                }
            }

            return mostlyAppropriateResult;
        }

        private static ListResultDescriptor GetListResultDescriptionFromJson(string listResultJson)
        {
            ListResultType resultType;
            var jsonObject = JObject.Parse(listResultJson);
            if (!jsonObject.TryGetEnumValue(PropertyNames.ResultType, out resultType))
            {
                throw new InvalidOperationException("Can't determine list result type from input json");
            }

            var mainAttribute = jsonObject[PropertyNames.MainAttribute].Value<string>();
            var count = jsonObject[PropertyNames.RowCount].Value<int>();

            // ������ ������������� jsonObject[PropertyNames.Data] JArray ��� raw value ��� ����������� �������������� � �������������� ���������
            // ���������� ������� (����� reader) ������������ raw json ��� ������, �.�. �� JArray �� ������� ��������� �������� ���, ���� � �������������� DotNetDateTimeConverter
            // ������� - ��� �������������� �������� ������ � ���� JArray - ���������� ���������� �������� ������� � ������ � ��� ����������� ������������ DotNetDateTimeConverter
            // �� ����� ��������� ��� �������� - ��� ��� ������ DateTime - ��������� �� ��������� "FirstEmitDate": new Date("03/01/2013 00:00:00") ������� ����������  StartConstructor tokenType
            // �� ��� ���� ��� value = null, �.�. ���������� ��� ��� ������ ��� Date �� ��������, �������� ����:
            // 1).������� ���������:
            //  - ������ �� ���������
            //  - ������ � ��������� �������� jsonreader - � ������ �������� �������� �������� ��������������� ����
            // 2). ������ �� JArray � �������� �� �����������, raw �������� json - ���� ������ ���� �������
            // GetPropertyRawValue ����������� ������������ ������ ������ (Data : []), ������� ��� ���� �������.
            var data = count > 0 ? GetPropertyRawValue(listResultJson, PropertyNames.Data) : "[]";

            return new ListResultDescriptor { ResultType = resultType, MainAttribute = mainAttribute, RowCount = count, DataRawValue = data, ListResultJObject = jsonObject };
        }

        private static string GetPropertyRawValue(string inputJson, string propertyName)
        {
            using (var reader = new JsonTextReader(new StringReader(inputJson)))
            {
                const int InitialTargetPropertyLevel = -1;
                int targetPropertyLevel = InitialTargetPropertyLevel;
                int targetPropertyStartPosition = 0;
                bool insidePropertyJson = false;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName
                        && targetPropertyLevel == InitialTargetPropertyLevel
                        && !string.IsNullOrEmpty(reader.Path) 
                        && string.Compare(propertyName, reader.Path) == 0)
                    {
                        targetPropertyLevel = reader.Depth;
                        targetPropertyStartPosition = reader.LinePosition;
                        continue;
                    }
                        
                    if (!insidePropertyJson 
                        && targetPropertyLevel != InitialTargetPropertyLevel 
                        && reader.Depth > targetPropertyLevel)
                    {   // ����� ������ json value ��� �������� �������� - ������ ������ json value
                        insidePropertyJson = true;
                    }

                    if (insidePropertyJson 
                        && reader.Depth == targetPropertyLevel)
                    {   // ����� �� json value �������� �������� - ������ ��������� json value
                        var targetPropertyEndPosition = reader.LinePosition;
                        return inputJson.Substring(targetPropertyStartPosition, targetPropertyEndPosition - targetPropertyStartPosition);
                    }
                }
            }

            throw new InvalidOperationException("Can't find property " + propertyName);
        }

        private static bool TryGetEnumValue<TEnum>(this JToken token, string enumPropertyName, out TEnum enumValue, TEnum defaultValue = default(TEnum)) 
            where TEnum : struct
        {
            enumValue = defaultValue;

            var enumStringValue = token[enumPropertyName];
            if (enumStringValue == null)
            {
                return false;
            }

            if (!Enum.TryParse(enumStringValue.Value<string>(), out enumValue))
            {
                return false;
            }

            return true;
        }

        private static class PropertyNames
        {
            private class FakeEntity : IEntityKey
            {
                #region Implementation of IEntityKey

                public long Id { get; set; }

                #endregion
            }

            private class ListFakeEntityDto : IOperationSpecificEntityDto
            {
            }

            static PropertyNames()
            {
                ResultType = ExtractPropertyName<ListResult, ListResultType>(t => t.ResultType);
                RowCount = ExtractPropertyName<ListResult, int>(t => t.RowCount);
                MainAttribute = ExtractPropertyName<ListResult, string>(t => t.MainAttribute);
                EntityDtosData = ExtractPropertyName<EntityDtoListResult, ICollection>(t => t.Data);

                Data = EntityDtosData;
                //EntityDtosListDtoType = ExtractPropertyName<EntityDtoListResult<FakeEntity, ListFakeEntityDto>, Type>(t => t.DtoType);
            }

            public static string ResultType { get; private set; }
            public static string RowCount { get; private set; }
            public static string MainAttribute { get; private set; }
            private static string EntityDtosData { get; set; }
            public static string Data { get; private set; }
            public static string EntityDtosListDtoType { get; private set; }
        }

        private static string ExtractPropertyName<THostType, TProperty>(Expression<Func<THostType, TProperty>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
            }

            return memberExpression.Member.Name;
        }
    }
}