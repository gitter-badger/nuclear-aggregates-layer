using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class ClientGridDocMapper : IDocMapper<ClientGridDoc>
    {
        private readonly IEnumLocalizer _enumLocalizer;
        private readonly IRelationalDocsFinder _finder;

        public ClientGridDocMapper(IEnumLocalizer enumLocalizer, IRelationalDocsFinder finder)
        {
            if (enumLocalizer == null)
            {
                throw new ArgumentNullException("enumLocalizer");
            }

            if (finder == null)
            {
                throw new ArgumentNullException("finder");
            }

            _enumLocalizer = enumLocalizer;
            _finder = finder;
        }

        public void UpdateDocByEntity(IEnumerable<ClientGridDoc> docs, IEntityKey entity)
        {
            if (docs == null)
            {
                throw new ArgumentNullException("docs");
            }

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            foreach (var doc in docs)
            {
                bool updated = TryUpdate(doc, entity as Client)
                || TryUpdate(doc, entity as User)
                || TryUpdate(doc, entity as Territory);

                if (!updated)
                    throw new NotSupportedException(entity.GetType().FullName);
            }
        }

        private bool TryUpdate(ClientGridDoc d, Territory t)
        {
            if (t == null) return false;

            d.TerritoryName = t.Name;

            return true;
        }

        private bool TryUpdate(ClientGridDoc d, Client c)
        {
            if (c == null) return false;

            if (d.OwnerCode != c.OwnerCode)
                UpdateOwner(d, c);

            if (d.TerritoryId != c.TerritoryId)
                UpdateTerriroty(d, c);

            d.Id = c.Id;
            d.Name = c.Name;
            d.MainAddress = c.MainAddress;
            d.OwnerCode = c.OwnerCode;
            d.TerritoryId = c.TerritoryId;
            d.IsActive = c.IsActive;
            d.CreatedOn = c.CreatedOn;
            d.ReplicationCode = c.ReplicationCode.ToString();
            d.InformationSource = GetLocalizedInformationSource(c.InformationSource);

            return true;
        }

        private void UpdateOwner(ClientGridDoc clientGridDoc, Client client)
        {
            var userDocs = _finder.FindDocsByIndirectRelationPart<UserDoc>(client);

            var userDoc = new UserDoc();
            if (userDocs != null && userDocs.Any())
                userDoc = userDocs.Single();

            clientGridDoc.OwnerName = userDoc.Name;
        }

        private void UpdateTerriroty(ClientGridDoc clientGridDoc, Client client)
        {
            var territories = _finder.FindDocsByIndirectRelationPart<TerritoryDoc>(client);

            var territory = new TerritoryDoc();
            if (territories != null && territories.Any())
                territory = territories.Single();

            clientGridDoc.TerritoryName = territory.Name;
        }

        private bool TryUpdate(ClientGridDoc d, User u)
        {
            if (u == null) return false;

            d.OwnerName = u.DisplayName;

            return true;
        }

        private string GetLocalizedInformationSource(int informationSourceId)
        {
            return _enumLocalizer.GetLocalizedString((InformationSource)informationSourceId);
        }
    }
}