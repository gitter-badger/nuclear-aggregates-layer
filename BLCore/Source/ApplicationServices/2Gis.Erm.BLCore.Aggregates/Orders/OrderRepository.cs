using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
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
using DoubleGis.Erm.Platform.Model.Entities;
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
        private readonly IRepository<FileWithContent> _fileRepository;
        private readonly IUserContext _userContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public OrderRepository(IFinder finder,
                               ISecureFinder secureFinder,
                               IFileContentFinder fileContentFinder,
                               IRepository<Order> orderGenericRepository,
                               IRepository<OrderPosition> orderPositionEntityRepository,
                               IRepository<OrderPositionAdvertisement> orderPositionAdvertisementEntityRepository,
                               IRepository<Bill> billGenericRepository,
                               IRepository<OrderFile> orderFileGenericRepository,
                               IRepository<FileWithContent> fileRepository,
                               IUserContext userContext,
                               IOrderPersistenceService orderPersistenceService,
                               ISecureRepository<Order> orderSecureGenericRepository,
                               IIdentityProvider identityProvider,
                               IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _secureFinder = secureFinder;
            _orderGenericRepository = orderGenericRepository;
            _orderPositionGenericRepository = orderPositionEntityRepository;
            _orderPositionAdvertisementGenericRepository = orderPositionAdvertisementEntityRepository;
            _billGenericRepository = billGenericRepository;
            _orderFileGenericRepository = orderFileGenericRepository;
            _userContext = userContext;
            _orderPersistenceService = orderPersistenceService;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
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
    }
}
