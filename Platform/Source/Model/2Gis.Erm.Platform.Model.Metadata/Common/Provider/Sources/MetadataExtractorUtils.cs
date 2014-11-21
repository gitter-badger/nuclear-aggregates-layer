using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources
{
    public static class MetadataSourceUtils
    {
        public delegate void MetadataElementProcessor<TMetadataSettings>(TMetadataSettings metadataElement);

        public static IEnumerable<TMetadataSettings> Extract<TMetadataSettings>(
            this Type metadataHostType, 
            MetadataElementProcessor<TMetadataSettings> processor, 
            params Expression<Func<TMetadataSettings>>[] excludedMembers)
        {
            var settingsList = new List<TMetadataSettings>();

            var excludedMembersNames = new List<string>();
            if (excludedMembers != null && excludedMembers.Length > 0)
            {
                foreach (var excludedMember in excludedMembers)
                {
                    if (excludedMember == null)
                    {
                        continue;
                    }

                    string excludedMemberName = StaticReflection.GetMemberName(excludedMember);
                    Type excludedMemberHostType = StaticReflection.GetMemberDeclaringType(excludedMember);
                    if (excludedMemberHostType != metadataHostType)
                    {
                        throw new InvalidOperationException("Invalid excluded member specified. " + excludedMember);
                    }

                    excludedMembersNames.Add(excludedMemberName);
                }
            }

            foreach (var metadataSettings in metadataHostType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (metadataSettings.FieldType != typeof(TMetadataSettings))
                {
                    continue;
                }

                if (excludedMembersNames.Contains(metadataSettings.Name))
                {
                    continue;
                }

                var settings = (TMetadataSettings)metadataSettings.GetValue(null);
                settingsList.Add(settings);
                if (processor != null)
                {
                    processor(settings);
                }
            }

            return settingsList;
        }

        public static IEnumerable<TMetadataSettings> Extract<TMetadataHost, TMetadataSettings>(
            MetadataElementProcessor<TMetadataSettings> processor, 
            params Expression<Func<TMetadataSettings>>[] excludedMembers)
        {
            return typeof(TMetadataHost).Extract(processor, excludedMembers);
        }
    }
}
