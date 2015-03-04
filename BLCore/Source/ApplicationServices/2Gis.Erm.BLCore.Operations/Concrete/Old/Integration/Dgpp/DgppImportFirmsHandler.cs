using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Dgpp;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.Dgpp
{
    [Obsolete("usecase оставлен просто для подстраховки - пока все города не откажутся от ДГПП, на практике он уже не используется")]
    public sealed class DgppImportFirmsHandler : RequestHandler<DgppImportFirmsRequest, ImportResponse>
    {
        private const string XmlHeader = "header";
        private const string XmlHeaderId = "mguid";
        private const string XmlHeaderCreateDate = "createDate";
        private const string XmlHeaderExportStartDate = "exportStartDate";
        private const string XmlHeaderExportEndDate = "exportEndDate";
        private const string XmlHeaderOrganizationUnitId = "exportOrganizationUnitId";

        private const string XmlFirms = "firms";

        private const string XmlFirm = "firm";
        private const string XmlFirmId = "id";
        private const string XmlFirmName = "name";
        private const string XmlFirmTerritoryId = "territoryId";
        private const string XmlFirmOrganizationUnitId = "organizationUnitId";
        private const string XmlFirmIsActive = "isActive";
        private const string XmlFirmIsDeleted = "isDeleted";
        private const string XmlFirmClosedForAscertainment = "closedForAscertainment";
        private const string XmlFirmAddresses = "firmAddresses";

        private const string XmlFirmAddress = "firmAddress";
        private const string XmlFirmAddressId = "id";
        private const string XmlFirmAddressSortingPosition = "sortingPosition";
        private const string XmlFirmAddressName = "name";
        private const string XmlFirmAddressAddress = "address";
        private const string XmlFirmAddressPaymentMethods = "paymentMethods";
        private const string XmlFirmAddressWorkingTime = "workingTime";
        private const string XmlFirmAddressWorkingTimeComment = "workingTimeComment";
        private const string XmlFirmAddressIsActive = "isActive";
        private const string XmlFirmAddressClosedForAscertainment = "closedForAscertainment";
        private const string XmlFirmAddressFirmContacts = "firmContacts";
        private const string XmlFirmAddressCategories = "categories";

        private const string XmlFirmContact = "firmContact";
        private const string XmlFirmContactId = "id";
        private const string XmlFirmContactSortingPosition = "sortingPosition";
        private const string XmlFirmContactContactType = "contactType";
        private const string XmlFirmContactContact = "contact";
        private const string XmlFirmContactIsActive = "isActive";
        private const string XmlFirmContactClosedForAscertainment = "closedForAscertainment";

        private const string XmlFirmCategory = "category";
        private const string XmlFirmCategoryId = "id";
        private const string XmlFirmCategorySortingPosition = "sortingPosition";
        private const string XmlFirmCategoryIsPrimary = "IsPrimary";

        private const string HandlerName = "Импорт фирм из ДГПП";

        private readonly ISecurityServiceUserIdentifier _securityService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ITracer _logger;

        private readonly IFirmRepository _firmRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DgppImportFirmsHandler(ISecurityServiceUserIdentifier securityService,
            ITracer logger,
            IFirmRepository firmRepository, 
            IUnitOfWork unitOfWork,
            IOperationScopeFactory operationScopeFactory)
        {
            _securityService = securityService;
            _logger = logger;
            _firmRepository = firmRepository;
            _unitOfWork = unitOfWork;
            _operationScopeFactory = operationScopeFactory;
        }

        protected override ImportResponse Handle(DgppImportFirmsRequest request)
        {
            try
            {
                _logger.InfoFormat("{0}: начало", HandlerName);
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    ImportFirmsHeaderDto header;
                    ICollection<ImportFirmDto> firms;
                    using (var streamReader = new StreamReader(request.MessageStream, Encoding.UTF8))
                    using (var xmlReader = XmlReader.Create(streamReader, request.XmlReaderSettings))
                    {
                        ReadXml(xmlReader, out header, out firms);
                    }

                    var result = UpdateFirms(header, firms);
                    transaction.Complete();
                    return result;
                }
            }
            catch (NotificationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NotificationException("Ошибка обработки сообщения", ex);
            }
            finally
            {
                _logger.InfoFormat("{0}: окончание", HandlerName);
            }
        }

        private ImportResponse UpdateFirms(ImportFirmsHeaderDto header, ICollection<ImportFirmDto> firms)
        {
            var context = InitializeImportContext(header);
            var importedFirms = ImportFirms(firms, context);
            var importedAddresses = ImportAddresses(importedFirms, context);
            ImportContactsAndCategories(importedAddresses, context);

            return new ImportResponse { Processed = firms.Count(), Total = firms.Count(), OrganizationUnitId = header.OrganizationUnitId };
        }

        private void ImportContactsAndCategories(IEnumerable<Tuple<FirmAddress, ImportFirmAddressDto>> importedAddresses, FirmImportContext context)
        {
            // COMMENT {all, 05.12.2014}: здесь использование UoWScope оставил, т.к. решил что разбираться необходима или нет изоляция между DomainContext для фирм и т.п. не целесообразно, лучше оставить
            // Данный usecase фактически уже не используется, код интеграции с ДГПП пока оставлен на всякий случай, но скоро пойдет идет под снос, так что код будет выпилен вместе со всей интеграцией с ДГПП.
            // В данном usecase после выпиливания отложенного сохранения просела бы производительность , если бы он массово использовался (т.к. UoWScope дает теперь только изоляцию, но не batch cохранение изменений),
            // однако, т.к. этого нет, и, скорее всего, не будет существенный рефакторинг не выполнялся
            using (var scope = _unitOfWork.CreateScope())
            {
                var firmRepository = scope.CreateRepository<IFirmRepository>();
                foreach (var address in importedAddresses)
                {
                    try
                    {
                        firmRepository.ImportAddressContacts(address.Item1, address.Item2);
                        firmRepository.ImportAddressCategories(address.Item1, address.Item2, context);
                    }
                    catch (Exception e)
                    {
                        var message = string.Format("Ошибка импорта рубрик и контактов адреса с кодом [{0}]", address.Item1.Id);
                        throw new NotificationException(message, e);
                    }
                }

                scope.Complete();
            }
        }

        private IEnumerable<Tuple<FirmAddress, ImportFirmAddressDto>> ImportAddresses(IEnumerable<Tuple<Firm, ImportFirmDto>> importedFirms, FirmImportContext context)
        {
            // COMMENT {all, 05.12.2014}: здесь использование UoWScope оставил, т.к. решил что разбираться необходима или нет изоляция между DomainContext для фирм и т.п. не целесообразно, лучше оставить
            // Данный usecase фактически уже не используется, код интеграции с ДГПП пока оставлен на всякий случай, но скоро пойдет идет под снос, так что код будет выпилен вместе со всей интеграцией с ДГПП.
            // В данном usecase после выпиливания отложенного сохранения просела бы производительность, если бы он массово использовался (т.к. UoWScope дает теперь только изоляцию, но не batch cохранение изменений),
            // однако, т.к. этого нет, и, скорее всего, не будет существенный рефакторинг не выполнялся
            var importedAddresses = new List<Tuple<FirmAddress, ImportFirmAddressDto>>();
            using (var scope = _unitOfWork.CreateScope())
            {
                var firmRepository = scope.CreateRepository<IFirmRepository>();
                foreach (var firm in importedFirms)
                {
                    try
                    {
                        if (firm.Item1.IsDeleted)
                        {
                            firmRepository.DeleteFirmRelatedObjects(firm.Item1);
                            continue;
                        }

                        var firmAddresses = _firmRepository.ImportFirmAddresses(firm.Item1, firm.Item2, context);
                        foreach (var firmAddress in firmAddresses)
                        {
                            var addressDto = firm.Item2.Addresses.Single(dto => dto.DgppId == firmAddress.Id);
                            importedAddresses.Add(new Tuple<FirmAddress, ImportFirmAddressDto>(firmAddress, addressDto));
                        }
                    }
                    catch (Exception e)
                    {
                        var message = string.Format("Ошибка импорта адресов фирмы с кодом [{0}]", firm.Item1.Id);
                        throw new NotificationException(message, e);
                    }
                }

                scope.Complete();
            }

            return importedAddresses;
        }

        private IEnumerable<Tuple<Firm, ImportFirmDto>> ImportFirms(IEnumerable<ImportFirmDto> firms, FirmImportContext context)
        {
            var importedFirms = new List<Tuple<Firm, ImportFirmDto>>();
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<ImportFirmIdentity>())
            using (var scope = _unitOfWork.CreateScope())
            {
                var firmRepository = scope.CreateRepository<IFirmRepository>();
                var clientRepository = scope.CreateRepository<IClientRepository>();

                foreach (var firmDto in firms)
                {
                    try
                    {
                        var firmEntity = firmRepository.ImportFirmFromDgpp(firmDto, context);
                        importedFirms.Add(new Tuple<Firm, ImportFirmDto>(firmEntity, firmDto));

                        if (firmEntity.ClosedForAscertainment || !firmEntity.IsActive || firmEntity.IsDeleted)
                        {
                            clientRepository.HideFirm(firmEntity.Id);
                        }
                    }
                    catch (Exception e)
                    {
                        var message = string.Format("Ошибка импорта объекта фирмы с кодом ДГПП [{0}]", firmDto.DgppId);
                        throw new NotificationException(message, e);
                    }
                }

                scope.Complete();

                operationScope.Updated<Firm>(importedFirms.Select(tuple => tuple.Item1.Id).ToArray());
                operationScope.Complete();
            }

            return importedFirms;
        }

        private FirmImportContext InitializeImportContext(ImportFirmsHeaderDto header)
        {
            var organizationUnit = _firmRepository.GetOrganizationUnit(header.DgppId);
            if (organizationUnit.InfoRussiaLaunchDate != null)
            {
                throw new NotificationException(string.Format("Город {0} переведён на InfoRussia, импорт фирм из ДГПП невозможен", organizationUnit.Name));
            }

            header.OrganizationUnitId = organizationUnit.Id;

            return new FirmImportContext
                {
                    Categories = _firmRepository.GetCategoriesOfOrganizationUnit(header.OrganizationUnitId),
                    OrganizationUnits = _firmRepository.GetOrganizationUnits(),
                    ReserveUserId = _securityService.GetReserveUserIdentity().Code,
                    Territories = _firmRepository.GetTerritoriesOfOrganizationUnit(header.OrganizationUnitId)
                };
        }

        #region Xml Reading
        private void ReadXml(XmlReader xmlReader, out ImportFirmsHeaderDto header, out ICollection<ImportFirmDto> firms)
        {
            header = default(ImportFirmsHeaderDto);
            firms = default(ICollection<ImportFirmDto>);

            xmlReader.MoveToContent();
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (string.Equals(xmlReader.Name, XmlHeader, StringComparison.OrdinalIgnoreCase))
                {
                    header = LocalMessageHelper.Read<ImportFirmsHeaderDto>(HandlerName, xmlReader, XmlHeader, Read);                    
                }
                else if (string.Equals(xmlReader.Name, XmlFirms, StringComparison.OrdinalIgnoreCase))
                {
                    firms = LocalMessageHelper.Read<List<ImportFirmDto>>(HandlerName, xmlReader, XmlFirms, Read);
                }
            }

            if (header == null)
            {
                throw new NotificationException(string.Format("В сообщении не найден узел '{0}'", XmlHeader));
            }

            if (firms == null)
            {
                throw new NotificationException(string.Format("В сообщении не найден узел '{0}'", XmlFirms));
            }
        }

        private bool Read(XmlReader reader, List<ImportFirmDto> accumulator)
        {
            if (!string.Equals(reader.Name, XmlFirm, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            accumulator.Add(LocalMessageHelper.Read<ImportFirmDto>(HandlerName, reader, XmlFirm, Read));
            return true;
        }

        private bool Read(XmlReader reader, ImportFirmDto accumulator)
        {
            string value;
            if (LocalMessageHelper.TryReadElementText(reader, XmlFirmId, out value))
            {
                accumulator.DgppId = long.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmName, out value))
            {
                accumulator.Name = value;
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmTerritoryId, out value))
            {
                accumulator.TerritoryDgppId = long.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmOrganizationUnitId, out value))
            {
                accumulator.OrganizationUnitDgppId = int.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmIsActive, out value))
            {
                accumulator.IsActive = bool.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmIsDeleted, out value))
            {
                accumulator.IsDeleted = bool.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmClosedForAscertainment, out value))
            {
                accumulator.IsClosedForAscertainment = bool.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, "showFlampLink", out value))
            {
                return true;
            }
            else if (string.Equals(reader.Name, XmlFirmAddresses, StringComparison.OrdinalIgnoreCase))
            {
                accumulator.Addresses = LocalMessageHelper.Read<List<ImportFirmAddressDto>>(HandlerName, reader, XmlFirmAddresses, Read);
            }
            else
            {
                return false;
            }

            return true;
        }

        private bool Read(XmlReader reader, List<ImportFirmAddressDto> accumulator)
        {
            if (!string.Equals(reader.Name, XmlFirmAddress, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            accumulator.Add(LocalMessageHelper.Read<ImportFirmAddressDto>(HandlerName, reader, XmlFirmAddress, Read));
            return true;
        }

        private bool Read(XmlReader reader, ImportFirmAddressDto accumulator)
        {
            string value;
            if (LocalMessageHelper.TryReadElementText(reader, XmlFirmAddressId, out value))
            {
                accumulator.DgppId = long.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmAddressSortingPosition, out value))
            {
                accumulator.SortingPosition = int.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmAddressName, out value))
            {
                accumulator.Name = value;
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmAddressAddress, out value))
            {
                accumulator.Address = value;
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmAddressPaymentMethods, out value))
            {
                accumulator.PaymentMethods = value;
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmAddressWorkingTime, out value))
            {
                accumulator.WorkingTime = value;
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmAddressWorkingTimeComment, out value))
            {
                accumulator.WorkingTimeComment = value;
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmAddressIsActive, out value))
            {
                accumulator.IsActive = bool.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmAddressClosedForAscertainment, out value))
            {
                accumulator.IsClosedForAscertainment = bool.Parse(value);
            }
            else if (string.Equals(reader.Name, XmlFirmAddressFirmContacts, StringComparison.OrdinalIgnoreCase))
            {
                accumulator.Contacts = LocalMessageHelper.Read<List<ImportFirmAddressContactDto>>(HandlerName, reader, XmlFirmAddressFirmContacts, Read);
            }
            else if (string.Equals(reader.Name, XmlFirmAddressCategories, StringComparison.OrdinalIgnoreCase))
            {
                accumulator.Categories = LocalMessageHelper.Read<List<ImportFirmAddressCategoryDto>>(HandlerName, reader, XmlFirmAddressCategories, Read);
            }
            else
            {
                return false;
            }

            return true;
        }

        private bool Read(XmlReader reader, List<ImportFirmAddressCategoryDto> accumulator)
        {
            if (!string.Equals(reader.Name, XmlFirmCategory, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            accumulator.Add(LocalMessageHelper.Read<ImportFirmAddressCategoryDto>(HandlerName, reader, XmlFirmCategory, Read));
            return true;
        }

        private bool Read(XmlReader reader, ImportFirmAddressCategoryDto accumulator)
        {
            if (!string.Equals(reader.Name, XmlFirmCategory, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var id = reader.GetAttribute(XmlFirmCategoryId);
            if (!string.IsNullOrEmpty(id))
            {
                accumulator.DgppId = long.Parse(id);
            }

            var sortingPosition = reader.GetAttribute(XmlFirmCategorySortingPosition);
            if (!string.IsNullOrEmpty(sortingPosition))
            {
                accumulator.SortingPosition = int.Parse(sortingPosition);
            }

            var isPrimary = reader.GetAttribute(XmlFirmCategoryIsPrimary);
            if (!string.IsNullOrEmpty(isPrimary))
            {
                accumulator.IsPrimary = bool.Parse(isPrimary);
            }

            return true;
        }

        private bool Read(XmlReader reader, List<ImportFirmAddressContactDto> accumulator)
        {
            if (!string.Equals(reader.Name, XmlFirmContact, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var importFirmAddressContactDto = LocalMessageHelper.Read<ImportFirmAddressContactDto>(HandlerName, reader, XmlFirmContact, Read);
            if (!importFirmAddressContactDto.IsActive || importFirmAddressContactDto.IsClosedForAscertainment)
            {
                return true;
            }

            accumulator.Add(importFirmAddressContactDto);
            return true;
        }

        private bool Read(XmlReader reader, ImportFirmAddressContactDto accumulator)
        {
            string value;
            if (LocalMessageHelper.TryReadElementText(reader, XmlFirmContactId, out value))
            {
                accumulator.DgppId = long.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmContactSortingPosition, out value))
            {
                accumulator.SortingPosition = int.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmContactContactType, out value))
            {
                accumulator.ContactType = int.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmContactContact, out value))
            {
                accumulator.Contact = value;
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmContactIsActive, out value))
            {
                accumulator.IsActive = bool.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlFirmContactClosedForAscertainment, out value))
            {
                accumulator.IsClosedForAscertainment = bool.Parse(value);
            }
            else
            {
                return false;
            }

            return true;
        }

        private bool Read(XmlReader reader, ImportFirmsHeaderDto accumulator)
        {
            string value;
            if (LocalMessageHelper.TryReadElementText(reader, XmlHeaderId, out value))
            {
                accumulator.MessageId = Guid.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlHeaderCreateDate, out value))
            {
                accumulator.CreateDate = DateTime.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlHeaderExportEndDate, out value))
            {
                accumulator.ExportEndDate = DateTime.Parse(value);                
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlHeaderExportStartDate, out value))
            {
                accumulator.ExportStartDate = DateTime.Parse(value);                
            }
            else if (LocalMessageHelper.TryReadElementText(reader, XmlHeaderOrganizationUnitId, out value))
            {
                accumulator.DgppId = int.Parse(value);
            }
            else
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
