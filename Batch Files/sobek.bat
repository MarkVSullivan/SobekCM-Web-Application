echo off
REM ******** Some values used to support alternate locations **********
set source=C:\GitRepository\SobekCM-Web-Application
set iis=C:\inetpub\wwwroot\SobekCM
set installer=C:\GitRepository\SobekWebInstaller
echo.
echo COPYING WEB FILES INTO PRECOMPILE DIRECTORY
echo ....sobekcm
robocopy "%source%\SobekCM" "%iis%\SobekCM" /mir /NFL /NDL /NJH /NJS /nc /ns /np 
echo ....sobekcm_library
robocopy "%source%\SobekCM_Library" "%iis%\SobekCM_Library" /mir /NFL /NDL /NJH /NJS /nc /ns /np 
echo ....sobekcm_resource_object
robocopy "%source%\SobekCM_Resource_Object" "%iis%\SobekCM_Resource_Object" /mir /NFL /NDL /NJH /NJS /nc /ns /np 
echo ....sobekcm_tools
robocopy "%source%\SobekCM_Tools" "%iis%\SobekCM_Tools" /mir /NFL /NDL /NJH /NJS /nc /ns /np 
echo ....sobekcm_url_rewriter
robocopy "%source%\SobekCM_URL_Rewriter" "%iis%\SobekCM_URL_Rewriter" /mir /NFL /NDL /NJH /NJS /nc /ns /np 
echo ....sobekcm_core
robocopy "%source%\SobekCM_Core" "%iis%\SobekCM_Core" /mir /NFL /NDL /NJH /NJS /nc /ns /np 
echo ....sobekcm_engine_library
robocopy "%source%\SobekCM_Engine_Library" "%iis%\SobekCM_Engine_Library" /mir /NFL /NDL /NJH /NJS /nc /ns /np 
echo ....sobekcm_ui_library
robocopy "%source%\SobekCM_UI_Library" "%iis%\SobekCM_UI_Library" /mir /NFL /NDL /NJH /NJS /nc /ns /np 
echo REMOVING ANY EXISTING STAGING DIRECTORIES
rmdir "%installer%\SobekCM_WiX_Installer\Staging64" /s /q
rmdir "%installer%\SobekCM_WiX_Installer\Staging32" /s /q
echo.
echo REMOVING DIRECTORIES TO NOT MOVE TO STAGING
rmdir %iis%\SobekCM\config /s /q
rmdir %iis%\SobekCM\design /s /q
rmdir %iis%\SobekCM\default /s /q
rmdir %iis%\SobekCM\iipimage /s /q
rmdir %iis%\SobekCM\mysobek /s /q
rmdir %iis%\SobekCM\dev /s /q
rmdir %iis%\SobekCM\temp /s /q
rmdir %iis%\SobekCM\obj /s /q
rmdir %iis%\SobekCM\Properties /s /q
rmdir %iis%\SobekCM\content /s /q
echo.
echo COMPILING 32-BIT VERSION
cd c:\windows\microsoft.net\framework\v4.0.30319
aspnet_compiler -v /SobekCM "%installer%\SobekCM_WiX_Installer\Staging32"
echo.
echo DELETING UNNECESSARY 32-BIT FILES
del "%installer%\SobekCM_WiX_Installer\Staging32\web.config"
del "%installer%\SobekCM_WiX_Installer\Staging32\SobekCM.csproj"
del "%installer%\SobekCM_WiX_Installer\Staging32\SobekCM.csproj.user"
del "%installer%\SobekCM_WiX_Installer\Staging32\Web.Debug.config"
del "%installer%\SobekCM_WiX_Installer\Staging32\Web.Release.config"
echo.

echo COMPILING 32-BIT VERSION
cd c:\windows\microsoft.net\framework64\v4.0.30319
aspnet_compiler -v /SobekCM "%installer%\SobekCM_WiX_Installer\Staging64"
echo.
echo DELETING UNNECESSARY 32-BIT FILES
del "%installer%\SobekCM_WiX_Installer\Staging64\web.config"
del "%installer%\SobekCM_WiX_Installer\Staging64\SobekCM.csproj"
del "%installer%\SobekCM_WiX_Installer\Staging64\SobekCM.csproj.user"
del "%installer%\SobekCM_WiX_Installer\Staging64\Web.Debug.config"
del "%installer%\SobekCM_WiX_Installer\Staging64\Web.Release.config"
echo.
