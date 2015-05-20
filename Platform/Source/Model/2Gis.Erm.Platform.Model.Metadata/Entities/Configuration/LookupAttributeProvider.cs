using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties.Configuration
{
    public static class LookupAttributeProvider
    {
        private const string DefaultKeyAttribute = "Id";
        private const string DefaultValueAttribute = "Name";

        private static readonly Dictionary<IEntityType, string> KeyAttributes = new Dictionary<IEntityType, string>();

        private static readonly Dictionary<IEntityType, string> ValueAttributes = new Dictionary<IEntityType, string>()
            {
                { EntityType.Instance.Order(), StaticReflection.GetMemberName((OrderDomainEntityDto x) => x.Number) },
                { EntityType.Instance.LegalPerson(), StaticReflection.GetMemberName((LegalPersonDomainEntityDto x) => x.LegalName) },
                { EntityType.Instance.Bargain(), StaticReflection.GetMemberName((BargainDomainEntityDto x) => x.Number) },
                { EntityType.Instance.BranchOfficeOrganizationUnit(), "ShortLegalName" }
            };

        public static string GetDefaultKeyAttribute(IEntityType entityName)
        {
            string key;
            KeyAttributes.TryGetValue(entityName, out key);
            return key ?? DefaultKeyAttribute;
        }

        public static string GetDefaultValueAttribute(IEntityType entityName)
        {
            string value;
            ValueAttributes.TryGetValue(entityName, out value);
            return value ?? DefaultValueAttribute;
        }
    }
}