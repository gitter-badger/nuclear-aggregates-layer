--DECLARE
--	@issuedate date = '20141001'
--	, @city int = 1
--	, @IsAdvertisingAgency int = 0
	

SELECT
	t.[Куратор]
	, t.[Фирма]
	, t.[Город назначения]
	, t.[№ Бланк-заказа]
	, t.[Клиент]
	, t.[Юр. лицо]
	, t.[К оплате (план)]
	, t.[К оплате (план) по прайс-листу]
	, t.[Начало размещения]
	, t.[Окончание размещения]
	, t.[На срок]
	, t.[К оплате(план)/срок]
	, t.[К оплате(план)/срок по прайс-листу]
	, [Сумма по выставленному счету (первый платеж)] = ISNULL(t.PayablePlan, 0)
	, [Количество предоплаченных месяцев (первый платеж)] = 
						CASE WHEN ISNULL(t.[К оплате (план)], 0) = 0 THEN 0 ELSE CAST(ISNULL(t.PayablePlan, 0) / t.[К оплате(план)/срок] AS INT) END
	, [Корректировка РГ] = CAST(0 AS MONEY)
	, [Корректировка РГ по прайс-листу] = NULL
	, [Примечание] = NULL
	, [Сумма предоплаты] = CAST(0 AS MONEY)
	, [Планируемое количество предоплаченных месяцев (первый платеж)] = CAST(NULL AS INT)
	, [Сравнение] = CAST(NULL AS INT)
	, [Оплаченный объем по продлениям (план)] = CAST(2 AS MONEY)
	, t.[Платформа]
	, [ППС: Письмо-сервис] = NULL
	, [ППС: Письмо-уведомление о поступлении денег] = NULL
	, [ППС: Письмо-поздравление с проф. праздником] = NULL
	, [ППС: Письмо-поздравление с ДР ЛПР] = NULL
	, [ППС: Письмо-поздравление с ДР Компании] = NULL
	, [ППС: Звонок-сервис] = NULL
	, [ППС: Звонок-оплата] = NULL
	, [ППС: Встреча-сервис] = NULL
	, [ППС: Встреча-продление] = NULL
	, [ППС: Встреча-поздравление] = NULL
FROM
	(
		SELECT
			[Куратор] = mgrs.DisplayName
			, [Фирма] = f.Name
			, [Город назначения] = OU.Name
			, [№ Бланк-заказа] = o.Number
			, [К оплате (план)] = o.PayablePlan
			, [Начало размещения] = o.BeginDistributionDate
			, [Окончание размещения] = o.EndDistributionDatePlan
			, [На срок] = o.ReleaseCountPlan
			, [К оплате(план)/срок по прайс-листу] = o.PayablePrice/o.ReleaseCountPlan
			, [К оплате (план) по прайс-листу] = o.PayablePrice
			, [К оплате(план)/срок] = ot.AmountToWithdraw
			, [Платформа] = p.Name
			, ROW_NUMBER() OVER (PARTITION BY o.Id ORDER BY b.PaymentDatePlan ASC) rnk
			, b.PayablePlan
			, [Клиент] = c.Name
			, [Юр. лицо] = lp.LegalName
		FROM
			Billing.Orders o with(nolock)
			JOIN Billing.BranchOfficeOrganizationUnits AS boou WITH (NOLOCK) ON boou.Id = o.BranchOfficeOrganizationUnitId
			JOIN billing.Platforms AS p  with(nolock) ON 
				p.Id = o.PlatformId
			JOIN Billing.OrderReleaseTotals ot with(nolock) ON
				ot.OrderId = o.Id
			LEFT JOIN Billing.Bills b with(nolock) ON
				b.OrderId = o.Id
				AND b.IsDeleted = 0
			JOIN Billing.LegalPersons lp with(nolock) ON
				o.LegalPersonId = lp.Id
			JOIN Security.Users mgrs with(nolock) ON
				o.OwnerCode = mgrs.Id
			LEFT JOIN BusinessDirectory.Firms f with(nolock) ON
				o.FirmId = f.Id
	LEFT JOIN Billing.Clients c with(nolock) ON
		c.id = f.ClientId
			JOIN Billing.OrganizationUnits OU with(nolock) ON 
				OU.id = o.DestOrganizationUnitId
		WHERE
			boou.OrganizationUnitId = @City
			AND convert(DATE, o.EndDistributionDateFact) = DATEADD(d, -1, DATEADD(m, 1, @IssueDate))
			AND o.EndDistributionDateFact = o.EndDistributionDatePlan
			AND ot.ReleaseBeginDate <= @IssueDate
			AND ot.ReleaseEndDate >= @IssueDate
			AND o.IsDeleted = 0
			AND o.WorkflowStepId IN (5, 6)
			AND o.PayablePlan > 0
			AND ((@IsAdvertisingAgency = 1 AND c.IsAdvertisingAgency = 1) OR @IsAdvertisingAgency = 0)
	) t
WHERE
	t.rnk = 1
ORDER BY
	t.[Куратор]
	, t.[Фирма]