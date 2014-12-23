CREATE NONCLUSTERED INDEX IX_AppointmentReferences
ON [Activity].[AppointmentReferences] ([Reference],[ReferencedType],[ReferencedObjectId])
INCLUDE ([AppointmentId])

CREATE NONCLUSTERED INDEX IX_LetterReferences
ON [Activity].[LetterReferences] ([Reference],[ReferencedType],[ReferencedObjectId])
INCLUDE ([LetterId])

CREATE NONCLUSTERED INDEX IX_TaskReferences
ON [Activity].[TaskReferences] ([Reference],[ReferencedType],[ReferencedObjectId])
INCLUDE ([TaskId])

CREATE NONCLUSTERED INDEX IX_PhonecallReferences
ON [Activity].[PhonecallReferences] ([Reference],[ReferencedType],[ReferencedObjectId])
INCLUDE ([PhonecallId])
