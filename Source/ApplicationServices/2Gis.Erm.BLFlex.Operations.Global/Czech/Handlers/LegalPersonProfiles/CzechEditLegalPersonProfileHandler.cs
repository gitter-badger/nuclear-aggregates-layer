using System;
using System.Linq;

using DoubleGis.Erm.BL.Aggregates.LegalPersons;
using DoubleGis.Erm.BL.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Handlers.LegalPersonProfiles
{
    public sealed class CzechEditLegalPersonProfileHandler : RequestHandler<EditRequest<LegalPersonProfile>, EmptyResponse>, ICzechAdapted
    {
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public CzechEditLegalPersonProfileHandler(
            ILegalPersonRepository legalPersonRepository,
            IOperationScopeFactory scopeFactory)
        {
            _legalPersonRepository = legalPersonRepository;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(EditRequest<LegalPersonProfile> request)
        {
            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(request.Entity))
            {
                var legalPersonWithProfiles = _legalPersonRepository.GetLegalPersonWithProfiles(request.Entity.LegalPersonId);
                if (legalPersonWithProfiles == null)
                {
                    throw new NotificationException(BLResources.LegalPersonNotFound);
                }

                if (legalPersonWithProfiles.Profiles.Any(legalPersonProfile => legalPersonProfile.Name == request.Entity.Name && legalPersonProfile.Id != request.Entity.Id))
                {
                    throw new NotificationException(BLResources.LegalPersonProfileNameIsNotUnique);
                }

                if (!legalPersonWithProfiles.Profiles.Any())
                {
                    request.Entity.IsMainProfile = true;
                }
                
                if (string.IsNullOrEmpty(request.Entity.Name))
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.Name));
                }

                if (string.IsNullOrEmpty(request.Entity.ChiefNameInNominative))
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.ChiefNameInNominative));
                }

                if (string.IsNullOrEmpty(request.Entity.ChiefNameInGenitive))
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.ChiefNameInGenitive));
                }

                if (string.IsNullOrEmpty(request.Entity.PersonResponsibleForDocuments))
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.PersonResponsibleForDocuments));
                }

                if ((DocumentsDeliveryMethod)request.Entity.DocumentsDeliveryMethod == DocumentsDeliveryMethod.Undefined)
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.DocumentsDeliveryMethod));
                }

                if (request.Entity.OperatesOnTheBasisInGenitive == null)
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.OperatesOnTheBasisInGenitive));
                }

                if (request.Entity.PaymentMethod == null)
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.PaymentMethod));
                }

                if ((PaymentMethod)request.Entity.PaymentMethod == PaymentMethod.BankTransaction)
                {
                    if (string.IsNullOrEmpty(request.Entity.AccountNumber))
                    {
                        throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.AccountNumber));
                    }

                    if (string.IsNullOrEmpty(request.Entity.BankCode))
                    {
                        throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.BankCode));
                    }

                    if (string.IsNullOrEmpty(request.Entity.BankName))
                    {
                        throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.BankName));
                    }
                }

                if ((LegalPersonType)legalPersonWithProfiles.LegalPerson.LegalPersonTypeEnum == LegalPersonType.LegalPerson)
                {
                    if (string.IsNullOrEmpty(request.Entity.Registered))
                    {
                        throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.Registered));
                    }
                }

                switch ((OperatesOnTheBasisType)request.Entity.OperatesOnTheBasisInGenitive)
                {
                    case OperatesOnTheBasisType.Underfined:
                    case OperatesOnTheBasisType.FoundingBargain:
                    case OperatesOnTheBasisType.Charter:
                    case OperatesOnTheBasisType.Certificate:
                    case OperatesOnTheBasisType.Bargain:
                    case OperatesOnTheBasisType.RegistrationCertificate:
                        throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.OperatesOnTheBasisInGenitive)); 

                    case OperatesOnTheBasisType.None:
                        break;

                    case OperatesOnTheBasisType.Warranty:
                        {
                            if (request.Entity.WarrantyBeginDate == null)
                            {
                                throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.WarrantyBeginDate));
                            }
                            
                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                bool isNewProfile = request.Entity.IsNew();
                try
                {
                    _legalPersonRepository.CreateOrUpdate(request.Entity);
                }
                catch (Exception ex)
                {
                    throw new NotificationException(BLResources.ErrorWhileSavingLegalPersonProfile, ex);
                }

                if (isNewProfile)
                {
                    operationScope.Added<LegalPersonProfile>(request.Entity.Id);
                }
                else
                {
                    operationScope.Updated<LegalPersonProfile>(request.Entity.Id);
                }

                operationScope.Complete();
            }

            return Response.Empty;
        }
    }
}