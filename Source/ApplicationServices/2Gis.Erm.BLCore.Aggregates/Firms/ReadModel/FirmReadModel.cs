using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Firms.DTO.FirmInfo;
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

        public bool HasFirmClient(long firmId)
        {
            return _finder.Find(Specs.Find.ById<Firm>(firmId)).Select(x => x.ClientId != null).Single();
        }
    }
}