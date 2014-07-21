using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting
{
    public sealed class EvaluateBargainNumberService : IEvaluateBargainNumberService
    {
        private readonly string _clientBargainNumberTemplate;
        private readonly string _agentBargainNumberTemplate;

        public EvaluateBargainNumberService(string clientBargainNumberTemplate,
                                            string agentBargainNumberTemplate)
        {
            _clientBargainNumberTemplate = clientBargainNumberTemplate;
            _agentBargainNumberTemplate = agentBargainNumberTemplate;
        }

        public string Evaluate(BargainKind bargainKind, string legalPersonOrganizationUnitCode, string branchOfficeOrganizationUnitCode, long bargainUniqueIndex)
        {
            switch (bargainKind)
            {
                case BargainKind.Agent:
                    return string.Format(_agentBargainNumberTemplate, legalPersonOrganizationUnitCode, branchOfficeOrganizationUnitCode, bargainUniqueIndex);
                case BargainKind.Client:
                    return string.Format(_clientBargainNumberTemplate, legalPersonOrganizationUnitCode, branchOfficeOrganizationUnitCode, bargainUniqueIndex);
                default:
                    throw new ArgumentOutOfRangeException("bargainKind");
            }
        }
    }
}