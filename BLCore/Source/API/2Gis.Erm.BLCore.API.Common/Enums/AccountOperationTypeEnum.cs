namespace DoubleGis.Erm.BLCore.API.Common.Enums
{
    /// <summary>
    /// Выдержка из таблицы OperationType - операций по лицевому счету
    /// </summary>
    public enum AccountOperationTypeEnum
    {
        /// <summary>
        /// Поступление на расчетный счет (Платежное поручение входящее)_IsPlus
        /// </summary>
        AccountChargeIsPlus = 1,

        /// <summary>
        /// Возврат с расчетного счета (Платежное поручение исходящее)
        /// </summary>
        AccountDebit = 2,

        /// <summary>
        /// Поступление в кассу (Приходный кассовый ордер)_IsPlus
        /// </summary>
        CashIncomeIsPlus = 3,

        /// <summary>
        /// Возврат из кассы (Расходный кассовый ордер)
        /// </summary>
        CashReturn = 4,

        /// <summary>
        /// Перенос средств между юрлицами клиента (Распоряжение клиента)
        /// </summary>
        TransfersBetweenLegalPersons = 5,

        /// <summary>
        /// Перенос средств между юрлицами клиента (Распоряжение клиента)_IsPlus
        /// </summary>
        TransfersBetweenLegalPersonsIsPlus = 6,

        /// <summary>
        /// Списание в счет оплаты БЗ
        /// </summary>
        WithdrawalForOrderPayment = 7,

        /// <summary>
        /// Списание дебиторской задолженности_IsPlus
        /// </summary>
        WithdrawalReceivablesIsPlus = 8,

        /// <summary>
        /// Зачет встречных требований (Акт зачета)_IsPlus
        /// </summary>
        SetOffCounterClaimsIsPlus = 9,

        /// <summary>
        /// Отмена зачета встречных требований (Расторжение Акта зачета)
        /// </summary>
        CancelSetOffCounterClaim = 10,

        /// <summary>
        /// Зачет невыясненного платежа (Распоряжение клиента)_IsPlus
        /// </summary>
        PassedUnexplainedPaymentIsPlus = 11,

        /// <summary>
        /// Отмена зачета невыясненного платежа (Распоряжение клиента)
        /// </summary>
        CancelPassedUnexplainedPayment = 12,

        /// <summary>
        /// Инициализация баланса_IsPlus
        /// </summary>
        BalanceInitializationIsPlus = 13,

        /// <summary>
        /// Инициализация баланса минус
        /// </summary>
        BalanceInitialization = 14,

        /// <summary>
        /// Списание дебиторской задолженности
        /// </summary>
        WithdrawalReceivables = 15,

        /// <summary>
        /// Перенос средств между собственными юрлицами
        /// </summary>
        TransfersBetweenBranchOffices = 16,

        /// <summary>
        /// Перенос средств между собственными юрлицами_IsPlus
        /// </summary>
        TransfersBetweenBranchOfficesIsPlus = 17,
    }
}
