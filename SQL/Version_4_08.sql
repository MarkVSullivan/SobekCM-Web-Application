
create table dbo.SobekCM_Item_Alias(
	ItemAliasID int IDENTITY(1,1) NOT NULL,
	Alias varchar(50) NOT NULL,
	ItemID int NOT NULL,
	CONSTRAINT [PK_SobekCM_Item_Alias] PRIMARY KEY CLUSTERED 
(
	ItemAliasID ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].SobekCM_Item_Alias  WITH CHECK ADD  CONSTRAINT [FK_SobekCM_Item_Alias_SobekCM_Item] FOREIGN KEY([ItemID])
REFERENCES [dbo].[SobekCM_Item] ([ItemID])
GO



ALTER PROCEDURE [dbo].[mySobek_Delete_User_Group]
	@usergroupid int,
	@message int output
AS
begin transaction

	if ( exists ( select 1 from mySobek_User_Group_Link where UserGroupID=@usergroupid ))
	begin
		set @message = -1;
	end
	else if ( exists ( select 1 from mySobek_User_Group where UserGroupID=@usergroupid and isSpecialGroup = 'true' ))
	begin
		set @message = -2;
	end
	else
	begin

		delete from mySobek_User_Group_DefaultMetadata_Link where UserGroupID=@usergroupid;
		delete from mySobek_User_Group_Edit_Aggregation where UserGroupID=@usergroupid;
		delete from mySobek_User_Group_Item_Permissions where UserGroupID=@usergroupid;
		delete from mySobek_User_Group_Editable_Link where UserGroupID=@usergroupid;
		delete from mySobek_User_Group_Template_Link where UserGroupID=@usergroupid;
		delete from mySobek_User_Group where UserGroupID = @usergroupid;

		set @message = 1;
	end;

commit transaction;
GO




-- Add builder module table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SobekCM_Builder_Module]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[SobekCM_Builder_Module](
		[ModuleID] [int] identity(1,1) NOT NULL,
		[ModuleSetID] [int] NOT NULL,
		[ModuleDesc] [varchar](200) NOT NULL,
		[Assembly] [varchar](250) NULL,
		[Class] [varchar](500) NOT NULL,
		[Enabled] [bit] NOT NULL,
		[Order] [int] NOT NULL,
		[Argument1] [varchar](max) NULL,
		[Argument2] [varchar](max) NULL,
		[Argument3] [varchar](max) NULL
	 CONSTRAINT [PK_SobekCM_Builder_Module] PRIMARY KEY CLUSTERED 
	(
		[ModuleID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

-- Add builder module schedule table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SobekCM_Builder_Module_Schedule]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[SobekCM_Builder_Module_Schedule](
		[ModuleScheduleID] [int] identity(1,1) NOT NULL,
		[ModuleSetID] [int] NOT NULL,
		[DaysOfWeek] [varchar](7) NOT NULL,
		[Enabled] [bit] NOT NULL,
		[TimesOfDay] [varchar](100) NOT NULL,
	 CONSTRAINT [PK_SobekCM_Builder_Module_Schedule] PRIMARY KEY CLUSTERED 
	(
		[ModuleScheduleID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

-- Add builder module set table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SobekCM_Builder_Module_Set]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[SobekCM_Builder_Module_Set](
		[ModuleSetID] [int] identity(1,1) NOT NULL,
		[ModuleTypeID] [int] NOT NULL,
		[SetName] [varchar](50) NOT NULL,
		SetOrder int NOT NULL
	 CONSTRAINT [PK_SobekCM_Builder_Module_Set] PRIMARY KEY CLUSTERED 
	(
		[ModuleSetID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY];

	alter table SobekCM_Builder_Module_Set add [Enabled] bit not null default('true');

END
GO

-- Add builder module type table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SobekCM_Builder_Module_Type]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SobekCM_Builder_Module_Type](
	[ModuleTypeID] [int] identity(1,1) NOT NULL,
	[TypeAbbrev] [varchar](4) NOT NULL,
	[TypeDescription] [varchar](200) NOT NULL,
 CONSTRAINT [PK_SobekCM_Builder_Module_Types] PRIMARY KEY CLUSTERED 
(
	[ModuleTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

-- Add all foreign keys for the builder modules
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SobekCM_Builder_Module_SobekCM_Builder_Module_Set]') AND parent_object_id = OBJECT_ID(N'[dbo].[SobekCM_Builder_Module]'))
ALTER TABLE [dbo].[SobekCM_Builder_Module]  WITH CHECK ADD  CONSTRAINT [FK_SobekCM_Builder_Module_SobekCM_Builder_Module_Set] FOREIGN KEY([ModuleSetID])
REFERENCES [dbo].[SobekCM_Builder_Module_Set] ([ModuleSetID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SobekCM_Builder_Module_SobekCM_Builder_Module_Set]') AND parent_object_id = OBJECT_ID(N'[dbo].[SobekCM_Builder_Module]'))
ALTER TABLE [dbo].[SobekCM_Builder_Module] CHECK CONSTRAINT [FK_SobekCM_Builder_Module_SobekCM_Builder_Module_Set]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SobekCM_Builder_Module_Schedule_SobekCM_Builder_Module_Set]') AND parent_object_id = OBJECT_ID(N'[dbo].[SobekCM_Builder_Module_Schedule]'))
ALTER TABLE [dbo].[SobekCM_Builder_Module_Schedule]  WITH CHECK ADD  CONSTRAINT [FK_SobekCM_Builder_Module_Schedule_SobekCM_Builder_Module_Set] FOREIGN KEY([ModuleSetID])
REFERENCES [dbo].[SobekCM_Builder_Module_Set] ([ModuleSetID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SobekCM_Builder_Module_Schedule_SobekCM_Builder_Module_Set]') AND parent_object_id = OBJECT_ID(N'[dbo].[SobekCM_Builder_Module_Schedule]'))
ALTER TABLE [dbo].[SobekCM_Builder_Module_Schedule] CHECK CONSTRAINT [FK_SobekCM_Builder_Module_Schedule_SobekCM_Builder_Module_Set]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SobekCM_Builder_Module_Set_SobekCM_Builder_Module_Type]') AND parent_object_id = OBJECT_ID(N'[dbo].[SobekCM_Builder_Module_Set]'))
ALTER TABLE [dbo].[SobekCM_Builder_Module_Set]  WITH CHECK ADD  CONSTRAINT [FK_SobekCM_Builder_Module_Set_SobekCM_Builder_Module_Type] FOREIGN KEY([ModuleTypeID])
REFERENCES [dbo].[SobekCM_Builder_Module_Type] ([ModuleTypeID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SobekCM_Builder_Module_Set_SobekCM_Builder_Module_Type]') AND parent_object_id = OBJECT_ID(N'[dbo].[SobekCM_Builder_Module_Set]'))
ALTER TABLE [dbo].[SobekCM_Builder_Module_Set] CHECK CONSTRAINT [FK_SobekCM_Builder_Module_Set_SobekCM_Builder_Module_Type]
GO

-- Add link to the builder modules for incoming folders
IF ( NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'SobekCM_Builder_Incoming_Folders'
                 AND COLUMN_NAME = 'ModuleSetID'))
BEGIN
	alter table SobekCM_Builder_Incoming_Folders
	add ModuleSetID int null;
END;
GO

-- Delete ModuleConfig from the incoming folder table definition, if it exists
IF ( EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'SobekCM_Builder_Incoming_Folders'
                 AND COLUMN_NAME = 'ModuleConfig'))
begin

	alter table SobeKCM_Builder_Incoming_Folders
	drop column ModuleConfig;

end;
GO

CREATE TABLE [dbo].[SobekCM_Builder_Module_Scheduled_Run](
	[ModuleSchedRunID] [int] IDENTITY(1,1) NOT NULL,
	[ModuleScheduleID] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[Outcome] [varchar](100) NOT NULL,
	[Message] [varchar](max) NULL,
 CONSTRAINT [PK_SobekCM_Builder_Module_Scheduled_Run] PRIMARY KEY CLUSTERED 
(
	[ModuleSchedRunID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[SobekCM_Builder_Module_Scheduled_Run]  WITH CHECK ADD  CONSTRAINT [FK_SobekCM_Builder_Module_Scheduled_Run_SobekCM_Builder_Module_Schedule] FOREIGN KEY([ModuleScheduleID])
REFERENCES [dbo].[SobekCM_Builder_Module_Schedule] ([ModuleScheduleID])
GO

ALTER TABLE [dbo].[SobekCM_Builder_Module_Scheduled_Run] CHECK CONSTRAINT [FK_SobekCM_Builder_Module_Scheduled_Run_SobekCM_Builder_Module_Schedule]
GO




-- Add the foreign key from the folders table to the module sets table 
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SobekCM_Builder_Incoming_Folders_SobekCM_Builder_Module_Set]') AND parent_object_id = OBJECT_ID(N'[dbo].[SobekCM_Builder_Incoming_Folders]'))
ALTER TABLE [dbo].[SobekCM_Builder_Incoming_Folders]  WITH CHECK ADD  CONSTRAINT [FK_SobekCM_Builder_Incoming_Folders_SobekCM_Builder_Module_Set] FOREIGN KEY([ModuleSetID])
REFERENCES [dbo].[SobekCM_Builder_Module_Set] ([ModuleSetID])
GO

-- Add the module types
if (( select count(*) from SobekCM_Builder_Module_Type ) = 0 )
begin

	insert into SobekCM_Builder_Module_Type ( TypeAbbrev, TypeDescription ) values ( 'PRE', 'Pre-Process modules run each time BEFORE processing any pending items/requests' );
	insert into SobekCM_Builder_Module_Type ( TypeAbbrev, TypeDescription ) values ( 'POST', 'Post-Process modules run each time AFTER processing any pending items/requests' );
	insert into SobekCM_Builder_Module_Type ( TypeAbbrev, TypeDescription ) values ( 'NEW', 'Submission modules run for each incoming item (or items set to reprocess)' );
	insert into SobekCM_Builder_Module_Type ( TypeAbbrev, TypeDescription ) values ( 'DELT', 'Submission modules run for each incoming DELETE request' );
	insert into SobekCM_Builder_Module_Type ( TypeAbbrev, TypeDescription ) values ( 'SCHD', 'Schedulable modules run as a scheduled task by the builder' );
	insert into SobekCM_Builder_Module_Type ( TypeAbbrev, TypeDescription ) values ( 'FOLD', 'Folder-level modules are run to prepare and find items in incoming folders' );

end;
GO

-- Add the default module sets and modules
if (( select count(*) from SobekCM_Builder_Module_Set ) = 0 )
begin

	-- insert all the standard module sets
	insert into SobekCM_Builder_Module_Set ( ModuleTypeID, SetName, SetOrder ) values ( 1, 'Standard PRE-process modules', 1 );
	insert into SobekCM_Builder_Module_Set ( ModuleTypeID, SetName, SetOrder ) values ( 2, 'Standard POST-process modules', 1 );
	insert into SobekCM_Builder_Module_Set ( ModuleTypeID, SetName, SetOrder ) values ( 3, 'Incoming item processing', 1 );
	insert into SobekCM_Builder_Module_Set ( ModuleTypeID, SetName, SetOrder ) values ( 4, 'Incoming delete processing', 1 );
	insert into SobekCM_Builder_Module_Set ( ModuleTypeID, SetName, SetOrder, [Enabled] ) values ( 5, 'Expire old builder logs', 1, 'false' );
	insert into SobekCM_Builder_Module_Set ( ModuleTypeID, SetName, SetOrder, [Enabled] ) values ( 5, 'Rebuild all aggregation browse files', 1, 'false' );
	insert into SobekCM_Builder_Module_Set ( ModuleTypeID, SetName, SetOrder, [Enabled] ) values ( 5, 'Send new item emails', 1, 'false' );
	insert into SobekCM_Builder_Module_Set ( ModuleTypeID, SetName, SetOrder, [Enabled] ) values ( 5, 'Solr/Lucene index optimization', 1, 'false' );
	insert into SobekCM_Builder_Module_Set ( ModuleTypeID, SetName, SetOrder ) values ( 5, 'Update cached aggregation browses', 1 );
	insert into SobekCM_Builder_Module_Set ( ModuleTypeID, SetName, SetOrder ) values ( 6, 'Standard folder processing', 1 );

	-- Add each standard module - to the appropriate sets
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 1, 'Load reports from FDA (Florida Digital Archives) for Florida universities', 'SobekCM.Builder_Library.Modules.PreProcess.ProcessPendingFdaReportsModule', 'false', 1 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 2, 'Build the aggregation browse files', 'SobekCM.Builder_Library.Modules.PostProcess.BuildAggregationBrowsesModule', 'true', 1 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Convert office files to PDFs', 'SobekCM.Builder_Library.Modules.Items.ConvertOfficeFilesToPdfModule', 'true', 1 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Extract text from all PDFs', 'SobekCM.Builder_Library.Modules.Items.ExtractTextFromPdfModule', 'true', 2 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Create thumbnails for all PDFs', 'SobekCM.Builder_Library.Modules.Items.CreatePdfThumbnailModule', 'true', 3 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Extract the text from included HTML files', 'SobekCM.Builder_Library.Modules.Items.ExtractTextFromHtmlModule', 'true', 4 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Extract the text from included (non-standard) XML files', 'SobekCM.Builder_Library.Modules.Items.ExtractTextFromXmlModule', 'true', 5 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'OCR tiff files', 'SobekCM.Builder_Library.Modules.Items.OcrTiffsModule', 'true', 6 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Clean any dirty ocr (non-unicode friendly)', 'SobekCM.Builder_Library.Modules.Items.CleanDirtyOcrModule', 'false', 7 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Check for SSNs in any loaded text', 'SobekCM.Builder_Library.Modules.Items.CheckForSsnModule', 'true', 8 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Create image derivatives (jpegs and jpeg2000s)', 'SobekCM.Builder_Library.Modules.Items.CreateImageDerivativesModule', 'true', 9 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Copy all incoming files to the archive folder', 'SobekCM.Builder_Library.Modules.Items.CopyToArchiveFolderModule', 'true', 10 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Move files to the image server', 'SobekCM.Builder_Library.Modules.Items.MoveFilesToImageServerModule', 'true', 11 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Reload the METS and basic database info', 'SobekCM.Builder_Library.Modules.Items.ReloadMetsAndBasicDbInfoModule', 'true', 12 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Update JPEG attributes (width and height)', 'SobekCM.Builder_Library.Modules.Items.UpdateJpegAttributesModule', 'true', 13 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Attach all non-image files to the item', 'SobekCM.Builder_Library.Modules.Items.AttachAllNonImageFilesModule', 'true', 14 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Add new image files (and associated views) to the item', 'SobekCM.Builder_Library.Modules.Items.AddNewImagesAndViewsModule', 'true', 15 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Ensure a main thumbnail is referenced', 'SobekCM.Builder_Library.Modules.Items.EnsureMainThumbnailModule', 'true', 16 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Get number of pages for PDF-only types', 'SobekCM.Builder_Library.Modules.Items.GetPageCountFromPdfModule', 'true', 17 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Update the web.config for restricted items', 'SobekCM.Builder_Library.Modules.Items.UpdateWebConfigModule', 'true', 18 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Save the service METS file', 'SobekCM.Builder_Library.Modules.Items.SaveServiceMetsModule', 'true', 19 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Save a Marc21 XML file', 'SobekCM.Builder_Library.Modules.Items.SaveMarcXmlModule', 'true', 20 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Save to the database', 'SobekCM.Builder_Library.Modules.Items.SaveToDatabaseModule', 'true', 21 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Save to solr/lucene', 'SobekCM.Builder_Library.Modules.Items.SaveToSolrLuceneModule', 'true', 22 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Clean the web resource folder', 'SobekCM.Builder_Library.Modules.Items.CleanWebResourceFolderModule', 'true', 23 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Build statice version for SEO', 'SobekCM.Builder_Library.Modules.Items.CreateStaticVersionModule', 'true', 24 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 3, 'Add tracking information', 'SobekCM.Builder_Library.Modules.Items.AddTrackingWorkflowModule', 'true', 25 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 4, 'Delete item in database and folder', 'SobekCM.Builder_Library.Modules.Items.DeleteItemModule', 'true', 1 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 5, 'Expire old builder logs', 'SobekCM.Builder_Library.Modules.Schedulable.ExpireOldLogEntriesModule', 'true', 1 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 6, 'Rebuild all aggregation browse files', 'SobekCM.Builder_Library.Modules.Schedulable.RebuildAllAggregationBrowsesModule', 'true', 1 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 7, 'Send new item emails', 'SobekCM.Builder_Library.Modules.Schedulable.SendNewItemEmailsModule', 'true', 1 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 8, 'Solr/Lucene index optimization', 'SobekCM.Builder_Library.Modules.Schedulable.SolrLuceneIndexOptimizationModule', 'true', 1 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 9, 'Update cached aggregation browses', 'SobekCM.Builder_Library.Modules.Schedulable.UpdatedCachedAggregationMetadataModule', 'true', 1 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 10, 'Check packages for age and move', 'SobekCM.Builder_Library.Modules.Folders.MoveAgedPackagesToProcessModule', 'true', 1 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 10, 'Check for any bib id restrictions on this folder', 'SobekCM.Builder_Library.Modules.Folders.ApplyBibIdRestrictionModule', 'true', 2 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 10, 'Validate each folder and classify (delete v. new/update)', 'SobekCM.Builder_Library.Modules.Folders.ValidateAndClassifyModule', 'true', 3 );

end;
GO

update SobeKCM_Builder_Incoming_Folders set ModuleSetID=10;
GO


-- Sends an email via database mail and additionally logs that the email was sent
ALTER PROCEDURE [dbo].[SobekCM_Send_Email] 
	@recipients_list varchar(250),
	@subject_line varchar(500),
	@email_body nvarchar(max),
	@from_address nvarchar(250),
	@reply_to nvarchar(250), 
	@html_format bit,
	@contact_us bit,
	@replytoemailid int,
	@userid int
AS
begin transaction

	if (( @userid < 0 ) or (( select count(*) from SobekCM_Email_Log where UserID = @userid and Sent_Date > DateAdd( DAY, -1, GETDATE())) < 20 ))
	begin
		-- Log this email
		insert into SobekCM_Email_Log( Sender, Receipt_List, Subject_Line, Email_Body, Sent_Date, HTML_Format, Contact_Us, ReplyToEmailId, UserID )
		values ( 'sobekcm noreply profile', @recipients_list, @subject_line, @email_body, GETDATE(), @html_format, @contact_us, @replytoemailid, @userid );
		
		-- Send the email
		if ( @html_format = 'true' )
		begin
			if ( len(coalesce(@from_address,'')) > 0 )
			begin
				if ( len(coalesce(@reply_to,'')) > 0 )
				begin
					EXEC msdb.dbo.sp_send_dbmail
						@profile_name= 'sobekcm noreply profile',
						@recipients = @recipients_list,
						@body = @email_body,
						@subject = @subject_line,
						@body_format = 'html',
						@from_address = @from_address,
						@reply_to = @reply_to;
				end
				else
				begin
					EXEC msdb.dbo.sp_send_dbmail
						@profile_name= 'sobekcm noreply profile',
						@recipients = @recipients_list,
						@body = @email_body,
						@subject = @subject_line,
						@body_format = 'html',
						@from_address = @from_address;
				end;
			end
			else
			begin
				if ( len(coalesce(@reply_to,'')) > 0 )
				begin
					EXEC msdb.dbo.sp_send_dbmail
						@profile_name= 'sobekcm noreply profile',
						@recipients = @recipients_list,
						@body = @email_body,
						@subject = @subject_line,
						@body_format = 'html',
						@reply_to = @reply_to;
				end
				else
				begin
					EXEC msdb.dbo.sp_send_dbmail
						@profile_name= 'sobekcm noreply profile',
						@recipients = @recipients_list,
						@body = @email_body,
						@subject = @subject_line,
						@body_format = 'html';
				end;
			end;
		end
		else
		begin
			if ( len(coalesce(@from_address,'')) > 0 )
			begin
				if ( len(coalesce(@reply_to,'')) > 0 )
				begin
					EXEC msdb.dbo.sp_send_dbmail
						@profile_name= 'sobekcm noreply profile',
						@recipients = @recipients_list,
						@body = @email_body,
						@subject = @subject_line,
						@from_address = @from_address,
						@reply_to = @reply_to;
				end
				else
				begin
					EXEC msdb.dbo.sp_send_dbmail
						@profile_name= 'sobekcm noreply profile',
						@recipients = @recipients_list,
						@body = @email_body,
						@subject = @subject_line,
						@from_address = @from_address;
				end;
			end
			else
			begin
				if ( len(coalesce(@reply_to,'')) > 0 )
				begin
					EXEC msdb.dbo.sp_send_dbmail
						@profile_name= 'sobekcm noreply profile',
						@recipients = @recipients_list,
						@body = @email_body,
						@subject = @subject_line,
						@reply_to = @reply_to;
				end
				else
				begin
					EXEC msdb.dbo.sp_send_dbmail
						@profile_name= 'sobekcm noreply profile',
						@recipients = @recipients_list,
						@body = @email_body,
						@subject = @subject_line;
				end;
			end;
		end;
	end;
	
commit transaction;
GO

-- Gets the list of all system-wide settings from the database, including the full list of all
-- metadata search fields, possible workflows, and all disposition data
ALTER PROCEDURE [dbo].[SobekCM_Get_Settings]
AS
begin

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Get all the standard SobekCM settings
	select Setting_Key, Setting_Value
	from SobekCM_Settings;

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


/****** Object:  StoredProcedure [dbo].[SobekCM_Get_OAI_Data_Codes]    Script Date: 12/20/2013 05:43:37 ******/
-- Gets the distinct data codes present in the database for OAI (such as 'oai_dc')
ALTER PROCEDURE [dbo].[SobekCM_Get_OAI_Data_Codes]
AS
BEGIN
	-- Dirty read here won't hurt anything
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Return distinct codes used in the OAI table
	select distinct(Data_Code)
	from SobekCM_Item_OAI;
END;
GO


/****** Object:  StoredProcedure [dbo].[SobekCM_Get_OAI_Data_Item]    Script Date: 12/20/2013 05:43:37 ******/
-- Returns the OAI data for a single item from the oai source tables
ALTER PROCEDURE [dbo].[SobekCM_Get_OAI_Data_Item]
	@bibid varchar(10),
	@vid varchar(5),
	@data_code varchar(20)
AS
begin
	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Select the matching rows
	select G.GroupID, BibID, O.OAI_Data, O.OAI_Date, VID
	from SobekCM_Item_Group G, SobekCM_Item I, SobekCM_Item_OAI O
	where G.BibID = @bibid
	  and G.GroupID = I.GroupID
	  and I.VID = @vid
	  and I.ItemID = O.ItemID	
	  and O.Data_Code = @data_code;
end;
GO


/****** Object:  StoredProcedure [dbo].[SobekCM_Get_OAI_Data]    Script Date: 12/20/2013 05:43:37 ******/
-- Return a list of the OAI data to server through the OAI-PMH server
ALTER PROCEDURE [dbo].[SobekCM_Get_OAI_Data]
	@aggregationcode varchar(20),
	@data_code varchar(20),
	@from date,
	@until date,
	@pagesize int, 
	@pagenumber int,
	@include_data bit
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Do not need to maintain row counts
	SET NoCount ON;

	-- Create the temporary tables first
	-- Create the temporary table to hold all the item id's
		
	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	
	-- Rowend is calculated normally, but then an additional item is
	-- added at the end which will be used to determine if a resumption
	-- token should be issued
	set @rowend = (@rowstart + @pagesize - 1) + 1; 
	
	-- Ensure there are date values
	if ( @from is null )
		set @from = CONVERT(date,'19000101');
	if ( @until is null )
		set @until = GETDATE();
	
	-- Is this for a single aggregation
	if (( @aggregationcode is not null ) and ( LEN(@aggregationcode) > 0 ) and ( @aggregationcode != 'all' ))
	begin	
		-- Determine the aggregationid
		declare @aggregationid int;
		set @aggregationid = ( select ISNULL(AggregationID,-1) from SobekCM_Item_Aggregation where Code=@aggregationcode );
			  
		-- Should the actual data be returned, or just the identifiers?
		if ( @include_data='true')
		begin
			-- Create saved select across items/title for row numbers
			with ITEMS_SELECT AS
			(	select BibID, I.ItemID, VID,
				ROW_NUMBER() OVER (order by O.OAI_Date ASC ) as RowNumber
				from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item_Group G, SobekCM_Item_OAI O
				where ( CL.ItemID = I.ItemID )
				  and ( CL.AggregationID = @aggregationid )
				  and ( I.GroupID = G.GroupID )
				  and ( I.ItemID = O.ItemID )
				  and ( G.Suppress_OAI = 'false' )
				  and ( O.OAI_Date >= @from )
				  and ( O.OAI_Date <= @until )
				  and ( O.Data_Code = @data_code ))
				
			-- Select the matching rows
			select BibID, T.VID, O.OAI_Data, O.OAI_Date
			from ITEMS_SELECT T, SobekCM_Item_OAI O
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend
			  and T.ItemID = O.ItemID			  
			  and O.Data_Code = @data_code;		 
		end
		else
		begin
			-- Create saved select across titles for row numbers
			with ITEMS_SELECT AS
			(	select BibID, I.ItemID, VID,
				ROW_NUMBER() OVER (order by O.OAI_Date ASC ) as RowNumber
				from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item_Group G, SobekCM_Item_OAI O
				where ( CL.ItemID = I.ItemID )
				  and ( CL.AggregationID = @aggregationid )
				  and ( I.GroupID = G.GroupID )
				  and ( I.ItemID = O.ItemID )
				  and ( G.Suppress_OAI = 'false' )
				  and ( O.OAI_Date >= @from )
				  and ( O.OAI_Date <= @until )
				  and ( O.Data_Code = @data_code ))
				
			-- Select the matching rows
			select BibID, T.VID, O.OAI_Date
			from ITEMS_SELECT T, SobekCM_Item_OAI O
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend
			  and T.ItemID = O.ItemID
			  and O.Data_Code = @data_code;	
		end;		  
	end
	else
	begin
				  
		-- Should the actual data be returned, or just the identifiers?
		if ( @include_data='true')
		begin
			-- Create saved select across titles for row numbers
			with ITEMS_SELECT AS
			(	select BibID, I.ItemID, VID,
				ROW_NUMBER() OVER (order by O.OAI_Date ASC) as RowNumber
				from SobekCM_Item_Group G, SobekCM_Item I, SobekCM_Item_OAI O
				where ( G.GroupID = I.GroupID )
				  and ( I.ItemID = O.ItemID )
				  and ( G.Suppress_OAI = 'false' )
				  and ( O.OAI_Date >= @from )
				  and ( O.OAI_Date <= @until )
				  and ( O.Data_Code = @data_code ))				
								
			-- Select the matching rows
			select BibID, T.VID, O.OAI_Data, O.OAI_Date
			from ITEMS_SELECT T, SobekCM_Item_OAI O
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend
			  and T.ItemID = O.ItemID
			  and O.Data_Code = @data_code;				 
		end
		else
		begin
			-- Create saved select across titles for row numbers
			with ITEMS_SELECT AS
			(	select BibID, I.ItemID, VID,
				ROW_NUMBER() OVER (order by O.OAI_Date ASC) as RowNumber
				from SobekCM_Item_Group G, SobekCM_Item I, SobekCM_Item_OAI O
				where ( G.GroupID = I.GroupID )
				  and ( I.ItemID = O.ItemID )
				  and ( G.Suppress_OAI = 'false' )
				  and ( O.OAI_Date >= @from )
				  and ( O.OAI_Date <= @until )
				  and ( O.Data_Code = @data_code ))				
								
			-- Select the matching rows
			select BibID, T.VID, O.OAI_Date
			from ITEMS_SELECT T, SobekCM_Item_OAI O
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend
			  and T.ItemID = O.ItemID
			  and O.Data_Code = @data_code;	
		end;
	end;
end;
GO

CREATE procedure [dbo].[SobekCM_Builder_Get_Settings]
	@include_disabled bit
as
begin

	-- Always return all the incoming folders
	select IncomingFolderId, NetworkFolder, ErrorFolder, ProcessingFolder, Perform_Checksum_Validation, Archive_TIFF, Archive_All_Files,
		   Allow_Deletes, Allow_Folders_No_Metadata, Allow_Metadata_Updates, FolderName, Can_Move_To_Content_Folder, BibID_Roots_Restrictions,
		   ModuleSetID
	from SobekCM_Builder_Incoming_Folders F;

	-- Return all the non-scheduled type modules
	if ( @include_disabled = 'true' )
	begin
		select M.ModuleID, M.[Assembly], M.Class, M.ModuleDesc, M.Argument1, M.Argument2, M.Argument3, M.[Enabled], S.ModuleSetID, S.SetName, S.[Enabled] as SetEnabled, T.TypeAbbrev, T.TypeDescription
		from SobekCM_Builder_Module M, SobekCM_Builder_Module_Set S, SobekCM_Builder_Module_Type T
		where M.ModuleSetID = S.ModuleSetID
		  and S.ModuleTypeID = T.ModuleTypeID
		  and T.TypeAbbrev <> 'SCHD'
		order by TypeAbbrev, S.SetOrder, M.[Order];
	end
	else
	begin
		select M.ModuleID, M.[Assembly], M.Class, M.ModuleDesc, M.Argument1, M.Argument2, M.Argument3, M.[Enabled], S.ModuleSetID, S.SetName, S.[Enabled] as SetEnabled, T.TypeAbbrev, T.TypeDescription
		from SobekCM_Builder_Module M, SobekCM_Builder_Module_Set S, SobekCM_Builder_Module_Type T
		where M.ModuleSetID = S.ModuleSetID
		  and S.ModuleTypeID = T.ModuleTypeID
		  and M.[Enabled] = 'true'
		  and S.[Enabled] = 'true'
		  and T.TypeAbbrev <> 'SCHD'
		order by TypeAbbrev, S.SetOrder, M.[Order];
	end;

	-- Return all the scheduled type modules, with the schedule and the last run info
	if ( @include_disabled = 'true' )
	begin
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
	end 
	else
	begin
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
		  and M.[Enabled] = 'true'
		  and S.[Enabled] = 'true'
		  and C.[Enabled] = 'true'
		order by TypeAbbrev, S.SetOrder, M.[Order];
	end;

end;
GO

GRANT EXECUTE ON SobekCM_Builder_Get_Settings to sobek_user;
GO
GRANT EXECUTE ON SobekCM_Builder_Get_Settings to sobek_builder;
GO



if (( select count(*) from SobekCM_Database_Version ) = 0 )
begin
	insert into SobekCM_Database_Version ( Major_Version, Minor_Version, Release_Phase )
	values ( 4, 8, '0' );
end
else
begin
	update SobekCM_Database_Version
	set Major_Version=4, Minor_Version=8, Release_Phase='0';
end;
GO