using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Properties;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV
{
    public static class DynamicEntityMetadataRegistry
    {
        private static readonly Dictionary<Type, IEnumerable<IEntityPropertyIdentity>> ActivityPropertiesMapping =
            new Dictionary<Type, IEnumerable<IEntityPropertyIdentity>>
                {
                    {
                        typeof(Appointment), new IEntityPropertyIdentity[]
                            {
                                HeaderIdentity.Instance,
                                ScheduledStartIdentity.Instance,
                                ScheduledEndIdentity.Instance,
                                PriorityIdentity.Instance,
                                StatusIdentity.Instance,
                                PurposeIdentity.Instance,
                                AfterSaleServiceTypeIdentity.Instance,
                                DescriptionIdentity.Instance,
                                ActualEndIdentity.Instance
                            }
                    },
                    {
                        typeof(Phonecall), new IEntityPropertyIdentity[]
                            {
                                HeaderIdentity.Instance,
                                ScheduledStartIdentity.Instance,
                                ScheduledEndIdentity.Instance,
                                PriorityIdentity.Instance,
                                StatusIdentity.Instance,
                                PurposeIdentity.Instance,
                                AfterSaleServiceTypeIdentity.Instance,
                                DescriptionIdentity.Instance,
                                ActualEndIdentity.Instance
                            }
                    },
                    {
                        typeof(Task), new IEntityPropertyIdentity[]
                            {
                                HeaderIdentity.Instance,
                                ScheduledStartIdentity.Instance,
                                ScheduledEndIdentity.Instance,
                                PriorityIdentity.Instance,
                                StatusIdentity.Instance,
                                DescriptionIdentity.Instance,
                                TaskTypeIdentity.Instance,
                                ActualEndIdentity.Instance
                            }
                    }
                };

        private static readonly Dictionary<Type, IEnumerable<IEntityPropertyIdentity>> DictionaryEntityPropertiesMapping =
            new Dictionary<Type, IEnumerable<IEntityPropertyIdentity>>
                {
                    {
                        typeof(Bank), new IEntityPropertyIdentity[]
                            {
                                NameIdentity.Instance
                            }
                    }
                };

        private static readonly Dictionary<Type, IEnumerable<IEntityPropertyIdentity>> BusinessEntityPropertiesMapping =
            new Dictionary<Type, IEnumerable<IEntityPropertyIdentity>>
                {
                    {
                        typeof(LegalPersonProfilePart), new IEntityPropertyIdentity[]
                            {
                                AccountTypeIdentity.Instance,
                                BankIdIdentity.Instance,
                                RutIdentity.Instance,
                                IssuedOnIdentity.Instance,
                                IssuedByIdentity.Instance
                            }
                    }
                };


        public static IEnumerable<IEntityPropertyIdentity> GetPropertyIdentities<T>() where T : IEntity
        {
            return GetPropertyIdentities<T>(ActivityPropertiesMapping) ??
                   GetPropertyIdentities<T>(DictionaryEntityPropertiesMapping) ??
                   GetPropertyIdentities<T>(BusinessEntityPropertiesMapping);
        }

        private static IEnumerable<IEntityPropertyIdentity> GetPropertyIdentities<T>(IReadOnlyDictionary<Type, IEnumerable<IEntityPropertyIdentity>> mapping)
        {
            IEnumerable<IEntityPropertyIdentity> identities;
            return mapping.TryGetValue(typeof(T), out identities) ? identities : null;
        }
    }
}
