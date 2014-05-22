echo off
REM ******** Some values used to support alternate locations **********
set source=C:\GitRepository\SobekCM\SobekCM-Web-Application
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
echo REMOVING ANY EXISTING STAGING DIRECTORY
rmdir "%source%\Installer\SobekCM_WiX_Installer\Staging64" /s /q
echo.

echo REMOVING DIRECTORIES TO NOT MOVE TO STAGING
rmdir %iis%\SobekCM\config /s /q
rmdir %iis%\SobekCM\design /s /q
rmdir %iis%\SobekCM\dev /s /q
rmdir %iis%\SobekCM\temp /s /q
rmdir %iis%\SobekCM\mySobek\projects /s /q
rmdir %iis%\SobekCM\mySobek\inProcess /s /q
rmdir %iis%\SobekCM\obj /s /q
rmdir %iis%\SobekCM\Properties /s /q
echo.

echo REMOVING ANY EXISTING STAGING DIRECTORIES
rmdir "%source%\Installer\SobekCM_WiX_Installer\Staging32" /s /q
rmdir "%source%\Installer\SobekCM_WiX_Installer\Staging64" /s /q
echo.

echo COMPILING 32-BIT VERSION
cd c:\windows\microsoft.net\framework\v4.0.30319
aspnet_compiler -v /SobekCM "%source%\Installer\SobekCM_WiX_Installer\Staging32"

echo DELETING UNNECESSARY 32-BIT FILES
del "%source%\Installer\SobekCM_WiX_Installer\Staging32\web.config"
del "%source%\Installer\SobekCM_WiX_Installer\Staging32\SobekCM.csproj"
del "%source%\Installer\SobekCM_WiX_Installer\Staging32\SobekCM.csproj.user"
del "%source%\Installer\SobekCM_WiX_Installer\Staging32\Web.Debug.config"
del "%source%\Installer\SobekCM_WiX_Installer\Staging32\Web.Release.config"
del "%source%\Installer\SobekCM_WiX_Installer\Staging32\SobekCM.sln"
echo.

echo COMPILING 64-BiT VERSION
cd c:\windows\microsoft.net\framework64\v4.0.30319
aspnet_compiler -v /SobekCM "%source%\Installer\SobekCM_WiX_Installer\Staging64"

echo DELETING UNNECESSARY 64-BIT FILES
del "%source%\Installer\SobekCM_WiX_Installer\Staging64\web.config"
del "%source%\Installer\SobekCM_WiX_Installer\Staging64\SobekCM.csproj"
del "%source%\Installer\SobekCM_WiX_Installer\Staging64\SobekCM.csproj.user"
del "%source%\Installer\SobekCM_WiX_Installer\Staging64\Web.Debug.config"
del "%source%\Installer\SobekCM_WiX_Installer\Staging64\Web.Release.config"
del "%source%\Installer\SobekCM_WiX_Installer\Staging64\SobekCM.sln"
echo.

cd c:\


echo COPY FILES TO INCLUDE FOLDER OVER
xcopy "%source%\Installer\FilesToInclude" "%source%\Installer\SobekCM_WiX_Installer\Staging32" /s /e /i /y /q
xcopy "%source%\Installer\FilesToInclude" "%source%\Installer\SobekCM_WiX_Installer\Staging64" /s /e /i /y /q
echo.

echo CREATE SOURCE WiX FILES
"C:\Program Files (x86)\WiX Toolset v3.8\bin\heat" dir "%source%\Installer\SobekCM_WiX_Installer\Staging64" -nologo -dr Staging64 -ke -sfrag -srd -gg -out "%source%\Installer\SobekCM_WiX_Installer\FileLists\Web64.wxs" -cg SobekWebComponent_x64 -dr WEBINSTALLFOLDER -var var.WebStagingSource -t "%source%\Installer\SobekCM_WiX_Installer\post_heat_transform.xsl"
"C:\Program Files (x86)\WiX Toolset v3.8\bin\heat" dir "%source%\Installer\SobekCM_WiX_Installer\Staging32" -nologo -dr Staging32 -ke -sfrag -srd -gg -out "%source%\Installer\SobekCM_WiX_Installer\FileLists\Web32.wxs" -cg SobekWebComponent_x32 -dr WEBINSTALLFOLDER -var var.WebStagingSource -t "%source%\Installer\SobekCM_WiX_Installer\post_heat_transform.xsl"


