using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Russia;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Properties;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV
{
    public static class DynamicEntityMetadataRegistry
    {
        private static readonly Dictionary<Type, IEnumerable<IEntityPropertyIdentity>> DictionaryEntityPropertiesMapping =
            new Dictionary<Type, IEnumerable<IEntityPropertyIdentity>>
                {
                    {
                        typeof(Bank), new IEntityPropertyIdentity[]
                            {
                                NameIdentity.Instance
                            }
                    },
                    {
                        typeof(Commune), new IEntityPropertyIdentity[]
                            {
                                NameIdentity.Instance
                            }
                    },
                    {
                        typeof(AcceptanceReportsJournalRecord), new IEntityPropertyIdentity[]
                            {
                                OrganizationUnitIdIdentity.Instance,
                                EndDistributionDateIdentity.Instance,
                                DocumentsAmountIdentity.Instance,
                                AuthorIdIdentity.Instance
                            }
                    },
                };

        private static readonly Dictionary<Type, IEnumerable<IEntityPropertyIdentity>> BusinessEntityPropertiesMapping =
            new Dictionary<Type, IEnumerable<IEntityPropertyIdentity>>
                {
                    {
                        typeof(ChileLegalPersonPart), new IEntityPropertyIdentity[]
                            {
                                CommuneIdIdentity.Instance,
                                OperationsKindIndentity.Instance
                            }
                    },
                    {
                        typeof(UkraineLegalPersonPart), new IEntityPropertyIdentity[]
                            {
                                EgrpouIdentity.Instance,
                                TaxationTypeIdentity.Instance
                            }
                    },
                    {
                        typeof(ChileLegalPersonProfilePart), new IEntityPropertyIdentity[]
                            {
                                AccountTypeIdentity.Instance,
                                BankIdIdentity.Instance,
                                RepresentativeRutIdentity.Instance,
                                RepresentativeAuthorityDocumentIssuedOnIdentity.Instance,
                                RepresentativeAuthorityDocumentIssuedByIdentity.Instance
                            }
                    },
                    {
                        typeof(RussiaLegalPersonProfilePart), new IEntityPropertyIdentity[]
                            {
                                ChiefFullNameInNominativeIdentity.Instance,
                            }
                    },
                    {
                        typeof(UkraineLegalPersonProfilePart), new IEntityPropertyIdentity[]
                            {
                                MfoIdentity.Instance
                            }
                    },
                    {
                        typeof(KazakhstanLegalPersonPart), new IEntityPropertyIdentity[]
                            {
                                RnnIdentity.Instance,
                                IdentityCardNumberIdentity.Instance,
                                IdentityCardIssuedByIdentity.Instance,
                                IdentityCardIssuedOnIdentity.Instance
                            }
                    },
                    {
                        typeof(KazakhstanLegalPersonProfilePart), new IEntityPropertyIdentity[]
                            {
                                ActualAddressIdentity.Instance,
                                OtherAuthorityDocumentIdentity.Instance,
                                DecreeNumberIdentity.Instance,
                                DecreeDateIdentity.Instance
                            }
                    },
                    {
                        typeof(ChileBranchOfficeOrganizationUnitPart), new IEntityPropertyIdentity[]
                            {
                                RepresentativeRutIdentity.Instance
                            }
                    },
                    {
                        typeof(UkraineBranchOfficePart), new IEntityPropertyIdentity[]
                            {
                                IpnIdentity.Instance
                            }
                    },
                    {
                        typeof(EmiratesClientPart), new IEntityPropertyIdentity[]
                            {
                                PoBoxIdentity.Instance
                            }
                    },
                    {
                        typeof(EmiratesLegalPersonPart), new IEntityPropertyIdentity[]
                            {
                                CommercialLicenseBeginDateIdentity.Instance,
                                CommercialLicenseEndDateIdentity.Instance,
                            }
                    },
                    {
                        typeof(EmiratesLegalPersonProfilePart), new IEntityPropertyIdentity[]
                            {
                                PhoneIdentity.Instance,
                                FaxIdentity.Instance                                
                            }
                    },
                    {
                        typeof(EmiratesBranchOfficeOrganizationUnitPart), new IEntityPropertyIdentity[]
                            {
                                FaxIdentity.Instance                                
                            }
                    },
                    {
                        typeof(EmiratesFirmAddressPart), new IEntityPropertyIdentity[]
                            {
                                PoBoxIdentity.Instance
                            }
                    },
                };


        public static IEnumerable<IEntityPropertyIdentity> GetPropertyIdentities<T>() where T : IEntity
        {
            return GetPropertyIdentities<T>(DictionaryEntityPropertiesMapping) ??
                   GetPropertyIdentities<T>(BusinessEntityPropertiesMapping);
        }

        private static IEnumerable<IEntityPropertyIdentity> GetPropertyIdentities<T>(IReadOnlyDictionary<Type, IEnumerable<IEntityPropertyIdentity>> mapping)
        {
            IEnumerable<IEntityPropertyIdentity> identities;
            return mapping.TryGetValue(typeof(T), out identities) ? identities : null;
        }
    }
}
