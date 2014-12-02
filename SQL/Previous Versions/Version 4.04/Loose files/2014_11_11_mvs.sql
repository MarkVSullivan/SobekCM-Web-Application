

EXEC sp_RENAME 'SobekCM_Item.DiskSize_MB' , 'DiskSize_KB', 'COLUMN';
GO


-- Determine the size of the online and archived spaces for the whole
-- system, a single item aggregation, or the intersection between two
-- aggregations.
-- Both include_online and include_archive args function as follows:
--   1 = provide complete sum
--   2 = break into year/month (can take a good bit of server cpu)
-- For the TIVOLI data to be up to date, you may need to run the
-- Tivoli_Admin_Update stored procedure first
ALTER PROCEDURE [dbo].[SobekCM_Online_Archived_Space]
	@code1 varchar(20),
	@code2 varchar(20),
	@include_online smallint,
	@include_archive smallint
AS
begin

	-- No need to perform any locks here, especially given the possible
	-- length of this query
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- If there are two provided codes, show the union of the two codes
	if ( LEN( ISNULL ( @code2, '' )) > 0 )
	begin
	
		-- Get the amount online by year/month/item for the intersect between these two aggregations
		select I.CreateYear, I.CreateMonth, I.DiskSize_KB AS DiskSize, I.ItemID, TivoliSize_MB
		into #TEMP_ITEMS_ONLINE
		from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
		where ( I.ItemID = L.ItemID )
		  and ( L.AggregationID = A.AggregationID )
		  and ( A.Code = @code1 )
		  and ( CreateYear > 0 )
		  and ( CreateMonth > 0 )
		intersect
		select I2.CreateYear, I2.CreateMonth, I2.DiskSize_KB AS DiskSize, I2.ItemID, TivoliSize_MB
		from SobekCM_Item I2, SobekCM_Item_Aggregation_Item_Link L2, SobekCM_Item_Aggregation A2
		where ( I2.ItemID = L2.ItemID )
		  and ( L2.AggregationID = A2.AggregationID )
		  and ( A2.Code = @code2 )
		  and ( CreateYear > 0 )
		  and ( CreateMonth > 0 );
		  
	
						 
		-- If the online flag is ONE, just return total size
		if ( @include_online = 1 )
		begin
			-- Get the total online 
			select CAST((SUM(DiskSize))/(1024*1024) as varchar(15)) + ' GB'
			from #TEMP_ITEMS_ONLINE;
		end;
			 
		-- If the online flag is TWO, return by month/year		 
		if ( @include_online = 2 )
		begin
			-- Get the total online by year/month
			select CreateYear, CreateMonth, SUM(DiskSize)
			from #TEMP_ITEMS_ONLINE
			group by CreateYear, CreateMonth
			order by CreateYear, CreateMonth;
		end;
		
		-- If the archive flag is ONE, just return total size
		if ( @include_archive = 1 )
		begin
			-- Get the total tivolid
			select CAST((SUM(TivoliSize_MB))/(1024*1024) as varchar(15)) + ' GB'
			from #TEMP_ITEMS_ONLINE;
		end;
		
		-- If the archive flag is TWO, return by month/year
		if ( @include_archive = 2 )
		begin
			-- Get the archived amount by year/month/item for this aggregation
			select ArchiveYear, ArchiveMonth, SUM(Size)/(1024*1024) AS DiskSize
			from #TEMP_ITEMS_ONLINE T, Tivoli_File_Log A
			where ( A.ItemID=T.ItemID )
			group by ArchiveYear, ArchiveMonth;
		end;
		
		-- drop the temporary tables
		drop table #TEMP_ITEMS_ONLINE;
	
	end
	else
	begin	
	
		-- Is this for ALL items?
		if (( LEN(@code1 ) > 0 ) and ( @code1 != 'all' ))
		begin
			-- If the online flag is ONE, just return total size
			if ( @include_online = 1 )
			begin
				-- Get the total online 
				select CAST((SUM(DiskSize_KB))/(1024*1024) as varchar(15)) + ' GB'
				from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
				where ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Code = @code1 );
			end;
				 
			-- If the online flag is TWO, return by month/year		 
			if ( @include_online = 2 )
			begin
				-- Get the total online by year/month
				select I.CreateYear, I.CreateMonth, SUM(I.DiskSize_KB) AS DiskSize
				from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
				where ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Code = @code1 )
				  and ( CreateYear > 0 )
				  and ( CreateMonth > 0 )
				group by I.CreateYear, I.CreateMonth
				order by I.CreateYear, I.CreateMonth;
			end;
			
			-- If the archive flag is ONE, just return total size
			if ( @include_archive = 1 )
			begin
				-- Get the TOTAL archived amount for this aggregation
				select CAST((SUM(TivoliSize_MB))/(1024*1024) as varchar(15)) + ' GB'
				from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
				where ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Code = @code1 );
			end;

			-- If the archive flag is TWO, return by month/year
			if ( @include_archive = 2 )
			begin
				-- Get the total archived by year/month for this aggregation
				select T.ArchiveYear, T.ArchiveMonth, SUM(T.Size)/(1024*1024) AS DiskSize
				from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A, Tivoli_File_Log T
				where ( T.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Code = @code1 )
				group by T.ArchiveYear, T.ArchiveMonth
				order by T.ArchiveYear, T.ArchiveMonth;
			end;
		end
		else
		begin -- Just return the COMPLETE sums
			
			-- If the online flag is ONE, just return total size
			if ( @include_online = 1 )
			begin
				-- Get the total online 
				select CAST((SUM(DiskSize_KB))/(1024*1024) as varchar(15)) + ' GB'
				from SobekCM_Item I
			end;
				 
			-- If the online flag is TWO, return by month/year		 
			if ( @include_online = 2 )
			begin
				-- Get the total online by year/month
				select I.CreateYear, I.CreateMonth, SUM(I.DiskSize_KB) AS DiskSize
				from SobekCM_Item I
				where ( CreateYear > 0 )
				  and ( CreateMonth > 0 )
				group by I.CreateYear, I.CreateMonth
				order by I.CreateYear, I.CreateMonth;
			end;
				  
			-- Get the TOTAL archived amount for this system
			select CAST((SUM(TivoliSize_MB))/(1024*1024) as varchar(15)) + ' GB'
			from SobekCM_Item I
		end;
	end;
end;
GO


-- Saves all the main data about an item in UFDC (but not behaviors)
-- Written by Mark Sullivan ( September 2005, Edited Decemver 2010 )
ALTER PROCEDURE [dbo].[SobekCM_Save_Item]
	@GroupID int,
	@VID varchar(5),
	@PageCount int,
	@FileCount int,
	@Title nvarchar(500),
	@SortTitle nvarchar(500), --NEW
	@AccessMethod int,
	@Link varchar(500),
	@CreateDate datetime,
	@PubDate nvarchar(100),
	@SortDate bigint,
	@HoldingCode varchar(20),
	@SourceCode varchar(20),
	@Author nvarchar(1000),
	@Spatial_KML varchar(4000),
	@Spatial_KML_Distance float,
	@DiskSize_KB bigint,
	@Spatial_Display nvarchar(1000), 
	@Institution_Display nvarchar(1000), 
	@Edition_Display nvarchar(1000),
	@Material_Display nvarchar(1000),
	@Measurement_Display nvarchar(1000), 
	@StylePeriod_Display nvarchar(1000), 
	@Technique_Display nvarchar(1000), 
	@Subjects_Display nvarchar(1000), 
	@Donor nvarchar(250),
	@Publisher nvarchar(1000),
	@ItemID int output,
	@Existing bit output,
	@New_VID varchar(5) output
AS
begin transaction

	-- Set the return VID value first
	set @New_VID = @VID;

	-- If this already exists (BibID, VID) then just update
	if ( (	 select count(*) from SobekCM_Item I where ( I.VID = @VID ) and ( I.GroupID = @GroupID ) )  > 0 )
	begin
		-- Save the item id
		select @ItemID = I.ItemID
		from SobekCM_Item I
		where  ( I.VID = @VID ) and ( I.GroupID = @GroupID );

		--Update the main item
		update SobekCM_Item
		set [PageCount] = @PageCount, 
			Deleted = 0, Title=@Title, SortTitle=@SortTitle, AccessMethod=@AccessMethod, Link=@Link,
			PubDate=@PubDate, SortDate=@SortDate, FileCount=@FileCount, Author=@Author, 
			Spatial_KML=@Spatial_KML, Spatial_KML_Distance=@Spatial_KML_Distance,  
			Donor=@Donor, Publisher=@Publisher, 
			GroupID = GroupID, LastSaved=GETDATE(), Spatial_Display=@Spatial_Display, Institution_Display=@Institution_Display, 
			Edition_Display=@Edition_Display, Material_Display=@Material_Display, Measurement_Display=@Measurement_Display, 
			StylePeriod_Display=@StylePeriod_Display, Technique_Display=@Technique_Display, Subjects_Display=@Subjects_Display 
		where ( ItemID = @ItemID );

		-- Set the existing flag to true (1)
		set @Existing = 1;
	end
	else
	begin
	
		-- Verify the VID is a complete bibid, otherwise find the next one
		if ( LEN(@VID) < 5 )
		begin
			declare @next_vid_number int;

			-- Find the next vid number
			select @next_vid_number = isnull(CAST(MAX(VID) as int) + 1,-1)
			from SobekCM_Item
			where GroupID = @GroupID;
			
			-- If no matches to this BibID, just start at 00001
			if ( @next_vid_number < 0 )
			begin
				select @New_VID = '00001';
			end
			else
			begin
				select @New_VID = RIGHT('0000' + (CAST( @next_vid_number as varchar(5))), 5);	
			end;	
		end;
		
		-- Add the values to the main SobekCM_Item table first
		insert into SobekCM_Item ( VID, [PageCount], FileCount, Deleted, Title, SortTitle, AccessMethod, Link, CreateDate, PubDate, SortDate, Author, Spatial_KML, Spatial_KML_Distance, GroupID, LastSaved, Donor, Publisher, Spatial_Display, Institution_Display, Edition_Display, Material_Display, Measurement_Display, StylePeriod_Display, Technique_Display, Subjects_Display )
		values (  @New_VID, @PageCount, @FileCount, 0, @Title, @SortTitle, @AccessMethod, @Link, @CreateDate, @PubDate, @SortDate, @Author, @Spatial_KML, @Spatial_KML_Distance, @GroupID, GETDATE(), @Donor, @Publisher, @Spatial_Display, @Institution_Display, @Edition_Display, @Material_Display, @Measurement_Display, @StylePeriod_Display, @Technique_Display, @Subjects_Display  );

		-- Get the item id identifier for this row
		set @ItemID = @@identity;

		-- Set existing flag to false
		set @Existing = 0;
		
		-- Add the default views
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, 1, '', '');
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, 2, '', '');
	end;

	-- Check for Holding Institution Code
	declare @AggregationID int;
	if ( len ( isnull ( @HoldingCode, '' ) ) > 0 )
	begin
		-- Does this institution already exist?
		if (( select count(*) from SobekCM_Item_Aggregation where Code = @HoldingCode ) = 0 )
		begin
			-- Add new institution
			insert into SobekCM_Item_Aggregation ( Code, [Name], ShortName, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, ContactEmail, HasNewItems )
			values ( @HoldingCode, 'Added automatically', 'Added automatically', 'Added automatically', -1, 'Institution', 'false', 'true', '', 0, 0, 'false', '', 'false' );
		end;
		
		-- Add the link to this holding code ( and any legitimate parent aggregations )
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @HoldingCode;		
	end;

	-- Check for Source Institution Code
	if ( len ( isnull ( @SourceCode, '' ) ) > 0 )
	begin
		-- Does this institution already exist?
		if (( select count(*) from SobekCM_Item_Aggregation where Code = @SourceCode ) = 0 )
		begin
			-- Add new institution
			insert into SobekCM_Item_Aggregation ( Code, [Name], ShortName, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, ContactEmail, HasNewItems )
			values ( @SourceCode, 'Added automatically', 'Added automatically', 'Added automatically', -1, 'Institution', 'false', 'true', '', 0, 0, 'false', '', 'false' );
		end;

		-- Add the link to this holding code ( and any legitimate parent aggregations )
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @SourceCode;	
	end;
	
	-- If a size was included, set that value
	if ( @DiskSize_KB > 0 )
	begin
		update SobekCM_Item set DiskSize_KB = @DiskSize_KB where ItemID=@ItemID;
	end;

	-- Finally set the volume count for this group correctly
	update SobekCM_Item_Group
	set ItemCount = ( select count(*) from SobekCM_Item I where ( I.GroupID = @GroupID ) and ( I.Deleted = 'false' ))
	where GroupID = @GroupID;

commit transaction;
GO


-- Saves all the main data for a new item in a SobekCM library, 
-- including the serial hierarchy, behaviors, tracking, and basic item information
-- Written by Mark Sullivan ( January 2011 )
ALTER PROCEDURE [dbo].[SobekCM_Save_New_Item]
	@GroupID int,
	@VID varchar(5),
	@PageCount int,
	@FileCount int,
	@Title nvarchar(500),
	@SortTitle nvarchar(500), --NEW
	@AccessMethod int,
	@Link varchar(500),
	@CreateDate datetime,
	@PubDate nvarchar(100),
	@SortDate bigint,
	@Author nvarchar(1000),
	@Spatial_KML varchar(4000),
	@Spatial_KML_Distance float,
	@DiskSize_KB bigint,
	@Spatial_Display nvarchar(1000), 
	@Institution_Display nvarchar(1000), 
	@Edition_Display nvarchar(1000),
	@Material_Display nvarchar(1000),
	@Measurement_Display nvarchar(1000), 
	@StylePeriod_Display nvarchar(1000), 
	@Technique_Display nvarchar(1000), 
	@Subjects_Display nvarchar(1000), 
	@Donor nvarchar(250),
	@Publisher nvarchar(1000),
	@TextSearchable bit,
	@MainThumbnail varchar(100),
	@MainJPEG varchar(100),
	@IP_Restriction_Mask smallint,
	@CheckoutRequired bit,
	@AggregationCode1 varchar(20),
	@AggregationCode2 varchar(20),
	@AggregationCode3 varchar(20),
	@AggregationCode4 varchar(20),
	@AggregationCode5 varchar(20),
	@AggregationCode6 varchar(20),
	@AggregationCode7 varchar(20),
	@AggregationCode8 varchar(20),
	@HoldingCode varchar(20),
	@SourceCode varchar(20),
	@Icon1_Name varchar(50),
	@Icon2_Name varchar(50),
	@Icon3_Name varchar(50),
	@Icon4_Name varchar(50),
	@Icon5_Name varchar(50),
	@Viewer1_TypeID int,
	@Viewer1_Label nvarchar(50),
	@Viewer1_Attribute nvarchar(250),
	@Viewer2_TypeID int,
	@Viewer2_Label nvarchar(50),
	@Viewer2_Attribute nvarchar(250),
	@Viewer3_TypeID int,
	@Viewer3_Label nvarchar(50),
	@Viewer3_Attribute nvarchar(250),
	@Viewer4_TypeID int,
	@Viewer4_Label nvarchar(50),
	@Viewer4_Attribute nvarchar(250),
	@Viewer5_TypeID int,
	@Viewer5_Label nvarchar(50),
	@Viewer5_Attribute nvarchar(250),
	@Viewer6_TypeID int,
	@Viewer6_Label nvarchar(50),
	@Viewer6_Attribute nvarchar(250),
	@Level1_Text varchar(255),
	@Level1_Index int,
	@Level2_Text varchar(255),
	@Level2_Index int,
	@Level3_Text varchar(255),
	@Level3_Index int,
	@Level4_Text varchar(255),
	@Level4_Index int,
	@Level5_Text varchar(255),
	@Level5_Index int,
	@VIDSource varchar(150),
	@CopyrightIndicator smallint, 
	@Born_Digital bit,
	@Dark bit,
	@Material_Received_Date datetime,
	@Material_Recd_Date_Estimated bit,
	@Disposition_Advice int,
	@Disposition_Advice_Notes varchar(150),
	@Internal_Comments nvarchar(1000),
	@Tracking_Box varchar(25),
	@Online_Submit bit,
	@User varchar(50),
	@UserNotes varchar(1000),
	@UserID_To_Link int,
	@ItemID int output,
	@New_VID varchar(5) output
AS
begin transaction

	-- Set the return VID value and itemid first
	set @New_VID = @VID;
	set @ItemID = -1;

	-- Verify this is a new item before doing anything
	if ( (	 select count(*) from SobekCM_Item I where ( I.VID = @VID ) and ( I.GroupID = @GroupID ))  =  0 )
	begin
	
		-- Verify the VID is a complete bibid, otherwise find the next one
		if ( LEN(@VID) < 5 )
		begin
			declare @next_vid_number int;

			-- Find the next vid number
			select @next_vid_number = isnull(CAST(MAX(VID) as int) + 1,-1)
			from SobekCM_Item
			where GroupID = @GroupID;
			
			-- If no matches to this BibID, just start at 00001
			if ( @next_vid_number < 0 )
			begin
				select @New_VID = '00001'
			end
			else
			begin
				select @New_VID = RIGHT('0000' + (CAST( @next_vid_number as varchar(5))), 5);	
			end;	
		end;

		-- Add the values to the main SobekCM_Item table first
		insert into SobekCM_Item ( VID, [PageCount], FileCount, Deleted, Title, SortTitle, AccessMethod, Link, CreateDate, PubDate, SortDate, Author, Spatial_KML, Spatial_KML_Distance, GroupID, LastSaved, Donor, Publisher, TextSearchable, MainThumbnail, MainJPEG, CheckoutRequired, IP_Restriction_Mask, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, Last_MileStone, VIDSource, Born_Digital, Dark, Material_Received_Date, Material_Recd_Date_Estimated, Disposition_Advice, Internal_Comments, Tracking_Box, Disposition_Advice_Notes, Spatial_Display, Institution_Display, Edition_Display, Material_Display, Measurement_Display, StylePeriod_Display, Technique_Display, Subjects_Display )
		values (  @New_VID, @PageCount, @FileCount, 0, @Title, @SortTitle, @AccessMethod, @Link, @CreateDate, @PubDate, @SortDate, @Author, @Spatial_KML, @Spatial_KML_Distance, @GroupID, GETDATE(), @Donor, @Publisher, @TextSearchable, @MainThumbnail, @MainJPEG, @CheckoutRequired, @IP_Restriction_Mask, @Level1_Text, @Level1_Index, @Level2_Text, @Level2_Index, @Level3_Text, @Level3_Index, @Level4_Text, @Level4_Index, @Level5_Text, @Level5_Index, 0, @VIDSource, @Born_Digital, @Dark, @Material_Received_Date, @Material_Recd_Date_Estimated, @Disposition_Advice, @Internal_Comments, @Tracking_Box, @Disposition_Advice_Notes, @Spatial_Display, @Institution_Display, @Edition_Display, @Material_Display, @Measurement_Display, @StylePeriod_Display, @Technique_Display, @Subjects_Display  );
		
		-- Get the item id identifier for this row
		set @ItemID = @@identity;	
		
		-- Set the milestones to complete if this is NON-PRIVATE, NON-DARK, and BORN DIGITAL
		if (( @IP_Restriction_Mask >= 0 ) and ( @Dark = 'false' ) and ( @Born_Digital = 'true' ))
		begin
			update SobekCM_Item
			set Last_MileStone = 4, Milestone_DigitalAcquisition = CreateDate, Milestone_ImageProcessing=CreateDate, Milestone_QualityControl=CreateDate, Milestone_OnlineComplete=CreateDate 
			where ItemID=@ItemID;		
		end;
				
		-- If a size was included, set that value
		if ( @DiskSize_KB > 0 )
		begin
			update SobekCM_Item set DiskSize_KB = @DiskSize_KB where ItemID=@ItemID;
		end;

		-- Finally set the volume count for this group correctly
		update SobekCM_Item_Group
		set ItemCount = ( select count(*) from SobekCM_Item I where ( I.GroupID = @GroupID ) and ( I.Deleted = 'false' ))
		where GroupID = @GroupID;
		
		-- Add the first icon to this object  (this requires the icons have been pre-established )
		declare @IconID int;
		if ( len( isnull( @Icon1_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon1_Name;

			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				values ( @ItemID, @IconID, 1 );
			end;
		end;

		-- Add the second icon to this object  (this requires the icons have been pre-established )
		if ( len( isnull( @Icon2_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon2_Name;

			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				values ( @ItemID, @IconID, 2 );
			end;
		end;

		-- Add the third icon to this object  (this requires the icons have been pre-established )
		if ( len( isnull( @Icon3_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon3_Name;

			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				values ( @ItemID, @IconID, 3 );
			end;
		end;

		-- Add the fourth icon to this object  (this requires the icons have been pre-established )
		if ( len( isnull( @Icon4_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon4_Name;
			
			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				values ( @ItemID, @IconID, 4 );
			end;
		end;

		-- Add the fifth icon to this object  (this requires the icons have been pre-established )
		if ( len( isnull( @Icon5_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon5_Name;

			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				values ( @ItemID, @IconID, 5 );
			end;
		end;

		-- Clear all links to aggregations
		delete from SobekCM_Item_Aggregation_Item_Link where ItemID = @ItemID;

		-- Add all of the aggregations
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode1;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode2;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode3;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode4;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode5;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode6;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode7;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode8;
		
		-- Create one string of all the aggregation codes
		declare @aggregationCodes varchar(100);
		set @aggregationCodes = rtrim(isnull(@AggregationCode1,'') + ' ' + isnull(@AggregationCode2,'') + ' ' + isnull(@AggregationCode3,'') + ' ' + isnull(@AggregationCode4,'') + ' ' + isnull(@AggregationCode5,'') + ' ' + isnull(@AggregationCode6,'') + ' ' + isnull(@AggregationCode7,'') + ' ' + isnull(@AggregationCode8,''));
	
		-- Update matching items to have the aggregation codes value
		update SobekCM_Item set AggregationCodes = @aggregationCodes where ItemID=@ItemID;

		-- Check for Holding Institution Code
		declare @AggregationID int;
		if ( len ( isnull ( @HoldingCode, '' ) ) > 0 )
		begin
			-- Does this institution already exist?
			if (( select count(*) from SobekCM_Item_Aggregation where Code = @HoldingCode ) = 0 )
			begin
				-- Add new institution
				insert into SobekCM_Item_Aggregation ( Code, [Name], ShortName, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, ContactEmail, HasNewItems )
				values ( @HoldingCode, 'Added automatically', 'Added automatically', 'Added automatically', -1, 'Institution', 'false', 'true', '', 0, 0, 'false', '', 'false' );
			end;
			
			-- Add the link to this holding code ( and any legitimate parent aggregations )
			exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @HoldingCode;
		end;

		-- Check for Source Institution Code
		if ( len ( isnull ( @SourceCode, '' ) ) > 0 )
		begin
			-- Does this institution already exist?
			if (( select count(*) from SobekCM_Item_Aggregation where Code = @SourceCode ) = 0 )
			begin
				-- Add new institution
				insert into SobekCM_Item_Aggregation ( Code, [Name], ShortName, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, ContactEmail, HasNewItems )
				values ( @SourceCode, 'Added automatically', 'Added automatically', 'Added automatically', -1, 'Institution', 'false', 'true', '', 0, 0, 'false', '', 'false' );
			end;

			-- Add the link to this holding code ( and any legitimate parent aggregations )
			exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @SourceCode;
		end;
		
		-- Clear the links to all existing viewers
		delete from SobekCM_Item_Viewers where ItemID=@ItemID;
		
		-- Add the first viewer information
		if ( @Viewer1_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer1_TypeID, @Viewer1_Attribute, @Viewer1_Label );
		end;
		
		-- Add the second viewer information
		if ( @Viewer2_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer2_TypeID, @Viewer2_Attribute, @Viewer2_Label );
		end;
		
		-- Add the third viewer information
		if ( @Viewer3_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer3_TypeID, @Viewer3_Attribute, @Viewer3_Label );
		end;
		
		-- Add the fourth viewer information
		if ( @Viewer4_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer4_TypeID, @Viewer4_Attribute, @Viewer4_Label );
		end;
		
		-- Add the fifth viewer information
		if ( @Viewer5_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer5_TypeID, @Viewer5_Attribute, @Viewer5_Label );
		end;
		
		-- Add the first viewer information
		if ( @Viewer6_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer6_TypeID, @Viewer6_Attribute, @Viewer6_Label );
		end;

		-- Add the workhistory for this item being loaded
		if ( @Online_Submit = 'true' )
		begin
			-- Add progress for online submission completed
			insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
			values ( @itemid, 29, getdate(), @user, @usernotes, '' );
		end
		else
		begin  
			-- Add progress for bulk loaded into the system through the Builder
			insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
			values ( @itemid, 40, getdate(), @user, @usernotes, '' );	
		end;		
		
		-- Link this to the user?
		if ( @UserID_To_Link >= 1 )
		begin
			-- Link this user to the bibid, if not already linked
			if (( select COUNT(*) from mySobek_User_Bib_Link where UserID=@UserID_To_Link and GroupID = @groupid ) = 0 )
			begin
				insert into mySobek_User_Bib_Link ( UserID, GroupID )
				values ( @UserID_To_Link, @groupid );
			end;
			
			-- First, see if this user already has a folder named 'Submitted Items'
			declare @userfolderid int
			if (( select count(*) from mySobek_User_Folder where UserID=@UserID_To_Link and FolderName='Submitted Items') > 0 )
			begin
				-- Get the existing folder id
				select @userfolderid = UserFolderID from mySobek_User_Folder where UserID=@UserID_To_Link and FolderName='Submitted Items';
			end
			else
			begin
				-- Add this folder
				insert into mySobek_User_Folder ( UserID, FolderName, isPublic )
				values ( @UserID_To_Link, 'Submitted Items', 'false' );

				-- Get the new id
				select @userfolderid = @@identity;
			end;
			
			-- Add a new link then
			insert into mySobek_User_Item( UserFolderID, ItemID, ItemOrder, UserNotes, DateAdded )
			values ( @userfolderid, @itemid, 1, '', getdate() );
			
			-- Also link using the newer system, which links for statistical reporting, etc..
			-- This will likely replace the 'submitted items' folder technique from above
			insert into mySobek_User_Item_Link( UserID, ItemID, RelationshipID )
			values ( @UserID_To_Link, @ItemID, 1 );
		
		end;
	end;

commit transaction;
GO


-- Procedure updates the information about page count, file count, and disk 
-- size for the online files.
ALTER PROCEDURE [dbo].[SobekCM_Update_Item_Online_Statistics]
	@bibid varchar(10),
	@vid varchar(5),
	@pagecount int,
	@filecount int,
	@disksize_kb bigint
AS
begin
	-- Get the item id
	declare @itemid int
	select @itemid = ItemID
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( BibID = @bibid )
	    and ( I.GroupID = G.GroupID ) 
	    and ( VID = @vid);

	-- Now, update the item row
	update SobekCM_Item
	set [PageCount]=@pagecount, FileCount=@filecount, DiskSize_KB=@disksize_kb
	where ItemID=@itemid;
end;
GO


