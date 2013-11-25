

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
	[BuilderLogID] [int] IDENTITY(1,1) NOT NULL,
	[RelatedBuilderLogID] [int] NULL,
	[LogDate] [datetime] NULL,
	[BibID_VID] [varchar](16) NULL,
	[LogType] [varchar](25) NULL,
	[LogMessage] [varchar](2000) NULL,
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
	
	-- Delete all logs from before this time
	delete from SobekCM_Builder_Log
	where LogDate <= @expiredate;

END;
GO

CREATE PROCEDURE SobekCM_Builder_Add_Log
	@RelatedBuilderLogID int,
	@BibID_VID varchar(16),
	@LogType varchar(25),
	@LogMessage varchar(2000),
	@BuilderLogID int output
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
