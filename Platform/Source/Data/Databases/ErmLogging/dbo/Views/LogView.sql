CREATE View dbo.[LogView] as
Select (select m.Name from Modules m where m.Id=s.ModuleId) ModuleName, 
	lj.MessageDate, 
	ud.UserName,
	lj.MessageLevel, 
	lj.MessageText, 
	lj.ExceptionData,
	ud.SessionID,  
	ud.UserIP, 
	ud.UserBrowser,
	ud.SeanceID 
from LogJournal lj
join UserData ud on lj.UserDataID=ud.Id
join Seances s on s.Id=ud.SeanceID
where s.Id=(Select top 1 Id from Seances order by Id desc)