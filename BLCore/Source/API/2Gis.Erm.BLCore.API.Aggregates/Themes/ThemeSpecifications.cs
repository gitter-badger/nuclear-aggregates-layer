using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Themes
{
    public class ThemeSpecifications
    {
        public static class Find
        {
            public static FindSpecification<Theme> ById(long id)
            {
                return new FindSpecification<Theme>(x => x.Id == id);
            }

            public static FindSpecification<ThemeTemplate> ThemeTemplateById(long id)
            {
                return new FindSpecification<ThemeTemplate>(x => x.Id == id);
            }

            public static FindSpecification<Theme> Default()
            {
                return new FindSpecification<Theme>(x => x.IsDefault);
            }

            public static FindSpecification<Theme> SkyScrapper()
            {
                return new FindSpecification<Theme>(x => x.ThemeTemplate.IsSkyScraper);
            }

            public static FindSpecification<Theme> NotSkyScrapperAndNotDefault()
            {
                return new FindSpecification<Theme>(x => !x.ThemeTemplate.IsSkyScraper && !x.IsDefault);
            }

            public static FindSpecification<Theme> InPeriod(DateTime start, DateTime end)
            {
                return new FindSpecification<Theme>(x => x.EndDistribution > start && x.BeginDistribution < end);
            }
        }
    }
}