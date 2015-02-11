CREATE NONCLUSTERED INDEX IX_Appointments_ReplicationCode
ON [Activity].[AppointmentBase] ([ReplicationCode])

CREATE NONCLUSTERED INDEX IX_Phonecalls_ReplicationCode
ON [Activity].[PhonecallBase] ([ReplicationCode])

CREATE NONCLUSTERED INDEX IX_Tasks_ReplicationCode
ON [Activity].[TaskBase] ([ReplicationCode])

CREATE NONCLUSTERED INDEX IX_Letters_ReplicationCode
ON [Activity].[LetterBase] ([ReplicationCode])
