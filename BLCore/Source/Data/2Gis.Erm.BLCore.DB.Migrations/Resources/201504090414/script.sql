declare @RussiaTimeZone1_Id uniqueidentifier = '78465910-6B41-4510-B20D-CC48792811F3'
declare @RussiaTimeZone1_UserInterfaceName nvarchar(100) = '(GMT+02:00) Калининград (RTZ 1)'
declare @RussiaTimeZone1_StandardName nvarchar(100) = 'Kaliningrad Standard Time'
declare @RussiaTimeZone1_TimeZoneCode int = 1001
declare @RussiaTimeZone1_Bias int = -120

declare @RussiaTimeZone2_Id uniqueidentifier = (select TimeZoneDefinitionId from [dbo].[TimeZoneDefinitionBase] where TimeZoneCode = 145)
declare @RussiaTimeZone2_Bias int = -180

declare @RussiaTimeZone3_Id uniqueidentifier = '034B41B5-6296-4017-95F8-D5A63758CC2E'
declare @RussiaTimeZone3_UserInterfaceName nvarchar(100) = '(GMT+04:00) Ижевск, Самара (RTZ 3)'
declare @RussiaTimeZone3_StandardName nvarchar(100) = 'Russia Time Zone 3'
declare @RussiaTimeZone3_TimeZoneCode int = 1003
declare @RussiaTimeZone3_Bias int = -240

declare @RussiaTimeZone4_Id uniqueidentifier = (select TimeZoneDefinitionId from [dbo].[TimeZoneDefinitionBase] where TimeZoneCode = 180)
declare @RussiaTimeZone4_Bias int = -300

declare @RussiaTimeZone5_Id uniqueidentifier = (select TimeZoneDefinitionId from [dbo].[TimeZoneDefinitionBase] where TimeZoneCode = 201)
declare @RussiaTimeZone5_Bias int = -360

declare @RussiaTimeZone6_Id uniqueidentifier = (select TimeZoneDefinitionId from [dbo].[TimeZoneDefinitionBase] where TimeZoneCode = 207)
declare @RussiaTimeZone6_Bias int = -420

declare @RussiaTimeZone7_Id uniqueidentifier = (select TimeZoneDefinitionId from [dbo].[TimeZoneDefinitionBase] where TimeZoneCode = 227)
declare @RussiaTimeZone7_Bias int = -480

declare @RussiaTimeZone8_Id uniqueidentifier = (select TimeZoneDefinitionId from [dbo].[TimeZoneDefinitionBase] where TimeZoneCode = 240)
declare @RussiaTimeZone8_Bias int = -540

declare @RussiaTimeZone9_Id uniqueidentifier = (select TimeZoneDefinitionId from [dbo].[TimeZoneDefinitionBase] where TimeZoneCode = 270)
declare @RussiaTimeZone9_Bias int = -600

declare @RussiaTimeZone10_Id uniqueidentifier = 'BD1A5141-4300-4CD1-AF13-361BE8C2AC05'
declare @RussiaTimeZone10_UserInterfaceName nvarchar(100) = '(GMT+11:00) Чокурдах (RTZ 10)'
declare @RussiaTimeZone10_StandardName nvarchar(100) = 'Russia Time Zone 10'
declare @RussiaTimeZone10_TimeZoneCode int = 1010
declare @RussiaTimeZone10_Bias int = -660

declare @RussiaTimeZone11_Id uniqueidentifier = '458D79AF-5EBC-4F7B-9501-B6FA61D4476D'
declare @RussiaTimeZone11_UserInterfaceName nvarchar(100) = '(GMT+12:00) Анадырь, Петропавловск-Камчатский (RTZ 11)'
declare @RussiaTimeZone11_StandardName nvarchar(100) = 'Russia Time Zone 11'
declare @RussiaTimeZone11_TimeZoneCode int = 1011
declare @RussiaTimeZone11_Bias int = -720

-- Создаю четыре новых зоны
insert into [dbo].[TimeZoneDefinitionBase](ModifiedOn, TimeZoneCode, OrganizationId, TimeZoneDefinitionId, CreatedOn, Bias, DaylightName, CreatedBy, UserInterfaceName, StandardName, RetiredOrder, ModifiedBy, DeletionStateCode)
values 
    ('2015-04-09', @RussiaTimeZone1_TimeZoneCode, NULL, @RussiaTimeZone1_Id, '2015-04-09', NULL, '', NULL, @RussiaTimeZone1_UserInterfaceName, @RussiaTimeZone1_StandardName, 0, NULL, 0),
    ('2015-04-09', @RussiaTimeZone3_TimeZoneCode, NULL, @RussiaTimeZone3_Id, '2015-04-09', NULL, '', NULL, @RussiaTimeZone3_UserInterfaceName, @RussiaTimeZone3_StandardName, 0, NULL, 0),
    ('2015-04-09', @RussiaTimeZone10_TimeZoneCode, NULL, @RussiaTimeZone10_Id, '2015-04-09', NULL, '', NULL, @RussiaTimeZone10_UserInterfaceName, @RussiaTimeZone10_StandardName, 0, NULL, 0),
    ('2015-04-09', @RussiaTimeZone11_TimeZoneCode, NULL, @RussiaTimeZone11_Id, '2015-04-09', NULL, '', NULL, @RussiaTimeZone11_UserInterfaceName, @RussiaTimeZone11_StandardName, 0, NULL, 0)

insert into [dbo].[TimeZoneLocalizedNameBase](CultureId, ModifiedOn, DeletionStateCode, CreatedOn, TimeZoneDefinitionId, StandardName, ModifiedBy, TimeZoneLocalizedNameId, UserInterfaceName, DaylightName, CreatedBy, OrganizationId)
values 
    (1049, '2014-04-09', 0, '2014-04-09', @RussiaTimeZone1_Id, @RussiaTimeZone1_StandardName, NULL, newid(), @RussiaTimeZone1_UserInterfaceName, '', NULL, NULL),
    (1049, '2014-04-09', 0, '2014-04-09', @RussiaTimeZone3_Id, @RussiaTimeZone3_StandardName, NULL, newid(), @RussiaTimeZone3_UserInterfaceName, '', NULL, NULL),
    (1049, '2014-04-09', 0, '2014-04-09', @RussiaTimeZone10_Id, @RussiaTimeZone10_StandardName, NULL, newid(), @RussiaTimeZone10_UserInterfaceName, '', NULL, NULL),
    (1049, '2014-04-09', 0, '2014-04-09', @RussiaTimeZone11_Id, @RussiaTimeZone11_StandardName, NULL, newid(), @RussiaTimeZone11_UserInterfaceName, '', NULL, NULL)

insert into [dbo].[TimeZoneRuleBase](ModifiedBy, StandardDay, ModifiedOn, StandardMinute, StandardBias, StandardYear, DaylightMonth, StandardDayOfWeek, DaylightSecond, Bias, TimeZoneRuleVersionNumber, DaylightBias, StandardMonth, EffectiveDateTime, CreatedBy, DaylightHour, StandardHour, CreatedOn, DaylightYear, StandardSecond, DaylightMinute, TimeZoneDefinitionId, DaylightDayOfWeek, TimeZoneRuleId, DaylightDay, OrganizationId, DeletionStateCode)
values
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, 0, 0, 0, 10, '1900-01-01', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone1_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, 0, 0, 0, 10, '1900-01-01', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone3_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, 0, 0, 0, 10, '1900-01-01', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone10_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, 0, 0, 0, 10, '1900-01-01', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone11_Id, 0, newid(), 5, NULL, 0)


-- Исправляю имена существующих зон
update [dbo].[TimeZoneLocalizedNameBase] set UserInterfaceName = '(GMT+03:00) Волгоград, Москва, Санкт-Петербург (RTZ 2)' where CultureId = 1049 and TimeZoneDefinitionId = @RussiaTimeZone2_Id
update [dbo].[TimeZoneLocalizedNameBase] set UserInterfaceName = '(GMT+05:00) Екатеринбург (RTZ 4)' where CultureId = 1049 and TimeZoneDefinitionId = @RussiaTimeZone4_Id
update [dbo].[TimeZoneLocalizedNameBase] set UserInterfaceName = '(GMT+06:00) Новосибирск (RTZ 5)' where CultureId = 1049 and TimeZoneDefinitionId = @RussiaTimeZone5_Id
update [dbo].[TimeZoneLocalizedNameBase] set UserInterfaceName = '(GMT+07:00) Красноярск (RTZ 6)' where CultureId = 1049 and TimeZoneDefinitionId = @RussiaTimeZone6_Id
update [dbo].[TimeZoneLocalizedNameBase] set UserInterfaceName = '(GMT+08:00) Иркутск (RTZ 7)' where CultureId = 1049 and TimeZoneDefinitionId = @RussiaTimeZone7_Id
update [dbo].[TimeZoneLocalizedNameBase] set UserInterfaceName = '(GMT+09:00) Якутск (RTZ 8)' where CultureId = 1049 and TimeZoneDefinitionId = @RussiaTimeZone8_Id
update [dbo].[TimeZoneLocalizedNameBase] set UserInterfaceName = '(GMT+10:00) Владивосток, Магадан (RTZ 9)' where CultureId = 1049 and TimeZoneDefinitionId = @RussiaTimeZone9_Id
update [dbo].[TimeZoneLocalizedNameBase] set UserInterfaceName = '(GMT+11:00) Магадан — устаревшее' where CultureId = 1049 and UserInterfaceName = '(GMT+11:00) Магадан'

-- Добавляю правила с 2011-08-31 (вечное лето)
insert into [dbo].[TimeZoneRuleBase](ModifiedBy, StandardDay, ModifiedOn, StandardMinute, StandardBias, StandardYear, DaylightMonth, StandardDayOfWeek, DaylightSecond, Bias, TimeZoneRuleVersionNumber, DaylightBias, StandardMonth, EffectiveDateTime, CreatedBy, DaylightHour, StandardHour, CreatedOn, DaylightYear, StandardSecond, DaylightMinute, TimeZoneDefinitionId, DaylightDayOfWeek, TimeZoneRuleId, DaylightDay, OrganizationId, DeletionStateCode)
values
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone1_Bias-60, 0, 0, 10, '2011-08-31 04:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone1_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone2_Bias-60, 0, 0, 10, '2011-08-31 04:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone2_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone3_Bias-60, 0, 0, 10, '2011-08-31 04:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone3_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone4_Bias-60, 0, 0, 10, '2011-08-31 04:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone4_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone5_Bias-60, 0, 0, 10, '2011-08-31 04:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone5_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone6_Bias-60, 0, 0, 10, '2011-08-31 04:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone6_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone7_Bias-60, 0, 0, 10, '2011-08-31 04:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone7_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone8_Bias-60, 0, 0, 10, '2011-08-31 04:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone8_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone9_Bias-60, 0, 0, 10, '2011-08-31 04:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone9_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone10_Bias-60, 0, 0, 10, '2011-08-31 04:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone10_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone11_Bias-60, 0, 0, 10, '2011-08-31 04:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone11_Id, 0, newid(), 5, NULL, 0)

-- Добавляю правила с 2014-10-26 (вечная зима)
insert into [dbo].[TimeZoneRuleBase](ModifiedBy, StandardDay, ModifiedOn, StandardMinute, StandardBias, StandardYear, DaylightMonth, StandardDayOfWeek, DaylightSecond, Bias, TimeZoneRuleVersionNumber, DaylightBias, StandardMonth, EffectiveDateTime, CreatedBy, DaylightHour, StandardHour, CreatedOn, DaylightYear, StandardSecond, DaylightMinute, TimeZoneDefinitionId, DaylightDayOfWeek, TimeZoneRuleId, DaylightDay, OrganizationId, DeletionStateCode)
values
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone1_Bias, 0, 0, 10, '2014-10-26 02:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone1_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone2_Bias, 0, 0, 10, '2014-10-26 02:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone2_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone3_Bias, 0, 0, 10, '2014-10-26 02:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone3_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone4_Bias, 0, 0, 10, '2014-10-26 02:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone4_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone5_Bias, 0, 0, 10, '2014-10-26 02:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone5_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone6_Bias, 0, 0, 10, '2014-10-26 02:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone6_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone7_Bias, 0, 0, 10, '2014-10-26 02:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone7_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone8_Bias, 0, 0, 10, '2014-10-26 02:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone8_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone9_Bias, 0, 0, 10, '2014-10-26 02:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone9_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone10_Bias, 0, 0, 10, '2014-10-26 02:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone10_Id, 0, newid(), 5, NULL, 0),
    (NULL, 5, '2015-04-09', 0, 0, 0, 3, 0, 0, @RussiaTimeZone11_Bias, 0, 0, 10, '2014-10-26 02:00', NULL, 2, 3, '2015-04-09', 0, 0, 0, @RussiaTimeZone11_Id, 0, newid(), 5, NULL, 0)
