using System;
using System.Collections.Generic;

namespace Nuclear.Settings.API
{
    public static class SettingsContainerUtils
    {
        public static bool TryGetSettings<TSettings>(this ISettingsContainer settingsContainer, out TSettings settings)
            where TSettings : class, ISettings
        {
            settings = settingsContainer as TSettings;
            if (settings != null)
            {
                return true;
            }

            foreach (var aspect in settingsContainer.SettingsAspects)
            {
                settings = aspect as TSettings;
                if (settings != null)
                {
                    return true;
                }

                var childContainer = aspect as ISettingsContainer;
                if (childContainer == null)
                {
                    continue;
                }

                if (childContainer.TryGetSettings(out settings))
                {
                    return true;
                }
            }

            return false;
        }

        public static TSettings AsSettings<TSettings>(this ISettingsContainer settingsContainer)
            where TSettings : class, ISettings
        {
            TSettings settings;
            if (!settingsContainer.TryGetSettings(out settings))
            {
                throw new InvalidOperationException(string.Format("Can't find settings part of type {0} in settings container {1}", typeof(TSettings), settingsContainer.GetType()));
            }

            return settings;
        }

        public static ICollection<ISettingsAspect> Use<TSettingsAspect>(this ICollection<ISettingsAspect> aspects)
            where TSettingsAspect : class, ISettingsAspect, new()
        {
            aspects.Add(new TSettingsAspect());
            return aspects;
        }

        public static ICollection<ISettingsAspect> Use(this ICollection<ISettingsAspect> aspects, IEnumerable<ISettingsAspect> attachedAspects)
        {
            foreach (var aspect in attachedAspects)
            {
                aspects.Add(aspect);
            }

            return aspects;
        }

        public static ICollection<ISettingsAspect> Use(this ICollection<ISettingsAspect> aspects, params ISettingsAspect[] attachedAspects)
        {
            foreach (var aspect in attachedAspects)
            {
                aspects.Add(aspect);
            }

            return aspects;
        }
    }
}