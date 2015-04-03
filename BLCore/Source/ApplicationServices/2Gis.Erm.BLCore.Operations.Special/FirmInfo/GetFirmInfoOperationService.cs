using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO.FirmInfo;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Special.FirmInfo;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Special.FirmInfo
{
    public class GetFirmInfoOperationService : IGetFirmInfoOperationService
    {
        private readonly IFirmReadModel _firmReadModel;
        private readonly IUserReadModel _userReadModel;

        public GetFirmInfoOperationService(IFirmReadModel firmReadModel, IUserReadModel userReadModel)
        {
            _firmReadModel = firmReadModel;
            _userReadModel = userReadModel;
        }

        public IEnumerable<FirmInfoDto> GetFirmInfosByIds(IEnumerable<long> ids)
        {
            var idsWithMssingFirms = new List<long>();
            var idsWithMissingProjects = new List<long>();
            var result = new List<FirmInfoDto>();

            var firms = _firmReadModel.GetFirmInfosByIds(ids);
            var users = _userReadModel.GetUserNames(firms.Values.Select(f => f.OwnerCode));

            foreach (var dto in firms.Values)
            {
                dto.Owner = users[dto.OwnerCode];
            }

            foreach (var id in ids)
            {
                FirmWithAddressesAndProjectDto firm;
                if (!firms.TryGetValue(id, out firm))
                {
                    idsWithMssingFirms.Add(id);
                    continue;
                }

                if (firm.Project == null)
                {
                    idsWithMissingProjects.Add(id);
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
                    Owner = firm.Owner,
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

        private static bool TryGetErrorReport(IEnumerable<long> idsWithMssingFirms, IEnumerable<long> idsWithMissingProjects, out string report)
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