SELECT
	[Рук. Группы] = rg.DisplayName
	, [Куратор] = mgrs.DisplayName
	, [Фирма] = f.Name
	, [Город назначения] = OU.Name
	, [№ Бланк-заказа] = o.Number
	, [К оплате (план)] = o.PayablePlan
	, [К оплате (план) по прайс-листу] = o.PayablePrice
	, [Начало размещения] = o.BeginDistributionDate
	, [Окончание размещения] = o.EndDistributionDatePlan
	, [На срок] = o.ReleaseCountPlan
	, [К оплате(план)/срок] = ot.AmountToWithdraw
	, [К оплате(план)/срок по прайс-листу] = o.PayablePrice/o.ReleaseCountPlan
	, [Платформа] = p.Name
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
	, [ППС: Встреча(звонок)-мобильная версия] = NULL
FROM
	Billing.Orders o with(nolock)
	JOIN billing.Platforms AS p  with(nolock) ON 
		p.Id = o.PlatformId
	LEFT JOIN ##Users u ON
		u.UserId = o.OwnerCode
	JOIN Security.Users mgrs with(nolock) ON
		o.OwnerCode = mgrs.Id
	LEFT JOIN Security.Users rg with(nolock) ON
		u.GmId = rg.Id
	left JOIN Billing.OrderReleaseTotals ot with(nolock) ON
		ot.OrderId = o.Id
		AND ot.ReleaseBeginDate >= @IssueDate
		AND ot.ReleaseEndDate < DATEADD(m, 1, @IssueDate)
	LEFT JOIN BusinessDirectory.Firms f with(nolock) ON
		o.FirmId = f.Id
	LEFT JOIN Billing.Clients c with(nolock) ON
		c.id = f.ClientId
	JOIN Billing.OrganizationUnits OU with(nolock) ON OU.id = o.DestOrganizationUnitId
	JOIN Billing.BranchOfficeOrganizationUnits AS boou WITH (NOLOCK) ON boou.Id = o.BranchOfficeOrganizationUnitId
WHERE boou.OrganizationUnitId = @City
	AND o.BeginDistributionDate < DATEADD(m, 1, @IssueDate)
	AND o.EndDistributionDateFact >= DATEADD(m, 1, @IssueDate)
	AND o.IsDeleted = 0
	AND o.WorkflowStepId IN (4,5)
	AND o.PayablePlan > 0
	AND ((@IsAdvertisingAgency = 1 AND c.IsAdvertisingAgency = 1) OR @IsAdvertisingAgency = 0)
ORDER BY
	mgrs.DisplayName
	, f.Name