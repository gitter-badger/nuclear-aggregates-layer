using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.API.Operations.Replication.Metadata.Features;
using DoubleGis.Erm.Qds.Operations.Metadata;

using NuClear.Metamodeling.Provider;
using NuClear.Storage;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    // FIXME {m.pashuk, 15.09.2014}: Тут надо разобраться в нужных параметрах и свести все к описательной модели, см EntityToDocumentProjectionMetadataSource
    public sealed class UserToUserAuthorizationDocRelation : IEntityToDocumentRelation<User, UserAuthorizationDoc>
    {
        private readonly EntityToDocumentRelation<User, UserAuthorizationDoc> _relation;

        public UserToUserAuthorizationDocRelation(IFinder finder, IMetadataProvider metadataProvider)
        {
            var departments = finder.Find(Specs.Find.Active<Department>()).ToArray();
            var entityToDocumentProjectionMetadatas = metadataProvider.GetEntityToDocumentProjectionMetadatas();
            var relationFeatures = entityToDocumentProjectionMetadatas.SelectMany(x => x.Value).Distinct().ToArray();

            var feature = new EntityRelationFeature<UserAuthorizationDoc, User>(
                ProjectionSpecs.Users.SelectUserPrivilegesContainer(),
                ProjectionSpecs.Users.ProjectToUserAuthorizationDoc(departments, relationFeatures));

            _relation = new EntityToDocumentRelation<User, UserAuthorizationDoc>(finder, feature);
        }

        public IEnumerable<IIndexedDocumentWrapper> SelectAllDocuments(IProgress<long> progress = null)
        {
            return _relation.SelectAllDocuments();
        }

        public IEnumerable<IIndexedDocumentWrapper> SelectDocuments(IReadOnlyCollection<long> ids)
        {
            return _relation.SelectDocuments(ids);
        }
    }
}