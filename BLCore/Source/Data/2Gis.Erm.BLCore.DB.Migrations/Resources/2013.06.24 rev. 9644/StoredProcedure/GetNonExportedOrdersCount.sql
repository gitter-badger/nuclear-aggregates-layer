-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Integration].[GetNonExportedOrdersCount]
	@releaseInfoId bigint
AS

SET NOCOUNT ON;
SELECT COUNT(*) 
FROM
(			
	SELECT MaxTimeStamp =
				CASE
					WHEN OP.MaxPositionsTimestamp > O.Timestamp THEN OP.MaxPositionsTimestamp
					ELSE O.Timestamp
				END,
			T.LastIntegrationTimeStamp
	FROM Billing.Orders AS O
	INNER JOIN Billing.ReleaseInfos AS RI ON RI.Id = @releaseInfoId 
											 AND O.DestOrganizationUnitId = RI.OrganizationUnitId
											 AND O.BeginDistributionDate <= RI.PeriodStartDate
											 AND O.EndDistributionDateFact >= RI.PeriodEndDate
	LEFT JOIN
	(
		SELECT OrderId, MAX(MaxOPATimeStamp) AS MaxPositionsTimestamp FROM
		(
			SELECT OP.OrderId, 
				   MaxOPATimeStamp =		
						CASE
							WHEN OPA.MaxTimestamp IS NULL THEN OP.TimeStamp
							WHEN OPA.MaxTimestamp > OP.TimeStamp THEN OPA.MaxTimestamp
							ELSE OP.TimeStamp
						END
			FROM Billing.OrderPositions AS OP
			LEFT JOIN
			(
				SELECT OPA.OrderPositionId, MAX(OPA.Timestamp) AS MaxTimestamp
				FROM Billing.OrderPositionAdvertisement AS OPA
				GROUP BY OrderPositionId
			) AS OPA ON OPA.OrderPositionId = OP.Id
		) AS OP
		GROUP BY OrderId
	) AS OP ON OP.OrderId = O.Id
	LEFT JOIN
	(
		SELECT O.Id AS OrderId,
			   MAX(ES.LastTimestamp) AS LastIntegrationTimeStamp
		FROM Billing.Orders AS O
		INNER JOIN Integration.ExportSessions ES ON ES.EntityType = 151 --Order
		INNER JOIN Integration.ExportSessionDetails ESD ON ES.Id = ESD.IntegrationExportSessionId
														AND ESD.EntityId = O.Id
														AND ESD.IsSuccessful = 1
		GROUP BY O.Id
	) AS T ON T.OrderId = O.Id
	WHERE O.WorkflowStepId = 5 --Approved 
		  OR O.WorkflowStepId = 4 --OnTermination
) AS TS
WHERE MaxTimeStamp > LastIntegrationTimeStamp



