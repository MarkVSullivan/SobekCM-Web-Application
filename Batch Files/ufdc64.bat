echo off
echo MOVING DIRECTORIES TO TEMP FOLDER
move /Y "c:\inetpub\wwwroot\sobekcm\sobekcm\data" c:\inetpub\wwwroot\temp\data
move /Y "c:\inetpub\wwwroot\sobekcm\sobekcm\default" c:\inetpub\wwwroot\temp\default
move /Y "c:\inetpub\wwwroot\sobekcm\sobekcm\design" c:\inetpub\wwwroot\temp\design
move /Y "c:\inetpub\wwwroot\sobekcm\sobekcm\mySobek" c:\inetpub\wwwroot\temp\mySobek
move /Y "c:\inetpub\wwwroot\sobekcm\sobekcm\temp" c:\inetpub\wwwroot\temp\temp
echo.
echo REMOVING ANY EXISTING STAGING DIRECTORY
rmdir c:\staging64 /s /q
echo.
cd c:\windows\microsoft.net\framework64\v4.0.30319
aspnet_compiler -v /UFDC C:\Staging64
echo DELETING UNNECESSARY FILES
del c:\staging64\error.html
del c:\staging64\internal.html
del c:\staging64\oai2.xsl
del c:\staging64\robots.txt
del c:\staging64\sobekcm.sln
echo.
echo MOVING DIRECTORIES BACK TO UFDC WEB FOLDER 
move /Y c:\inetpub\wwwroot\temp\data "c:\inetpub\wwwroot\sobekcm\sobekcm\data" 
move /Y c:\inetpub\wwwroot\temp\default "c:\inetpub\wwwroot\sobekcm\sobekcm\default"
move /Y c:\inetpub\wwwroot\temp\design "c:\inetpub\wwwroot\sobekcm\sobekcm\design"
move /Y c:\inetpub\wwwroot\temp\mySobek "c:\inetpub\wwwroot\sobekcm\sobekcm\mySobek"
move /Y c:\inetpub\wwwroot\temp\temp "c:\inetpub\wwwroot\sobekcm\sobekcm\temp"
cd c:\
