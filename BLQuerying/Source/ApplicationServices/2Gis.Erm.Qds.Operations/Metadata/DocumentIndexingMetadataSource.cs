using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public sealed class DocumentIndexingMetadataSource : MetadataSourceBase<DocumentIndexingIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public DocumentIndexingMetadataSource()
        {
            _metadata = InitializeMetadataContainer();
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }

        private static IReadOnlyDictionary<Uri, IMetadataElement> InitializeMetadataContainer()
        {
            IReadOnlyCollection<DocumentIndexingMetadata> metadataContainer =
                new DocumentIndexingMetadata[]
                    {
                        DocumentIndexingMetadata.Config
                                                .For<ClientGridDoc>()
                                                .Relation<ClientGridDoc, FirmGridDoc>(x => x.MainFirmId, (doc, part) => doc.MainFirmName = part.Name)
                                                .Relation<ClientGridDoc, UserGridDoc>(x => x.OwnerCode, (doc, part) => doc.OwnerName = part.DisplayName)
                                                .Relation<ClientGridDoc, TerritoryGridDoc>(x => x.TerritoryId, (doc, part) => doc.TerritoryName = part.Name),

                        DocumentIndexingMetadata.Config
                                                .For<CountryGridDoc>()
                                                .Relation<CountryGridDoc, CurrencyGridDoc>(x => x.CurrencyId, (doc, part) => doc.CurrencyName = part.Name),

                        DocumentIndexingMetadata.Config
                                                .For<OrgUnitGridDoc>()
                                                .Relation<OrgUnitGridDoc, CountryGridDoc>(x => x.CountryId, (doc, part) => doc.CountryName = part.Name),

                        DocumentIndexingMetadata.Config
                                                .For<DepartmentGridDoc>()
                                                .Relation<DepartmentGridDoc, DepartmentGridDoc>(x => x.ParentId, (doc, part) => doc.ParentName = part.Name),

                        DocumentIndexingMetadata.Config
                                                .For<UserGridDoc>()
                                                .Relation<UserGridDoc, UserGridDoc>(x => x.ParentId, (doc, part) => doc.ParentName = part.DisplayName)
                                                .Relation<UserGridDoc, DepartmentGridDoc>(x => x.DepartmentId, (doc, part) => doc.DepartmentName = part.Name),

                        DocumentIndexingMetadata.Config
                                                .For<FirmGridDoc>()
                                                .Relation<FirmGridDoc, UserGridDoc>(x => x.OwnerCode, (doc, part) => doc.OwnerName = part.DisplayName)
                                                .Relation<FirmGridDoc, OrgUnitGridDoc>(x => x.OrganizationUnitId, (doc, part) => doc.OrganizationUnitName = part.Name)
                                                .Relation<FirmGridDoc, ClientGridDoc>(x => x.ClientId, (doc, part) => doc.ClientName = part.Name)
                                                .Relation<FirmGridDoc, TerritoryGridDoc>(x => x.TerritoryId, (doc, part) => doc.TerritoryName = part.Name),

                        DocumentIndexingMetadata.Config
                                                .For<TerritoryGridDoc>()
                                                .Relation<TerritoryGridDoc, OrgUnitGridDoc>(x => x.OrganizationUnitId, (doc, part) => doc.OrganizationUnitName = part.Name),

                        DocumentIndexingMetadata.Config
                                                .For<LegalPersonGridDoc>()
                                                .Relation<LegalPersonGridDoc, UserGridDoc>(x => x.OwnerCode, (doc, part) => doc.OwnerName = part.DisplayName)
                                                .Relation<LegalPersonGridDoc, ClientGridDoc>(x => x.ClientId, (doc, part) => doc.ClientName = part.Name),

                        DocumentIndexingMetadata.Config
                                                .For<BargainGridDoc>()
                                                .Relation<BargainGridDoc, UserGridDoc>(x => x.OwnerCode, (doc, part) => doc.OwnerName = part.DisplayName)
                                                .Relation<BargainGridDoc, LegalPersonGridDoc>(x => x.LegalPersonId,
                                                                                                  (doc, part) =>
                                                                                                      {
                                                                                                          doc.LegalPersonLegalName = part.LegalName;
                                                                                                          doc.LegalPersonLegalAddress = part.LegalAddress;
                                                                                                      })
                                                .Relation<BargainGridDoc, ClientGridDoc>(x => x.ClientId, (doc, part) => doc.ClientName = part.Name),

                        DocumentIndexingMetadata.Config
                                                .For<OrderGridDoc>()
                                                .Relation<OrderGridDoc, FirmGridDoc>(x => x.FirmId, (doc, part) => doc.FirmName = part.Name)
                                                .Relation<OrderGridDoc, UserGridDoc>(x => x.OwnerCode, (doc, part) => doc.OwnerName = part.DisplayName)
                                                .Relation<OrderGridDoc, OrgUnitGridDoc>(x => x.SourceOrganizationUnitId,
                                                                                            (doc, part) => doc.SourceOrganizationUnitName = part.Name)
                                                .Relation<OrderGridDoc, OrgUnitGridDoc>(x => x.DestOrganizationUnitId, (doc, part) => doc.DestOrganizationUnitName = part.Name)
                                                .Relation<OrderGridDoc, LegalPersonGridDoc>(x => x.LegalPersonId, (doc, part) => doc.LegalPersonName = part.LegalName)
                                                .Relation<OrderGridDoc, BargainGridDoc>(x => x.BargainId, (doc, part) => doc.BargainNumber = part.Number)
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}