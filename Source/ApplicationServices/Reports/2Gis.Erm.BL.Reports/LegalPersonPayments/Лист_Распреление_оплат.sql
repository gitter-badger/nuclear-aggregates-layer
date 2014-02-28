SELECT
	[Куратор] = u.DisplayName
	, [Название ЮЛ] = lp.LegalName
	, [Сумма оплаты за месяц] = t.PaymentAll
	, [Оплаты по ДЗ, накопленной до 01.12] = DZBefore1210	
	, [Оплаты по ДЗ, накопленной с 01.12] = DZAfter1210
	, [Оплаты текущих размещений] = t.PaymentCont
	, [Предоплата по новым БЗ] = t.PaymentNew
	, [Предоплата по продлениям] = t.PaymentProlong
	, [Предоплата по остальным БЗ] = t.PaymentOther
	, [Невыясненные оплаты] = t.PaymentUnknown
	, [Возврат денежных средств] = -t.[Returns]
FROM
	##PaymentDistribution t
	JOIN Billing.LegalPersons lp ON
		lp.Id = t.CustJurPer
	JOIN Security.Users u ON
		u.Id = t.Curator
ORDER BY 1,2