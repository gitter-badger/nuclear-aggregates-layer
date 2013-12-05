using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.Entities.Enums.Infrastructure
{
    public static class PropertiesToEnumMapper
    {
        public static readonly IReadOnlyDictionary<string, Type> Map = new Dictionary<string, Type>
            {
                { CreateKey<AdvertisementElement>(entity => entity.FasCommentType), typeof(FasComment?) },
                { CreateKey<AdvertisementElement>(entity => entity.Status), typeof(AdvertisementElementStatus) },
                { CreateKey<AdvertisementElement>(entity => entity.Error), typeof(AdvertisementElementError) },
                { CreateKey<AdvertisementElementTemplate>(entity => entity.RestrictionType), typeof(AdvertisementElementRestrictionType) },
                { CreateKey<AssociatedPosition>(entity => entity.ObjectBindingType), typeof(ObjectBindingType) },
                { CreateKey<Bargain>(entity => entity.HasDocumentsDebt), typeof(DocumentsDebt) }, 
                { CreateKey<BargainFile>(entity => entity.FileKind), typeof(BargainFileKind) }, 
                { CreateKey<Client>(entity => entity.InformationSource), typeof(InformationSource) }, 
                { CreateKey<Contact>(entity => entity.GenderCode), typeof(Gender) }, 
                { CreateKey<Contact>(entity => entity.FamilyStatusCode), typeof(FamilyStatus) },
                { CreateKey<Contact>(entity => entity.AccountRole), typeof(AccountRole) },
                { CreateKey<Deal>(entity => entity.DealStage), typeof(DealStage) },
                { CreateKey<Deal>(entity => entity.StartReason), typeof(ReasonForNewDeal) },
                { CreateKey<Deal>(entity => entity.CloseReason), typeof(CloseDealReason) },
                { CreateKey<DeniedPosition>(entity => entity.ObjectBindingType), typeof(ObjectBindingType) },
                { CreateKey<Firm>(entity => entity.UsingOtherMedia), typeof(UsingOtherMediaOption) },
                { CreateKey<Firm>(entity => entity.MarketType), typeof(MarketType) },
                { CreateKey<Firm>(entity => entity.ProductType), typeof(ProductType) },
                { CreateKey<Firm>(entity => entity.BudgetType), typeof(BudgetType) },
                { CreateKey<Firm>(entity => entity.Geolocation), typeof(Geolocation) },
                { CreateKey<Firm>(entity => entity.InCityBranchesAmount), typeof(InCityBranchesAmount) },
                { CreateKey<Firm>(entity => entity.OutCityBranchesAmount), typeof(OutCityBranchesAmount) },
                { CreateKey<Firm>(entity => entity.StaffAmount), typeof(StaffAmount) },
                { CreateKey<FirmContact>(entity => entity.ContactType), typeof(FirmAddressContactType) },
                { CreateKey<LegalPerson>(entity => entity.LegalPersonTypeEnum), typeof(LegalPersonType) },
                { CreateKey<LegalPersonProfile>(entity => entity.OperatesOnTheBasisInGenitive), typeof(OperatesOnTheBasisType) },
                { CreateKey<LegalPersonProfile>(entity => entity.DocumentsDeliveryMethod), typeof(DocumentsDeliveryMethod) },
                { CreateKey<Limit>(entity => entity.Status), typeof(LimitStatus) },
                { CreateKey<LocalMessage>(entity => entity.Status), typeof(LocalMessageStatus) },
                { CreateKey<Operation>(entity => entity.Status), typeof(OperationStatus) },
                { CreateKey<Operation>(entity => entity.Type), typeof(BusinessOperation) },
                { CreateKey<Order>(entity => entity.WorkflowStepId), typeof(OrderState) },
                { CreateKey<Order>(entity => entity.DiscountReasonEnum), typeof(OrderDiscountReason) },
                { CreateKey<Order>(entity => entity.TerminationReason), typeof(OrderTerminationReason) },
                { CreateKey<Order>(entity => entity.OrderType), typeof(OrderType) },
                { CreateKey<Order>(entity => entity.BudgetType), typeof(OrderBudgetType) },
                { CreateKey<Order>(entity => entity.HasDocumentsDebt), typeof(DocumentsDebt) },
                { CreateKey<OrderFile>(entity => entity.FileKind), typeof(OrderFileKind) },
                { CreateKey<Platform.Model.Entities.Erm.Platform>(entity => entity.PlacementPeriodEnum), typeof(PositionPlatformPlacementPeriod) },
                { CreateKey<Platform.Model.Entities.Erm.Platform>(entity => entity.MinPlacementPeriodEnum), typeof(PositionPlatformMinPlacementPeriod) },
                { CreateKey<Position>(entity => entity.CalculationMethodEnum), typeof(PositionCalculationMethod) },
                { CreateKey<Position>(entity => entity.AccountingMethodEnum), typeof(PositionAccountingMethod) },
                { CreateKey<Position>(entity => entity.BindingObjectTypeEnum), typeof(PositionBindingObjectType) },
                { CreateKey<PricePosition>(entity => entity.AmountSpecificationMode), typeof(PricePositionAmountSpecificationMode) },
                { CreateKey<PrintFormTemplate>(entity => entity.TemplateCode), typeof(TemplateCode) },
                { CreateKey<ReleaseInfo>(entity => entity.Status), typeof(ReleaseStatus) },
                { CreateKey<WithdrawalInfo>(entity => entity.Status), typeof(WithdrawalStatus) },
                { CreateKey<LegalPersonProfile>(entity => entity.PaymentMethod), typeof(PaymentMethod) },
                { CreateKey<Order>(entity => entity.PaymentMethod), typeof(PaymentMethod) },
                { CreateKey<OrderProcessingRequest>(entity => entity.State), typeof(OrderProcessingRequestState) },
            };

        private static string CreateKey<TEntity>(Expression<Func<TEntity, object>> propertyExpression)
        {
            var propertyName = GetMemberName(propertyExpression);
            var declaringType = GetMemberDeclaringType(propertyExpression);
            if (declaringType.IsEnum)
            {
                throw new InvalidOperationException("Неверно указан тип сущности");
            }

            return string.Format("{0}.{1}", declaringType.Name, propertyName);
        }

        #region static reflection

        private static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentException("The expression cannot be null.");
            }

            return GetMemberName(expression.Body);
        }

        private static Type GetMemberDeclaringType<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentException("The expression cannot be null.");
            }

            return GetMemberDeclaringType(expression.Body);
        }

        private static Type GetMemberDeclaringType(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException("The expression cannot be null.");
            }

            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                // Reference type property or field
                return memberExpression.Member.DeclaringType;
            }

            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return methodCallExpression.Method.DeclaringType;
            }

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                // Property, field of method returning value type
                return GetMemberDeclaringType(unaryExpression);
            }

            throw new ArgumentException("Invalid expression");
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException("The expression cannot be null.");
            }

            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression = (MemberExpression)expression;
                return memberExpression.Member.Name;
            }

            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression = (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }

            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetMemberName(unaryExpression);
            }

            throw new ArgumentException("Invalid expression");
        }

        private static string GetMemberName(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression = (MethodCallExpression)unaryExpression.Operand;
                return methodExpression.Method.Name;
            }

            return ((MemberExpression)unaryExpression.Operand).Member.Name;
        }

        private static Type GetMemberDeclaringType(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression = (MethodCallExpression)unaryExpression.Operand;
                return methodExpression.Method.DeclaringType;
            }

            return ((MemberExpression)unaryExpression.Operand).Member.DeclaringType;
        }
        #endregion
    }
}
