tasklist /FI "IMAGENAME eq XDesProc.exe" 2>NUL | find /I /N "XDesProc.exe">NUL
if %ERRORLEVEL%==0 TASKKILL /F /IM XDesProc.exe /T
set %ERRORLEVEL%=0