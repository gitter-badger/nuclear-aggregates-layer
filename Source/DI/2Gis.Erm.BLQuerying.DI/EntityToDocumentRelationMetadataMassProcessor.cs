using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Operations.Indexing;
using DoubleGis.Erm.Qds.Operations.Metadata;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.DI
{
    public sealed class EntityToDocumentRelationMetadataMassProcessor
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IEntityToDocumentRelationMetadataContainer _metadataContainer;

        public EntityToDocumentRelationMetadataMassProcessor(IUnityContainer unityContainer, IEntityToDocumentRelationMetadataContainer metadataContainer)
        {
            _unityContainer = unityContainer;
            _metadataContainer = metadataContainer;
        }

        public void MassProcess(Func<LifetimeManager> lifetime)
        {
            //RegisterRelation<Client, ClientGridDoc>(EntityToDocumentRelationMappings.SelectClientGridDoc);
            RegisterRelation<Firm, FirmGridDoc>(EntityToDocumentRelationMappings.SelectFirmGridDoc, lifetime);
            RegisterRelation<User, UserGridDoc>(EntityToDocumentRelationMappings.SelectUserGridDoc, lifetime);
            RegisterRelation<Department, DepartmentGridDoc>(EntityToDocumentRelationMappings.SelectDepartmentGridDoc, lifetime);
            RegisterRelation<Currency, CurrencyGridDoc>(EntityToDocumentRelationMappings.SelectCurrencyGridDoc, lifetime);
            RegisterRelation<Country, CountryGridDoc>(EntityToDocumentRelationMappings.SelectCountryGridDoc, lifetime);
            RegisterRelation<OrganizationUnit, OrgUnitGridDoc>(EntityToDocumentRelationMappings.SelectOrgUnitGridDoc, lifetime);
            RegisterRelation<LegalPerson, LegalPersonGridDoc>(EntityToDocumentRelationMappings.SelectLegalPersonGridDoc, lifetime);
            RegisterRelation<Bargain, BargainGridDoc>(EntityToDocumentRelationMappings.SelectBargainGridDoc, lifetime);
            RegisterRelation<Order, OrderGridDoc>(EntityToDocumentRelationMappings.SelectOrderGridDoc, lifetime);
            
            // last one
            //RegisterRelation<User, UserPermissonsDoc>(() => _unityContainer.Resolve<UserToUserDocRelation>());
        }

        private void RegisterRelation<TEntity, TDocument>(Func<IQueryable<TEntity>, CultureInfo, IEnumerable<IDocumentWrapper<TDocument>>> selectDocuments, Func<LifetimeManager> lifetime)
            where TEntity : class, IEntity, IEntityKey
        {
            _unityContainer.RegisterType<DefaultEntityToDocumentRelation<TEntity, TDocument>>(lifetime());

            RegisterRelation(container =>
            {
                var relation = container.Resolve<DefaultEntityToDocumentRelation<TEntity, TDocument>>();
                relation.SelectDocumentsFunc = selectDocuments;
                return relation;
            }, lifetime);
        }

        private void RegisterRelation<TEntity, TDocument>(Func<IUnityContainer, IEntityToDocumentRelation<TEntity, TDocument>> func, Func<LifetimeManager> lifetime)
        {
            _unityContainer.RegisterType<IEntityToDocumentRelation<TEntity, TDocument>>(lifetime(), new InjectionFactory(func));

            var metadata = new EntityToDocumentRelationMetadata<TEntity, TDocument>();
            _metadataContainer.RegisterMetadata<TEntity, TDocument>(metadata);
        }
    }
}