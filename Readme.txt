-----------------------------------------------------------------------

         SobekCM Web Repository and Builder Solutions Readme

-----------------------------------------------------------------------


This package includes two Visual Studio 2012 solutions:

1) SobekCM Web Repository, which is located under the SobekCM folder.  

2) SobekCM Builder, which is located here at the top level.


-----------------------------------------------------------------------

Design folders:

To run the web repository successfully, you will need to copy the design
files from your instance of SobekCM.


-----------------------------------------------------------------------

More information:

For more information about this system, please see the help pages linked
off of sobek.ufl.edu


-----------------------------------------------------------------------

WiX Toolset:

 The SobekCM_Builder_WiX_Installer: project will cause warnings and not open in your Visual 
Studio unless you install the appropriate WiX Toolset.  This currently utilized Wix 3.6 
library due to issues with working with Wix3.7

You could also choose to ignore the warning if you are not going to be making a MSI file 
to install a custom version of the Builder.

-----------------------------------------------------------------------

Mark Sullivan ( 10/18/2013 )