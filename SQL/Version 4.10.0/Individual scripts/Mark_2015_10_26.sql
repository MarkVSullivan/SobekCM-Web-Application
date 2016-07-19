

if ( NOT EXISTS (select * from sys.columns where Name = N'TabPage' and Object_ID = Object_ID(N'SobekCM_Settings')))
BEGIN
	ALTER TABLE [dbo].SobekCM_Settings add TabPage nvarchar(75) null;
END;
GO

if ( NOT EXISTS (select * from sys.columns where Name = N'Heading' and Object_ID = Object_ID(N'SobekCM_Settings')))
BEGIN
	ALTER TABLE [dbo].SobekCM_Settings add Heading nvarchar(75) null;
END;


if ( NOT EXISTS (select * from sys.columns where Name = N'Hidden' and Object_ID = Object_ID(N'SobekCM_Settings')))
BEGIN
	ALTER TABLE [dbo].SobekCM_Settings add Hidden bit not null default('false');
END;
GO

if ( NOT EXISTS (select * from sys.columns where Name = N'Reserved' and Object_ID = Object_ID(N'SobekCM_Settings')))
BEGIN
	ALTER TABLE [dbo].SobekCM_Settings add Reserved smallint not null default(0);
END;
GO

if ( NOT EXISTS (select * from sys.columns where Name = N'Help' and Object_ID = Object_ID(N'SobekCM_Settings')))
BEGIN
	ALTER TABLE [dbo].SobekCM_Settings add Help varchar(max) null;
END;
GO

if ( NOT EXISTS (select * from sys.columns where Name = N'Options' and Object_ID = Object_ID(N'SobekCM_Settings')))
BEGIN
	ALTER TABLE [dbo].SobekCM_Settings add Options varchar(max) null;
END;
GO

if ( NOT EXISTS (select * from sys.columns where Name = N'SettingID' and Object_ID = Object_ID(N'SobekCM_Settings')))
BEGIN
	ALTER TABLE [dbo].SobekCM_Settings add SettingID int identity(1,1);
END;
GO

if ( NOT EXISTS (select * from sys.columns where Name = N'Dimensions' and Object_ID = Object_ID(N'SobekCM_Settings')))
BEGIN
	ALTER TABLE [dbo].SobekCM_Settings add Dimensions varchar(100);
END;
GO

-- Gets the list of all system-wide settings from the database, including the full list of all
-- metadata search fields, possible workflows, and all disposition data
ALTER PROCEDURE [dbo].[SobekCM_Get_Settings]
	@IncludeAdminViewInfo bit 
AS
begin

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Get all the standard SobekCM settings
	if ( @IncludeAdminViewInfo = 'true' )
	begin
		select Setting_Key, Setting_Value, TabPage, Heading, Hidden, Reserved, Help, Options, SettingID, Dimensions
		from SobekCM_Settings
		where Hidden = 'false'
		order by TabPage, Heading, Setting_Key;
	end 
	else
	begin
		select Setting_Key, Setting_Value
		from SobekCM_Settings;
	end;

	-- Return all the metadata search fields
	select MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse
	from SobekCM_Metadata_Types
	order by DisplayTerm;

	-- Return all the possible workflow types
	select WorkFlowID, WorkFlowName, WorkFlowNotes, Start_Event_Number, End_Event_Number, Start_And_End_Event_Number, Start_Event_Desc, End_Event_Desc
	from Tracking_WorkFlow;

	-- Return all the possible disposition options
	select DispositionID, DispositionFuture, DispositionPast, DispositionNotes
	from Tracking_Disposition_Type;

end;
GO

-- Update all the settings
update SobekCM_Settings set TabPage='Deprecated', Heading='Deprecated', Hidden=0, Reserved=0, Options='true|false', Help='Help for Allow Page Image File Management' where Setting_Key='Allow Page Image File Management'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Server Settings', Hidden=0, Reserved=0, Help='Server share for the web application''s network location.\n\nExample: ''\\\\lib-sandbox\\Production\\''' where Setting_Key='Application Server Network'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Server Settings', Hidden=0, Reserved=0, Help='Base URL which points to the web application.\n\nExamples: ''http://localhost/sobekcm/'', ''http://ufdc.ufl.edu/'', etc..' where Setting_Key='Application Server URL'
update SobekCM_Settings set TabPage='Builder', Heading='Archive Settings', Hidden=0, Reserved=0, Help='Network location for the archive drop box.  If this is set to a value, the builder/bulk loader will place a copy of the package in this folder for archiving purposes.  This folder is where any of your archiving processes should look for new packages.' where Setting_Key='Archive DropBox'
update SobekCM_Settings set TabPage='Builder', Heading='Builder Settings', Hidden=0, Reserved=0, Options='true|false', Help='Flag indicates if the page turner view should be added automatically to all items with four or more pages.' where Setting_Key='Builder Add PageTurner ItemViewer'
update SobekCM_Settings set TabPage='Deprecated', Heading='Deprecated', Hidden=0, Reserved=0, Help='IIS web log location (usually a network share) for the builder to read the logs and add the usage statistics to the database.' where Setting_Key='Builder IIS Logs Directory'
update SobekCM_Settings set TabPage='Builder', Heading='Status', Hidden=0, Reserved=0, Help='Help for Builder Last Message' where Setting_Key='Builder Last Message'
update SobekCM_Settings set TabPage='Builder', Heading='Status', Hidden=0, Reserved=0, Help='Help for Builder Last Run Finished' where Setting_Key='Builder Last Run Finished'
update SobekCM_Settings set TabPage='Builder', Heading='Builder Settings', Hidden=0, Reserved=0, Options='10|30|365|99999', Help='Number of days the SobekCM Builder logs are retained.' where Setting_Key='Builder Log Expiration in Days'
update SobekCM_Settings set TabPage='Builder', Heading='Status', Hidden=0, Reserved=0, Options='STANDARD OPERATION|PAUSE REQUESTED|ABORT REQUESTED|NO BUILDER REQUESTED ', Help='Last flag set when the builder/bulk loader ran.' where Setting_Key='Builder Operation Flag'
update SobekCM_Settings set TabPage='Builder', Heading='Builder Settings', Hidden=0, Reserved=0, Options='15|60|300|600', Help='Number of seconds the builder remains idle before checking for new incoming package again.' where Setting_Key='Builder Seconds Between Polls'
update SobekCM_Settings set TabPage='Builder', Heading='Builder Settings', Hidden=0, Reserved=0, Options='true|false', Help='Flag indicates is usage emails should be sent automatically after the stats usage has been calculated and added to the database.' where Setting_Key='Builder Send Usage Emails'
update SobekCM_Settings set TabPage='Builder', Heading='Status', Hidden=0, Reserved=0, Help='Help for Builder Version' where Setting_Key='Builder Version'
update SobekCM_Settings set TabPage='Deprecated', Heading='Deprecated', Hidden=0, Reserved=0, Help='URL for the AppFabric Cache host machine, if a caching server/cluster is in use in this system.' where Setting_Key='Caching Server'
update SobekCM_Settings set TabPage='General Settings', Heading='Search Settings', Hidden=0, Reserved=0, Options='true|false', Help='When this is set to TRUE, users can remove a single search term from their current search.  Setting this to FALSE, makes the display slightly cleaner.' where Setting_Key='Can Remove Single Search Term'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='Online Management Settings', Hidden=0, Reserved=0, Options='true|false', Help='Flag dictates if users can submit items online, or if this is disabled in this system.' where Setting_Key='Can Submit Edit Online'
update SobekCM_Settings set TabPage='Deprecated', Heading='Deprecated', Hidden=0, Reserved=0, Options='true|false', Help='Flag dictates if users can submit items online, or if this is disabled in this system.' where Setting_Key='Can Submit Items Online'
update SobekCM_Settings set TabPage='Builder', Heading='Builder Settings', Hidden=0, Reserved=0, Options='true|false', Help='Flag dictates if users can submit items online, or if this is disabled in this system.' where Setting_Key='Convert Office Files to PDF'
update SobekCM_Settings set TabPage='Builder', Heading='Builder Settings', Hidden=0, Reserved=0, Options='true|false', Help='Flag indicates if the builder/bulk loader should create the MARC feed by default when operating in background mode.' where Setting_Key='Create MARC Feed By Default'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='System Settings', Hidden=0, Reserved=0, Options='true|false', Help='Flag indicates if more refined user permissions can be assigned, such as if a user can edit behaviors of an item in a collection vs. a more general flag that says a RequestSpecificValues.Current_User can make all changes to an item in a collection.' where Setting_Key='Detailed User Permissions'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='System Settings', Hidden=0, Reserved=0, Options='true|false', Help='Flag indicates if non system administrators are temporarily barred from logging on.' where Setting_Key='Disable Standard User Logon Flag'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='System Settings', Hidden=0, Reserved=0, Help='Message displayed if non syste administrators are temporarily barred from logging on.' where Setting_Key='Disable Standard User Logon Message'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='System Settings', Hidden=0, Reserved=0, Help='URL for the document-level solr index.\n\nExample: ''http://localhost:8080/documents''' where Setting_Key='Document Solr Index URL'
update SobekCM_Settings set TabPage='General Settings', Heading='Email Settings', Hidden=0, Reserved=0, Dimensions='300', Help='Email address that emails from this system should utilize' where Setting_Key='Email Default From Address'
update SobekCM_Settings set TabPage='General Settings', Heading='Email Settings', Hidden=0, Reserved=0, Dimensions='300', Help='Display name to associate with emails sent from this system (otherwise the instance/portal name will be used)' where Setting_Key='Email Default From Name'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Email Setup', Hidden=0, Reserved=0, Options='DATABASE MAIL|SMTP DIRECT', Help='Indicated whether the database mail system or the SMTP direct email system should be utilizied' where Setting_Key='Email Method'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Email Setup', Hidden=0, Reserved=0, Dimensions='70', Help='If direct SMTP email sending is used, the port to utilize.  This must be numeric.' where Setting_Key='Email SMTP Port'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Email Setup', Hidden=0, Reserved=0, Help='If direct SMTP email sending is used, the server name to send emails to.' where Setting_Key='Email SMTP Server'
update SobekCM_Settings set TabPage='General Settings', Heading='Search Settings', Hidden=0, Reserved=0, Options='true|false', Help='Flag determines if the facets are collapsible like an accordian, or if they all start fully expanded.' where Setting_Key='Facets Collapsible'
update SobekCM_Settings set TabPage='Florida SUS Settings', Heading='General Settings', Hidden=1, Reserved=0, Help='Location for the builder/bulk loader to look for incoming Florida Dark Archive XML reports to process and add to the history of digital resources.' where Setting_Key='FDA Report DropBox'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='General Settings', Hidden=0, Reserved=0, Help='Regular expressions used to exclude files from being added by default to the downloads of resources.\n\nExample: ''((.*?)\\.(jpg|tif|jp2|jpx|bmp|jpeg|gif|png|txt|pro|mets|db|xml|bak|job)$|qc_error.html)''' where Setting_Key='Files To Exclude From Downloads'
update SobekCM_Settings set TabPage='General Settings', Heading='Help Settings', Hidden=0, Reserved=0, Help='URL used for the help pages when users request help on metadata elements during online submit and editing.\n\nExample (and default): ''http://sobekrepository.org/''' where Setting_Key='Help Metadata URL'
update SobekCM_Settings set TabPage='General Settings', Heading='Help Settings', Hidden=0, Reserved=0, Help='URL used for the main help pages about this system''s basic functionality.\n\nExample (and default): ''http://sobekrepository.org/''' where Setting_Key='Help URL'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Server Settings', Hidden=0, Reserved=0, Help='Network location to the content for all of the digital resources (images, metadata, etc.).\n\nExample: ''C:\\inetpub\\wwwroot\\UFDC Web\\SobekCM\\content\\'' or ''\\\\ufdc-images\\content\\''' where Setting_Key='Image Server Network'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Server Settings', Hidden=0, Reserved=0, Help='URL which points to the digital resource images.\n\nExample: ''http://localhost/sobekcm/content/'' or ''http://ufdcimages.uflib.ufl.edu/''' where Setting_Key='Image Server URL'
update SobekCM_Settings set TabPage='General Settings', Heading='Instance Settings', Hidden=0, Reserved=0, Options='true|false', Help='This option controls whether a PARTNERS option appears on the main system home page, assuming there are multiple institutional aggregations.' where Setting_Key='Include Partners On System Home'
update SobekCM_Settings set TabPage='General Settings', Heading='Instance Settings', Hidden=0, Reserved=0, Options='true|false', Help='This option controls whether a TREE VIEW option appears on the main system home page which displays all the active aggregations hierarchically in a tree view.' where Setting_Key='Include TreeView On System Home'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='Image Settings', Hidden=0, Reserved=0, Dimensions='60', Help='Restriction on the size of the jpeg page images'' height (in pixels) when generated automatically by the builder/bulk loader.\n\nDefault: ''1000''' where Setting_Key='JPEG Height'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='Image Settings', Hidden=0, Reserved=0, Dimensions='60', Help='Restriction on the size of the jpeg page images'' width (in pixels) when generated automatically by the builder/bulk loader.\n\nDefault: ''630''' where Setting_Key='JPEG Width'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Server Settings', Hidden=0, Reserved=0, Help='URL for the Aware JPEG2000 Server for displaying and zooming into JPEG2000 images.' where Setting_Key='JPEG2000 Server'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Server Settings', Hidden=0, Reserved=0, Options='Built-In IIPImage|None', Help='Type of the JPEG2000 server found at the URL above.' where Setting_Key='JPEG2000 Server Type'
update SobekCM_Settings set TabPage='Builder', Heading='Builder Settings', Hidden=0, Reserved=0, Help='Kakadu JPEG2000 script will override the specifications used when creating zoomable images.\n\nIf this is blank, the default specifications will be used which match those used by the National Digital Newspaper Program and University of Florida Digital Collections.' where Setting_Key='Kakadu JPEG2000 Create Command'
update SobekCM_Settings set TabPage='Deprecated', Heading='Deprecated', Hidden=0, Reserved=0, Help='Network location for the share within which the builder/bulk loader logs should be copied to become web accessible.\n\nExample: ''\\\\lib-sandbox\\Design\\extra\\logs\\''' where Setting_Key='Log Files Directory'
update SobekCM_Settings set TabPage='Deprecated', Heading='Deprecated', Hidden=0, Reserved=0, Help='URL for the builder/bulk loader logs files.\n\nExample: ''http://ufdc.ufl.edu/design/extra/logs/''' where Setting_Key='Log Files URL'
update SobekCM_Settings set TabPage='Builder', Heading='Builder Settings', Hidden=0, Reserved=0, Help='This is the network location to the SobekCM Builder''s main incoming folder.\n\nThis is used by the SMaRT tool when doing bulk imports from spreadsheet or MARC records.' where Setting_Key='Main Builder Input Folder'
update SobekCM_Settings set TabPage='Florida SUS Settings', Heading='General Settings', Hidden=1, Reserved=0, Help='Florida SUS state-wide catalog base URL for determining the number of physical holdings which match a given search.\n\nExample: ''http://solrcits.fcla.edu/citsZ.jsp?type=search&base=uf''' where Setting_Key='Mango Union Search Base URL'
update SobekCM_Settings set TabPage='Florida SUS Settings', Heading='General Settings', Hidden=1, Reserved=0, Help='Text to display the number of hits found in the Florida SUS-wide catalog.\n\nUse the value ''%1'' in the string where the number of hits should be inserted.\n\nExample: ''%1 matches found in the statewide catalog''' where Setting_Key='Mango Union Search Text'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='Metadata Settings', Hidden=0, Reserved=0, Dimensions='60', Help='Cataloging source code for the 040 field, ( for example ''FUG'' for University of Florida )' where Setting_Key='MARC Cataloging Source Code'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='Metadata Settings', Hidden=0, Reserved=0, Dimensions='60', Help='Location code for the 852 |a - if none is given the system abbreviation will be used. Otherwise, the system abbreviation will be put in the 852 |b field.' where Setting_Key='MARC Location Code'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='Metadata Settings', Hidden=0, Reserved=0, Help='Agency responsible for reproduction, or primary agency associated with the SobekCM instance ( for the added 533 |c field )\n\nThis 533 is not added for born digital items.' where Setting_Key='MARC Reproduction Agency'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='Metadata Settings', Hidden=0, Reserved=0, Help='Place of reproduction, or primary location associated with the SobekCM instance ( for the added 533 |b field ).\n\nThis 533 is not added for born digital items.' where Setting_Key='MARC Reproduction Place'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='Metadata Settings', Hidden=0, Reserved=0, Help='XSLT file to use as a final transform, after the standard MarcXML file is written.\n\nThis only affects generated MarcXML ( for the feeds and OAI ) not the dispayed in-system MARC ( as of January 2015 ).  This file should appear in the config/users folder.' where Setting_Key='MARC XSLT File'
update SobekCM_Settings set TabPage='Builder', Heading='Builder Settings', Hidden=0, Reserved=0, Help='Network location or share where any geneated MarcXML feed should be written.\n\nExample: ''\\\\lib-sandbox\\Data\\''' where Setting_Key='MarcXML Feed Location'
update SobekCM_Settings set TabPage='Builder', Heading='Builder Settings', Hidden=0, Reserved=0, Help='If you wish to utilize an OCR engine in the builder/bulk loader, add the command-line call to the engine here.\n\nUse %1 as a place holder for the ingoing image file name and %2 as a placeholder for the output text file name.\n\nExample: ''C:\\OCR\\Engine.exe -in %1 -out %2''' where Setting_Key='OCR Engine Command'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Server Settings', Hidden=0, Reserved=0, Help='URL for the resource-level solr index used when searching for matching pages within a single document.\n\nExample: ''http://localhost:8080/pages''' where Setting_Key='Page Solr Index URL'
update SobekCM_Settings set TabPage='Builder', Heading='Archive Settings', Hidden=0, Reserved=0, Help='Regular expression indicates which files should be deleted AFTER being archived by the builder/bulk loader.\n\nExample: ''(.*?)\\.(tif)''' where Setting_Key='PostArchive Files To Delete'
update SobekCM_Settings set TabPage='Builder', Heading='Archive Settings', Hidden=0, Reserved=0, Help='Regular expression indicates which files should be deleted BEFORE being archived by the builder/bulk loader.\n\nExample: ''(.*?)\\.(QC.jpg)''' where Setting_Key='PreArchive Files To Delete'
update SobekCM_Settings set TabPage='General Settings', Heading='Email Settings', Hidden=0, Reserved=0, Help='Email address which receives notification if personal information (such as Social Security Numbers) is potentially found while loading or post-processing an item.\n\nIf you are using multiple email addresses, seperate them with a semi-colon.\n\nExample: ''person1@corp.edu;person2@corp.edu''' where Setting_Key='Privacy Email Address'
update SobekCM_Settings set TabPage='General Settings', Heading='Email Settings', Hidden=0, Reserved=0, Options='Always|Never', Help='Flag indicates when emails should be sent after new item aggregations are added through the web interface.' where Setting_Key='Send Email On Added Aggregation'
update SobekCM_Settings set TabPage='Deprecated', Heading='Deprecated', Hidden=0, Reserved=0, Options='true|false', Help='Some system settings are only applicable to institutions which are part of the Florida State University System.  Setting this value to TRUE will show these settings, while FALSE will suppress them.\n\nIf this value is changed, you willl need to save the settings for it to reload and reflect the change.' where Setting_Key='Show Florida SUS Settings'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Server Settings', Hidden=0, Reserved=0, Dimensions='200', Help='IP address for the web server running this web repository software.\n\nThis is used for setting restricted or dark material to only be available for the web server, which then acts as a proxy/web server to serve that content to authenticated users.' where Setting_Key='SobekCM Web Server IP'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='System Settings', Hidden=0, Reserved=0, Help='License code (encrypted) for spreadsheet library' where Setting_Key='Spreadsheet Library License'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Caching Settings', Hidden=0, Reserved=0, Help='Location where the static files are located for providing the full citation and text for indexing, either on the same server as the web application or as a network share.\n\nIt is recommended that these files be on the same server as the web server, rather than remote storage, to increase the speed in which requests from search engine indexers can be fulfilled.\n\nExample: ''C:\\inetpub\\wwwroot\\UFDC Web\\SobekCM\\data\\''.' where Setting_Key='Static Pages Location'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Server Settings', Hidden=0, Reserved=0, Help='Indicates the general source of all the static resources, such as javascript, system default stylesheets, images, and included libraries.\n\nUsing CDN will result in better performance, but can only be used when users will have access to the database.\n\nThis actually indicates which configuration file to read to determine the base location of the default resources.' where Setting_Key='Static Resources Source'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Caching Settings', Hidden=0, Reserved=0, Options='true|false', Help='Flag indicates if the basic usage and item count information should be cached for up to 24 hours as static XML files written in the web server''s temp directory.\n\nThis should be enabled if your library is quite large as it can take a fair amount of time to retrieve this information and these screens are made available for search engine index robots for indexing.' where Setting_Key='Statistics Caching Enabled'
update SobekCM_Settings set TabPage='General Settings', Heading='Instance Settings', Hidden=0, Reserved=0, Dimensions='100', Help='Base abbreviation to be used when the system refers to itself to the RequestSpecificValues.Current_User, such as the main tabs to take a user to the home pages.\n\nThis abbreviation should be kept as short as possible.\n\nExamples: ''UFDC'', ''dLOC'', ''Sobek'', etc..' where Setting_Key='System Base Abbreviation'
update SobekCM_Settings set TabPage='General Settings', Heading='Instance Settings', Hidden=0, Reserved=0, Help='Overall name of the system, to be used when creating MARC records and in several other locations.' where Setting_Key='System Base Name'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Server Settings', Hidden=0, Reserved=0, Help='Base URL which points to the web application.\n\nExamples: ''http://localhost/sobekcm/'', ''http://ufdc.ufl.edu/'', etc..' where Setting_Key='System Base URL'
update SobekCM_Settings set TabPage='General Settings', Heading='Instance Settings', Hidden=0, Reserved=0, Help='Default system user interface language.  If the user''s HTML request does not include a language supported by the interface or which does not include specific translations for a field, this default language is utilized.' where Setting_Key='System Default Language'
update SobekCM_Settings set TabPage='General Settings', Heading='Email Settings', Hidden=0, Reserved=0, Help='Default email address for the system, which is sent emails when users opt to contact the administrators.\n\nThis can be changed for individual aggregations but at least one email is required for the overall system.\n\nIf you are using multiple email addresses, seperate them with a semi-colon.\n\nExample: ''person1@corp.edu;person2@corp.edu''' where Setting_Key='System Email'
update SobekCM_Settings set TabPage='General Settings', Heading='Email Settings', Hidden=0, Reserved=0, Help='Email address used when a critical system error occurs which may require investigation or correction.\n\nIf you are using multiple email addresses, seperate them with a semi-colon.\n\nExample: ''person1@corp.edu;person2@corp.edu''' where Setting_Key='System Error Email'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='Image Settings', Hidden=0, Reserved=0, Dimensions='60', Help='Restriction on the size of the page image thumbnails'' height (in pixels) when generated automatically by the builder/bulk loader.\n\nDefault: ''300''' where Setting_Key='Thumbnail Height'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='Image Settings', Hidden=0, Reserved=0, Dimensions='60', Help='Restriction on the size of the page image thumbnails'' width (in pixels) when generated automatically by the builder/bulk loader.\n\nDefault: ''150''' where Setting_Key='Thumbnail Width'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='Online Management Settings', Hidden=0, Reserved=0, Dimensions='600|3', Help='List of non-image extensions which are allowed to be uploaded into a digital resource.\n\nList should be the extensions, with the period, separated by commas.\n\nExample: .aif,.aifc,.aiff,.au,.avi,.bz2,.c,.c++,.css,.dbf,.ddl,...' where Setting_Key='Upload File Types'
update SobekCM_Settings set TabPage='Digital Resource Settings', Heading='Online Management Settings', Hidden=0, Reserved=0, Dimensions='600', Help='List of page image extensions which are allowed to be uploaded into a digital resource to display as page images.\n\nList should be the extensions, with the period, separated by commas.\n\nExample: .txt,.tif,.jpg,.jp2,.pro' where Setting_Key='Upload Image Types'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Server Settings', Hidden=0, Reserved=0, Help='Location where packages are built by users during online submissions and metadata updates.\n\nThis generally needs to be on the web server and have appropriate access for read/write.\n\nIf nothing is indicated in this field, the system will automatically use the ''mySobek\\InProcess'' subfolder under the web application.' where Setting_Key='Web In Process Submission Location'
update SobekCM_Settings set TabPage='System / Server Settings', Heading='Caching Settings', Hidden=0, Reserved=0, Options='0|1|2|3|5|10|15', Help='This setting controls how long the client''s browser is instructed to cache the served web page.\n\nSetting this value higher removes the round-trip when requesting a recently requested page.  It also means that some changes may not be reflected until the refresh button is pressed.\n\nIn general, this setting is only applied to public-style pages, and not personalized pages, such as the bookshelf views.' where Setting_Key='Web Output Caching Minutes'


