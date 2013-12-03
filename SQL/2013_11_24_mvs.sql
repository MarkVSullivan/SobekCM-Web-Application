/* NEW PROCEDURES TO REPLACE OLD ONES *
** PalmmRightsMD_Save_Access_Embargo_UMI --> SobekCM_RightsMD_Save_Access_Embargo_UMI
** Edit_OCR_Progress --> Tracking_Edit_OCR_Progress
** Builder_Get_Minimum_Item_Information -->
**                      SobekCM_Builder_Get_Minimum_Item_Information
** Importer_Load_Lookup_Tables -->
**                       SobekCM_Importer_Load_Lookup_Tables
** Save_Item_Complete_KML --> SobekCM_Save_Item_Complete_KML
**
** DELETE: TEMP_Get_Group_Info_By_BibID
*/


alter table SobekCM_Builder_Incoming_Folders add Can_Move_To_Content_Folder bit null;
alter table SobekCM_Builder_Incoming_Folders add BibID_Roots_Restrictions varchar(255) not null default('');
GO

alter table SobekCM_Builder_Incoming_Folders drop column Contains_Institutional_Folders;
GO

-- Add a new incoming folder for the builder/bulk loader, or edit
-- an existing incoming folder (by incoming folder id)
ALTER PROCEDURE [dbo].[SobekCM_Builder_Incoming_Folder_Edit]
	@IncomingFolderId int,
	@NetworkFolder varchar(255),
	@ErrorFolder varchar(255),
	@ProcessingFolder varchar(255),
	@Perform_Checksum_Validation bit,
	@Archive_TIFF bit,
	@Archive_All_Files bit,
	@Allow_Deletes bit,
	@Allow_Folders_No_Metadata bit,
	@FolderName nvarchar(150),
	@Can_Move_To_Content_Folder bit,
	@BibID_Roots_Restrictions varchar(255),
	@NewID int output
AS 
BEGIN

	-- Is this a new incoming folder?
	if (( select COUNT(*) from SobekCM_Builder_Incoming_Folders where IncomingFolderId=@IncomingFolderId ) = 0 )
	begin	
		-- Insert new incoming folder
		insert into SobekCM_Builder_Incoming_Folders ( NetworkFolder, ErrorFolder, ProcessingFolder, Perform_Checksum_Validation, Archive_TIFF, Archive_All_Files, Allow_Deletes, Allow_Folders_No_Metadata, FolderName, Allow_Metadata_Updates, Can_Move_To_Content_Folder, BibID_Roots_Restrictions )
		values ( @NetworkFolder, @ErrorFolder, @ProcessingFolder, @Perform_Checksum_Validation, @Archive_TIFF, @Archive_All_Files, @Allow_Deletes, @Allow_Folders_No_Metadata, @FolderName, 'true', @Can_Move_To_Content_Folder, @BibID_Roots_Restrictions );
		
		-- Save the new id
		set @NewID = @@Identity;
	end
	else
	begin
		-- update existing incoming folder
		update SobekCM_Builder_Incoming_Folders
		set NetworkFolder=@NetworkFolder, ErrorFolder=@ErrorFolder, ProcessingFolder=@ProcessingFolder, 
			Perform_Checksum_Validation=@Perform_Checksum_Validation, Archive_TIFF=@Archive_TIFF, 
			Archive_All_Files=@Archive_All_Files, Allow_Deletes=@Allow_Deletes, 
			Allow_Folders_No_Metadata=@Allow_Folders_No_Metadata, FolderName=@FolderName,
			BibID_Roots_Restrictions=@BibID_Roots_Restrictions, Can_Move_To_Content_Folder=@Can_Move_To_Content_Folder
		where IncomingFolderId = @IncomingFolderId;
		
		-- Just return the same id
		set @NewID = @IncomingFolderId;	
	end;
END;
GO

update SobekCM_Builder_Incoming_Folders
set Can_Move_To_Content_Folder = 'true'
GO

CREATE TABLE [dbo].[SobekCM_Builder_Log](
	[BuilderLogID] [bigint] IDENTITY(1,1) NOT NULL,
	[RelatedBuilderLogID] [bigint] NULL,
	[LogDate] [datetime] NULL,
	[BibID_VID] [varchar](16) NULL,
	[LogType] [varchar](25) NULL,
	[LogMessage] [varchar](2000) NULL,
	SuccessMessage varchar(500) null,
	METS_Type varchar(50) null,
 CONSTRAINT [PK_SobekCM_Builder_Log] PRIMARY KEY CLUSTERED 
(
	[BuilderLogID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[SobekCM_Builder_Log]  WITH CHECK ADD  CONSTRAINT [FK_Self_SobekCM_Builder_Log] FOREIGN KEY([RelatedBuilderLogID])
REFERENCES [dbo].[SobekCM_Builder_Log] ([BuilderLogID])
GO


-- Procedure to remove expired log files
CREATE PROCEDURE SobekCM_Builder_Expire_Log_Entries
	@Retain_For_Days int
AS 
BEGIN
	-- Calculate the expiration date time
	declare @expiredate datetime;
	set @expiredate = dateadd(day, (-1 * @Retain_For_Days), getdate());
	set @expiredate = dateadd(hour, -1 * datepart(hour,@expiredate), @expiredate);
	
	-- Delete all non-errors logs from before this time
	delete from SobekCM_Builder_Log
	where LogDate <= @expiredate;
END;
GO

ALTER PROCEDURE SobekCM_Builder_Add_Log
	@RelatedBuilderLogID bigint,
	@BibID_VID varchar(16),
	@LogType varchar(25),
	@LogMessage varchar(2000),
	@METS_Type varchar(50),
	@BuilderLogID bigint output
AS
BEGIN

	insert into SobekCM_Builder_Log ( RelatedBuilderLogID, LogDate, BibID_VID, LogType, LogMessage )
	values ( @RelatedBuilderLogID, getdate(), @BibID_VID, @LogType, @LogMessage );
	
	set @BuilderLogID = @@IDENTITY;
END;
GO

GRANT EXECUTE ON SobekCM_Builder_Expire_Log_Entries to sobek_builder;
GRANT EXECUTE ON SobekCM_Builder_Add_Log to sobek_builder;
GO

-- Adds an OCR Workflow progress for a specific volume.
-- Created:		04/08/09
-- Project:		OCR Automation Project-
-- Developer:	Tom Bielicke
CREATE PROCEDURE [dbo].[Tracking_Edit_OCR_Progress]
	@BibID		varchar(10),
	@VIDNumber	varchar (5)	
AS
begin

	exec Tracking_Add_Workflow_Once_Per_Day @bibid, @VIDNumber, '', '', 6, '';
	
end;
GO

grant execute on [Tracking_Edit_OCR_Progress] to sobek_user;
GO


CREATE PROCEDURE [dbo].[SobekCM_RightsMD_Save_Access_Embargo_UMI]
	@ItemID int,
	@Original_AccessCode varchar(25),
	@EmbargoEnd date,
	@UMI varchar(20)
AS
BEGIN

	-- Only insert if it doesn't exist
	if ( exists ( select * from Tracking_Item where ItemID=@ItemID ))
	begin
		--update existing, not updating 'original_' columns
		update Tracking_Item
		set EmbargoEnd = @EmbargoEnd, UMI=@UMI
		where ItemID=@ItemID;
	end
	else
	begin
		-- Insert ALL the data
		insert into Tracking_Item ( ItemID, Original_AccessCode, Original_EmbargoEnd, EmbargoEnd, UMI )
		values ( @ItemID, @Original_AccessCode, @EmbargoEnd, @EmbargoEnd, @UMI );
	end;
END;
GO

grant execute on [SobekCM_RightsMD_Save_Access_Embargo_UMI] to sobek_user;
grant execute on [SobekCM_RightsMD_Save_Access_Embargo_UMI] to sobek_builder;
GO


CREATE PROCEDURE [dbo].[SobekCM_Builder_Get_Minimum_Item_Information]
	@bibid varchar(10),
	@vid varchar(5)
AS
begin

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get the item id and mainthumbnail
	select I.ItemID, I.MainThumbnail, I.IP_Restriction_Mask, I.Born_Digital, G.ItemCount, I.Dark
	from SobekCM_Item I, SobekCM_Item_Group G
	where ( I.VID = @vid )
	  and ( G.BibID = @bibid )
	  and ( I.GroupID = G.GroupID );

	-- Get the links to the aggregations
	select A.Code, A.Name
	from SobekCM_Item I, SobekCM_Item_Group G, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
	where ( I.VID = @vid )
	  and ( G.BibID = @bibid )
	  and ( I.GroupID = G.GroupID )
	  and ( I.ItemID = L.ItemID )
	  and ( L.AggregationID = A.AggregationID );

end;

GO

GRANT EXECUTE ON SobekCM_Builder_Get_Minimum_Item_Information to sobek_builder;
GRANT EXECUTE ON SobekCM_Builder_Get_Minimum_Item_Information to sobek_user;
GO


-- This procedure calls other procedures to load data into the 
-- various look up tables used in the application.
CREATE PROCEDURE [dbo].[SobekCM_Importer_Load_Lookup_Tables]
 AS
begin

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get the list of items with identifiers for match checking
	select G.GroupID, ItemID, BibID, VID, G.ALEPH_Number, G.OCLC_Number, GroupTitle, Title
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( G.GroupID = I.GroupID );
	
	-- Get all the institutions and other aggregations
	select *
	from SobekCM_Item_Aggregation A
	order by Code;

	-- Get all the wordmarks
	select * 
	from SobekCM_Icon
    order by Icon_Name;
	
end;
GO

GRANT EXECUTE ON [dbo].[SobekCM_Importer_Load_Lookup_Tables] to sobek_builder;
GRANT EXECUTE ON [dbo].[SobekCM_Importer_Load_Lookup_Tables] to sobek_user;
GRANT EXECUTE ON [dbo].[SobekCM_Importer_Load_Lookup_Tables] to sobek_itemeditor;
GO

CREATE PROCEDURE [dbo].[SobekCM_Save_Item_Complete_KML]
	@ItemID int,
	@CompleteKML varchar(max)
AS
BEGIN

	update SobekCM_Item
	set Complete_KML = @CompleteKML
	where ItemID=@ItemID;
END;
GO

GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Complete_KML] to sobek_builder;
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Complete_KML] to sobek_user;
GO


CREATE PROCEDURE [dbo].[SobekCM_Get_Last_Open_Workflow_By_ItemID]
	@ItemID int,
	@EventNumber int
AS
BEGIN

	-- Get the workflow id
	declare @workflowid int;
	set @workflowid = coalesce((select WorkFlowID from Tracking_Workflow where Start_Event_Number = @EventNumber or End_Event_Number = @EventNumber ), -1);
	
	-- If there is a match continue
	if ( @workflowid > 0 )
	begin
	
		select W.WorkFlowName, W.Start_Event_Desc, W.End_Event_Desc, W.Start_Event_Number, W.End_Event_Number, W.Start_And_End_Event_Number,
		       P.DateStarted, P.DateCompleted, P.RelatedEquipment, P.WorkPerformedBy, P.WorkingFilePath, P.ProgressNote
		from Tracking_Progress P, Tracking_Workflow W
		where ItemID = @ItemID
		  and P.WorkFlowID = @workflowid
		  and P.WorkFlowID = W.WorkFlowID
		  and ( DateCompleted is null );
		  
	
	end;
END;
GO

GRANT EXECUTE ON SobekCM_Get_Last_Open_Workflow_By_ItemID to sobek_user;
GO
