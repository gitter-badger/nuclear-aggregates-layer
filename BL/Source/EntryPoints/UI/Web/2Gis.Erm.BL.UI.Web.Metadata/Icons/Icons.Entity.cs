using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Icons
{
    public static partial class Icons
    {
        public static class Entity
        {
            public const string Default = "en_ico_lrg_Default.gif";
            public const string DefaultSmall = "en_ico_16_Default.gif";

            private static readonly IEnumerable<IEntityType> SupportedSmallIcons
                = new IEntityType[]
                      {
                          EntityType.Instance.Account(),
                          EntityType.Instance.Activity(),
                          EntityType.Instance.Advertisement(),
                          EntityType.Instance.AdvertisementElement(),
                          EntityType.Instance.AdvertisementElementTemplate(),
                          EntityType.Instance.AdvertisementTemplate(),
                          EntityType.Instance.Appointment(),
                          EntityType.Instance.Bargain(),
                          EntityType.Instance.BargainType(),
                          EntityType.Instance.BranchOffice(),
                          EntityType.Instance.BranchOfficeOrganizationUnit(),
                          EntityType.Instance.Category(),
                          EntityType.Instance.Client(),
                          EntityType.Instance.ClientLink(),
                          EntityType.Instance.Contact(),
                          EntityType.Instance.ContributionType(),
                          EntityType.Instance.Country(),
                          EntityType.Instance.Currency(),
                          EntityType.Instance.Deal(),
                          EntityType.Instance.Firm(),
                          EntityType.Instance.LegalPerson(),
                          EntityType.Instance.Letter(),
                          EntityType.Instance.LocalMessage(),
                          EntityType.Instance.Note(),
                          EntityType.Instance.Order(),
                          EntityType.Instance.OrganizationUnit(),
                          EntityType.Instance.Phonecall(),
                          EntityType.Instance.Platform(),
                          EntityType.Instance.Position(),
                          EntityType.Instance.PositionCategory(),
                          EntityType.Instance.Price(),
                          EntityType.Instance.PricePosition(),
                          EntityType.Instance.Role(),
                          EntityType.Instance.Task(),
                          EntityType.Instance.Territory(),
                          EntityType.Instance.User()
                      };

            private static readonly IEnumerable<IEntityType> SupportedLargeIcons
                = new IEntityType[]
                      {
                          EntityType.Instance.Appointment(),
                          EntityType.Instance.Bargain(),
                          EntityType.Instance.BargainType(),
                          EntityType.Instance.BranchOffice(),
                          EntityType.Instance.Category(),
                          EntityType.Instance.Contact(),
                          EntityType.Instance.ContributionType(),
                          EntityType.Instance.Country(),
                          EntityType.Instance.Currency(),
                          EntityType.Instance.Deal(),
                          EntityType.Instance.Department(),
                          EntityType.Instance.LegalPerson(),
                          EntityType.Instance.Letter(),
                          EntityType.Instance.LocalMessage(),
                          EntityType.Instance.Note(),
                          EntityType.Instance.OrganizationUnit(),
                          EntityType.Instance.Phonecall(),
                          EntityType.Instance.Platform(),
                          EntityType.Instance.Position(),
                          EntityType.Instance.PositionCategory(),
                          EntityType.Instance.Price(),
                          EntityType.Instance.PricePosition(),
                          EntityType.Instance.Role(),
                          EntityType.Instance.RolePrivilege(),
                          EntityType.Instance.Task(),
                          EntityType.Instance.Territory(),
                          EntityType.Instance.User()
                      };

            public static string Large(IEntityType entity)
            {
                if (!SupportedLargeIcons.Contains(entity))
                {
                    throw new NotSupportedException(string.Format("Large icon for entity {0} is not supported. Consider using default icon instead", entity));
                }

                return string.Format("en_ico_lrg_{0}.gif", entity);
            }

            public static string Small(IEntityType entity)
            {
                if (!SupportedSmallIcons.Contains(entity))
                {
                    throw new NotSupportedException(string.Format("Small icon for entity {0} is not supported. Consider using default icon instead", entity));
                }

                return string.Format("en_ico_16_{0}.gif", entity);
            }
        }
    }
}