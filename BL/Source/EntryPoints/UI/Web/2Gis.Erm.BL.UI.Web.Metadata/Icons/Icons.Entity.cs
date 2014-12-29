using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Icons
{
    public static partial class Icons
    {
        public static class Entity
        {
            public const string Default = "en_ico_lrg_Default.gif";
            public const string DefaultSmall = "en_ico_16_Default.gif";

            private static readonly IEnumerable<EntityName> SupportedSmallIcons
                = new[]
                      {
                          EntityName.Account,
                          EntityName.Activity,
                          EntityName.Advertisement,
                          EntityName.AdvertisementElement,
                          EntityName.AdvertisementElementTemplate,
                          EntityName.AdvertisementTemplate,
                          EntityName.Appointment,
                          EntityName.Bargain,
                          EntityName.BargainType,
                          EntityName.BranchOffice,
                          EntityName.BranchOfficeOrganizationUnit,
                          EntityName.Category,
                          EntityName.Client,
                          EntityName.ClientLink,
                          EntityName.Contact,
                          EntityName.ContributionType,
                          EntityName.Country,
                          EntityName.Currency,
                          EntityName.Deal,
                          EntityName.Firm,
                          EntityName.LegalPerson,
                          EntityName.Letter,
                          EntityName.LocalMessage,
                          EntityName.Note,
                          EntityName.Order,
                          EntityName.OrganizationUnit,
                          EntityName.Phonecall,
                          EntityName.Platform,
                          EntityName.Position,
                          EntityName.PositionCategory,
                          EntityName.Price,
                          EntityName.PricePosition,
                          EntityName.Role,
                          EntityName.Task,
                          EntityName.Territory,
                          EntityName.User
                      };

            private static readonly IEnumerable<EntityName> SupportedLargeIcons
                = new[]
                      {
                          EntityName.Appointment,
                          EntityName.Bargain,
                          EntityName.BargainType,
                          EntityName.BranchOffice,
                          EntityName.Category,
                          EntityName.Contact,
                          EntityName.ContributionType,
                          EntityName.Country,
                          EntityName.Currency,
                          EntityName.Deal,
                          EntityName.Department,
                          EntityName.LegalPerson,
                          EntityName.Letter,
                          EntityName.LocalMessage,
                          EntityName.Note,
                          EntityName.OrganizationUnit,
                          EntityName.Phonecall,
                          EntityName.Platform,
                          EntityName.Position,
                          EntityName.PositionCategory,
                          EntityName.Price,
                          EntityName.PricePosition,
                          EntityName.Role,
                          EntityName.RolePrivilege,
                          EntityName.Task,
                          EntityName.Territory,
                          EntityName.User
                      };

            public static string Large(EntityName entity)
            {
                if (!SupportedLargeIcons.Contains(entity))
                {
                    throw new NotSupportedException(string.Format("Large icon for entity {0} is not supported. Consider using default icon instead", entity));
                }

                return string.Format("en_ico_lrg_{0}.gif", entity);
            }

            public static string Small(EntityName entity)
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