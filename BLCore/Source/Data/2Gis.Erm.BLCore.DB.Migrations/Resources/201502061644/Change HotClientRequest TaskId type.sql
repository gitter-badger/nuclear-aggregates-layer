ALTER TABLE [Integration].[HotClientRequests] ADD TaskNewId bigint 

GO

UPDATE [Integration].[HotClientRequests] 
SET TaskNewId = taskBase.Id
FROM [Integration].[HotClientRequests] hotClients
INNER JOIN [Activity].[TaskBase] taskBase ON
	hotClients.TaskId = taskBase.ReplicationCode

GO

ALTER TABLE [Integration].[HotClientRequests] DROP COLUMN TaskId

GO

sp_rename 'Integration.HotClientRequests.TaskNewId', 'TaskId', 'COLUMN'

GO 


