using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Modify.Old
{
    public sealed class CyprusEditLegalPersonProfileHandler : RequestHandler<EditRequest<LegalPersonProfile>, EmptyResponse>, ICyprusAdapted
    {
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public CyprusEditLegalPersonProfileHandler(
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

                var modelLegalPersonType = (LegalPersonType)legalPersonWithProfiles.LegalPerson.LegalPersonTypeEnum;

                if (string.IsNullOrEmpty(request.Entity.Name))
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.Name));
                }

                if (string.IsNullOrEmpty(request.Entity.Name))
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.Name));
                }

                if (string.IsNullOrEmpty(request.Entity.ChiefNameInNominative))
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.ChiefNameInNominative));
                }

                if (string.IsNullOrEmpty(request.Entity.PersonResponsibleForDocuments))
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.PersonResponsibleForDocuments));
                }

                switch (modelLegalPersonType)
                {
                    case LegalPersonType.LegalPerson:

                        if (string.IsNullOrEmpty(request.Entity.PositionInNominative))
                        {
                            throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.PositionInNominative));
                        }

                        if (request.Entity.OperatesOnTheBasisInGenitive == null)
                        {
                            throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.OperatesOnTheBasisInGenitive));
                        }

                        if (request.Entity.OperatesOnTheBasisInGenitive == (int)OperatesOnTheBasisType.Undefined)
                        {
                            throw new NotificationException(string.Format(BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson));
                        }

                        if (request.Entity.OperatesOnTheBasisInGenitive == (int)OperatesOnTheBasisType.Certificate)
                        {
                            throw new NotificationException(string.Format(BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson));
                        }

                        break;
                    case LegalPersonType.Businessman:

                        if (string.IsNullOrEmpty(request.Entity.PositionInNominative))
                        {
                            throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.PositionInNominative));
                        }

                        if (request.Entity.OperatesOnTheBasisInGenitive == null)
                        {
                            throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.OperatesOnTheBasisInGenitive));
                        }

                        if (request.Entity.OperatesOnTheBasisInGenitive == (int)OperatesOnTheBasisType.Undefined)
                        {
                            throw new NotificationException(string.Format(BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson));
                        }

                        if (request.Entity.OperatesOnTheBasisInGenitive == (int)OperatesOnTheBasisType.Charter)
                        {
                            throw new NotificationException(string.Format(BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson));
                        }

                        if (request.Entity.OperatesOnTheBasisInGenitive == (int)OperatesOnTheBasisType.Bargain)
                        {
                            throw new NotificationException(string.Format(BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson));
                        }

                        if (request.Entity.OperatesOnTheBasisInGenitive == (int)OperatesOnTheBasisType.FoundingBargain)
                        {
                            throw new NotificationException(string.Format(BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson));
                        }

                        break;
                    case LegalPersonType.NaturalPerson:
                        if (request.Entity.OperatesOnTheBasisInGenitive == (int)OperatesOnTheBasisType.Charter)
                        {
                            throw new NotificationException(string.Format(BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson));
                        }

                        if (request.Entity.OperatesOnTheBasisInGenitive == (int)OperatesOnTheBasisType.Certificate)
                        {
                            throw new NotificationException(string.Format(BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson));
                        }

                        if (request.Entity.OperatesOnTheBasisInGenitive == (int)OperatesOnTheBasisType.Bargain)
                        {
                            throw new NotificationException(string.Format(BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson));
                        }

                        if (request.Entity.OperatesOnTheBasisInGenitive == (int)OperatesOnTheBasisType.FoundingBargain)
                        {
                            throw new NotificationException(string.Format(BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson));
                        }

                        break;
                    default:
                        throw new NotSupportedException();
                }

                if (request.Entity.OperatesOnTheBasisInGenitive != null)
                {
                    switch ((OperatesOnTheBasisType)request.Entity.OperatesOnTheBasisInGenitive)
                    {
                        case OperatesOnTheBasisType.Undefined:
                        case OperatesOnTheBasisType.FoundingBargain:
                        case OperatesOnTheBasisType.Charter:
                            break;
                        case OperatesOnTheBasisType.Certificate:
                            {
                                if (string.IsNullOrWhiteSpace(request.Entity.CertificateNumber))
                                {
                                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.CertificateNumber));
                                }

                                if (request.Entity.CertificateDate == null)
                                {
                                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.CertificateDate));
                                }

                                break;
                            }

                        case OperatesOnTheBasisType.Warranty:
                            {
                                if (string.IsNullOrWhiteSpace(request.Entity.WarrantyNumber))
                                {
                                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.WarrantyNumber));
                                }

                                if (request.Entity.WarrantyBeginDate == null)
                                {
                                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.WarrantyBeginDate));
                                }

                                if (request.Entity.WarrantyEndDate == null)
                                {
                                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.WarrantyEndDate));
                                }

                                if (request.Entity.WarrantyBeginDate.Value.Date > request.Entity.WarrantyEndDate.Value.Date)
                                {
                                    throw new NotificationException(BLResources.WarrantyBeginDateMustBeGreaterOrEqualThanEndDate);
                                }

                                break;
                            }

                        case OperatesOnTheBasisType.Bargain:
                            {
                                if (string.IsNullOrWhiteSpace(request.Entity.BargainNumber))
                                {
                                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.BargainNumber));
                                }

                                if (request.Entity.BargainBeginDate == null)
                                {
                                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.BargainBeginDate));
                                }

                                if (request.Entity.BargainEndDate == null)
                                {
                                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.BargainEndDate));
                                }

                                if (request.Entity.BargainBeginDate.Value.Date > request.Entity.BargainEndDate.Value.Date)
                                {
                                    throw new NotificationException(BLResources.BargainBeginDateMustBeGreaterOrEqualThanEndDate);
                                }

                                break;
                            }

                        case OperatesOnTheBasisType.RegistrationCertificate:
                        {
                            if (request.Entity.RegistrationCertificateDate == null)
                            {
                                throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.RegistrationCertificateDate));
                            }

                            if (string.IsNullOrEmpty(request.Entity.RegistrationCertificateNumber))
                            {
                                throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.RegistrationCertificateNumber));
                            }

                            break;
                        }

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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