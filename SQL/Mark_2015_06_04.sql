
-- Add the main web content table
IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SobekCM_WebContent'))
BEGIN
	CREATE TABLE [dbo].[SobekCM_WebContent](
		[WebContentID] [int] IDENTITY(1,1) NOT NULL,
		[Level1] [varchar](100) NOT NULL,
		[Level2] [varchar](100) NULL,
		[Level3] [varchar](100) NULL,
		[Level4] [varchar](100) NULL,
		[Level5] [varchar](100) NULL,
		[Level6] [varchar](100) NULL,
		[Level7] [varchar](100) NULL,
		[Level8] [varchar](100) NULL,
		[Deleted] bit NOT NULL default('false'),
		[Title] nvarchar(255) NULL,
		[Summary] nvarchar(1000) NULL
	 CONSTRAINT [PK_SobekCM_WebContent] PRIMARY KEY CLUSTERED 
	(
		[WebContentID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY];
END;
GO

-- Just double check these columns were added
if ( NOT EXISTS (select * from sys.columns where Name = N'Title' and Object_ID = Object_ID(N'SobekCM_WebContent')))
BEGIN
	ALTER TABLE [dbo].SobekCM_WebContent add Title nvarchar(255) null;
END;
GO

-- Just double check these columns were added
if ( NOT EXISTS (select * from sys.columns where Name = N'Summary' and Object_ID = Object_ID(N'SobekCM_WebContent')))
BEGIN
	ALTER TABLE [dbo].SobekCM_WebContent add Summary nvarchar(1000) null;
END;
GO


-- Add the main web content table
IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SobekCM_WebContent_Milestones'))
BEGIN
	CREATE TABLE [dbo].[SobekCM_WebContent_Milestones](
		[WebContentMilestoneID] [int] IDENTITY(1,1) NOT NULL,
		[WebContentID] [int] NOT NULL,
		[Milestone] [nvarchar](max) NOT NULL,
		[MilestoneDate] [date] NOT NULL,
		[MilestoneUser] [nvarchar](100) NOT NULL,
	 CONSTRAINT [PK_SobekCM_WebContent_Milestones] PRIMARY KEY CLUSTERED 
	(
		[WebContentMilestoneID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

	
	ALTER TABLE [dbo].[SobekCM_WebContent_Milestones]  WITH CHECK ADD  CONSTRAINT [FK_SobekCM_WebContent_Milestones_SobekCM_WebContent] FOREIGN KEY([WebContentID])
	REFERENCES [dbo].[SobekCM_WebContent] ([WebContentID]);
END;
GO

-- Add column to reference back to the SobekCM_Web_Content table
if ( NOT EXISTS (select * from sys.columns where Name = N'WebContentID' and Object_ID = Object_ID(N'SobekCM_WebContent_Statistics')))
BEGIN
	ALTER TABLE [dbo].SobekCM_WebContent_Statistics add WebContentID int;
END;
GO

-- Drop index, if it exists (it shouldn't though)
if ( EXISTS ( select 1 from sys.indexes WHERE name='IX_SobekCM_WebContent_Levels' AND object_id = OBJECT_ID('SobekCM_WebContent')))
	DROP INDEX [IX_SobekCM_WebContent_Levels] ON [dbo].[SobekCM_WebContent]
GO

/****** Object:  Index [IX_SobekCM_WebContent_Levels]    Script Date: 6/4/2015 6:54:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_SobekCM_WebContent_Levels] ON [dbo].[SobekCM_WebContent]
(
	[Level1] ASC,
	[Level2] ASC,
	[Level3] ASC,
	[Level4] ASC,
	[Level5] ASC,
	[Level6] ASC,
	[Level7] ASC,
	[Level8] ASC
)
INCLUDE ( 	[WebContentID], [Deleted], [Title], [Summary])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- Drop index, if it exists (it shouldn't though)
if ( EXISTS ( select 1 from sys.indexes WHERE name='IX_SobekCM_WebContent_Milestones_Date_ID' AND object_id = OBJECT_ID('SobekCM_WebContent_Milestones')))
	DROP INDEX IX_SobekCM_WebContent_Milestones_Date_ID ON [dbo].SobekCM_WebContent_Milestones
GO

/****** Object:  Index [IX_SobekCM_WebContent_Milestones_Date_ID]    Script Date: 6/4/2015 6:55:43 AM ******/
CREATE NONCLUSTERED INDEX [IX_SobekCM_WebContent_Milestones_Date_ID] ON [dbo].[SobekCM_WebContent_Milestones]
(
	[WebContentID] ASC,
	[MilestoneDate] ASC
)
INCLUDE ( 	[MilestoneUser]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- Drop index, if it exists (it shouldn't though)
if ( EXISTS ( select 1 from sys.indexes WHERE name='IX_SobekCM_WebContent_Stats_ID' AND object_id = OBJECT_ID('SobekCM_WebContent_Statistics')))
	DROP INDEX IX_SobekCM_WebContent_Stats_ID ON [dbo].SobekCM_WebContent_Statistics
GO


/****** Object:  Index [IX_SobekCM_WebContent_Stats_ID]    Script Date: 6/4/2015 7:08:06 AM ******/
CREATE NONCLUSTERED INDEX [IX_SobekCM_WebContent_Stats_ID] ON [dbo].[SobekCM_WebContent_Statistics]
(
	[WebContentID] ASC
)
INCLUDE ( 	[Year],
	[Month],
	[Hits],
	[Hits_Complete]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO



-- Esure the SobekCM_WebContent_Add stored procedure exists
IF object_id('SobekCM_WebContent_Add') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Add as select 1;');
GO

-- Add a new web content page
ALTER PROCEDURE SobekCM_WebContent_Add
	@Level1 varchar(100),
	@Level2 varchar(100),
	@Level3 varchar(100),
	@Level4 varchar(100),
	@Level5 varchar(100),
	@Level6 varchar(100),
	@Level7 varchar(100),
	@Level8 varchar(100),
	@UserName nvarchar(100),
	@Title nvarchar(255),
	@Summary nvarchar(1000),
	@WebContentID int output
AS
BEGIN	
	-- Is there a match already for this?
	if ( EXISTS ( select 1 from SobekCM_WebContent where Level1=@Level1 and Level2=@Level2 and Level3=@Level3 and Level4=@Level4 and Level5=@Level5 and Level6=@Level6 and Level7=@Level7 and Level8=@Level8 ))
	begin
		-- Get the web content id
		set @WebContentID = ( select WebContentID from SobekCM_WebContent where Level1=@Level1 and Level2=@Level2 and Level3=@Level3 and Level4=@Level4 and Level5=@Level5 and Level6=@Level6 and Level7=@Level7 and Level8=@Level8 );

		-- Ensure the title and summary are correct
		update SobekCM_WebContent set Title=@Title, Summary=@Summary where WebContentID=@WebContentID;
		
		-- Was this previously deleted?
		if ( EXISTS ( select 1 from SobekCM_WebContent where Deleted='true' and WebContentID=@WebContentID ))
		begin
			-- Undelete this 
			update SobekCM_WebContent
			set Deleted='false'
			where WebContentID = @WebContentID;

			-- Mark this in the milestones then
			insert into SobekCM_WebContent_Milestones ( WebContentID, Milestone, MilestoneDate, MilestoneUser )
			values ( @WebContentID, 'Restored previously deleted page', getdate(), @UserName );
		end;
	end
	else
	begin
		-- Add the new web content then
		insert into SobekCM_WebContent ( Level1, Level2, Level3, Level4, Level5, Level6, Level7, Level8, Title, Summary, Deleted )
		values ( @Level1, @Level2, @Level3, @Level4, @Level5, @Level6, @Level7, @Level8, @Title, @Summary, 'false' );

		-- Get the new ID for this
		set @WebContentID = SCOPE_IDENTITY();

		-- Now, add this to the milestones table
		insert into SobekCM_WebContent_Milestones ( WebContentID, Milestone, MilestoneDate, MilestoneUser )
		values ( @WebContentID, 'Add new page', getdate(), @UserName );
	end;
END;
GO


-- Esure the SobekCM_WebContent_Edit stored procedure exists
IF object_id('SobekCM_WebContent_Edit') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Edit as select 1;');
GO

-- Edit basic information on an existing web content page
ALTER PROCEDURE SobekCM_WebContent_Edit
	@WebContentID int,
	@UserName nvarchar(100),
	@Title nvarchar(255),
	@Summary nvarchar(1000)
AS
BEGIN	
	-- Make the change
	update SobekCM_WebContent
	set Title=@Title, Summary=@Summary
	where WebContentID=@WebContentID;
END;
GO


-- Esure the SobekCM_WebContent_Get_Page stored procedure exists
IF object_id('SobekCM_WebContent_Get_Page') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Get_Page as select 1;');
GO

-- Get basic details about an existing web content page
ALTER PROCEDURE SobekCM_WebContent_Get_Page
	@Level1 varchar(100),
	@Level2 varchar(100),
	@Level3 varchar(100),
	@Level4 varchar(100),
	@Level5 varchar(100),
	@Level6 varchar(100),
	@Level7 varchar(100),
	@Level8 varchar(100)
AS
BEGIN	
	-- Return the couple of requested pieces of information
	select top 1 W.WebContentID, W.Title, W.Summary, W.Deleted, M.MilestoneDate, M.MilestoneUser
	from SobekCM_WebContent W left outer join
	     SobekCM_WebContent_Milestones M on W.WebContentID=M.WebContentID
	where ( Level1=@Level1 and Level2=@Level2 and Level3=@Level3 and Level4=@Level4 and Level5=@Level5 and Level6=@Level6 and Level7=@Level7 and Level8=@Level8 )
	order by M.MilestoneDate DESC;
END;
GO

-- Esure the SobekCM_WebContent_Add_Milestone stored procedude exists
IF object_id('SobekCM_WebContent_Add_Milestone') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Add_Milestone as select 1;');
GO

-- Add a new milestone to an existing web content page
ALTER PROCEDURE SobekCM_WebContent_Add_Milestone
	@WebContentID int,
	@Milestone nvarchar(max),
	@MilestoneUser nvarchar(100)
AS
BEGIN

	-- Insert milestone
	insert into SobekCM_WebContent_Milestones ( WebContentID, Milestone, MilestoneUser, MilestoneDate )
	values ( @WebContentID, @Milestone, @MilestoneUser, getdate());

END;
GO



-- Esure the SobekCM_WebContent_Delete stored procedude exists
IF object_id('SobekCM_WebContent_Delete') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Delete as select 1;');
GO

-- Delete an existing web content page (and mark in the milestones)
ALTER PROCEDURE SobekCM_WebContent_Delete
	@WebContentID int,
	@Reason nvarchar(max),
	@MilestoneUser nvarchar(100)
AS
BEGIN

	-- Mark web page as deleted
	update SobekCM_WebContent
	set Deleted='true'
	where WebContentID=@WebContentID;

	-- Add a milestone for this
	if (( @Reason is not null ) and ( len(@Reason) > 0 ))
	begin
		insert into SobekCM_WebContent_Milestones ( WebContentID, Milestone, MilestoneUser, MilestoneDate )
		values ( @WebContentID, 'Page Deleted - ' + @Reason, @MilestoneUser, getdate());
	end
	else
	begin
		insert into SobekCM_WebContent_Milestones ( WebContentID, Milestone, MilestoneUser, MilestoneDate )
		values ( @WebContentID, 'Page Deleted', @MilestoneUser, getdate());
	end;

END;
GO

-- Esure the SobekCM_WebContent_Get_Usage stored procedude exists
IF object_id('SobekCM_WebContent_Get_Usage') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Get_Usage as select 1;');
GO

-- Get the usage stats for a webcontent page (by ID)
ALTER PROCEDURE SobekCM_WebContent_Get_Usage
	@WebContentID int
AS
BEGIN

	-- Get all stats
	select [Year], [Month], Hits, Hits_Complete
	from SobekCM_WebContent_Statistics
	where WebContentID=@WebContentID
	order by [Year], [Month];

END;
GO

-- Esure the SobekCM_WebContent_Get_Milestones stored procedude exists
IF object_id('SobekCM_WebContent_Get_Milestones') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Get_Milestones as select 1;');
GO

-- Get the milestones for a webcontent page (by ID)
ALTER PROCEDURE SobekCM_WebContent_Get_Milestones
	@WebContentID int
AS
BEGIN

	-- Get all milestones
	select Milestone, MilestoneDate, MilestoneUser
	from SobekCM_WebContent_Milestones
	where WebContentID=@WebContentID
	order by MilestoneDate;

END;
GO

-- Esure the SobekCM_WebContent_Get_Recent_Changes stored procedude exists
IF object_id('SobekCM_WebContent_Get_Recent_Changes') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Get_Recent_Changes as select 1;');
GO

-- Get the list of recent changes to all web content pages
ALTER PROCEDURE SobekCM_WebContent_Get_Recent_Changes
	@WebContentID int
AS
BEGIN

	-- Get all milestones
	select W.WebContentID, W.Level1, W.Level2, W.Level3, W.Level4, W.Level5, W.Level6, W.Level7, W.Level8, MilestoneDate, MilestoneUser, Milestone
	from SobekCM_WebContent_Milestones M, SobekCM_WebContent W
	where M.WebContentID = W.WebContentID
	order by MilestoneDate DESC;

END;
GO
