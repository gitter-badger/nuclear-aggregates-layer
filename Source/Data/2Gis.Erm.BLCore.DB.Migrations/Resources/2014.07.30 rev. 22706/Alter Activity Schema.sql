IF object_id('[Activity].[AppointmentReferences]') IS NOT NULL DROP TABLE [Activity].[AppointmentReferences];
IF object_id('[Activity].[AppointmentBase]') IS NOT NULL DROP TABLE [Activity].[AppointmentBase];

CREATE TABLE [Activity].[AppointmentBase](
	[Id] [bigint] NOT NULL CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED,
	[ReplicationCode] [uniqueidentifier] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedOn] [datetime2](2) NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedOn] [datetime2](2) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_Appointment_IsActive]  DEFAULT 1,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_Appointment_IsDeleted]  DEFAULT 0,
	[Timestamp] [timestamp] NOT NULL,

	[OwnerCode] [bigint] NOT NULL,
	[Subject] [nvarchar](256) NULL,
	[Description] [nvarchar](max) NULL,
	[ScheduledStart] [datetime] NOT NULL,
	[ScheduledEnd] [datetime] NOT NULL,
	[ActualEnd] [datetime] NULL,
	[Priority] [int] NOT NULL CONSTRAINT [DF_Appointment_Priority]  DEFAULT 0,
	[Status] [int] NOT NULL CONSTRAINT [DF_Appointment_Status]  DEFAULT 0,

	[IsAllDayEvent] [bit] NOT NULL CONSTRAINT [DF_Appointment_IsAllDayEvent]  DEFAULT 0,
	[Location] [nvarchar](256) NULL,
	[Purpose] [int] NOT NULL CONSTRAINT [DF_Appointment_Purpose]  DEFAULT 0
)

-- holds regarding object, organizer and attendees
CREATE TABLE [Activity].[AppointmentReferences](
	[AppointmentId] [bigint] NOT NULL CONSTRAINT [FK_Appointments] REFERENCES [Activity].[AppointmentBase]([Id]),
	[Reference] [int] NOT NULL,
	[ReferencedType] [int] NOT NULL,
	[ReferencedObjectId] [bigint] NOT NULL
	CONSTRAINT [PK_AppointmentReferences] PRIMARY KEY CLUSTERED ([AppointmentId], [Reference], [ReferencedType], [ReferencedObjectId])
)

IF object_id('[Activity].[PhonecallReferences]') IS NOT NULL DROP TABLE [Activity].[PhonecallReferences];
IF object_id('[Activity].[PhonecallBase]') IS NOT NULL DROP TABLE [Activity].[PhonecallBase];

CREATE TABLE [Activity].[PhonecallBase](
	[Id] [bigint] NOT NULL CONSTRAINT [PK_Phonecalls] PRIMARY KEY CLUSTERED,
	[ReplicationCode] [uniqueidentifier] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedOn] [datetime2](2) NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedOn] [datetime2](2) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_Phonecall_IsActive]  DEFAULT 1,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_Phonecall_IsDeleted]  DEFAULT 0,
	[Timestamp] [timestamp] NOT NULL,

	[OwnerCode] [bigint] NOT NULL,
	[Subject] [nvarchar](256) NULL,
	[Description] [nvarchar](max) NULL,
	[ScheduledStart] [datetime] NOT NULL,
	[ScheduledEnd] [datetime] NOT NULL,
	[ActualEnd] [datetime] NULL,
	[Priority] [int] NOT NULL CONSTRAINT [DF_Phonecall_Priority]  DEFAULT 0,
	[Status] [int] NOT NULL CONSTRAINT [DF_Phonecall_Status]  DEFAULT 0,

	[Direction] [bit] NOT NULL CONSTRAINT [DF_Phonecall_Direction]  DEFAULT 0,
	[PhoneNumber] [nvarchar](200) NULL,
	[Purpose] [int] NOT NULL CONSTRAINT [DF_Phonecall_Purpose]  DEFAULT 0
)

-- holds regarding object and from/to contacts
CREATE TABLE [Activity].[PhonecallReferences](
	[PhonecallId] [bigint] NOT NULL CONSTRAINT [FK_Phonecalls] REFERENCES [Activity].[PhonecallBase]([Id]),
	[Reference] [int] NOT NULL,
	[ReferencedType] [int] NOT NULL,
	[ReferencedObjectId] [bigint] NOT NULL
	CONSTRAINT [PK_PhonecallReferences] PRIMARY KEY CLUSTERED ([PhonecallId], [Reference], [ReferencedType], [ReferencedObjectId])
)

IF object_id('[Activity].[TaskReferences]') IS NOT NULL DROP TABLE [Activity].[TaskReferences];
IF object_id('[Activity].[TaskBase]') IS NOT NULL DROP TABLE [Activity].[TaskBase];

CREATE TABLE [Activity].[TaskBase](
	[Id] [bigint] NOT NULL CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED,
	[ReplicationCode] [uniqueidentifier] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedOn] [datetime2](2) NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedOn] [datetime2](2) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_Task_IsActive]  DEFAULT 1,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_Task_IsDeleted]  DEFAULT 0,
	[Timestamp] [timestamp] NOT NULL,

	[OwnerCode] [bigint] NOT NULL,
	[Subject] [nvarchar](256) NULL,
	[Description] [nvarchar](max) NULL,
	[ScheduledStart] [datetime] NOT NULL,
	[ScheduledEnd] [datetime] NOT NULL,
	[ActualEnd] [datetime] NULL,
	[Priority] [int] NOT NULL CONSTRAINT [DF_Task_Priority]  DEFAULT 0,
	[Status] [int] NOT NULL CONSTRAINT [DF_Task_Status]  DEFAULT 0,

	[TaskType] [int] NOT NULL CONSTRAINT [DF_Task_Type]  DEFAULT 0
)

-- holds regarding object references
CREATE TABLE [Activity].[TaskReferences](
	[TaskId] [bigint] NOT NULL CONSTRAINT [FK_Tasks] REFERENCES [Activity].[TaskBase]([Id]),
	[Reference] [int] NOT NULL,
	[ReferencedType] [int] NOT NULL,
	[ReferencedObjectId] [bigint] NOT NULL
	CONSTRAINT [PK_TaskReferences] PRIMARY KEY CLUSTERED ([TaskId], [Reference], [ReferencedType], [ReferencedObjectId])
)
