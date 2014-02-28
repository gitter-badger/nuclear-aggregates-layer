using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.Aggregates.Firms.DTO.FirmInfo;
using DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Special.FirmInfo;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Special.FirmInfo
{
    public class GetFirmInfoService : IGetFirmInfoService
    {
        private readonly IFirmReadModel _firmReadModel;

        public GetFirmInfoService(IFirmReadModel firmReadModel)
        {
            _firmReadModel = firmReadModel;
        }

        public IEnumerable<FirmInfoDto> GetFirmInfosByCrmIds(IEnumerable<Guid> crmIds)
        {
            var idsWithMssingFirms = new List<Guid>();
            var idsWithMissingProjects = new List<Guid>();
            var result = new List<FirmInfoDto>();

            var firms = _firmReadModel.GetFirmInfosByCrmIds(crmIds);

            foreach (var crmId in crmIds)
            {
                FirmWithAddressesAndProjectDto firm;
                if (!firms.TryGetValue(crmId, out firm))
                {
                    idsWithMssingFirms.Add(crmId);
                    continue;
                }

                if (firm.Project == null)
                {
                    idsWithMissingProjects.Add(crmId);
                    continue;
                }

                result.Add(CreateFirmInfo(firm));
            }

            string report;
            if (TryGetErrorReport(idsWithMssingFirms, idsWithMissingProjects, out report))
            {
                throw new NotificationException(report);
            }

            return result;
        }

        private static FirmInfoDto CreateFirmInfo(FirmWithAddressesAndProjectDto firm)
        {
            var firmDto = new FirmInfoDto
                {
                    Id = firm.Id,
                    Name = firm.Name,
                    Project = new ProjectInfoDto
                        {
                            Code = firm.Project.Code,
                            Name = firm.Project.Name
                        },
                    FirmAddresses = firm.FirmAddresses.Select(fa => new FirmAddressInfoDto
                        {
                            Id = fa.Id,
                            Address = fa.Address,
                            Categories = fa.Categories.Select(c => new CategoryInfoDto
                                {
                                    Id = c.Id,
                                    Name = c.Name
                                })
                        })
                };
            return firmDto;
        }

        private static bool TryGetErrorReport(IEnumerable<Guid> idsWithMssingFirms, IEnumerable<Guid> idsWithMissingProjects, out string report)
        {
            var sb = new StringBuilder();

            foreach (var id in idsWithMssingFirms)
            {
                sb.AppendFormat("Can't find firm with id [{0}]", id).AppendLine();
            }

            foreach (var id in idsWithMissingProjects)
            {
                sb.AppendFormat("Can't find project for firm with id [{0}]", id).AppendLine();
            }

            report = sb.ToString();

            return sb.Length != 0;
        }
    }
}