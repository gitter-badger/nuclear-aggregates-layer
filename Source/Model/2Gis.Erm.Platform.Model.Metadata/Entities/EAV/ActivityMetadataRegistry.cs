using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Properties;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV
{
    /// <summary>
    /// Когда наступит светлое будущее с динамически настраиваемыми пропертями у действий, содержимое 
    /// этого файла переедет в xml файл, а то и в базу.
    /// </summary>
    public static class ActivityMetadataRegistry
    {
        private static readonly Dictionary<Type, IEnumerable<IEntityPropertyIdentity>> EntityPropertiesMapping = new Dictionary<Type, IEnumerable<IEntityPropertyIdentity>>();

        static ActivityMetadataRegistry()
        {
            EntityPropertiesMapping[typeof(Appointment)] = new IEntityPropertyIdentity[]
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
                };
            EntityPropertiesMapping[typeof(Phonecall)] = new IEntityPropertyIdentity[]
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
                };
            EntityPropertiesMapping[typeof(Task)] = new IEntityPropertyIdentity[]
                {
                    HeaderIdentity.Instance,
                    ScheduledStartIdentity.Instance,
                    ScheduledEndIdentity.Instance,
                    PriorityIdentity.Instance,
                    StatusIdentity.Instance,
                    DescriptionIdentity.Instance,
                    TaskTypeIdentity.Instance,
                    ActualEndIdentity.Instance
                };
        }

        public static IEnumerable<IEntityPropertyIdentity> GetPropertyIdentities<T>() where T : ActivityBase
        {
            return EntityPropertiesMapping[typeof(T)];
        }
    }
}
