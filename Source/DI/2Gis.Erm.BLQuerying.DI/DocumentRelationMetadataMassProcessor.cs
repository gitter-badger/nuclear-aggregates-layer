using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Operations.Indexing;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.DI
{
    public sealed class DocumentRelationMetadataMassProcessor
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IDocumentRelationMetadataContainer _metadataContainer;

        public DocumentRelationMetadataMassProcessor(IUnityContainer unityContainer, IDocumentRelationMetadataContainer metadataContainer)
        {
            _unityContainer = unityContainer;
            _metadataContainer = metadataContainer;
        }

        public void MassProcess()
        {
            RegisterMetadata<CountryGridDoc, CurrencyGridDoc>(x => x.CurrencyId, (doc, part) => doc.CurrencyName = part.Name);

            RegisterMetadata<OrgUnitGridDoc, CountryGridDoc>(x => x.CountryId, (doc, part) => doc.CountryName = part.Name);

            RegisterMetadata<DepartmentGridDoc, DepartmentGridDoc>(x => x.ParentId, (doc, part) => doc.ParentName = part.Name);

            RegisterMetadata<UserGridDoc, UserGridDoc>(x => x.ParentId, (doc, part) => doc.ParentName = part.DisplayName);
            RegisterMetadata<UserGridDoc, DepartmentGridDoc>(x => x.DepartmentId, (doc, part) => doc.DepartmentName = part.Name);

            RegisterMetadata<FirmGridDoc, UserGridDoc>(x => x.OwnerCode, (doc, part) => doc.OwnerName = part.DisplayName);
            RegisterMetadata<FirmGridDoc, OrgUnitGridDoc>(x => x.OrganizationUnitId, (doc, part) => doc.OrganizationUnitName = part.Name);

            RegisterMetadata<LegalPersonGridDoc, UserGridDoc>(x => x.OwnerCode, (doc, part) => doc.OwnerName = part.DisplayName);

            RegisterMetadata<BargainGridDoc, UserGridDoc>(x => x.OwnerCode, (doc, part) => doc.OwnerName = part.DisplayName);
            RegisterMetadata<BargainGridDoc, LegalPersonGridDoc>(x => x.LegalPersonId, (doc, part) => { doc.LegalPersonLegalName = part.LegalName;  doc.LegalPersonLegalAddress = part.LegalAddress; });

            RegisterMetadata<OrderGridDoc, FirmGridDoc>(x => x.FirmId, (doc, part) => doc.FirmName = part.Name);
            RegisterMetadata<OrderGridDoc, UserGridDoc>(x => x.OwnerCode, (doc, part) => doc.OwnerName = part.DisplayName);
            RegisterMetadata<OrderGridDoc, OrgUnitGridDoc>(x => x.SourceOrganizationUnitId, (doc, part) => doc.SourceOrganizationUnitName = part.Name);
            RegisterMetadata<OrderGridDoc, OrgUnitGridDoc>(x => x.DestOrganizationUnitId, (doc, part) => doc.DestOrganizationUnitName = part.Name);
            RegisterMetadata<OrderGridDoc, LegalPersonGridDoc>(x => x.LegalPersonId, (doc, part) => doc.LegalPersonName = part.LegalName);
            RegisterMetadata<OrderGridDoc, BargainGridDoc>(x => x.BargainId, (doc, part) => doc.BargainNumber = part.Number);
        }

        private void RegisterMetadata<TDocument, TDocumentPart>(Expression<Func<TDocument, string>> documentPartIdExpression, Action<TDocument, TDocumentPart> insertDocumentPartFunc)
            where TDocument : class, new()
            where TDocumentPart : class
        {
            _unityContainer.RegisterType<IDocumentRelation<TDocument, TDocumentPart>, DocumentRelation<TDocument, TDocumentPart>>(Lifetime.Singleton);

            var metadata = new DefaultDocumentRelationMetadata<TDocument, TDocumentPart>();
            metadata.SetDocumentPartId(documentPartIdExpression);
            metadata.InsertDocumentPartFunc = insertDocumentPartFunc;

            _metadataContainer.RegisterMetadata<TDocument, TDocumentPart>(metadata);
        }
    }
}