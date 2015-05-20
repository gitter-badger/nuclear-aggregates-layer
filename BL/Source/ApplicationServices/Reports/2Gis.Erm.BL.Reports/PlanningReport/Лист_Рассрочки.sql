--DECLARE
--	@IssueDate date = '20140701'
--	, @City int = 115
--	, @IsAdvertisingAgency int = 0

SELECT
	[Куратор] = u.DisplayName
	, [Юр.лицо заказчика] = lp.LegalName
	, [Юр.лицо исполнителя] = bou.ShortLegalName
	, [Клиент] = c.Name
	, [Планируемый объем оплат] = bills.PayPlan
	, [Прогнозируемый объем оплат (кроме ДЗ до 01.12.2014)] = CASE WHEN bills.PayPlan - (ISNULL(payments.Balance, 0) + ISNULL(withdraw.Amount, 0) - bills.PayPast - ISNULL(locks2.Amount, 0)) > 0 then bills.PayPlan - (ISNULL(payments.Balance, 0) + ISNULL(withdraw.Amount, 0) - bills.PayPast - ISNULL(locks2.Amount, 0)) ELSE 0 END
	, [Баланс клиента на начало прогнозируемого периода] = ISNULL(payments.Balance2, 0) - ISNULL(locks.Amount, 0)
	, [Корректировка РГ] = NULL
	, [Примечание] = NULL
FROM Billing.Accounts a with(nolock)
JOIN (	SELECT t.AccountId
				, [PayPlan] = SUM(CASE WHEN t.IsTerminated = 0 THEN t.PayPlan WHEN t.OrderPayableFact - t.PayPast <= 0 THEN 0 WHEN t.OrderPayableFact - t.PayPast >= t.PayPlan THEN t.PayPlan ELSE t.OrderPayableFact - t.PayPast END)
				, [PayPast] = SUM(CASE WHEN t.IsTerminated = 0 THEN t.PayPast ELSE t.OrderPayableFact END)
				, [IsPlaced] = MAX(t.IsPlaced)
		FROM(	SELECT o.AccountId
						, [OrderId] = o.Id
						, [PayPlan] = ISNULL(SUM(CASE WHEN b.PaymentDatePlan >= @IssueDate THEN b.PayablePlan ELSE 0 END), 0)
						, [PayPast] = ISNULL(SUM(CASE WHEN b.PaymentDatePlan < @IssueDate THEN b.PayablePlan ELSE 0 END), 0)
						, [IsTerminated] = CASE WHEN MAX(o.EndDistributionDatePlan) = MAX(o.EndDistributionDateFact) THEN 0 ELSE 1 END
						, [OrderPayableFact] = MAX(CASE WHEN o.ReleaseCountPlan = 0 THEN 0 ELSE o.PayablePlan/o.ReleaseCountPlan*o.ReleaseCountFact END)
						, [IsPlaced] = MAX(CASE WHEN @IssueDate BETWEEN o.BeginDistributionDate AND o.EndDistributionDateFact THEN 1 ELSE 0 END)
				FROM Billing.Orders o with(nolock)
				JOIN Billing.BranchOfficeOrganizationUnits AS boou WITH (NOLOCK) ON boou.Id = o.BranchOfficeOrganizationUnitId
				LEFT JOIN Billing.Bills b with(nolock) ON b.OrderId = o.Id AND b.PaymentDatePlan < DATEADD(m, 1, @IssueDate)	AND b.IsDeleted = 0
		     	WHERE boou.OrganizationUnitId = @City
					AND o.WorkflowStepId NOT IN (1,2)
					AND o.IsDeleted = 0
					AND o.IsActive = 1
					AND o.EndDistributionDateFact > @IssueDate
					AND o.PayablePlan > 0
					and o.BeginDistributionDate < dateadd(m,1,@IssueDate) -- отсекаем будующие заказы
				GROUP BY o.Id, o.AccountId) t
		GROUP BY t.AccountId) bills ON a.Id = bills.AccountId AND bills.IsPlaced = 1
LEFT JOIN(	SELECT ad.AccountId
					, [Balance] = SUM(CASE WHEN ad.TransactionDate < @IssueDate THEN ABS(ad.Amount)*(2*ot.IsPlus-1) ELSE 0 END) - SUM(CASE WHEN ad.TransactionDate between @IssueDate and dateadd(dd, 7 ,  @IssueDate) AND ot.Id = 7 THEN ad.Amount ELSE 0 END)--баланс, считаемый как сумма операций до начала прогнозируемого месяца
					, [Balance2] = SUM( CASE WHEN ot.Id = 7 AND l.PeriodStartDate <= @IssueDate THEN ABS(ad.Amount)*(2*ot.IsPlus-1) WHEN ot.Id <> 7 AND ad.TransactionDate < @IssueDate THEN ABS(ad.Amount)*(2*ot.IsPlus-1) ELSE 0 END) --баланс, считаемый как сумма операций до начала прогнозируемого месяца плюс списания за прогнозируемый месяц
			FROM Billing.Accounts a with(nolock)
			JOIN Billing.AccountDetails ad with(nolock) ON ad.AccountId = a.Id AND ad.IsDeleted = 0
			JOIN Billing.OperationTypes ot with(nolock) ON ot.Id = ad.OperationTypeId
			LEFT JOIN Billing.Locks l with(nolock) ON l.DebitAccountDetailId = ad.Id AND l.IsDeleted = 0
			WHERE a.IsDeleted = 0
			GROUP BY ad.AccountId) payments ON a.Id = payments.AccountId
LEFT JOIN(	SELECT l.AccountId
					, [Amount] = SUM(l.PlannedAmount)
			FROM Billing.Locks l with(nolock)
			WHERE l.PeriodStartDate <= DATEADD(m, 1, @IssueDate)
				AND l.IsDeleted = 0
				AND l.IsActive = 1
		GROUP BY l.AccountId) locks ON a.Id = locks.AccountId
LEFT JOIN(	SELECT l.AccountId
					, [Amount] = SUM(l.PlannedAmount)
			FROM Billing.Locks l with(nolock)
			join Billing.Orders o with(nolock) ON o.Id = l.OrderId  AND o.WorkflowStepId NOT IN (1,2) AND o.IsDeleted = 0 AND o.IsActive = 1 AND o.EndDistributionDateFact < @IssueDate
			WHERE l.IsDeleted = 0
				AND l.IsActive = 1
		GROUP BY l.AccountId) locks2 ON a.Id = locks2.AccountId
LEFT JOIN(	SELECT l.AccountId
					, [Amount] = SUM(ad.Amount)
			FROM Billing.Locks l with(nolock)
			JOIN Billing.Orders o with(nolock) ON o.Id = l.OrderId  AND o.WorkflowStepId NOT IN (1,2) AND o.IsDeleted = 0 AND o.IsActive = 1 AND o.EndDistributionDateFact > @IssueDate
			JOIN Billing.BranchOfficeOrganizationUnits AS boou WITH (NOLOCK) ON boou.Id = o.BranchOfficeOrganizationUnitId
			JOIN Billing.AccountDetails ad with(nolock) ON ad.IsDeleted = 0 AND	l.DebitAccountDetailId = ad.Id
			WHERE l.IsDeleted = 0 AND l.PeriodStartDate <= @IssueDate AND boou.OrganizationUnitId = @City
			GROUP BY l.AccountId ) withdraw ON a.Id = withdraw.AccountId
JOIN Billing.LegalPersons lp with(nolock) ON a.LegalPersonId = lp.Id AND lp.IsDeleted = 0
	LEFT JOIN Billing.Clients c with(nolock) ON
		c.id = lp.ClientId
JOIN Security.Users u with(nolock) ON u.Id = a.OwnerCode
JOIN Billing.BranchOfficeOrganizationUnits bou with(nolock) ON a.BranchOfficeOrganizationUnitId = bou.Id
WHERE bou.OrganizationUnitId = @City AND a.IsDeleted = 0 AND ( bills.PayPlan > 0 OR bills.PayPlan - (ISNULL(payments.Balance, 0) + ISNULL(withdraw.Amount, 0) - bills.PayPast) > 0 )
	AND ((@IsAdvertisingAgency = 1 AND c.IsAdvertisingAgency = 1) OR @IsAdvertisingAgency = 0)
ORDER BY u.DisplayName, lp.LegalName