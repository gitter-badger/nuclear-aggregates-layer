using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO.FirmInfo;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel
{
    public class FirmReadModel : IFirmReadModel
    {
        private readonly ISecureFinder _finder;

        public FirmReadModel(ISecureFinder finder)
        {
            _finder = finder;
        }

        public long GetOrderFirmId(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId)).Select(x => x.FirmId).Single();
        }

        public IReadOnlyDictionary<Guid, FirmWithAddressesAndProjectDto> GetFirmInfosByCrmIds(IEnumerable<Guid> crmIds)
        {
            return _finder.Find(FirmSpecs.Firms.Find.ByReplicationCodes(crmIds))
                          .Select(f => new
                              {
                                  CrmId = f.ReplicationCode,
                                  Dto = new FirmWithAddressesAndProjectDto
                                      {
                                          Id = f.Id,
                                          Name = f.Name,
                                          FirmAddresses = f.FirmAddresses 
                                                           .Where(fa => fa.IsActive && !fa.IsDeleted && !fa.ClosedForAscertainment)
                                                           .Select(fa => new FirmAddressWithCategoriesDto
                                                               {
                                                                   Id = fa.Id,
                                                                   Address = fa.Address,
                                                                   Categories = fa.CategoryFirmAddresses
                                                                                  .Where(cfa => cfa.IsActive && !cfa.IsDeleted)
                                                                                  .Select(cfa => new CategoryDto
                                                                                      {
                                                                                          Id = cfa.CategoryId,
                                                                                          Name = cfa.Category.Name
                                                                                      })
                                                               }),
                                          Project = f.OrganizationUnit.Projects
                                                     .Where(p => p.IsActive)
                                                     .Select(p => new ProjectDto
                                                         {
                                                             Code = p.Code,
                                                             Name = p.DisplayName
                                                         }).FirstOrDefault()

                                      }
                              })
                          .ToDictionary(x => x.CrmId, x => x.Dto);
        }

        public IEnumerable<long> GetFirmNonArchivedOrderIds(long firmId)
        {
            return _finder.Find(OrderSpecs.Orders.Find.ActiveOrdersForFirm(firmId)).Select(x => x.Id).ToArray();
        }

        public long GetOrgUnitId(long firmId)
        {
            return _finder.Find(Specs.Find.ById<Firm>(firmId)).Select(x => x.OrganizationUnitId).Single();
        }

        public bool HasFirmClient(long firmId)
        {
            return _finder.Find(Specs.Find.ById<Firm>(firmId)).Select(x => x.ClientId != null).Single();
        }

        public IEnumerable<CategoryGroup> GetFirmAddressCategoryGroups(long firmAddressId)
        {
            var organizationUnitId = _finder.Find(Specs.Find.ById<FirmAddress>(firmAddressId))
                                            .Select(address => address.Firm.Territory.OrganizationUnitId)
                                            .SingleOrDefault();

            var categoryIds = _finder.Find(Specs.Find.ById<FirmAddress>(firmAddressId))
                                     .SelectMany(address => address.Firm.FirmAddresses)
                                     .Where(Specs.Find.ActiveAndNotDeleted<FirmAddress>())
                                     .SelectMany(address => address.CategoryFirmAddresses)
                                     .Where(Specs.Find.ActiveAndNotDeleted<CategoryFirmAddress>())
                                     .Select(categoryFirmAddress => categoryFirmAddress.CategoryId)
                                     .Distinct()
                                     .ToArray();

            var groups = _finder.Find(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>())
                                .Where(link => link.OrganizationUnitId == organizationUnitId && categoryIds.Contains(link.CategoryId))
                                .Select(link => link.CategoryGroup)
                                .Distinct()
                                .ToArray();

            return groups;
        }

        public FirmAndClientDto GetFirmAndClientByFirmAddress(long firmAddressCode)
        {
            return _finder.Find(Specs.Find.ById<FirmAddress>(firmAddressCode) && Specs.Find.NotDeleted<FirmAddress>())
                          .Select(x => new FirmAndClientDto
                          {
                              Firm = x.Firm != null && !x.Firm.IsDeleted ? x.Firm : (Firm)null,
                              Client = x.Firm != null && !x.Firm.IsDeleted ? (!x.Firm.Client.IsDeleted ? x.Firm.Client : (Client)null) : (Client)null
                          })
                          .FirstOrDefault();
        }
    }
}