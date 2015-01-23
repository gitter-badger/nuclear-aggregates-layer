using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Replication.Metadata;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public class EntityToDocumentProjectionMetadataSource : MetadataSourceBase<EntityToDocumentProjectionsIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public EntityToDocumentProjectionMetadataSource(ILocalizationSettings localizationSettings)
        {
            _metadata = InitializeMetadataContainer(localizationSettings.ApplicationCulture);
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }

        private static IReadOnlyDictionary<Uri, IMetadataElement> InitializeMetadataContainer(CultureInfo cultureInfo)
        {
            IReadOnlyCollection<EntityToDocumentProjectionMetadata> metadataContainer =
                new EntityToDocumentProjectionMetadata[]
                    {
                        EntityToDocumentProjectionMetadata.Config
                                                          .For<ClientGridDoc>()
                                                          .Use<ClientGridDoc, Client>(ProjectionSpecs.Clients.Select(), ProjectionSpecs.Clients.Project()),

                        EntityToDocumentProjectionMetadata.Config
                                                          .For<CurrencyGridDoc>()
                                                          .Use<CurrencyGridDoc, Currency>(ProjectionSpecs.Currencies.Select(), ProjectionSpecs.Currencies.Project()),

                        EntityToDocumentProjectionMetadata.Config
                                                          .For<CountryGridDoc>()
                                                          .Use<CountryGridDoc, Country>(ProjectionSpecs.Countries.Select(), ProjectionSpecs.Countries.Project()),

                        EntityToDocumentProjectionMetadata.Config
                                                          .For<OrgUnitGridDoc>()
                                                          .Use<OrgUnitGridDoc, OrganizationUnit>(ProjectionSpecs.OrganizationUnits.Select(), ProjectionSpecs.OrganizationUnits.Project()),

                        EntityToDocumentProjectionMetadata.Config
                                                          .For<DepartmentGridDoc>()
                                                          .Use<DepartmentGridDoc, Department>(ProjectionSpecs.Departments.Select(), ProjectionSpecs.Departments.Project()),

                        EntityToDocumentProjectionMetadata.Config
                                                          .For<UserGridDoc>()
                                                          .Use<UserGridDoc, User>(ProjectionSpecs.Users.Select(), ProjectionSpecs.Users.Project()),

                        EntityToDocumentProjectionMetadata.Config
                                                          .For<FirmGridDoc>()
                                                          .Use<FirmGridDoc, Firm>(ProjectionSpecs.Firms.Select(), ProjectionSpecs.Firms.Project()),

                        EntityToDocumentProjectionMetadata.Config
                                                          .For<TerritoryGridDoc>()
                                                          .Use<TerritoryGridDoc, Territory>(ProjectionSpecs.Territories.Select(), ProjectionSpecs.Territories.Project()),

                        EntityToDocumentProjectionMetadata.Config
                                                          .For<LegalPersonGridDoc>()
                                                          .Use<LegalPersonGridDoc, LegalPerson>(ProjectionSpecs.LegalPersons.Select(), ProjectionSpecs.LegalPersons.Project()),

                        EntityToDocumentProjectionMetadata.Config
                                                          .For<BargainGridDoc>()
                                                          .Use<BargainGridDoc, Bargain>(ProjectionSpecs.Bargains.Select(), ProjectionSpecs.Bargains.Project(cultureInfo)),

                        EntityToDocumentProjectionMetadata.Config
                                                          .For<OrderGridDoc>()
                                                          .Use<OrderGridDoc, Order>(ProjectionSpecs.Orders.Select(), ProjectionSpecs.Orders.Project(cultureInfo))
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}