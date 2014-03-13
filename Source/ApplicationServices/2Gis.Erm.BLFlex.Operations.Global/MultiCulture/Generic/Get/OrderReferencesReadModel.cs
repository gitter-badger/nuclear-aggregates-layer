using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class OrderReferencesReadModel : IOrderReferencesReadModel
    {
        private readonly ISecureFinder _finder;

        public OrderReferencesReadModel(ISecureFinder finder)
        {
            _finder = finder;
        }

        public bool TryGetReferences(EntityName parentEntityName,
                              long parentEntityId,
                              out EntityReference clientRef,
                              out EntityReference firmRef,
                              out EntityReference legalPersonRef,
                              out EntityReference destOrganizationUnitRef)
        {
            switch (parentEntityName)
            {
                case EntityName.Client:
                    return GetReferenceByClient(parentEntityId, out clientRef, out firmRef, out legalPersonRef, out destOrganizationUnitRef);
                case EntityName.Firm:
                    return GetReferencesByFirm(parentEntityId, out clientRef, out firmRef, out legalPersonRef, out destOrganizationUnitRef);
                case EntityName.LegalPerson:
                    return GetReferencesByLegalPerson(parentEntityId, out clientRef, out firmRef, out legalPersonRef, out destOrganizationUnitRef);
                default:
                    return EmptyResult(out clientRef, out firmRef, out legalPersonRef, out destOrganizationUnitRef);
            }
        }

        private bool GetReferencesByLegalPerson(long legalPersonId,
                                                         out EntityReference clientRef,
                                                         out EntityReference firmRef,
                                                         out EntityReference legalPersonRef,
                                                         out EntityReference destOrganizationUnitRef)
        {
            var data = _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                              .Select(person => new
                                  {
                                      Client = new { person.Client.Id, person.Client.Name },
                                      Firms = person.Client.Firms.Select(firm => new
                                          {
                                              firm.Id,
                                              firm.Name,
                                              firm.OrganizationUnitId,
                                              OrganizationUnitName = firm.OrganizationUnit.Name
                                          }),
                                      LegalPerson = new { person.Id, person.LegalName },
                                  })
                              .SingleOrDefault();

            if (data == null)
            {
                return EmptyResult(out clientRef, out firmRef, out legalPersonRef, out destOrganizationUnitRef);
            }

            firmRef = data.Firms.Count() == 1 ? new EntityReference(data.Firms.Single().Id, data.Firms.Single().Name) : null;
            destOrganizationUnitRef = data.Firms.Count() == 1 ? new EntityReference(data.Firms.Single().OrganizationUnitId, data.Firms.Single().OrganizationUnitName) : null;
            clientRef = data.Client != null ? new EntityReference(data.Client.Id, data.Client.Name) : null;
            legalPersonRef = new EntityReference(data.LegalPerson.Id, data.LegalPerson.LegalName);
            return true;
        }

        private bool GetReferencesByFirm(long firmId,
                                                  out EntityReference clientRef,
                                                  out EntityReference firmRef,
                                                  out EntityReference legalPersonRef,
                                                  out EntityReference destOrganizationUnitId)
        {
            var data = _finder.Find(Specs.Find.ById<Firm>(firmId))
                              .Select(firm => new
                              {
                                  Firm = new { firm.Id, firm.Name, firm.OrganizationUnitId, OrganizationUnitName = firm.OrganizationUnit.Name },
                                  Client = new { firm.Client.Id, firm.Client.Name },
                                  LegalPersons = firm.Client.LegalPersons.Select(person => new { person.Id, person.LegalName })
                              }).SingleOrDefault();

            if (data == null)
            {
                return EmptyResult(out clientRef, out firmRef, out legalPersonRef, out destOrganizationUnitId);
            }

            clientRef = new EntityReference(data.Client.Id, data.Client.Name);
            firmRef = new EntityReference(data.Firm.Id, data.Firm.Name);
            legalPersonRef = data.LegalPersons.Count() == 1 ? new EntityReference(data.LegalPersons.Single().Id, data.LegalPersons.Single().LegalName) : null;
            destOrganizationUnitId = new EntityReference(data.Firm.OrganizationUnitId, data.Firm.OrganizationUnitName);
            return true;
        }

        private bool GetReferenceByClient(long clientId,
                                                    out EntityReference clientRef,
                                                    out EntityReference firmRef,
                                                    out EntityReference legalPersonRef,
                                                    out EntityReference destOrganizationUnitRef)
        {
            var data = _finder.Find(Specs.Find.ById<Client>(clientId))
                              .Select(client => new
                              {
                                  Client = new { client.Id, client.Name },
                                  Firms = client.Firms.Select(firm => new
                                  {
                                      firm.Id,
                                      firm.Name,
                                      firm.OrganizationUnitId,
                                      OrganizationUNitName = firm.OrganizationUnit.Name
                                  }),
                                  LegalPersons = client.LegalPersons.Select(person => new { person.Id, person.LegalName })
                              })
                              .SingleOrDefault();

            if (data == null)
            {
                return EmptyResult(out clientRef, out firmRef, out legalPersonRef, out destOrganizationUnitRef);
            }

            clientRef = new EntityReference(data.Client.Id, data.Client.Name);
            firmRef = data.Firms.Count() == 1 ? new EntityReference(data.Firms.Single().Id, data.Firms.Single().Name) : null;
            legalPersonRef = data.LegalPersons.Count() == 1 ? new EntityReference(data.LegalPersons.Single().Id, data.LegalPersons.Single().LegalName) : null;
            destOrganizationUnitRef = firmRef != null ? new EntityReference(data.Firms.Single().OrganizationUnitId, data.Firms.Single().OrganizationUNitName) : null;
            return true;
        }

        private bool EmptyResult(out EntityReference clientRef,
                                 out EntityReference firmRef,
                                 out EntityReference legalPersonRef,
                                 out EntityReference destOrganizationUnit)
        {
            clientRef = firmRef = legalPersonRef = destOrganizationUnit = null;
            return false;
        }
    }
}
