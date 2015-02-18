echo off
REM ******** Some values used to support alternate locations **********
set source=C:\GitRepository\SobekCM-Web-Application
set iis=C:\inetpub\wwwroot\SobekCM
echo.
echo MINIFYING JAVASCRIPT AND CSS
cd "C:\Program Files (x86)\Microsoft\Microsoft Ajax Minifier"
echo ....sobekcm.css
ajaxminifier "%source%\SobekCM\default\SobekCM.css" -o "%source%\SobekCM\default\SobekCM.min.css" -silent
echo ....sobekcm_item.css
ajaxminifier "%source%\SobekCM\default\SobekCM_Item.css" -o "%source%\SobekCM\default\SobekCM_Item.min.css" -silent
echo ....sobekcm_mysobek.css
ajaxminifier "%source%\SobekCM\default\SobekCM_MySobek.css" -o "%source%\SobekCM\default\SobekCM_MySobek.min.css" -silent
echo ....sobekcm_admin.css
ajaxminifier "%source%\SobekCM\default\SobekCM_Admin.css" -o "%source%\SobekCM\default\SobekCM_Admin.min.css" -silent
echo ....sobekcm_metadata.css
ajaxminifier "%source%\SobekCM\default\SobekCM_Metadata.css" -o "%source%\SobekCM\default\SobekCM_Metadata.min.css" -silent
echo ....sobekcm_full.js
ajaxminifier "%source%\SobekCM\default\scripts\sobekcm_full.js" -o "%source%\SobekCM\default\scripts\sobekcm_full.min.js" -silent
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
echo REMOVING ANY EXISTING STAGING DIRECTORY
rmdir "%source%\SobekCM_Web_WiX_Installer\Staging64" /s /q
echo.
echo REMOVING DIRECTORIES TO NOT MOVE TO STAGING
rmdir %iis%\SobekCM\config\user /s /q
rmdir %iis%\SobekCM\design /s /q
rmdir %iis%\SobekCM\dev /s /q
rmdir %iis%\SobekCM\temp /s /q
rmdir %iis%\SobekCM\mySobek\projects /s /q
rmdir %iis%\SobekCM\mySobek\inProcess /s /q
rmdir %iis%\SobekCM\obj /s /q
rmdir %iis%\SobekCM\Properties /s /q
rmdir %iis%\SobekCM\content /s /q
echo.
cd c:\windows\microsoft.net\framework64\v4.0.30319
aspnet_compiler -v /SobekCM "%source%\SobekCM_Web_WiX_Installer\Staging64"
echo DELETING UNNECESSARY FILES
del "%source%\SobekCM_Web_WiX_Installer\Staging64\web.config"
del "%source%\SobekCM_Web_WiX_Installer\Staging64\SobekCM.csproj"
del "%source%\SobekCM_Web_WiX_Installer\Staging64\SobekCM.csproj.user"
del "%source%\SobekCM_Web_WiX_Installer\Staging64\Web.Debug.config"
del "%source%\SobekCM_Web_WiX_Installer\Staging64\Web.Release.config"
del "%source%\SobekCM_Web_WiX_Installer\Staging64\SobekCM.sln"
del "%source%\SobekCM_Web_WiX_Installer\Staging64\config\sobekcm.config"
echo.

cd c:\
