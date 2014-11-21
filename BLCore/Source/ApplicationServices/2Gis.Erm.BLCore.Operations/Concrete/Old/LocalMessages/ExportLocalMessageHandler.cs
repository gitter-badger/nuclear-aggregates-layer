using System;
using System.IO;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Dgpp;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.AutoMailer;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.LocalMessages
{
    public sealed class ExportLocalMessageHandler : RequestHandler<ExportLocalMessageRequest, Response>
    {
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly ISubRequestProcessor _subRequestProcessor;

        public ExportLocalMessageHandler(
            ISubRequestProcessor subRequestProcessor, 
            IBranchOfficeReadModel branchOfficeReadModel)
        {
            _subRequestProcessor = subRequestProcessor;
            _branchOfficeReadModel = branchOfficeReadModel;
        }

        protected override Response Handle(ExportLocalMessageRequest request)
        {
            Response response = null;
            var sucсessFlag = true;
            var errorMsg = string.Empty;
            BusinessLogicException ex2 = null;
            var statusOnSuccess = LocalMessageStatus.WaitForProcess;
            if (request.IntegrationType == IntegrationTypeExport.LegalPersonsTo1C ||
                request.IntegrationType == IntegrationTypeExport.AccountDetailsTo1C)
            {
                statusOnSuccess = LocalMessageStatus.Processed;
            }

            try
            {
                response = ProcessExportMessage(request);
            }
            catch (BusinessLogicException ex)
            {
                sucсessFlag = false;
                errorMsg = ex.Message;
                ex2 = new BusinessLogicException(string.Format(BLResources.ExportLocalMessageError, ex.Message), ex);
            }

            var integrationResponse = response as IntegrationResponse;

            var haveErrors = !sucсessFlag || (integrationResponse != null && integrationResponse.BlockingErrorsAmount > 0);

            // fix 3451, но нужна переработка локальных сообщений в части экспорта
            // не должно создаваться пустое сообщение с ошибкой - нужно просто логгировать ошибку, так же как сделано везде в системе
            // иными словать нижний if должен полностью удалиться
            if (!(response is EmptyResponse))
            {
                _subRequestProcessor
                    .HandleSubRequest(new CreateLocalMessageRequest
                        {
                            Content = sucсessFlag && integrationResponse != null ? integrationResponse.Stream : new MemoryStream(),
                            IntegrationType = (int)request.IntegrationType,
                            FileName = sucсessFlag && integrationResponse != null ? integrationResponse.FileName : string.Empty,
                            ContentType = sucсessFlag && integrationResponse != null ? integrationResponse.ContentType : string.Empty,
                            Entity =
                                new LocalMessage
                                    {
                                        EventDate = DateTime.UtcNow,
                                        Status = !haveErrors ? (int)statusOnSuccess : (int)LocalMessageStatus.Failed,
                                        OrganizationUnitId = request.OrganizationUnitId,
                                        ProcessResult = sucсessFlag && integrationResponse != null
                                                ? (integrationResponse.BlockingErrorsAmount > 0
                                                   || integrationResponse.NonBlockingErrorsAmount > 0
                                                       ? BLResources.ErrorsAreInTheFile
                                                       : string.Empty)
                                                : errorMsg
                                    }

                            // Если не sucсessFlag, то предыдущая транзакция была закрыта при выкидывании 
                            // BusinessLogicException. Поэтому открываем новую.
                        },
                    Context,
                    sucсessFlag);
            }

            if (!sucсessFlag)
            {
                throw ex2;
            }

            return response;
        }

        private Response ProcessExportMessage(ExportLocalMessageRequest request)
        {
            switch (request.IntegrationType)
            {
                case IntegrationTypeExport.FirmsWithActiveOrdersToDgpp:
                    {
                        return _subRequestProcessor.HandleSubRequest(new ExportFirmsWithActiveOrdersRequest(), Context);
                    }

                case IntegrationTypeExport.LegalPersonsTo1C:
                    {
                        return _subRequestProcessor.HandleSubRequest(
                            new ExportLegalPersonsRequest
                                {
                                    OrganizationUnitId = request.OrganizationUnitId,
                                    PeriodStart = request.PeriodStart
                                },
                            Context);
                    }

                case IntegrationTypeExport.DataForAutoMailer:
                    {
                        return _subRequestProcessor.HandleSubRequest(
                            new ExportDataForAutoMailerRequest
                                {
                                    SendingType = request.SendingType,
                                    PeriodStart = request.PeriodStart,
                                    IncludeRegionalAdvertisement = request.IncludeRegionalAdvertisement
                                },
                            Context);
                    }

                case IntegrationTypeExport.AccountDetailsTo1C:
                    {
                        return ExportAccountDetailsTo1C(request);
                    }

                case IntegrationTypeExport.AccountDetailsToServiceBus:
                {
                    var response = ExportAccountDetailsToServiceBus(request);
                    if (request.CreateCsvFile)
                    {
                        var exportTo1CResponse = ExportAccountDetailsTo1C(request);

                        if (response.BlockingErrorsAmount == 0 && exportTo1CResponse.BlockingErrorsAmount == 0)
                        {
                            response.Stream = response.Stream.UnzipStream()
                                                      .Concat(exportTo1CResponse.Stream.UnzipStream(x => x == "Acts.csv"))
                                                      .ToDictionary(x => x.Key, x => x.Value)
                                                      .ZipStreamDictionary();
                        }
                    }

                    return response;
                }

                default:
                    throw new NotSupportedException();
            }
        }

        private IntegrationResponse ExportAccountDetailsToServiceBus(ExportLocalMessageRequest request)
        {
            if (request.OrganizationUnitId == null)
            {
                throw new NotificationException("Не указано отделение организации");
            }

            var contributionType = _branchOfficeReadModel.GetOrganizationUnitContributionType(request.OrganizationUnitId.Value);
            Request exportRequest;

            switch (contributionType)
            {
                case ContributionTypeEnum.Branch:
                {
                    exportRequest = new ExportAccountDetailsToServiceBusForBranchRequest
                        {
                            OrganizationUnitId = request.OrganizationUnitId.Value,
                            StartPeriodDate = request.PeriodStart.GetFirstDateOfMonth(),
                            EndPeriodDate = request.PeriodStart.GetEndPeriodOfThisMonth()
                        };
                    break;
                }
                case ContributionTypeEnum.Franchisees:
                {
                    exportRequest = new ExportAccountDetailsToServiceBusForFranchiseesRequest
                        {
                            OrganizationUnitId = request.OrganizationUnitId.Value,
                            StartPeriodDate = request.PeriodStart.GetFirstDateOfMonth(),
                            EndPeriodDate = request.PeriodStart.GetEndPeriodOfThisMonth()
                        };
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return (IntegrationResponse)_subRequestProcessor.HandleSubRequest(exportRequest, Context);
        }

        private IntegrationResponse ExportAccountDetailsTo1C(ExportLocalMessageRequest request)
        {
            if (request.OrganizationUnitId == null)
            {
                throw new NotificationException("Не указано отделение организации");
            }

            var contributionType = _branchOfficeReadModel.GetOrganizationUnitContributionType(request.OrganizationUnitId.Value);
            Request exportRequest;

            switch (contributionType)
            {
                case ContributionTypeEnum.Branch:
                {
                    exportRequest = new ExportAccountDetailsTo1CForBranchRequest
                        {
                            OrganizationUnitId = request.OrganizationUnitId.Value,
                            StartPeriodDate = request.PeriodStart.GetFirstDateOfMonth(),
                            EndPeriodDate = request.PeriodStart.GetEndPeriodOfThisMonth(),
                        };
                    break;
                }
                case ContributionTypeEnum.Franchisees:
                {
                    exportRequest = new ExportAccountDetailsTo1CForFranchiseesRequest
                        {
                            OrganizationUnitId = request.OrganizationUnitId.Value,
                            StartPeriodDate = request.PeriodStart.GetFirstDateOfMonth(),
                            EndPeriodDate = request.PeriodStart.GetEndPeriodOfThisMonth(),
                        };
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return (IntegrationResponse)_subRequestProcessor.HandleSubRequest(exportRequest, Context);
        }
    }
}
