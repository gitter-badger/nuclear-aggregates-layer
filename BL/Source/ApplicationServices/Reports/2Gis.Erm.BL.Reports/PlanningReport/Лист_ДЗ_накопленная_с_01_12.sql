--declare 	@IssueDate date = '20130901'
--			,@City bigint = 1
DECLARE
	@DZCheckPoint date = '20141201'


DECLARE 
	@StartDate DATE = @DZCheckPoint
	, @EndDate DATE = @IssueDate
	, @OrganizationUnit bigint = @City
	, @ShowLimits BIT = 1;

--Отсюда начинается запрос отчета по оборотам лицевых счетов

;WITH Locks AS -- блокировки и списания для юр. лица
(
	SELECT
		AccountId = a.Id
		, [Списания на нач. периода]	= SUM(CASE WHEN CAST(l.PeriodEndDate AS DATE) < @StartDate THEN ABS(ad.Amount) ELSE 0 END)
		, [Списания за период]			= SUM(CASE WHEN l.IsActive = 0 AND CAST(l.PeriodStartDate AS DATE) >= @StartDate AND CAST(l.PeriodEndDate AS DATE) < DATEADD(MONTH, DATEDIFF(MONTH, 0, @EndDate) + 1, 0) THEN ABS(ad.Amount) ELSE 0 END)
		, [Блокировки]					= SUM(CASE WHEN l.IsActive = 1 AND CAST(l.PeriodEndDate AS DATE) < DATEADD(MONTH, DATEDIFF(MONTH, 0, @EndDate) + 1, 0)
												THEN l.PlannedAmount
												ELSE 0
											END)
	FROM 
		Billing.Accounts a with(nolock)
		INNER JOIN Billing.Locks l with(nolock) ON (a.Id = l.AccountId)
		LEFT JOIN Billing.AccountDetails ad with(nolock) ON (ad.Id = l.DebitAccountDetailId AND ad.OperationTypeId = 7 AND ad.IsDeleted = 0)
	WHERE
		a.IsDeleted = 0 
		 --Списание в счет оплаты БЗ
		AND l.IsDeleted = 0
	GROUP BY a.Id
),
Receives AS -- поступления для юр. лица
(
	SELECT
		[AccountId]						= a.Id
		,[Поступления за период]		= SUM(CASE WHEN (CAST(acd.TransactionDate AS DATE) BETWEEN @StartDate AND @EndDate) THEN (2 * IsPlus - 1) * ABS(acd.Amount) ELSE 0 END)
		,[Поступления на нач. периода]	= SUM(CASE WHEN (acd.TransactionDate < DATEADD(MONTH, DATEDIFF(MONTH, 0, @StartDate), 0)) THEN (2 * IsPlus - 1) * ABS(acd.Amount) ELSE 0 END)
	FROM	
		Billing.Accounts a with(nolock)
		INNER JOIN Billing.AccountDetails acd with(nolock) ON (acd.AccountId = a.Id AND acd.IsDeleted = 0)
		INNER JOIN Billing.OperationTypes opt with(nolock) ON (opt.Id = acd.OperationTypeId AND opt.IsDeleted = 0)
	WHERE		
		a.IsDeleted = 0 
		AND a.IsActive = 1
		AND opt.Id <> 7 --Списание в счет оплаты БЗ
		AND acd.IsActive = 1 
	GROUP BY a.Id
),
Limits AS -- лимиты юр.лица
(
	SELECT
		x.AccountId,
		x.Amount
	FROM
		(
			SELECT 
				rn = ROW_NUMBER() OVER (PARTITION BY AccountId ORDER BY EndPeriodDate DESC), -- номер лимита у аккаунта
				AccountId = acc.Id, 
				Amount, 
				EndPeriodDate
			FROM
				Billing.Limits l with(nolock)
				INNER JOIN Billing.Accounts acc with(nolock) ON acc.Id = l.AccountId
			WHERE
				-- окончание лимита между первым и последним числом последнего отчетного месяца
				EndPeriodDate BETWEEN DATEADD(MONTH, DATEDIFF(MONTH, 0, @EndDate), 0) AND DATEADD(SECOND, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0, @EndDate) + 1, 0))
				AND l.IsDeleted = 0
				AND l.IsActive = 1
				AND @ShowLimits = 1
		) x
	WHERE 
		x.rn = 1 -- для каждого account выбираем только первый лимит
),
Result AS
(
	SELECT 
		[Юр. лицо заказчика] = (SELECT LegalName FROM Billing.LegalPersons with(nolock) WHERE Id = lp.Id)
		,[Юр. лицо исполнителя] = 						
			(
				SELECT DISTINCT
					bou.ShortLegalName + ','  as 'data()'
				FROM
					Billing.Accounts acc  with(nolock)				
					INNER JOIN Billing.BranchOfficeOrganizationUnits bou with(nolock) ON (bou.Id = acc.BranchOfficeOrganizationUnitId)
				WHERE
					acc.LegalPersonId = lp.Id
					AND acc.IsDeleted = 0
					AND bou.OrganizationUnitId = @OrganizationUnit
				FOR XML PATH('')
			)
		, [Куратор] = (SELECT DisplayName FROM [Security].[Users] WHERE Id = u.Id)
		
		--, [Списания за период] = SUM(ISNULL(l.[Списания за период], 0))
		--, [Поступления за период] = SUM(ISNULL(r.[Поступления за период], 0))
		--, [Блокировки] = SUM(ISNULL(l.[Блокировки], 0)	)
		--, [Списания на нач. периода] = SUM(ISNULL(l.[Списания на нач. периода], 0))
		--, [Поступления на нач. периода] = SUM(ISNULL(r.[Поступления на нач. периода], 0))
		
		, [Задолженность за период] = 
		(
			+ SUM(ISNULL(l.[Списания за период], 0))		-- списания за период
			+ SUM(ISNULL(l.[Блокировки], 0))				-- все блокировки с датой окончания меньше месяца следующего за месяцем окончания периода
			- SUM(ISNULL(r.[Поступления за период], 0))	-- поступления за период
			
			-- баланс на начало периода
			- (CASE WHEN SUM((ISNULL(r.[Поступления на нач. периода], 0) - ISNULL(l.[Списания на нач. периода], 0))) > 0 
					THEN SUM((ISNULL(r.[Поступления на нач. периода], 0) - ISNULL(l.[Списания на нач. периода], 0)))
					ELSE 0 
				END) 
		)	
		, [Текущий лимит] = CASE WHEN @ShowLimits = 1 THEN SUM(ISNULL(lim.Amount, 0)) ELSE NULL END
		, [Дата действия лимита] = CASE WHEN @ShowLimits = 1 THEN DATEADD(MONTH, DATEDIFF(MONTH, 0, @EndDate), 0) ELSE NULL END
		, [Текущий баланс] = SUM(acc.Balance - ISNULL(l.[Блокировки], 0))
	FROM 
		Billing.Accounts acc  with(nolock)
		INNER JOIN Billing.BranchOfficeOrganizationUnits bou with(nolock) ON (bou.Id = acc.BranchOfficeOrganizationUnitId AND bou.IsDeleted = 0)	
		INNER JOIN Billing.LegalPersons lp with(nolock) ON (acc.LegalPersonId = lp.Id)
			LEFT JOIN Billing.Clients c with(nolock) ON
		c.id = lp.ClientId
		INNER JOIN [Security].[Users] u with(nolock) ON (u.Id = acc.OwnerCode)
		LEFT JOIN Receives r with(nolock) ON (acc.Id = r.AccountId)	
		LEFT JOIN Locks l with(nolock) ON (acc.Id = l.AccountId)
		LEFT JOIN Limits lim with(nolock) ON (lim.AccountId = acc.Id)
	WHERE
		acc.IsDeleted = 0 AND acc.IsActive = 1
		AND bou.OrganizationUnitId = @OrganizationUnit 	AND ((@IsAdvertisingAgency = 1 AND c.IsAdvertisingAgency = 1) OR @IsAdvertisingAgency = 0)
	GROUP BY lp.Id, u.Id
)
SELECT 
	* 
FROM Result
WHERE
	NOT [Задолженность за период] < 0.01 -- берем только тех кто должен (больше копейки)
ORDER BY [Юр. лицо заказчика]