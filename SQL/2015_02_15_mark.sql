

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
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 10, 'Check packages for age and move', 'MoveAgedPackagesToProcessModule', 'true', 1 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 10, 'Check for any bib id restrictions on this folder', 'ApplyBibIdRestrictionModule', 'true', 2 );
	insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] ) values ( 10, 'Validate each folder and classify (delete v. new/update)', 'ValidateAndClassifyModule', 'true', 3 );

end;
GO


