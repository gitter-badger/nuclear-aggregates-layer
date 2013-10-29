using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common
{
    public static class SettingsExtractorUtils
    {
        public delegate void SettingsElementProcessor<TSettings>(TSettings settingsElement);

        public static IEnumerable<TSettings> Extract<TSettings>(
            this Type settingsHostType, 
            SettingsElementProcessor<TSettings> processor, 
            params Expression<Func<TSettings>>[] excludedMembers)
        {
            var settingsList = new List<TSettings>();

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
                    if (excludedMemberHostType != settingsHostType)
                    {
                        throw new InvalidOperationException("Invalid excluded member specified. " + excludedMember);
                    }

                    excludedMembersNames.Add(excludedMemberName);
                }
            }

            foreach (var entitySettings in settingsHostType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (entitySettings.FieldType != typeof(TSettings))
                {
                    continue;
                }

                if (excludedMembersNames.Contains(entitySettings.Name))
                {
                    continue;
                }

                var settings = (TSettings)entitySettings.GetValue(null);
                settingsList.Add(settings);
                if (processor != null)
                {
                    processor(settings);
                }
            }

            return settingsList;
        }

        public static IEnumerable<TSettings> Extract<TSettingsHost, TSettings>(
            SettingsElementProcessor<TSettings> processor, 
            params Expression<Func<TSettings>>[] excludedMembers)
        {
            return typeof(TSettingsHost).Extract(processor, excludedMembers);
        }
    }
}
