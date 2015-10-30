

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
