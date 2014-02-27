--DECLARE
--	@City int = 5
--	, @ReportDate date = '20120724'
--	, @CurrentUser int = 96
--	, @UseOnRegistration bit = 1
--	, @UseOnApproval bit = 1

SET NOCOUNT ON
--некоторая точка отсчета - дата на которую считается дебиторская задолженность - 1 декабря предыдущего года
DECLARE @DZCheckpoint DATE = DATEADD(day, 1-DAY(@ReportDate),DATEADD(month, -MONTH(@ReportDate), @ReportDate)) 

DECLARE	@CurrentMonthInf DATE = DATEADD(d, 1-DAY(@ReportDate), @ReportDate)
DECLARE	@CurrentMonthSup DATE = DATEADD(m, 1, @CurrentMonthInf)
DECLARE @ReportDateSup DATE = DATEADD(d, 1, @ReportDate)

IF(OBJECT_ID('tempdb..#Users') IS NOT NULL)
	DROP TABLE #Users

;WITH 
	cteUserTree AS
	(
		SELECT 
			[UserId] = Id
			, [Parent] = ParentId
			, [Login] = Account
			, 0 AS [Level]
		FROM [Security].[Users]
		WHERE Id = @CurrentUser
		
		UNION ALL
		
		SELECT 
			u.Id
			, cte.[Parent]
			, u.Account
			, cte.[Level] + 1 AS [Level] 
		FROM 
			cteUserTree cte 
			INNER JOIN [Security].[Users] u ON 
				u.ParentId = cte.[UserId]
	)
SELECT [Id] = UserId
INTO #Users
FROM cteUserTree

IF(OBJECT_ID('tempdb..#OrdersAll') IS NOT NULL)
	DROP TABLE #OrdersAll

SELECT
	[OrderID] = o.Id
	, [IssueBegin] = CAST(o.BeginDistributionDate AS DATE)
	, [IssueEnd] = CAST(DATEADD(HH, 1, o.EndDistributionDateFact) AS DATE)
	, [IssueEndPlan] = CAST(DATEADD(HH, 1, o.EndDistributionDatePlan) AS DATE)
	, [Status] = 
				CASE
					WHEN
						CAST(DATEADD(HH, 1, o.EndDistributionDateFact) AS DATE) = @CurrentMonthSup
						AND o.EndDistributionDateFact <> o.EndDistributionDatePlan 
						THEN 0
					ELSE 1
				END
	, [ClientId] = f.ClientId
	, [FirmID] = o.FirmID
	, [CityDest] = o.DestOrganizationUnitId
	, [CitySource] = o.SourceOrganizationUnitId
	, [CustJurPer] = o.LegalPersonId
	, [Curator] = o.OwnerCode
	, [FactCost] = ISNULL(CAST( CASE WHEN o.ReleaseCountPlan = 0 THEN 0 ELSE o.PayablePlan / o.ReleaseCountPlan * o.ReleaseCountFact END AS MONEY), 0)
	, [AmountPlanPeriod] = CAST( CASE WHEN o.ReleaseCountPlan = 0 THEN 0 ELSE o.PayablePlan / o.ReleaseCountPlan END AS MONEY)
INTO #OrdersAll
FROM 
	Billing.Orders o
	JOIN Billing.LegalPersons lp ON
		lp.Id = o.LegalPersonId
	JOIN BusinessDirectory.Firms f ON
		f.Id = o.FirmId
WHERE
	o.PayablePlan > 0
	AND o.ReleaseCountFact > 0
	AND o.BudgetType = 2
	AND (
		o.WorkflowStepId IN (4,5,6)
		OR (o.WorkflowStepId = 1 AND @UseOnRegistration = 1)
		OR (o.WorkflowStepId = 2 AND @UseOnApproval = 1)
	)
	AND o.IsDeleted = 0

	AND o.BeginDistributionDate <= @CurrentMonthSup
	AND o.EndDistributionDateFact > DATEADD(m, -7, @CurrentMonthInf)


IF(OBJECT_ID('tempdb..#Orders') IS NOT NULL)
	DROP TABLE #Orders

SELECT
	o.*
INTO #Orders
FROM #OrdersAll o
	JOIN #Users u ON
		o.Curator = u.Id
WHERE
	o.IssueEnd > @CurrentMonthInf
	AND o.IssueBegin <= @CurrentMonthSup
	AND o.CitySource = @City
	AND o.Status IS NOT NULL


		if(OBJECT_ID('tempdb..#Payments') is not null)
			DROP TABLE #Payments

		select
			p.CustJurPer
			, p.Curator
			, [Amount] = p.Amount
			, [AmountABS] = p.Amount
			, [Payment] = p.Payment
			, [Return] = p.[Return]
			, [OnDZCheckpointRest] = CAST(0 AS MONEY)
			, [OnDZCheckPointAfter] = CAST(0 AS MONEY)
		INTO #Payments
		from 
			(
				SELECT
					[CustJurPer] = a.LegalPersonId
					, [Curator] = ad.OwnerCode
					, [Amount] = SUM(ad.Amount*(2*ot.IsPlus-1)) --"сумма поступлений (оплаты минус возвраты)"
					, [Payment] = SUM(CASE ot.IsPlus WHEN 1 THEN ad.Amount ELSE 0 END) --оплаты
					, [Return] = SUM(CASE ot.IsPlus WHEN 0 THEN ad.Amount ELSE 0 END) --возвраты
				FROM
					Billing.Accounts a
					JOIN Billing.AccountDetails ad ON
						a.Id = ad.AccountId
					JOIN Billing.BranchOfficeOrganizationUnits bou ON
						a.BranchOfficeOrganizationUnitId = bou.Id
						AND bou.OrganizationUnitId = @City
					JOIN #Users u ON
						ad.OwnerCode = u.Id
					JOIN Billing.OperationTypes ot ON
						ot.Id = ad.OperationTypeId
				WHERE
					ad.TransactionDate < @ReportDateSup
					and ad.TransactionDate >= @CurrentMonthInf
					AND ad.IsDeleted = 0
					and ad.OperationTypeId NOT IN (7, 13, 14)
				GROUP BY
					a.LegalPersonId
					, ad.OwnerCode
			) p


							IF(OBJECT_ID('tempdb..#Deductions') IS NOT NULL)
								DROP TABLE #Deductions

							SELECT
								[OrderID] = l.OrderId
								, [CustJurPer] = lp.CustJurPer
								, SUM(ABS(ISNULL(l.PlannedAmount, ad.Amount))) Amount
							INTO #Deductions
							FROM
								(
									SELECT DISTINCT CustJurPer
									FROM #Orders
								) lp
								JOIN Billing.Accounts a ON
									lp.CustJurPer = a.LegalPersonId
								JOIN Billing.BranchOfficeOrganizationUnits bou ON
									a.BranchOfficeOrganizationUnitId = bou.Id
									AND bou.OrganizationUnitId = @City
								JOIN Billing.Locks l ON
									l.AccountId = a.Id
									AND l.PeriodEndDate < @CurrentMonthSup
									--AND l.PeriodStartDate > '20090801' -- по эту дату обрезаны операции в ERM
									AND l.IsDeleted = 0
								LEFT JOIN Billing.AccountDetails ad ON
									ad.AccountId = a.Id
									AND ad.IsDeleted = 0
									AND l.DebitAccountDetailId = ad.Id
									AND ad.OperationTypeId = 7
							GROUP BY
								l.OrderId, lp.CustJurPer

IF(OBJECT_ID('tempdb..##OrderCategories') IS NOT NULL)
	DROP TABLE ##OrderCategories

/*
Считаем категории БЗ
	1 - Текущие
	2 - Новые
	3 - Продление
	4 - Изменения пакетов
	5 - Возвращенные
	6 - Расширение регионального размещения
	7 - К продлению
	8 - Техническое расторжение
	9 - Действительное расторжение
*/

;WITH 
	cte1 AS --Текущие
	(
		SELECT
			o.*
			, [Category] = 1
		FROM
			#Orders o
		WHERE
			o.IssueBegin < @CurrentMonthSup
			AND o.Status = 1
			AND o.IssueEnd > @CurrentMonthSup
	)
	, cte2 AS -- Новые
	(
		select
			o.*
			, [Category] = 2
		from 
			#Orders o 
			LEFT JOIN #OrdersAll o1 ON
				o1.ClientId = o.ClientId
				AND o1.CitySource = o.CitySource
				AND o1.IssueEnd > DATEADD(m, -7, @CurrentMonthInf)
				AND o1.IssueBegin < o.IssueBegin
		where
			o.IssueBegin = @CurrentMonthSup
			AND o.Status = 1
			AND o1.OrderID IS NULL
	)
	, cte3 AS -- Продления
	(
		select DISTINCT
			o.*
			, [Category] = 3
		from 
			#Orders o
			JOIN #OrdersAll o1 ON
				o1.CityDest = o.CityDest
				AND o1.ClientId = o.ClientId
				AND o1.Status = 1
				AND o1.IssueEnd = @CurrentMonthSup
		where
			o.IssueBegin = @CurrentMonthSup
			AND o.Status = 1
	)
	, cte4 AS -- Изменения пакетов
	(
		SELECT DISTINCT
			o.*
			, [Category] = 4
		FROM 
			#Orders o
			JOIN #OrdersAll o1 ON
				o1.CityDest = o.CityDest
				AND o1.ClientId = o.ClientId
				AND o1.Status = 1
				AND o1.IssueBegin < @CurrentMonthSup
				AND o1.IssueEnd > @CurrentMonthSup
		WHERE
			o.IssueBegin = @CurrentMonthSup
			AND o.Status = 1
		UNION
		SELECT DISTINCT
			o.*
			, [Category] = 4
		FROM 
			#Orders o
			LEFT JOIN #OrdersAll o2 ON
				o2.CityDest = o.CityDest
				AND o2.ClientId = o.ClientId
				AND o2.Status = 1
				AND o2.IssueEnd = @CurrentMonthSup
			JOIN #OrdersAll o1 ON
				o1.CityDest = o.CityDest
				AND o1.ClientId = o.ClientId
				AND o1.Status = 0
				AND o1.IssueEnd = @CurrentMonthSup
		WHERE
			o.IssueBegin = @CurrentMonthSup
			AND o.Status = 1
			AND o2.OrderID IS NULL
	)
	, cte5 AS --Возвращенные
	(
		SELECT DISTINCT
			o.*
			, [Category] = 5
		FROM
			#Orders o
			LEFT JOIN #OrdersAll o1 ON
				o1.ClientId = o.ClientId
				AND o1.IssueBegin <= @CurrentMonthInf
				AND o1.Status = 1
				AND o1.IssueEnd > @CurrentMonthSup
			JOIN #OrdersAll o2 ON
				o2.ClientId = o.ClientId
				AND o2.IssueEnd > DATEADD(m, -7, @CurrentMonthInf)
				AND o2.IssueEnd < o.IssueBegin
		where
			o.Status = 1
			AND o.IssueBegin = @CurrentMonthSup
			AND o1.OrderID IS NULL
	)
	, cte6 AS -- Расширение регионального размещения
	(
		select DISTINCT
			o.*
			, [Category] = 6
		from 
			#Orders o
			JOIN #OrdersAll o1 ON
				o1.CitySource = o.CitySource
				AND o1.ClientId = o.ClientId
				AND o1.CityDest <> o.CityDest
				AND o1.IssueBegin < @CurrentMonthSup
				AND o1.IssueEnd > @CurrentMonthInf
		where
			o.IssueBegin = @CurrentMonthSup
			AND o.Status = 1
	)
	, cte7 AS -- К продлению
	(
		select
			o.*
			, [Category] = 7
		from 
			#Orders o
		where
			o.Status = 1
			AND o.IssueEnd = @CurrentMonthSup
	)
	, cte8 AS -- Технические расторжения
	(
		select DISTINCT
			o.*
			, [Category] = 8
		from 
			#Orders o
			LEFT JOIN #OrdersAll o2 ON
				o2.CityDest = o.CityDest
				AND o2.ClientId = o.ClientId
				AND o2.Status = 1
				AND o2.IssueEnd = @CurrentMonthSup
			JOIN #OrdersAll o1 ON
				o1.CityDest = o.CityDest
				AND o1.ClientId = o.ClientId
				AND o1.Status = 1
				AND o1.IssueBegin = @CurrentMonthSup
		where
			o.IssueEnd = @CurrentMonthSup
			AND o.Status = 0
			AND o2.OrderID IS NULL
	)
	, cte9 AS -- Действительные расторжения
	(
		select DISTINCT
			o.*
			, [Category] = 9
		from 
			#Orders o
			LEFT JOIN #OrdersAll o1 ON
				o1.CityDest = o.CityDest
				AND o1.ClientId = o.ClientId
				AND o1.Status =1
				AND o1.IssueBegin = @CurrentMonthSup
			LEFT JOIN #OrdersAll o2 ON
				o2.CityDest = o.CityDest
				AND o2.ClientId = o.ClientId
				AND o2.Status = 1
				AND o2.IssueEnd = @CurrentMonthSup
		where
			o.IssueEnd = @CurrentMonthSup
			AND o.Status = 0
			AND (
				(o2.OrderID IS NOT NULL AND o1.OrderID IS NOT NULL)
				OR
				o1.OrderID IS NULL
			)
 			
	)
	, cteBills AS
	(
		select
			o.OrderID
			, [AmountNow] =  SUM(CASE WHEN b.PaymentDatePlan >= @CurrentMonthInf THEN b.PayablePlan ELSE 0 END)
			, [AmountPast] =  SUM(CASE WHEN b.PaymentDatePlan < @CurrentMonthInf THEN b.PayablePlan ELSE 0 END)
		from
			#Orders o
			join Billing.Bills b on
				b.OrderId = o.OrderID
				and b.PaymentDatePlan < @CurrentMonthSup
				AND b.IsDeleted = 0
		group by
			o.OrderID
	)
	, cteOrderCategory AS -- БЗ с категориями. Если БЗ попал в несколько категорий, то выбираем наименьшую
	(
		select
			o.CitySource
			, o.CityDest
			, o.CustJurPer
			, o.Curator
			, o.FirmID
			, o.ClientID
			, o.OrderID
			, o.AmountPlanPeriod
			, o.IssueBegin
			, o.IssueEnd
			, [PaymentPerPeriod] = CAST(0 AS MONEY)
			, [PaymentPerBalance] = CAST(0 AS MONEY)
			, [PaymentNeed] = 
							CASE
								WHEN o.Status = 0 AND o.FactCost - ISNULL(b.AmountPast,0) < ISNULL(b.AmountNow,0) THEN o.FactCost
								ELSE ISNULL(b.AmountNow,0) + ISNULL(b.AmountPast,0)
							END	- ISNULL(d.Amount, 0)
			, [PaymentMax] = o.FactCost - ISNULL(d.Amount, 0)
			, o.Category
			, [AmountBreak] = CASE WHEN o.IssueEnd <> o.IssueEndPlan THEN o.AmountPlanPeriod ELSE 0 END
		from
			(
				select 
					t.*
					, ROW_NUMBER() over (PARTITION by t.OrderID ORDER by t.Category ASC) rn
				from
					(
						select * from cte1
						union select * from cte2
						union select * from cte3
						union select * from cte4
						union select * from cte5
						union select * from cte6
						union select * from cte7
						union select * from cte8
						union select * from cte9
					) t
			) o
			left join #Deductions d on d.OrderID = o.OrderID
			left join cteBills b on b.OrderID = o.OrderID
		where
			o.rn = 1
	)
select * 
INTO ##OrderCategories
FROM cteOrderCategory


									IF(OBJECT_ID('tempdb..#Balance') is not null)
										DROP TABLE #Balance
									--ДЗ на начало месяца
									SELECT
										[CustJurPer] = t.CustJurPer
										, [Balance] = ISNULL(t.Balance, 0)
										, [DZCheckPointRest] = CASE WHEN t.DZCheckPointRest >= 0 THEN 0 ELSE -t.DZCheckPointRest END
										, [DZCheckPointAfter] =
														CASE
															WHEN t.Balance >= 0 THEN 0
															WHEN t.DZCheckPointRest >= 0 THEN -t.Balance
															ELSE -( t.Balance - t.DZCheckPointRest )
														END	

									INTO #Balance
									FROM
										(
											SELECT
												[CustJurPer] = op.LegalPersonId
												, [Balance] = 
															SUM(op.Amount)
												, [DZCheckPointRest] =
															SUM(
																CASE
																	WHEN
																		(op.TransactionDate < @DZCheckpoint )
																		OR
																		(op.TransactionDate >= @DZCheckpoint AND op.Amount>0 AND op.OperationTypeId <> 7)
																	THEN op.Amount
																	ELSE 0
																END
															)

											FROM
												(
													SELECT
														[LegalPersonId] = a.LegalPersonId
														, [Amount] = ad.Amount*(2*ot.IsPlus-1)
														, [TransactionDate] = CASE WHEN ot.Id = 7 THEN l.PeriodStartDate ELSE ad.TransactionDate END
														, [OperationTypeId] = ot.Id

													FROM
														Billing.Accounts a
														JOIN Billing.LegalPersons lp ON
															lp.Id = a.LegalPersonId
														JOIN Billing.BranchOfficeOrganizationUnits bou ON
															a.BranchOfficeOrganizationUnitId = bou.Id
															AND bou.OrganizationUnitId = @City
														JOIN #Users u ON
															u.Id = lp.OwnerCode
														JOIN Billing.AccountDetails ad ON
															ad.AccountId = a.Id
															AND ad.IsDeleted = 0
															AND a.IsDeleted = 0
														JOIN Billing.OperationTypes ot ON
															ad.OperationTypeId = ot.Id
														LEFT JOIN Billing.Locks l ON
															l.DebitAccountDetailId = ad.Id
															AND l.IsDeleted = 0
													WHERE
														(ot.Id = 7 AND l.PeriodStartDate <= @CurrentMonthInf)
														OR
														(ot.Id <> 7 AND ad.TransactionDate < @CurrentMonthInf)
													UNION ALL
													SELECT
														[LegalPersonId] = a.LegalPersonId
														, [Amount] = -l.PlannedAmount
														, [TransactionDate] = l.PeriodStartDate
														, [OperationTypeId] = 7

													FROM
														Billing.Accounts a
														JOIN Billing.LegalPersons lp ON
															lp.Id = a.LegalPersonId
														JOIN Billing.BranchOfficeOrganizationUnits bou ON
															a.BranchOfficeOrganizationUnitId = bou.Id
															AND bou.OrganizationUnitId = @City
														JOIN #Users u ON
															u.Id = lp.OwnerCode
														JOIN Billing.Locks l ON
															l.AccountId = a.Id
															AND l.IsDeleted = 0
															AND l.IsActive = 1
													WHERE
														l.PeriodStartDate = @CurrentMonthInf
												) op
											GROUP BY
												op.LegalPersonId
										) t
															

-- Закрываем ДЗ до 12.10
UPDATE p SET
	[OnDZCheckpointRest] = t.OnDZCheckpointRest
	, [Amount] = p.Amount - t.OnDZCheckpointRest
FROM
	(
		SELECT
			[CustJurPer] = p.CustJurPer

			, [OnDZCheckpointRest] = 
						CASE
							WHEN SUM(p.Amount) OVER (PARTITION BY p.CustJurPer) <= b.DZCheckPointRest THEN p.Amount
							ELSE b.DZCheckPointRest / SUM(p.Amount) OVER (PARTITION BY p.CustJurPer) * p.Amount
						END
		FROM
			#Payments p 
			JOIN #Balance b ON
				b.CustJurPer = p.CustJurPer
		WHERE
			p.Amount > 0
			AND b.DZCheckPointRest > 0
	) t
	JOIN #Payments p ON
		t.CustJurPer = p.CustJurPer

-- Закрываем ДЗ после 12.10
UPDATE p SET
	[OnDZCheckPointAfter] = t.OnDZCheckPointAfter
	, [Amount] = p.Amount - t.OnDZCheckPointAfter
FROM
	(
		SELECT
			[CustJurPer] = p.CustJurPer

			, [OnDZCheckPointAfter] = 
						CASE
							WHEN SUM(p.Amount) OVER (PARTITION BY p.CustJurPer) <= b.DZCheckPointAfter THEN p.Amount
							ELSE b.DZCheckPointAfter / SUM(p.Amount) OVER (PARTITION BY p.CustJurPer) * p.Amount
						END
		FROM
			#Payments p 
			JOIN #Balance b ON
				b.CustJurPer = p.CustJurPer
		WHERE
			p.Amount > 0
			AND b.DZCheckPointAfter > 0
	) t
	JOIN #Payments p ON
		t.CustJurPer = p.CustJurPer


--Итерация 1. Распределение балансов по текущим
;WITH 
	cteDiff AS
	(
		select
			o.OrderID
			, [Diff] =
				case
					WHEN o.PaymentNeed = 0 then cast(0.00 AS money)
					when balance.Balance >= (SUM(o.PaymentNeed) OVER (PARTITION BY o.CustJurPer)) then o.PaymentNeed
					else balance.Balance / (SUM(o.PaymentNeed) OVER (PARTITION BY o.CustJurPer)) * o.PaymentNeed
				end
		from
			##OrderCategories o
			join #Balance balance on
				balance.CustJurPer = o.CustJurPer
				and balance.Balance > 0
		where
			o.Category IN (1,7,8,9)
			AND	balance.Balance > 0 
			AND o.PaymentNeed > 0
			
	)
UPDATE o SET
	PaymentPerBalance = o.PaymentPerBalance + p.Diff
from
	##OrderCategories o
	join cteDiff p on
		o.OrderID = p.OrderID

--Итерация 2. Распределение балансов по «Новые», «Продление», «Изменения пакетов», «Прочий прирост»
;WITH 
	cteDiff AS
	(
		select
			o.OrderID
			, [Diff] =
				CASE
					WHEN o.PaymentNeed = 0 then cast(0.00 AS money)
					WHEN balance.Balance >= SUM(o.PaymentNeed) OVER (PARTITION BY o.CustJurPer) then o.PaymentNeed
					ELSE balance.Balance / (SUM(o.PaymentNeed) OVER (PARTITION BY o.CustJurPer)) * o.PaymentNeed
				END
		from
			##OrderCategories o
			join (
				select
					b.CustJurPer
					, [Balance] = 
						b.Balance - p.Payed
				from
					#Balance b
					join (
						select
							o.CustJurPer
							, [Payed] = SUM(o.PaymentPerBalance)
						from ##OrderCategories o
						group by
							o.CustJurPer
					) p on
						b.CustJurPer = p.CustJurPer
				WHERE
					b.Balance > p.Payed
			) balance on
				balance.CustJurPer = o.CustJurPer
				and balance.Balance > 0
		where
			o.Category in (2,3,4,5,6)
			AND balance.Balance > 0
			AND o.PaymentNeed > 0		
	)
UPDATE o SET
	PaymentPerBalance = o.PaymentPerBalance + p.Diff
from
	##OrderCategories o
	join cteDiff p on
		o.OrderID = p.OrderID
		
--Итерация 3. Распределение оплат по текущим

;WITH
	cteDiff AS
	(
		select
			o.OrderID
			, PaymentPerPeriod = 
				case
					WHEN o.PaymentNeed = 0 then cast(0.00 AS money)
					when p.Amount >= SUM(o.PaymentNeed - o.PaymentPerBalance) OVER (PARTITION BY o.CustJurPer, o.Curator) then o.PaymentNeed - o.PaymentPerBalance
					else isnull(p.Amount,0) / (SUM(o.PaymentNeed) OVER (PARTITION BY o.CustJurPer, o.Curator)) * o.PaymentNeed
				end
		from
			##OrderCategories o
			join #Payments p on
				p.CustJurPer = o.CustJurPer
				and p.Curator = o.Curator
		where
			o.Category IN (1,7,8,9)
			AND p.Amount > 0
			AND o.PaymentNeed > o.PaymentPerBalance		
	)
UPDATE o SET
	PaymentPerPeriod = o.PaymentPerPeriod + p.PaymentPerPeriod
from
	##OrderCategories o
	join cteDiff p on
		o.OrderID = p.OrderID

--Итерация 4. Распределение оплат по «Новые», «Продление», «Изменения пакетов», «Прочий прирост» до требуемой оплаты
;WITH 
	ctePayments AS
	(
		select
			b.CustJurPer
			, b.Curator
			, [Payment] = 
				b.Amount - p.Payment
		from
			#Payments b
			JOIN(
				select
					o.CustJurPer
					, o.Curator
					, SUM(o.PaymentPerPeriod) Payment
				from ##OrderCategories o
				group by
					o.CustJurPer
					, o.Curator
			) p on
				b.CustJurPer = p.CustJurPer
				and b.Curator = p.Curator
		WHERE
			b.Amount > p.Payment		
	)
	, cteDiff AS
	(
		SELECT
			o.OrderID
			, [Diff] =
				CASE
					WHEN o.AmountPlanPeriod = 0 then cast(0.00 AS money)
					WHEN p.Payment >= SUM(o.PaymentNeed - o.PaymentPerBalance - o.PaymentPerPeriod) OVER (PARTITION BY o.CustJurPer, o.Curator) 
						THEN o.PaymentNeed - o.PaymentPerBalance - o.PaymentPerPeriod
					ELSE isnull(p.Payment,0) / (SUM(o.AmountPlanPeriod) OVER (PARTITION BY o.CustJurPer, o.Curator)) * o.AmountPlanPeriod
				END
		FROM
			##OrderCategories o
			join ctePayments p on
				p.CustJurPer = o.CustJurPer
				and o.Curator = p.Curator
		WHERE
			o.Category in (2,3,4,5,6)	
			AND p.Payment > 0
			AND o.PaymentNeed > o.PaymentPerBalance + o.PaymentPerPeriod
	)
UPDATE o SET
	PaymentPerPeriod = o.PaymentPerPeriod + p.Diff
FROM
	##OrderCategories o
	join cteDiff p on
		o.OrderID = p.OrderID
		
--Итерация 5. Распределение оплат по «Новые», «Продление», «Изменения пакетов», «Прочий прирост» до плановой оплаты
;WITH 
	ctePayments AS
	(
		select
			b.CustJurPer
			, b.Curator
			, [Payment] = 
				b.Amount - p.Payment
		from
			#Payments b
			JOIN(
				select
					o.CustJurPer
					, o.Curator
					, SUM(o.PaymentPerPeriod) Payment
				from ##OrderCategories o
				group by
					o.CustJurPer
					, o.Curator
			) p on
				b.CustJurPer = p.CustJurPer
				and b.Curator = p.Curator
	)
	, cteDiff AS
	(
		SELECT
			o.OrderID
			, [Diff] =
				CASE
					WHEN o.AmountPlanPeriod = 0 then cast(0.00 AS money)
					WHEN p.Payment >= SUM(o.PaymentMax - o.PaymentPerBalance - o.PaymentPerPeriod) OVER (PARTITION BY o.CustJurPer, o.Curator) 
						THEN o.PaymentMax - o.PaymentPerBalance - o.PaymentPerPeriod
					ELSE isnull(p.Payment,0) / (SUM(o.AmountPlanPeriod) OVER (PARTITION BY o.CustJurPer, o.Curator)) * o.AmountPlanPeriod
				END
		FROM
			##OrderCategories o
			join ctePayments p on
				p.CustJurPer = o.CustJurPer
				and o.Curator = p.Curator
		WHERE
			o.Category in (1,2,3,4,5,6)	
			AND p.Payment > 0
			AND o.PaymentMax > o.PaymentPerBalance + o.PaymentPerPeriod
	)
UPDATE o SET
	PaymentPerPeriod = o.PaymentPerPeriod + p.Diff
FROM
	##OrderCategories o
	join cteDiff p on
		o.OrderID = p.OrderID

--Итерация 6. Поиск и учет невыясненных оплат
INSERT ##OrderCategories(OrderID, CustJurPer, Curator, PaymentPerPeriod, Category, CityDest, CitySource)
SELECT
	0 
	, p.CustJurPer
	, p.Curator
	, [Amount] = p.Amount - ISNULL(o.PaymentPerPeriod, 0)
	, 0
	, @City
	, @City
FROM
	(
		SELECT
			CustJurPer, Curator, SUM(Amount) Amount
		FROM 
			#Payments
		GROUP BY
			CustJurPer, Curator
	) p
	LEFT JOIN(
		SELECT
			CustJurPer, Curator, SUM(PaymentPerPeriod) PaymentPerPeriod
		FROM
			##OrderCategories
		GROUP BY
			CustJurPer, Curator
	) o ON
		p.CustJurPer = o.CustJurPer
		and p.Curator = o.Curator
WHERE
	p.Amount - ISNULL(o.PaymentPerPeriod, 0) > 0


if(OBJECT_ID('tempdb..##PaymentDistribution') IS NOT NULL)
	DROP TABLE ##PaymentDistribution
	
SELECT
	[CustJurPer] = p.CustJurPer
	, [Curator] = p.Curator
	, [PaymentCont] = ISNULL(oc.[PaymentCont], 0)
	, [PaymentNew] = ISNULL(oc.[PaymentNew], 0)
	, [PaymentProlong] = ISNULL(oc.[PaymentProlong],0)
	, [PaymentOther] = ISNULL(oc.[PaymentOther], 0)
	, [PaymentUnknown] = ISNULL(oc.[PaymentUnknown], 0)
	, [PaymentAll] = ISNULL(p.[PaymentAll], 0)
	, [Returns] = ISNULL(p.[Returns], 0)
	, [DZBefore1210] = ISNULL(p.[DZBefore1210], 0)
	, [DZAfter1210] = ISNULL(p.[DZAfter1210], 0)
INTO ##PaymentDistribution
FROM
	(
		SELECT
			CustJurPer
			, Curator
			, [PaymentAll] = SUM(AmountABS)
			, [Returns] = SUM([Return])
			, [DZBefore1210] = SUM(OnDZCheckpointRest)
			, [DZAfter1210] = SUM(OnDZCheckpointAfter)
		FROM #Payments
		GROUP BY 
			CustJurPer
			, Curator
	) p
	LEFT JOIN
	(
		SELECT
			[CustJurPer] = oc.CustJurPer
			, [Curator] = oc.Curator
			, [PaymentCont] = 
							SUM(
								CASE
									WHEN oc.Category IN (1,7,8,9) THEN oc.PaymentPerPeriod
									ELSE 0
								END
							)
			, [PaymentNew] = 
							SUM(
								CASE
									WHEN oc.Category = 2 THEN oc.PaymentPerPeriod
									ELSE 0
								END
							)
			, [PaymentProlong] = 
							SUM(
								CASE
									WHEN oc.Category = 3 THEN oc.PaymentPerPeriod
									ELSE 0
								END
							)
			, [PaymentOther] = 
							SUM(
								CASE
									WHEN oc.Category IN (4,5,6) THEN oc.PaymentPerPeriod
									ELSE 0
								END
							)
			, [PaymentUnknown] = 
							SUM(
								CASE
									WHEN oc.Category = 0 THEN oc.PaymentPerPeriod
									ELSE 0
								END
							)
		FROM ##OrderCategories oc
		GROUP BY
			oc.CustJurPer
			, oc.Curator
	) oc ON
		p.CustJurPer = oc.CustJurPer
		AND p.Curator = oc.Curator
	--LEFT JOIN  ON
	--	p.CustJurPer = lp.Id
		
if(OBJECT_ID('tempdb..##PaymentGrowth') IS NOT NULL)
	DROP TABLE ##PaymentGrowth

SELECT
	t.OrderID
	, t.Curator
	, t.Category
	, [Break] = t.[Break]
	, [AmountPlanPeriod] = t.AmountPlanPeriod
	, [Growth] = 
					CASE
						---конченый костыль для изменения пакета. Я с этой логикой НЕ СОГЛАСЕН!!!--
						WHEN t.Category = 4 AND t.AmountPlanPeriod - t.[Break] <= t.Payment THEN t.AmountPlanPeriod - t.[Break]
						WHEN t.Category = 4 AND t.AmountPlanPeriod - t.[Break] > t.Payment THEN t.Payment
						--------------------------------------------------------------------
						WHEN t.AmountPlanPeriod <= t.Payment THEN t.AmountPlanPeriod
						ELSE t.Payment
					END
	, [GrowthBreak] =
					CASE
						WHEN t.Category IN (8,9) THEN -t.[Break]
						WHEN t.AmountPlanPeriod - t.[Break] <= 0 THEN t.AmountPlanPeriod - t.[Break]
						WHEN t.Payment >= t.AmountPlanPeriod - t.[Break] THEN t.AmountPlanPeriod - t.[Break]
						ELSE t.Payment
					END
INTO ##PaymentGrowth
FROM
	( 
		SELECT DISTINCT
			[OrderID] = ISNULL(oc.OrderID, b.OrderID)
			, [Curator] = ISNULL(oc.Curator, b.Curator)
			, [Category] = ISNULL(oc.Category, b.Category)
			, [AmountPlanPeriod] = ISNULL(oc.AmountPlanPeriod, b.AmountPlanPeriod)

			, [Payment] = ISNULL(oc.PaymentPerPeriod + oc.PaymentPerBalance, 0)
			, [Break] = 
					CASE
						WHEN oc.OrderID IS NULL THEN b.AmountBreak
						WHEN oc.GrowthPriority = 1 
							THEN SUM(ISNULL(b.AmountBreak, 0)) OVER (PARTITION BY oc.ClientId, oc.CityDest)
						ELSE 0
					END
		FROM
			(
				SELECT
					*
					, [GrowthPriority] = --приоритет БЗ-прироста, для привязки расторжений. Сначала идут продления и изменения пакетов, потом все остальное
							ROW_NUMBER() OVER (
								PARTITION BY 
									ClientId
									, CityDest
								ORDER BY
									CASE
										WHEN Category IN (3,4) THEN 1
										ELSE 2
									END
									, OrderID
							)
				FROM
					##OrderCategories
				WHERE
					Category IN (2,3,4,5,6)
			) oc
			FULL JOIN (
				SELECT *
				FROM
					##OrderCategories
				WHERE
					Category IN (8,9)
			) b ON
				oc.ClientId = b.ClientId
				AND oc.CityDest = b.CityDest
				AND oc.GrowthPriority = 1
	) t