SELECT
	[Ordinal] = CAST(ROW_NUMBER() OVER (PARTITION BY u.GmId ORDER BY u.IsGM DESC, un.DisplayName) AS INT)
	, [DisplayName] = un.DisplayName
	, [GM] = gmn.DisplayName
	, [IsGM] = u.IsGM
	, [GroupOrdinal] = CAST(DENSE_RANK() OVER (ORDER BY gmn.DisplayName) AS INT)
FROM
	##Users u	
	JOIN Security.Users un with(nolock) ON
		un.Id = u.UserId
	JOIN Security.Users gmn with(nolock) ON
		gmn.Id = u.GmId
ORDER BY
	3, 4 DESC, 1