using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositionAdvertisementValidation;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IOrderPersistenceService _orderPersistenceService;
        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;
        private readonly IFileContentFinder _fileContentFinder;
        private readonly IRepository<Order> _orderGenericRepository;
        private readonly ISecureRepository<Order> _orderSecureGenericRepository;
        private readonly IRepository<OrderPosition> _orderPositionGenericRepository;
        private readonly IRepository<OrderPositionAdvertisement> _orderPositionAdvertisementGenericRepository;
        private readonly IRepository<Bill> _billGenericRepository;
        private readonly IRepository<OrderFile> _orderFileGenericRepository;
        private readonly IRepository<OrderReleaseTotal> _orderReleaseTotalGenericRepository;
        private readonly IRepository<FileWithContent> _fileRepository;
        private readonly IUserContext _userContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IValidateOrderPositionAdvertisementsService _validateOrderPositionAdvertisementsService;

        public OrderRepository(IFinder finder,
                               ISecureFinder secureFinder,
                               IFileContentFinder fileContentFinder,
                               IRepository<Order> orderGenericRepository,
                               IRepository<OrderPosition> orderPositionEntityRepository,
                               IRepository<OrderPositionAdvertisement> orderPositionAdvertisementEntityRepository,
                               IRepository<Bill> billGenericRepository,
                               IRepository<OrderFile> orderFileGenericRepository,
                               IRepository<OrderReleaseTotal> orderReleaseTotalGenericRepository,
                               IRepository<FileWithContent> fileRepository,
                               IUserContext userContext,
                               IOrderPersistenceService orderPersistenceService,
                               ISecureRepository<Order> orderSecureGenericRepository,
                               IIdentityProvider identityProvider,
                               IOperationScopeFactory scopeFactory,
                               IValidateOrderPositionAdvertisementsService validateOrderPositionAdvertisementsService)
        {
            _finder = finder;
            _secureFinder = secureFinder;
            _orderGenericRepository = orderGenericRepository;
            _orderPositionGenericRepository = orderPositionEntityRepository;
            _orderPositionAdvertisementGenericRepository = orderPositionAdvertisementEntityRepository;
            _billGenericRepository = billGenericRepository;
            _orderFileGenericRepository = orderFileGenericRepository;
            _orderReleaseTotalGenericRepository = orderReleaseTotalGenericRepository;
            _userContext = userContext;
            _orderPersistenceService = orderPersistenceService;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
            _validateOrderPositionAdvertisementsService = validateOrderPositionAdvertisementsService;
            _fileContentFinder = fileContentFinder;
            _orderSecureGenericRepository = orderSecureGenericRepository;
            _fileRepository = fileRepository;
        }

        public long GenerateNextOrderUniqueNumber()
        {
            return _orderPersistenceService.GenerateNextOrderUniqueNumber();
        }

        public int CreateOrUpdate(OrderFile entity)
        {
            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(entity))
            {
                if (entity.IsNew())
                {
                    _identityProvider.SetFor(entity);
                    _orderFileGenericRepository.Add(entity);
                    scope.Added<OrderFile>(entity.Id);
                }
                else
                {
                    _orderFileGenericRepository.Update(entity);
                    scope.Updated<OrderFile>(entity.Id);
                }

                var cnt = _orderFileGenericRepository.Save();
                scope.Complete();

                return cnt;
            }
        }

        public int Create(Order order)
        {
            CheckOrderApprovalDateSpecified(order);
            CheckOrderPlatformSpecified(order);
            CheckOrderLegalPersonProfileBelongsToOrderLegalPerson(order);

            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Order>())
            {
                _identityProvider.SetFor(order);
                _orderGenericRepository.Add(order);

                int cnt = _orderGenericRepository.Save();

                scope.Added<Order>(order.Id)
                     .Complete();
                return cnt;
            }
        }

        public int Update(OrderPosition orderPosition)
        {
            return CreateOrUpdate(orderPosition);
        }

        public int CreateOrUpdate(OrderPosition orderPosition)
        {
            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(orderPosition))
            {
                if (orderPosition.IsNew())
                {
                    _identityProvider.SetFor(orderPosition);
                    _orderPositionGenericRepository.Add(orderPosition);
                    operationScope.Added<OrderPosition>(orderPosition.Id);
                }
                else
                {
                    _orderPositionGenericRepository.Update(orderPosition);
                    operationScope.Updated<OrderPosition>(orderPosition.Id);
                }

                var result = _orderPositionGenericRepository.Save();
                operationScope.Complete();
                return result;
            }
        }

        public Order CreateCopiedOrder(Order order, IEnumerable<OrderPositionWithAdvertisementsDto> orderPositionDtos)
        {
            // Чтобы система считала заказ новым
            order.ResetToNew();

            // Чтобы сгенерировать новые номера
            order.Number = string.Empty;
            order.RegionalNumber = null;

            // Чтобы заказ по логике являлся новым
            order.WorkflowStepId = OrderState.OnRegistration;
            order.TerminationReason = OrderTerminationReason.None;
            order.HasDocumentsDebt = DocumentsDebt.Absent;
            order.SignupDate = DateTime.UtcNow;
            order.Comment = null;
            order.ApprovalDate = null;
            order.RejectionDate = null;
            order.DocumentsComment = null;
            order.DgppId = null;
            order.IsTerminated = false;

            // Деньги должны пересчитываться заново
            order.AmountWithdrawn = 0;
            order.PayableFact = 0;
            order.PayablePlan = 0;

            Create(order);

            // TODO {all, 24.09.2013}: при рефакторинге ApplicationServices, попробовать перевести на Bulk режим внесения изменений
            foreach (var orderPositionDto in orderPositionDtos)
            {
                // Превращаем полученную из БД сущность в новую и привязываем её к новому заказу
                var orderPosition = orderPositionDto.OrderPosition;
                orderPosition.ResetToNew();
                _identityProvider.SetFor(orderPosition);

                orderPosition.OrderId = order.Id;
                orderPosition.DgppId = null;
                orderPosition.Comment = null;

                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, OrderPosition>())
                {
                    _orderPositionGenericRepository.Add(orderPosition);
                    _orderPositionGenericRepository.Save();

                    scope.Added<OrderPosition>(orderPosition.Id)
                         .Complete();
                }

                foreach (var advertisement in orderPositionDto.Advertisements)
                {
                    // Превращаем полученную из БД сущность в новую и связываем её с новой позицией заказа
                    advertisement.ResetToNew();
                    _identityProvider.SetFor(advertisement);

                    advertisement.OrderPositionId = orderPositionDto.OrderPosition.Id;

                    using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, OrderPositionAdvertisement>())
                    {
                        _orderPositionAdvertisementGenericRepository.Add(advertisement);
                        _orderPositionAdvertisementGenericRepository.Save();

                        scope.Added<OrderPositionAdvertisement>(advertisement.Id)
                             .Complete();
                    }
                }
            }

            return order;
        }

        public void SetInspector(long orderId, long? inspectorId)
        {
            using (var operationScope = _scopeFactory.CreateNonCoupled<SetInspectorIdentity>())
            {
                var order = _secureFinder.Find(Specs.Find.ById<Order>(orderId)).Single();
                order.InspectorCode = inspectorId;

                Update(order);

                operationScope
                    .Updated<Order>(orderId)
                    .Complete();
            }
        }

        public int Update(Order order)
        {
            CheckOrderApprovalDateSpecified(order);
            CheckOrderPlatformSpecified(order);
            CheckOrderDistributionPeriodNotOverlapsThemeDistributionPeriod(order);
            CheckOrderLegalPersonProfileBelongsToOrderLegalPerson(order);


            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(order))
            {
                _orderGenericRepository.Update(order);
                var cnt = _orderGenericRepository.Save();

                scope.Updated<Order>(order.Id)
                     .Complete();
                return cnt;
            }
        }

        public int Assign(Order order, long ownerCode)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<AssignIdentity, Order>())
            {
                var orderPositions = _finder.Find<OrderPosition>(x => x.OrderId == order.Id && !x.IsDeleted && x.IsActive).ToArray();

                foreach (var orderPosition in orderPositions)
                {
                    orderPosition.OwnerCode = ownerCode;
                    _orderPositionGenericRepository.Update(orderPosition);
                    scope.Updated<OrderPosition>(orderPosition.Id);
                }

                _orderPositionGenericRepository.Save();

                order.OwnerCode = ownerCode;

                CheckOrderApprovalDateSpecified(order);
                CheckOrderPlatformSpecified(order);
                _orderSecureGenericRepository.Update(order);
                var count = _orderSecureGenericRepository.Save();

                scope
                    .Updated<Order>(order.Id)
                    .Complete();

                return count;
            }
        }

        public int Delete(OrderPosition entity)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, OrderPosition>())
            {
                _orderPositionGenericRepository.Delete(entity);
                scope.Deleted<OrderPosition>(entity.Id)
                     .Complete();
            }

            return _orderPositionGenericRepository.Save();
        }

        public int Delete(Bill entity)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Bill>())
            {
                _billGenericRepository.Delete(entity);
                var cnt = _billGenericRepository.Save();

                scope.Deleted<Bill>(entity.Id)
                     .Complete();

                return cnt;
            }
        }

        public int Delete(IEnumerable<OrderPositionAdvertisement> advertisements)
        {
            int cnt = 0;
            foreach (var advertisement in advertisements)
            {
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, OrderPositionAdvertisement>())
                {
                    _orderPositionAdvertisementGenericRepository.Delete(advertisement);
                    cnt += _orderPositionAdvertisementGenericRepository.Save();

                    scope.Deleted<OrderPositionAdvertisement>(advertisement.Id)
                         .Complete();
                }
            }

            return cnt;
        }

        public long[] DeleteOrderReleaseTotalsForOrder(long orderId)
        {
            var orderReleaseTotals = _finder.Find(Specs.Find.ById<Order>(orderId))
                                            .SelectMany(order => order.OrderReleaseTotals)
                                            .ToArray();

            foreach (var orderReleaseTotal in orderReleaseTotals)
            {
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, OrderReleaseTotal>())
                {
                    _orderReleaseTotalGenericRepository.Delete(orderReleaseTotal);
                    _orderReleaseTotalGenericRepository.Save();

                    scope.Deleted<OrderReleaseTotal>(orderReleaseTotal.Id)
                         .Complete();
                }
            }

            return orderReleaseTotals.Select(total => total.Id).ToArray();
        }

        /// <summary>
        /// Обновляет в объекте заказа поля Number, RegionalNumber. После обновления нужно отдельно вызвать <see><cref>Update</cref></see>.
        /// </summary>
        public void UpdateOrderNumber(Order order)
        {
            var numbers = UpdateOrderNumber(order.Number, order.RegionalNumber, order.PlatformId);
            order.Number = numbers.Number;
            order.RegionalNumber = numbers.RegionalNumber;
        }


        public int SetOrderState(Order order, OrderState orderState)
        {
            order.WorkflowStepId = orderState;

            CheckOrderApprovalDateSpecified(order);
            CheckOrderPlatformSpecified(order);

            // TODO {all, 09.09.2013}: SetOrderStateIdentity
            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(order))
            {
                _orderGenericRepository.Update(order);
                var cnt = _orderGenericRepository.Save();

                scope.Updated<Order>(order.Id)
                     .Complete();
                return cnt;
            }
        }

        public void CloseOrder(Order order, string reason)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Order>())
            {
                order.Comment = reason;
                order.IsActive = false;

                // Удалить позиции
                var orderPositions = _finder.Find<OrderPosition>(x => x.OrderId == order.Id && !x.IsDeleted).ToArray();
                foreach (var orderPosition in orderPositions)
                {
                    _orderPositionGenericRepository.Delete(orderPosition);
                }

                _orderPositionGenericRepository.Save();
                operationScope.Deleted<OrderPosition>(orderPositions.Select(x => x.Id).ToArray());

                // Деактивировать счета на оплату
                var bills = _finder.Find<Bill>(x => x.OrderId == order.Id && x.IsActive && !x.IsDeleted).ToArray();
                foreach (var bill in bills)
                {
                    bill.IsActive = false;
                    _billGenericRepository.Update(bill);
                }

                _billGenericRepository.Save();
                operationScope.Updated<Bill>(bills.Select(x => x.Id).ToArray());

                // Деактивировать файлы к заказу
                var orderFiles = _finder.Find<OrderFile>(x => x.OrderId == order.Id && x.IsActive && !x.IsDeleted).ToArray();
                foreach (var orderFile in orderFiles)
                {
                    orderFile.IsActive = false;
                    _orderFileGenericRepository.Update(orderFile);
                }

                _orderFileGenericRepository.Save();
                operationScope.Updated<OrderFile>(orderFiles.Select(x => x.Id).ToArray());

                CheckOrderApprovalDateSpecified(order);
                CheckOrderPlatformSpecified(order);
                _orderGenericRepository.Update(order);
                _orderGenericRepository.Save();
                operationScope.Updated<Order>(order.Id);

                operationScope.Complete();
            }
        }

        public void ChangeOrderPositionBindingObjects(long orderPositionId, IEnumerable<AdvertisementDescriptor> advertisements)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var orderPositionAdvertisements = _finder.Find<OrderPositionAdvertisement>(x => x.OrderPositionId == orderPositionId).ToArray();
                Delete(orderPositionAdvertisements);

                var advertisementsToCreate = advertisements
                    .Select(dto => new OrderPositionAdvertisement
                        {
                            OrderPositionId = orderPositionId,
                            AdvertisementId = dto.AdvertisementId,
                            CategoryId = dto.CategoryId,
                            FirmAddressId = dto.FirmAddressId,
                            PositionId = dto.PositionId
                        });

                foreach (var advertisement in advertisementsToCreate)
                {
                    using (var scope = _scopeFactory.CreateOrUpdateOperationFor(advertisement))
                    {
                        _orderPositionAdvertisementGenericRepository.Add(advertisement);
                        _orderPositionAdvertisementGenericRepository.Save();
                        scope.Added<OrderPositionAdvertisement>(advertisement.Id)
                             .Complete();
                    }
                }

                transaction.Complete();
            }
        }

        public void CreateOrUpdateOrderPositionAdvertisements(long orderPositionId, AdvertisementDescriptor[] newAdvertisementsLinks, bool orderIsLocked)
        {
            var oldAdvertisementsLinks = _finder.Find<OrderPosition>(x => x.Id == orderPositionId).SelectMany(x => x.OrderPositionAdvertisements).ToArray();
            ValidateOrderPositionAdvertisementsInLockedOrder(oldAdvertisementsLinks, newAdvertisementsLinks, orderIsLocked);

            // повторяю прежнюю логику. По-хорошему все ошибки можно показать в окошечке. Сейчас этого не делаем, т.к.надо тестировать и релизить.
            var firstError = _validateOrderPositionAdvertisementsService.Validate(orderPositionId, newAdvertisementsLinks).FirstOrDefault();
            if (firstError != null)
            {
                throw new BusinessLogicException(firstError.ErrorMessage);
            }

            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity, OrderPositionAdvertisement>())
            {
                var deletedOrderPositionAdvertisements = new List<OrderPositionAdvertisement>();
                var insertedOrderPositionAdvertisements = new List<OrderPositionAdvertisement>();
                foreach (var oldAdvertisementsLink in oldAdvertisementsLinks)
                {
                    _orderPositionAdvertisementGenericRepository.Delete(oldAdvertisementsLink);
                    deletedOrderPositionAdvertisements.Add(oldAdvertisementsLink);
                }

                var orderPositionAdvertisements = newAdvertisementsLinks.Select(x => new OrderPositionAdvertisement
                    {
                        OrderPositionId = orderPositionId,
                        PositionId = x.PositionId,
                        AdvertisementId = x.AdvertisementId,
                        FirmAddressId = x.FirmAddressId,
                        CategoryId = x.CategoryId,
                        ThemeId = x.ThemeId,
                    }).ToArray();

                _identityProvider.SetFor(orderPositionAdvertisements);

                foreach (var orderPositionAdvertisement in orderPositionAdvertisements)
                {
                    _orderPositionAdvertisementGenericRepository.Add(orderPositionAdvertisement);
                    insertedOrderPositionAdvertisements.Add(orderPositionAdvertisement);
                }

                _orderPositionAdvertisementGenericRepository.Save();
                operationScope
                    .Deleted<OrderPositionAdvertisement>(deletedOrderPositionAdvertisements.Select(x => x.Id).ToArray())
                    .Added<OrderPositionAdvertisement>(insertedOrderPositionAdvertisements.Select(x => x.Id).ToArray())
                    .Complete();
            }
        }

        public void CreateOrderReleaseTotals(IEnumerable<OrderReleaseTotal> orderReleaseTotals)
        {
            // OrderReleaseTotal в числе тех сущностей, что не имеют Timestamp и не предназначены для редактирования: поддерживают только операции создания, чтения и удаления
            foreach (var orderReleaseTotal in orderReleaseTotals)
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, OrderReleaseTotal>())
                {
                    _identityProvider.SetFor(orderReleaseTotal);
                    _orderReleaseTotalGenericRepository.Add(orderReleaseTotal);
                    _orderReleaseTotalGenericRepository.Save();

                    scope.Added<OrderReleaseTotal>(orderReleaseTotal.Id)
                         .Complete();
                }
            }
        }

        int IAssignAggregateRepository<Order>.Assign(long entityId, long ownerCode)
        {
            var entity = _finder.Find(Specs.Find.ById<Order>(entityId)).Single();
            return Assign(entity, ownerCode);
        }

        int IDeleteAggregateRepository<OrderPosition>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<OrderPosition>(entityId)).Single();
            return Delete(entity);
        }

        StreamResponse IDownloadFileAggregateRepository<OrderFile>.DownloadFile(DownloadFileParams<OrderFile> downloadFileParams)
        {
            var file = _fileContentFinder.Find(Specs.Find.ById<FileWithContent>(downloadFileParams.FileId)).Single();
            return new StreamResponse { FileName = file.FileName, ContentType = file.ContentType, Stream = file.Content };
        }

        UploadFileResult IUploadFileAggregateRepository<OrderFile>.UploadFile(UploadFileParams<OrderFile> uploadFileParams)
        {
            if (uploadFileParams.Content != null && uploadFileParams.Content.Length > 10485760)
            {
                throw new BusinessLogicException(BLResources.FileSizeMustBeLessThan10MB);
            }

            var file = new FileWithContent
            {
                Id = uploadFileParams.FileId,
                ContentType = uploadFileParams.ContentType,
                ContentLength = uploadFileParams.ContentLength,
                Content = uploadFileParams.Content,
                FileName = Path.GetFileName(uploadFileParams.FileName)
            };

            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(file))
            {
                if (file.IsNew())
                {
                    _identityProvider.SetFor(file);
                    _fileRepository.Add(file);
                    operationScope.Added<FileWithContent>(file.Id);
                }
                else
                {
                    _fileRepository.Update(file);
                    operationScope.Updated<FileWithContent>(file.Id);
                }

                var orderFile = _finder.Find(Specs.Find.ByFileId<OrderFile>(uploadFileParams.FileId)).FirstOrDefault();
                if (orderFile != null)
                {
                    orderFile.ModifiedOn = DateTime.UtcNow;
                    orderFile.ModifiedBy = _userContext.Identity.Code;

                    _orderFileGenericRepository.Update(orderFile);
                    _orderFileGenericRepository.Save();
                    operationScope.Updated<OrderFile>(orderFile.Id);
                }

                operationScope.Complete();

                return new UploadFileResult
                    {
                        ContentType = file.ContentType,
                        ContentLength = file.ContentLength,
                        FileName = file.FileName,
                        FileId = file.Id
                    };
            }
        }

        int IDeleteAggregateRepository<Bill>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<Bill>(entityId)).Single();
            return Delete(entity);
        }

        private static void CheckOrderApprovalDateSpecified(Order order)
        {
            var state = order.WorkflowStepId;
            switch (state)
            {
                case OrderState.OnTermination:
                case OrderState.Approved:
                case OrderState.Archive:
                    if (!order.ApprovalDate.HasValue)
                    {
                        var message = string.Format(BLResources.ApprovalDateMustBeSpecified,
                            state.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                        throw new ArgumentException(message);
                    }

                    break;
            }
        }

        private static void CheckOrderPlatformSpecified(Order order)
        {
            if (order.WorkflowStepId != OrderState.OnRegistration && !order.PlatformId.HasValue)
            {
                throw new ArgumentException(BLResources.PlatformMustBeSpecified, "order");
            }
        }

        // ошибка если как-то смогли изменить позиции у заблокированного заказа
        private static void ValidateOrderPositionAdvertisementsInLockedOrder(OrderPositionAdvertisement[] oldAdvertisementsLinks,
                                                                             AdvertisementDescriptor[] newAdvertisementsLinks,
                                                                             bool orderIsLocked)
        {
            if (!orderIsLocked)
            {
                return;
            }

            bool throwError;

            if (newAdvertisementsLinks.Length != oldAdvertisementsLinks.Length)
            {
                throwError = true;
            }
            else
            {
                // поэлементная сортировка 
                oldAdvertisementsLinks = oldAdvertisementsLinks
                    .OrderBy(x => x.PositionId)
                    .ThenBy(x => x.FirmAddressId)
                    .ThenBy(x => x.CategoryId)
                    .ToArray();

                newAdvertisementsLinks = newAdvertisementsLinks
                    .OrderBy(x => x.PositionId)
                    .ThenBy(x => x.FirmAddressId)
                    .ThenBy(x => x.CategoryId)
                    .ToArray();

                throwError = false;

                for (var i = 0; i < newAdvertisementsLinks.Length; i++)
                {
                    var newAdvertisementsLink = newAdvertisementsLinks[i];
                    var oldAdvertisementsLink = oldAdvertisementsLinks[i];

                    if (newAdvertisementsLink.PositionId != oldAdvertisementsLink.PositionId ||
                        newAdvertisementsLink.FirmAddressId != oldAdvertisementsLink.FirmAddressId ||
                        newAdvertisementsLink.CategoryId != oldAdvertisementsLink.CategoryId)
                    {
                        throwError = true;
                        break;
                    }
                }
            }

            if (throwError)
            {
                throw new NotificationException(BLResources.ChangingAdvertisementLinksIsDeniedWhileOrderIsLocked);
            }
        }

        private void CheckOrderDistributionPeriodNotOverlapsThemeDistributionPeriod(Order order)
        {
            var usedThemes = _finder.Find<OrderPosition>(position => position.OrderId == order.Id)
                                    .Where(position => position.IsActive && !position.IsDeleted)
                                    .SelectMany(position => position.OrderPositionAdvertisements)
                                    .Where(advertisement => advertisement.ThemeId != null)
                                    .Select(advertisement => new { advertisement.Theme.BeginDistribution, advertisement.Theme.EndDistribution })
                                    .ToArray();

            if (!usedThemes.Any())
            {
                return;
            }

            var allowedBeginDistibutionDate = usedThemes.Select(arg => arg.BeginDistribution).Max();
            var allowedEndDistibutionDate = usedThemes.Select(arg => arg.EndDistribution).Min();

            if (order.BeginDistributionDate < allowedBeginDistibutionDate)
            {
                var message = string.Format(BLResources.OrderBeginDistibutionDateIsTooSmall, allowedBeginDistibutionDate.ToShortDateString());
                throw new BusinessLogicException(message);
            }

            if (order.EndDistributionDateFact > allowedEndDistibutionDate)
            {
                var message = string.Format(BLResources.OrderBeginDistibutionDateIsTooLarge, allowedEndDistibutionDate.ToShortDateString());
                throw new BusinessLogicException(message);
            }
        }

        private void CheckOrderLegalPersonProfileBelongsToOrderLegalPerson(Order order)
        {
            if (order.LegalPersonProfileId == null)
            {
                return;
            }

            var legalPersonId = _finder.Find<LegalPersonProfile>(x => x.Id == order.LegalPersonProfileId).Select(x => x.LegalPersonId).Single();
            if (order.LegalPersonId != legalPersonId)
            {
                throw new BusinessLogicException(BLResources.OrderLegalPersonProfileShouldBelongToOrderLegalPerson);
            }
        }

        private OrderNumberDto UpdateOrderNumber(string orderNumber, string orderRegionalNumber, long? orderPlatformId)
        {
            const string mobilePostfix = "-Mobile";
            const string apiPostfix = "-API";
            const string onlinePostfix = "-Online";

            var orderNumberRegex = new Regex(@"-[a-zA-Z]+", RegexOptions.Singleline | RegexOptions.Compiled);
            var numberMatch = orderNumberRegex.Match(orderNumber);
            if (!string.IsNullOrEmpty(orderRegionalNumber))
            {
                // Если один из номеров удовлетворяет формату, а второй задан и не удовлетворяет - это ошибка
                var regionalNumberMatch = orderNumberRegex.Match(orderRegionalNumber);
                var isNumbersFormatMatch = (numberMatch.Success && regionalNumberMatch.Success) || (!numberMatch.Success && !regionalNumberMatch.Success);
                if (!isNumbersFormatMatch)
                {
                    throw new ArgumentException(BLResources.OrderNumberAndRegionalNumberFormatsDoesNotMatch);
                }
            }

            // todo {all, 2013-07-24}: Если в рамках задачи ERM-104 свершится отказ от колонки DgppId, здесь не потребуется выборка
            //                         Кроме того, этот метод перестанет контактировать с хранилищем данных и его можно будет убрать из репозитория
            var orderPlatformType = orderPlatformId.HasValue
                                        ? (PlatformEnum?)_finder.Find(Specs.Find.ById<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform>(orderPlatformId.Value)).Single().DgppId
                                        : null;

            // Имеем схему вариантов (есть/нет суффикс платформы, есть/нет платформа):
            OrderNumberStates orderState = 0;
            orderState |= orderPlatformId.HasValue ? OrderNumberStates.HasPlatform : (OrderNumberStates)0;
            orderState |= numberMatch.Success ? OrderNumberStates.HasSuffix : (OrderNumberStates)0;

            switch (orderState)
            {
                case OrderNumberStates.HasSuffixHasPlatform:
                    // Если постфикс для платформы задан - обновляем
                    switch (orderPlatformType)
                    {
                        case PlatformEnum.Mobile:
                            return new OrderNumberDto
                            {
                                Number = orderNumberRegex.Replace(orderNumber, mobilePostfix),
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderNumberRegex.Replace(orderRegionalNumber, mobilePostfix)
                            };

                        case PlatformEnum.Api:
                            return new OrderNumberDto
                            {
                                Number = orderNumberRegex.Replace(orderNumber, apiPostfix),
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderNumberRegex.Replace(orderRegionalNumber, apiPostfix)
                            };
                        case PlatformEnum.Online:
                            return new OrderNumberDto
                            {
                                Number = orderNumberRegex.Replace(orderNumber, onlinePostfix),
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderNumberRegex.Replace(orderRegionalNumber, onlinePostfix)
                            };
                        default:
                            return new OrderNumberDto
                            {
                                Number = orderNumberRegex.Replace(orderNumber, string.Empty),
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderNumberRegex.Replace(orderRegionalNumber, string.Empty)
                            };
                    }

                case OrderNumberStates.NoSuffixHasPlatform:
                    // Если постфикс для платформы не задан - добавляем
                    switch (orderPlatformType)
                    {
                        case PlatformEnum.Mobile:
                            return new OrderNumberDto
                            {
                                Number = orderNumber + mobilePostfix,
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderRegionalNumber + mobilePostfix
                            };
                        case PlatformEnum.Api:
                            return new OrderNumberDto
                            {
                                Number = orderNumber + apiPostfix,
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderRegionalNumber + apiPostfix
                            };
                        case PlatformEnum.Online:
                            return new OrderNumberDto
                            {
                                Number = orderNumber + onlinePostfix,
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderRegionalNumber + onlinePostfix
                            };
                        default:
                            return new OrderNumberDto
                            {
                                Number = orderNumber,
                                RegionalNumber = orderRegionalNumber
                            };
                    }

                case OrderNumberStates.HasSuffixNoPlatform:
                    // Если постфикс для платформы задан - убираем его
                    return new OrderNumberDto
                    {
                        Number = orderNumberRegex.Replace(orderNumber, string.Empty),
                        RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                             ? null
                                             : orderNumberRegex.Replace(orderRegionalNumber, string.Empty)
                    };

                case OrderNumberStates.NoSuffixNoPlatform:
                default:
                    return new OrderNumberDto
                    {
                        Number = orderNumber,
                        RegionalNumber = orderRegionalNumber
                    };
            }
        }
    }
}
