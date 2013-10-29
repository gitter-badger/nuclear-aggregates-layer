using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties.Configuration
{
    public static class LookupAttributeProvider
    {
        private const string DefaultKeyAttribute = "Id";
        private const string DefaultValueAttribute = "Name";

        private static readonly Dictionary<EntityName, string> KeyAttributes = new Dictionary<EntityName, string>();

        private static readonly Dictionary<EntityName, string> ValueAttributes = new Dictionary<EntityName, string>()
            {
                { EntityName.Order, StaticReflection.GetMemberName((OrderDomainEntityDto x) => x.Number) },
                { EntityName.LegalPerson, StaticReflection.GetMemberName((LegalPersonDomainEntityDto x) => x.LegalName) },
                { EntityName.Bargain, StaticReflection.GetMemberName((BargainDomainEntityDto x) => x.Number) },
                { EntityName.BranchOfficeOrganizationUnit, "ShortLegalName" }
            };

        public static string GetDefaultKeyAttribute(EntityName entityName)
        {
            string key;
            KeyAttributes.TryGetValue(entityName, out key);
            return key ?? DefaultKeyAttribute;
        }

        public static string GetDefaultValueAttribute(EntityName entityName)
        {
            string value;
            ValueAttributes.TryGetValue(entityName, out value);
            return value ?? DefaultValueAttribute;
        }
    }
}