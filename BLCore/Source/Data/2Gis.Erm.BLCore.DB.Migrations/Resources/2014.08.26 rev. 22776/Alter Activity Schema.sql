-- drop appointment ActualEnd, AfterSaleType and IsAllDayEvent columns with constraints
ALTER TABLE [Activity].[AppointmentBase] 
DROP 
	CONSTRAINT [DF_Appointment_AfterSaleType], [DF_Appointment_IsAllDayEvent],
	COLUMN [ActualEnd], [AfterSaleType], [IsAllDayEvent];

-- alter appointment Priority constraint to set Normal as the default
ALTER TABLE [Activity].[AppointmentBase] DROP CONSTRAINT [DF_Appointment_Priority];
ALTER TABLE [Activity].[AppointmentBase] ADD CONSTRAINT [DF_Appointment_Priority] DEFAULT (2) FOR [Priority];

-- rename letter ScheduledStart to ScheduledOn
EXEC sys.sp_rename 
	@objname = N'Activity.LetterBase.ScheduledStart',
	@newname = 'ScheduledOn',
	@objtype = 'COLUMN';

-- drop letter ScheudledEnd and ActualEnd columns
ALTER TABLE [Activity].[LetterBase] DROP COLUMN [ScheduledEnd],[ActualEnd];

-- alter letter Priority constraint to set Normal as the default
ALTER TABLE [Activity].[LetterBase] DROP CONSTRAINT [DF_Letter_Priority];
ALTER TABLE [Activity].[LetterBase] ADD CONSTRAINT [DF_Letter_Priority] DEFAULT (2) FOR [Priority];

-- rename phonecall ScheduledStart to ScheduledOn
EXEC sys.sp_rename 
	@objname = N'Activity.PhonecallBase.ScheduledStart',
	@newname = 'ScheduledOn',
	@objtype = 'COLUMN';

-- drop appointment ScheduledEnd, ActualEnd, PhoneNumber, AfterSaleType and Direction columns with constraints
ALTER TABLE [Activity].[PhonecallBase]
DROP
	CONSTRAINT [DF_Phonecall_AfterSaleType], [DF_Phonecall_Direction],
	COLUMN [ScheduledEnd], [ActualEnd], [PhoneNumber], [AfterSaleType], [Direction];

-- alter phonecall Priority constraint to set Normal as the default
ALTER TABLE [Activity].[PhonecallBase] DROP CONSTRAINT [DF_Phonecall_Priority];
ALTER TABLE [Activity].[PhonecallBase] ADD CONSTRAINT [DF_Phonecall_Priority] DEFAULT (2) FOR [Priority];

-- rename task ScheduledStart to ScheduledOn
EXEC sys.sp_rename 
	@objname = N'Activity.TaskBase.ScheduledStart',
	@newname = 'ScheduledOn',
	@objtype = 'COLUMN';

-- drop task ScheudledEnd and ActualEnd columns
ALTER TABLE [Activity].[TaskBase]
DROP COLUMN [ScheduledEnd], [ActualEnd];

-- alter task Priority constraint to set Normal as the default
ALTER TABLE [Activity].[TaskBase] DROP CONSTRAINT [DF_Task_Priority];
ALTER TABLE [Activity].[TaskBase] ADD CONSTRAINT [DF_Task_Priority] DEFAULT (2) FOR [Priority];