using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public class DefaultEntityToDocumentRelation<TEntity, TDocument> : IEntityToDocumentRelation<TEntity, TDocument>
        where TEntity : class, IEntity, IEntityKey
    {
        private readonly IFinder _finder;
        private readonly ILocalizationSettings _localizationSettings;

        public DefaultEntityToDocumentRelation(IFinder finder, ILocalizationSettings localizationSettings)
        {
            _finder = finder;
            _localizationSettings = localizationSettings;
        }

        public Func<IQueryable<TEntity>, CultureInfo, IEnumerable<IDocumentWrapper<TDocument>>> SelectDocumentsFunc { private get; set; }

        public IEnumerable<IDocumentWrapper> SelectAllDocuments()
        {
            return SelectDocuments(q => q);
        }

        public IEnumerable<IDocumentWrapper> SelectDocuments(IReadOnlyCollection<long> ids)
        {
            return SelectDocuments(q => q.Where(x => ids.Contains(x.Id)));
        }

        private IEnumerable<IDocumentWrapper<TDocument>> SelectDocuments(Func<IQueryable<TEntity>, IQueryable<TEntity>> querySelector)
        {
            var query = querySelector(_finder.FindAll<TEntity>());
            var documentWrappers = SelectDocumentsFunc(query, _localizationSettings.ApplicationCulture);
            return documentWrappers;
        }
    }
}