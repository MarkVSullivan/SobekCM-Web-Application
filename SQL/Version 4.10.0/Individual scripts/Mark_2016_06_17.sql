


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

	-- Always return all the incoming folders
	select IncomingFolderId, NetworkFolder, ErrorFolder, ProcessingFolder, Perform_Checksum_Validation, Archive_TIFF, Archive_All_Files,
		   Allow_Deletes, Allow_Folders_No_Metadata, Allow_Metadata_Updates, FolderName, Can_Move_To_Content_Folder, BibID_Roots_Restrictions,
		   F.ModuleSetID, S.SetName
	from SobekCM_Builder_Incoming_Folders F left outer join 
	     SobekCM_Builder_Module_Set S on F.ModuleSetID=S.ModuleSetID;

	-- Return all the non-scheduled type modules
	select M.ModuleID, M.[Assembly], M.Class, M.ModuleDesc, M.Argument1, M.Argument2, M.Argument3, M.[Enabled], S.ModuleSetID, S.SetName, S.[Enabled] as SetEnabled, T.TypeAbbrev, T.TypeDescription
	from SobekCM_Builder_Module M, SobekCM_Builder_Module_Set S, SobekCM_Builder_Module_Type T
	where M.ModuleSetID = S.ModuleSetID
	  and S.ModuleTypeID = T.ModuleTypeID
	  and T.TypeAbbrev <> 'SCHD'
	order by TypeAbbrev, S.SetOrder, M.[Order];


	-- Return all the scheduled type modules, with the schedule and the last run info
	with last_run_cte ( ModuleScheduleID, LastRun) as 
	(
		select ModuleScheduleID, MAX([Timestamp])
		from SobekCM_Builder_Module_Scheduled_Run
		group by ModuleScheduleID
	)
	-- Return all the scheduled type modules, along with information on when it was last run
	select M.ModuleID, M.[Assembly], M.Class, M.ModuleDesc, M.Argument1, M.Argument2, M.Argument3, M.[Enabled], S.ModuleSetID, S.SetName, S.[Enabled] as SetEnabled, T.TypeAbbrev, T.TypeDescription, C.ModuleScheduleID, C.[Enabled] as ScheduleEnabled, C.DaysOfWeek, C.TimesOfDay, L.LastRun
	from SobekCM_Builder_Module M inner join
		 SobekCM_Builder_Module_Set S on M.ModuleSetID = S.ModuleSetID inner join
		 SobekCM_Builder_Module_Type T on S.ModuleTypeID = T.ModuleTypeID inner join
		 SobekCM_Builder_Module_Schedule C on C.ModuleSetID = S.ModuleSetID left outer join
		 last_run_cte L on L.ModuleScheduleID = C.ModuleScheduleID
	where T.TypeAbbrev = 'SCHD'
	order by TypeAbbrev, S.SetOrder, M.[Order];

	-- Return all the item viewer config information
	select ItemViewTypeID, ViewType, [Order], DefaultView, MenuOrder
	from SobekCM_item_Viewer_Types
	order by ViewType;
	
	-- Return all the information about the extensions from the database
	select ExtensionID, Code, Name, CurrentVersion, IsEnabled, EnabledDate, LicenseKey, UpgradeUrl, LatestVersion 
	from SobekCM_Extension
	order by Code;

end;
GO

-- Ensure the SobekCM_Extensions_Get_All stored procedure exists
IF object_id('SobekCM_Extensions_Get_All') IS NULL EXEC ('create procedure dbo.SobekCM_Extensions_Get_All as select 1;');
GO

-- Get the list of extensions in the system
ALTER PROCEDURE SobekCM_Extensions_Get_All
AS
BEGIN
	-- Return all the information about the extensions from the database
	select ExtensionID, Code, Name, CurrentVersion, IsEnabled, EnabledDate, LicenseKey, UpgradeUrl, LatestVersion 
	from SobekCM_Extension
	order by Code;
END;
GO

-- Ensure the SobekCM_Extensions_Remove stored procedure exists
IF object_id('SobekCM_Extensions_Remove') IS NULL EXEC ('create procedure dbo.SobekCM_Extensions_Remove as select 1;');
GO

-- Remove an extension completely from the database
ALTER PROCEDURE SobekCM_Extensions_Remove
	@Code nvarchar(50)
AS
BEGIN
	delete from SobekCM_Extension
	where Code=@Code;
END;
GO

-- Ensure the SobekCM_Extensions_Add_Update stored procedure exists
IF object_id('SobekCM_Extensions_Add_Update') IS NULL EXEC ('create procedure dbo.SobekCM_Extensions_Add_Update as select 1;');
GO

-- Add information about a new extension, or update an existing extension
ALTER PROCEDURE SobekCM_Extensions_Add_Update
	@Code nvarchar(50),
	@Name nvarchar(255),
	@CurrentVersion varchar(50),
	@LicenseKey nvarchar(max),
	@UpgradeUrl nvarchar(255),
	@LatestVersion nvarchar(50)
AS
BEGIN
	-- Does this already exist?
	if ( exists ( select 1 from SobekCM_Extension where Code=@Code ))
	begin
		update SobekCM_Extension
		set Name=@Name,
			CurrentVersion=@CurrentVersion,
			LicenseKey=@LicenseKey,
			UpgradeUrl=@UpgradeUrl,
			LatestVersion=@LatestVersion
		where Code=@Code;    
	end
	else
	begin
		insert into SobekCM_Extension (Code, Name, CurrentVersion, IsEnabled, LicenseKey, UpgradeUrl, LatestVersion )
		values ( @Code, @Name, @CurrentVersion, 'false', @LicenseKey, @UpgradeUrl, @LatestVersion );
	end;
	
END;
GO

-- Ensure the SobekCM_Extensions_Set_Enable stored procedure exists
IF object_id('SobekCM_Extensions_Set_Enable') IS NULL EXEC ('create procedure dbo.SobekCM_Extensions_Set_Enable as select 1;');
GO

ALTER PROCEDURE SobekCM_Extensions_Set_Enable
	@Code nvarchar(50),
	@EnableFlag bit,
	@Message varchar(255) output
AS
BEGIN
	-- If the code is missing, do nothing
	if ( not exists ( select 1 from SobekCM_Extension where Code=@Code ))
	begin
		set @Message = 'ERROR: Unable to find matching extension in the database!';
		return;
	end;

	-- If the enable flag in the database is already set that way, do nothing
	if ( exists ( select 1 from SobekCM_Extension where Code=@Code and IsEnabled=@EnableFlag ))
	begin
		set @Message = 'Enabled flag was already set as requested for this plug-in';
		return;
	end;

	-- plug-in exists and flag is new
	if ( @EnableFlag = 'false' )
	begin
		update SobekCM_Extension set IsEnabled='false', EnabledDate=null where Code=@Code;
		set @Message='Disabled ' + @Code + ' plugin';
	end
	else
	begin
		update SobekCM_Extension set IsEnabled='true', EnabledDate=getdate() where Code=@Code;
		set @Message='Enabled ' + @Code + ' plugin';
	end;

END;
GO


GRANT EXECUTE ON [dbo].SobekCM_Extensions_Get_All to sobek_user;
GRANT EXECUTE ON [dbo].SobekCM_Extensions_Get_All to sobek_builder;
GO

GRANT EXECUTE ON [dbo].SobekCM_Extensions_Remove to sobek_user;
GRANT EXECUTE ON [dbo].SobekCM_Extensions_Remove to sobek_builder;
GO


GRANT EXECUTE ON [dbo].SobekCM_Extensions_Add_Update to sobek_user;
GRANT EXECUTE ON [dbo].SobekCM_Extensions_Add_Update to sobek_builder;
GO

GRANT EXECUTE ON [dbo].SobekCM_Extensions_Set_Enable to sobek_user;
GRANT EXECUTE ON [dbo].SobekCM_Extensions_Set_Enable to sobek_builder;
GO


