using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    // FIXME {f.zaharov, 04.06.2014}: copypaste почистить
    public class ClientGridDocMapper : IDocMapper<ClientGridDoc>
    {
        private readonly IDocumentRelationsRegistry _documentRelationsRegistry;

        public ClientGridDocMapper(IDocumentRelationsRegistry documentRelationsRegistry)
        {
            _documentRelationsRegistry = documentRelationsRegistry;
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
                            || TryUpdate(doc, entity as Territory)
                            || TryUpdate(doc, entity as Account)
                            || TryUpdate(doc, entity as LegalPerson);

                if (!updated)
                    throw new NotSupportedException(entity.GetType().FullName);
            }
        }

        bool TryUpdate(ClientGridDoc clientDoc, LegalPerson legalPerson)
        {
            if (legalPerson == null)
            {
                return false;
            }

            var updatedOrRemoved = false;
            var added = false;

            updatedOrRemoved = TryUpdateOrRemoveLegalPersonDoc(clientDoc, legalPerson);
            added = TryAddLegalPersonDoc(clientDoc, legalPerson);

            return updatedOrRemoved || added;
        }

        bool TryAddLegalPersonDoc(ClientGridDoc clientDoc, LegalPerson legalPerson)
        {
            if (clientDoc.Id != legalPerson.ClientId.ToString())
            {
                return false;
            }

            var list = new List<LegalPersonDoc>(clientDoc.LegalPersons ?? new LegalPersonDoc[0]);
            var legalPersonDoc = new LegalPersonDoc();
            list.Add(legalPersonDoc);

            UpdateLegalPersonDoc(legalPerson, legalPersonDoc);

            clientDoc.LegalPersons = list.ToArray();

            return true;
        }

        void UpdateLegalPersonDoc(LegalPerson legalPerson, LegalPersonDoc legalPersonDoc)
        {
            legalPersonDoc.Id = legalPerson.Id.ToString();
            legalPersonDoc.IsActive = legalPerson.IsActive;
            legalPersonDoc.IsDeleted = legalPerson.IsDeleted;

            if (legalPerson.Accounts != null)
                legalPersonDoc.Accounts = legalPerson.Accounts.Select(a =>
                    {
                        var doc = new AccountDoc();
                        UpdateAccountDoc(a,doc);
                        return doc;
                    }).ToArray();
        }

        bool TryUpdateOrRemoveLegalPersonDoc(ClientGridDoc clientDoc, LegalPerson legalPerson)
        {
            if (clientDoc.LegalPersons == null)
            {
                return false;
            }

            var legalPersonDoc = clientDoc.LegalPersons.FirstOrDefault(doc => doc.Id == legalPerson.Id.ToString());
            if (legalPersonDoc == null)
            {
                return false;
            }

            if (clientDoc.Id != legalPerson.ClientId.ToString())
            {
                clientDoc.LegalPersons = clientDoc.LegalPersons.Where(a => a != legalPersonDoc).ToArray();
                return true;
            }

            UpdateLegalPersonDoc(legalPerson, legalPersonDoc);

            return true;
        }

        private bool TryUpdate(ClientGridDoc clientDoc, Account account)
        {
            if (account == null)
            {
                return false;
            }

            var updatedOrRemoved = false;
            var added = false;

            foreach (var legalPersonDoc in clientDoc.LegalPersons)
            {
                updatedOrRemoved = TryUpdateOrRemoveAccountDoc(legalPersonDoc, account);
                added = TryAddAccountDoc(legalPersonDoc, account);
            }

            return updatedOrRemoved || added;
        }

        private bool TryUpdate(ClientGridDoc clientDoc, Territory t)
        {
            if (t == null) return false;

            clientDoc.TerritoryName = t.Name;

            return true;
        }

        private bool TryUpdate(ClientGridDoc clientDoc, Client c)
        {
            if (c == null) return false;

            clientDoc.Id = c.Id.ToString();
            clientDoc.Name = c.Name;
            clientDoc.MainAddress = c.MainAddress;
            clientDoc.OwnerCode = c.OwnerCode.ToString();
            clientDoc.TerritoryId = c.TerritoryId.ToString();
            clientDoc.IsActive = c.IsActive;
            clientDoc.CreatedOn = c.CreatedOn;
            clientDoc.ReplicationCode = c.ReplicationCode.ToString();
            clientDoc.LastQualifyTime = c.LastQualifyTime;
            clientDoc.LastDisqualifyTime = c.LastDisqualifyTime;
            clientDoc.MainFirmId = c.MainFirmId.ToString();
            // MainFirmName = Client.Firm.Name, но фирмы нету, нужно пробросиьт ещё и фирму
            clientDoc.MainFirmName = null;
            clientDoc.MainPhoneNumber = c.MainPhoneNumber;
            clientDoc.IsAdvertisingAgency = c.IsAdvertisingAgency;
            clientDoc.InformationSourceEnum = (InformationSource)c.InformationSource;

            UpdateByRelations(clientDoc);

            return true;
        }

        bool TryAddAccountDoc(LegalPersonDoc legalPersonDoc, Account account)
        {
            if (legalPersonDoc.Id != account.LegalPersonId.ToString())
            {
                return false;
            }

            var list = new List<AccountDoc>(legalPersonDoc.Accounts ?? new AccountDoc[0]);
            var ad = new AccountDoc();
            list.Add(ad);

            UpdateAccountDoc(account, ad);

            legalPersonDoc.Accounts = list.ToArray();

            return true;
        }

        bool TryUpdateOrRemoveAccountDoc(LegalPersonDoc legalPersonDoc, Account account)
        {
            if (legalPersonDoc.Accounts == null)
            {
                return false;
            }

            var accountDoc = legalPersonDoc.Accounts.FirstOrDefault(doc => doc.Id == account.Id.ToString());
            if (accountDoc == null)
            {
                return false;
            }

            if (legalPersonDoc.Id != account.LegalPersonId.ToString())
            {
                legalPersonDoc.Accounts = legalPersonDoc.Accounts.Where(a => a != accountDoc).ToArray();
                return true;
            }

            UpdateAccountDoc(account, accountDoc);

            return true;
        }

        static void UpdateAccountDoc(Account account, AccountDoc accountDoc)
        {
            accountDoc.Id = account.Id.ToString();
            accountDoc.Balance = (double)account.Balance;
            accountDoc.IsActive = account.IsActive;
            accountDoc.IsDeleted = account.IsDeleted;
        }

        // FIXME {all, 29.04.2014}: Наврно я сейчас скапитаню, но это переиспользуемая в будущем логика.
        private void UpdateByRelations(ClientGridDoc clientGridDoc)
        {
            IEnumerable<IDocumentRelation<ClientGridDoc>> relations;
            if (!_documentRelationsRegistry.TryGetDocumentRelations(out relations))
            {
                return;
            }

            foreach (var relation in relations)
            {
                relation.OnDocumentUpdated(clientGridDoc);
            }
        }

        private bool TryUpdate(ClientGridDoc clientDoc, User u)
        {
            if (u == null) return false;

            clientDoc.OwnerName = u.DisplayName;

            return true;
        }
    }
}