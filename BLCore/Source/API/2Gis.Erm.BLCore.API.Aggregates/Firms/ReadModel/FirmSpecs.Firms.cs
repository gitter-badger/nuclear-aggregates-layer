using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO.FirmInfo;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel
{
    public static partial class FirmSpecs
    {
        public static class Firms
        {
            public static class Find
            {
                public static FindSpecification<Firm> HasClient()
                {
                    return new FindSpecification<Firm>(x => x.Client != null);
                }

                public static FindSpecification<Firm> ByClient(long clientId)
                {
                    return new FindSpecification<Firm>(x => x.ClientId == clientId);
                }

                public static FindSpecification<Firm> ByOrganizationUnit(long organizationUnitId)
                {
                    return new FindSpecification<Firm>(x => x.OrganizationUnitId == organizationUnitId);
                }

                public static FindSpecification<Firm> WithoutActiveOrders()
                {
                    return new FindSpecification<Firm>(x => !x.Orders.Any(y => y.IsActive && !y.IsDeleted));
                }

                public static IFindSpecification<Firm> ByReplicationCodes(IEnumerable<Guid> crmId)
                {
                    return new FindSpecification<Firm>(x => crmId.Contains(x.ReplicationCode));
                }

                public static IFindSpecification<Firm> ByClientIds(IEnumerable<long?> clientAndChild)
                {
                    return new FindSpecification<Firm>(x => clientAndChild.Contains(x.ClientId));
                }
            }

            public static class Select
            {
                public static ISelectSpecification<Firm, int> FirmCountForFirmClient()
                {
                    return new SelectSpecification<Firm, int>(x => x.Client.Firms.Count(y => y.IsActive && !y.IsDeleted));
                }

                public static ISelectSpecification<Firm, FirmWithAddressesAndProjectDto> FirmWithAddressesAndProject()
                {
                    return new SelectSpecification<Firm, FirmWithAddressesAndProjectDto>(firm => new FirmWithAddressesAndProjectDto
                        {
                            Id = firm.Id,
                            Name = firm.Name,
                            OwnerCode = firm.OwnerCode,
                            FirmAddresses = firm.FirmAddresses
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
                            Project = firm.OrganizationUnit.Projects
                                .Where(p => p.IsActive)
                                .Select(p => new ProjectDto
                                    {
                                        Code = p.Id,
                                        Name = p.DisplayName
                                    })
                                .FirstOrDefault()
                        });
                }
            }
        }
    }
}