echo off
REM ******** Some values used to support alternate locations **********
set source=C:\GitRepository\SobekCM-Web-Application
set iis=C:\inetpub\wwwroot\SobekCM

echo CREATE SOLR WiX FILE
"C:\Program Files (x86)\WiX Toolset v3.8\bin\heat" dir "%source%\Installer\SobekCM_WiX_Installer\Solr" -nologo -dr Solr -ke -sfrag -suid -gg -out "%source%\Installer\SobekCM_WiX_Installer\FileLists\Solr.wxs" -cg SobekSolrComponent -dr SOLRINSTALLFOLDER -var var.SolrSource