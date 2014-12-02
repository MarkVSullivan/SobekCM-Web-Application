

-- Add the select statement into the Item Aggregation search table
alter table SobekCM_Item_Aggregation
add Browse_Results_Display_SQL nvarchar(max) not null default 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID order by T.RowNumber;';
GO

-- Add the COINs statement into the item table, to be returned 
alter table SobekCM_Item
add COinS_OpenURL varchar(max);
GO

-- Create the type of table for use in the browse/search tables
CREATE TYPE TempPagedItemsTableType AS TABLE (
ItemID int NOT NULL, 
RowNumber int NOT NULL );
GO

GRANT EXECUTE ON TYPE::TempPagedItemsTableType to sobek_user;
GO


-- Get a browse of all items in a user's folder
ALTER PROCEDURE [dbo].[mySobek_Get_User_Folder_Browse]
	@userid int,
	@foldername varchar(255),
	@pagesize int, 
	@pagenumber int,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output
AS
BEGIN

	-- Declare the temporary tables
	create table #TEMP_ITEMS ( ItemID int, fk_TitleID int, ItemOrder int, SortDate bigint, UserNotes nvarchar(2000));
	create table #TEMP_PAGED_ITEMS ( ItemID int, RowNumber int, UserNotes nvarchar(2000) );
			
	-- Get the folder id
	declare @folderid int;
	set @folderid = ( select ISNULL(UserFolderID,-1) from mySobek_User_Folder where UserID=@userid and FolderName=@foldername );
	
	-- Determine the start and end rows
	declare @rowstart int; 
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 
	
	-- Get the list of items in the folder
	insert into #TEMP_ITEMS ( ItemID, fk_TitleID, ItemOrder, SortDate, UserNotes )
	select I.ItemID, I.GroupID, A.ItemOrder, isnull( I.SortDate,-1), ISNULL(A.UserNotes,'' )
	from mySobek_User_Item A, SobekCM_Item I
	where ( I.ItemID = A.ItemID )
	  and ( A.UserFolderID = @folderid );
	
	-- Items in a users folder always appear individually, rather than being aggregated into
	-- titles.  This is to allow individual actions to be done against them and for each 
	-- individual user notes to appear correctly
		
	-- Create saved select across items for row numbers
	with ITEMS_SELECT AS
	 (	select I.ItemID, UserNotes,
			ROW_NUMBER() OVER (order by ItemOrder ASC) as RowNumber
			from #TEMP_ITEMS I
			group by I.ItemID, ItemOrder, UserNotes )
				  
	-- Insert the correct rows into the temp item table	
	insert into #TEMP_PAGED_ITEMS ( ItemID, UserNotes, RowNumber )
	select ItemID, UserNotes, RowNumber
	from ITEMS_SELECT
	where RowNumber >= @rowstart
	  and RowNumber <= @rowend;
	  
	-- Return the title information for this page
	select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number as OPAC_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
	from #TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
	where ( T.ItemID = I.ItemID )
	  and ( I.GroupID = G.GroupID )
	order by RowNumber ASC;
	
	-- Return the basic system required item information for this page of results
	select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
	from SobekCM_Item I, #TEMP_PAGED_ITEMS T
	where ( T.ItemID = I.ItemID )
	order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
	
	-- Return the aggregation-specific display values for all the items in this page of results
	select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor, T.UserNotes as User_Notes
	from SobekCM_Metadata_Basic_Search_Table S, #TEMP_PAGED_ITEMS T 
	where S.ItemID = T.ItemID;
	  				
	-- drop the temporary paged table
	drop table #TEMP_PAGED_ITEMS;	

	-- Return the total counts ( since items and titles always equal, return same number for both)
	select @total_items=COUNT(*), @total_titles=COUNT(*)
	from #TEMP_ITEMS; 
	
	-- Return the facets if asked for
	if ( @include_facets = 'true' )
	begin	
		-- Build the aggregation list
		select distinct(A.Code), A.ShortName, Metadata_Count=Count(*)
		from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
		where ( T.ItemID = I.ItemID )
		  and ( I.ItemID = L.ItemID )
		  and ( L.AggregationID = A.AggregationID )
		group by A.Code, A.ShortName
		order by Metadata_Count DESC, ShortName ASC;
		
		-- Return the FIRST facet
		if ( @facettype1 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype1 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the SECOND facet
		if ( @facettype2 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype2 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the THIRD facet
		if ( @facettype3 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype3 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the FOURTH facet
		if ( @facettype4 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype4 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the FIFTH facet
		if ( @facettype5 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype5 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the SIXTH facet
		if ( @facettype6 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype6 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the SEVENTH facet
		if ( @facettype7 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype7 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the EIGHTH facet
		if ( @facettype8 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype8 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
	end;
	
	-- drop the temporary tables
	drop table #TEMP_ITEMS;
	
END;
GO


-- Get a browse of all items in a public folder
ALTER PROCEDURE [dbo].[mySobek_Get_Public_Folder_Browse]
	@folderid int,
	@pagesize int, 
	@pagenumber int,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output
AS
BEGIN

	-- Declare the temporary tables
	create table #TEMP_ITEMS ( ItemID int, fk_TitleID int, ItemOrder int, SortDate bigint, UserNotes nvarchar(2000));
	create table #TEMP_PAGED_ITEMS ( ItemID int, RowNumber int, UserNotes nvarchar(2000) );

	-- Make sure this is a public folder
	set @folderid = ( select ISNULL(UserFolderID, -1) from mySobek_User_Folder F where F.UserFolderID=@folderid and F.isPublic = 'true' );
	
	-- Determine the start and end rows
	declare @rowstart int; 
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 
	
	-- Get the list of items in the folder
	insert into #TEMP_ITEMS ( ItemID, fk_TitleID, ItemOrder, SortDate, UserNotes )
	select I.ItemID, I.GroupID, A.ItemOrder, isnull( I.SortDate,-1), ISNULL(A.UserNotes,'' )
	from mySobek_User_Item A, SobekCM_Item I
	where ( I.ItemID = A.ItemID )
	  and ( A.UserFolderID = @folderid );
	
	-- Items in a users folder always appear individually, rather than being aggregated into
	-- titles.  This is to allow individual actions to be done against them and for each 
	-- individual user notes to appear correctly
		
	-- Create saved select across items for row numbers
	with ITEMS_SELECT AS
	 (	select I.ItemID, UserNotes,
			ROW_NUMBER() OVER (order by ItemOrder ASC) as RowNumber
			from #TEMP_ITEMS I
			group by I.ItemID, ItemOrder, UserNotes )
				  
	-- Insert the correct rows into the temp item table	
	insert into #TEMP_PAGED_ITEMS ( ItemID, UserNotes, RowNumber )
	select ItemID, UserNotes, RowNumber
	from ITEMS_SELECT
	where RowNumber >= @rowstart
	  and RowNumber <= @rowend;
	  
	-- Return the title information for this page
	select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
	from #TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
	where ( T.ItemID = I.ItemID )
	  and ( I.GroupID = G.GroupID )
	order by RowNumber ASC;
	
	-- Return the basic system required item information for this page of results
	select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
	from SobekCM_Item I, #TEMP_PAGED_ITEMS T
	where ( T.ItemID = I.ItemID )
	order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
	
	-- Return the aggregation-specific display values for all the items in this page of results
	select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor, T.UserNotes as User_Notes
	from SobekCM_Metadata_Basic_Search_Table S, #TEMP_PAGED_ITEMS T 
	where S.ItemID = T.ItemID;
	  				
	-- drop the temporary paged table
	drop table #TEMP_PAGED_ITEMS;	

	-- Return the total counts ( since items and titles always equal, return same number for both)
	select @total_items=COUNT(*), @total_titles=COUNT(*)
	from #TEMP_ITEMS; 
	
	-- Return the facets if asked for
	if ( @include_facets = 'true' )
	begin	
		-- Build the aggregation list
		select distinct(A.Code), A.ShortName, Metadata_Count=Count(*)
		from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
		where ( T.ItemID = I.ItemID )
		  and ( I.ItemID = L.ItemID )
		  and ( L.AggregationID = A.AggregationID )
		group by A.Code, A.ShortName
		order by Metadata_Count DESC, ShortName ASC;	
		
		-- Return the FIRST facet
		if ( @facettype1 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype1 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the SECOND facet
		if ( @facettype2 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype2 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the THIRD facet
		if ( @facettype3 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype3 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;		
		
		-- Return the FOURTH facet
		if ( @facettype4 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype4 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the FIFTH facet
		if ( @facettype5 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype5 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the SIXTH facet
		if ( @facettype6 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype6 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the SEVENTH facet
		if ( @facettype7 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype7 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the EIGHTH facet
		if ( @facettype8 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype8 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end
	end;
	
	-- drop the temporary tables
	drop table #TEMP_ITEMS;
	
END;
GO

-- Procedure returns the items by a coordinate search
ALTER PROCEDURE [dbo].[SobekCM_Get_Items_By_Coordinates]
	@lat1 float,
	@long1 float,
	@lat2 float,
	@long2 float,
	@include_private bit,
	@aggregationcode varchar(20),
	@pagesize int, 
	@pagenumber int,
	@sort int,	
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Create the temporary tables first
	-- Create the temporary table to hold all the item id's
	create table #TEMPSUBZERO ( ItemID int );
	create table #TEMPZERO ( ItemID int );
	create table #TEMP_ITEMS ( ItemID int, fk_TitleID int, SortDate bigint, Spatial_KML varchar(4000), Spatial_KML_Distance float );

	-- Is this really just a point search?
	if (( isnull(@lat2,1000) = 1000 ) or ( isnull(@long2,1000) = 1000 ) or (( @lat1=@lat2 ) and ( @long1=@long2 )))
	begin

		-- Select all matching item ids
		insert into #TEMPZERO
		select distinct(itemid) 
		from SobekCM_Item_Footprint
		where (( Point_Latitude = @lat1 ) and ( Point_Longitude = @long1 ))
		   or (((( Rect_Latitude_A >= @lat1 ) and ( Rect_Latitude_B <= @lat1 )) or (( Rect_Latitude_A <= @lat1 ) and ( Rect_Latitude_B >= @lat1)))
	        and((( Rect_Longitude_A >= @long1 ) and ( Rect_Longitude_B <= @long1 )) or (( Rect_Longitude_A <= @long1 ) and ( Rect_Longitude_B >= @long1 ))));

	end
	else
	begin

		-- Select all matching item ids by rectangle
		insert into #TEMPSUBZERO
		select distinct(itemid)
		from SobekCM_Item_Footprint
		where ((( Point_Latitude <= @lat1 ) and ( Point_Latitude >= @lat2 )) or (( Point_Latitude >= @lat1 ) and ( Point_Latitude <= @lat2 )))
		  and ((( Point_Longitude <= @long1 ) and ( Point_Longitude >= @long2 )) or (( Point_Longitude >= @long1 ) and ( Point_Longitude <= @long2 )));
		
		-- Select rectangles which OVERLAP with this rectangle
		insert into #TEMPSUBZERO
		select distinct(itemid)
		from SobekCM_Item_Footprint
		where (((( Rect_Latitude_A >= @lat1 ) and ( Rect_Latitude_A <= @lat2 )) or (( Rect_Latitude_A <= @lat1 ) and ( Rect_Latitude_A >= @lat2 )))
			or ((( Rect_Latitude_B >= @lat1 ) and ( Rect_Latitude_B <= @lat2 )) or (( Rect_Latitude_B <= @lat1 ) and ( Rect_Latitude_B >= @lat2 ))))
		  and (((( Rect_Longitude_A >= @long1 ) and ( Rect_Longitude_A <= @long2 )) or (( Rect_Longitude_A <= @long1 ) and ( Rect_Longitude_A >= @long2 )))
			or ((( Rect_Longitude_B >= @long1 ) and ( Rect_Longitude_B <= @long2 )) or (( Rect_Longitude_B <= @long1 ) and ( Rect_Longitude_B >= @long2 ))));
		
		-- Select rectangles that INCLUDE this rectangle by picking overlaps with one point
		insert into #TEMPSUBZERO
		select distinct(itemid)
		from SobekCM_Item_Footprint
		where ((( @lat1 <= Rect_Latitude_A ) and ( @lat1 >= Rect_Latitude_B )) or (( @lat1 >= Rect_Latitude_A ) and ( @lat1 <= Rect_Latitude_B )))
		  and ((( @long1 <= Rect_Longitude_A ) and ( @long1 >= Rect_Longitude_B )) or (( @long1 >= Rect_Longitude_A ) and ( @long1 <= Rect_Longitude_B )));

		-- Make sure uniqueness applies here as well
		insert into #TEMPZERO
		select distinct(itemid)
		from #TEMPSUBZERO;
	end;
	
	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 

	-- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;

	-- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = coalesce(( select AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode ), -1);

	-- Get the sql which will be used to return the aggregation-specific display values for all the items in this page of results
	declare @item_display_sql nvarchar(max);
	if ( @aggregationid < 0 )
	begin
		select @item_display_sql=coalesce(Browse_Results_Display_SQL, 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID order by T.RowNumber;')
		from SobekCM_Item_Aggregation
		where Code='all';
	end
	else
	begin
		select @item_display_sql=coalesce(Browse_Results_Display_SQL, 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID order by T.RowNumber;')
		from SobekCM_Item_Aggregation
		where AggregationID=@aggregationid;
	end;
			
	-- Was an aggregation included?
	if ( LEN(ISNULL( @aggregationcode,'' )) > 0 )
	begin	
		-- Look for matching the provided aggregation
		insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate, Spatial_KML, Spatial_KML_Distance )
		select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1), Spatial_KML=isnull(Spatial_KML,''), Spatial_KML_Distance
		from #TEMPZERO T1, SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
		where ( CL.ItemID = I.ItemID )
		  and ( CL.AggregationID = @aggregationid )
		  and ( I.Deleted = 'false' )
		  and ( T1.ItemID = I.ItemID )
		  and ( I.IP_Restriction_Mask >= @lower_mask );
	end
	else
	begin	
		-- Look for matching the provided aggregation
		insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate, Spatial_KML, Spatial_KML_Distance )
		select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1), Spatial_KML=isnull(Spatial_KML,''), Spatial_KML_Distance
		from #TEMPZERO T1, SobekCM_Item I
		where ( I.Deleted = 'false' )
		  and ( T1.ItemID = I.ItemID )
		  and ( I.IP_Restriction_Mask >= @lower_mask );
	end;
	
	-- Create the temporary item table variable for paging purposes
	declare @TEMP_PAGED_ITEMS TempPagedItemsTableType;
	
	-- There are essentially THREE major paths of execution, depending on whether this should
	-- be grouped as items within the page requested titles ( sorting by title or the basic
	-- sorting by rank, which ranks this way ) or whether each item should be
	-- returned by itself, such as sorting by individual publication dates, etc..
	-- The default sort for this search is by spatial coordiantes, in which case the same 
	-- title should appear multiple times, if the items in the volume have different coordinates
	
	if ( @sort = 0 )
	begin
		-- create the temporary title table definition
		create table #TEMP_TITLES_ITEMS ( TitleID int, BibID varchar(10), RowNumber int, Spatial_KML varchar(4000), Spatial_Distance float );
		
		-- Compute the number of seperate titles/coordinates
		select fk_TitleID, (COUNT(Spatial_KML)) as assign_value
		into #TEMP1
		from #TEMP_ITEMS I
		group by fk_TitleID, Spatial_KML;
		
		-- Get the TOTAL count of spatial_kmls
		select @total_titles = isnull(SUM(assign_value), 0) from #TEMP1;
		drop table #TEMP1;
		
		-- Total items is simpler to computer
		select @total_items = COUNT(*) from #TEMP_ITEMS;	
		
		-- For now, always return the max lookahead pages
		set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
		
		-- Create saved select across titles for row numbers
		with TITLES_SELECT AS
			(	select GroupID, G.BibID, Spatial_KML, Spatial_KML_Distance,
					ROW_NUMBER() OVER (order by Spatial_KML_Distance ASC, Spatial_KML ASC) as RowNumber
				from #TEMP_ITEMS I, SobekCM_Item_Group G
				where I.fk_TitleID = G.GroupID
				group by G.GroupID, G.BibID, G.SortTitle, Spatial_KML, Spatial_KML_Distance )

		-- Insert the correct rows into the temp title table	
		insert into #TEMP_TITLES_ITEMS ( TitleID, BibID, RowNumber, Spatial_KML, Spatial_Distance )
		select GroupID, BibID, RowNumber, Spatial_KML, Spatial_KML_Distance
		from TITLES_SELECT
		where RowNumber >= @rowstart
		  and RowNumber <= @rowend;
		  
		-- Return the title information for this page
		select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number as OPAC_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier, Spatial_KML, Spatial_Distance
		from #TEMP_TITLES_ITEMS T, SobekCM_Item_Group G
		where ( T.TitleID = G.GroupID )
		order by RowNumber ASC;
		
		-- Get the item id's for the items related to these titles (using rownumber as the new group id)
		insert into @TEMP_PAGED_ITEMS
		select I.ItemID, RowNumber
		from #TEMP_TITLES_ITEMS T, #TEMP_ITEMS M, SobekCM_Item I
		where ( T.TitleID = M.fk_TitleID )
		  and ( M.ItemID = I.ItemID )
		  and ( M.Spatial_KML = T.Spatial_KML )
		  and ( M.Spatial_KML_Distance = T.Spatial_Distance );  
			
		-- Return the basic system required item information for this page of results
		select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
		from SobekCM_Item I, @TEMP_PAGED_ITEMS T
		where ( T.ItemID = I.ItemID )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
								
		-- Return the aggregation-specific display values for all the items in this page of results
		execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 
		
		-- drop the temporary table
		drop table #TEMP_TITLES_ITEMS;	
	end;
	
	if (( @sort < 10 ) and ( @sort > 0 ))
	begin	
		-- create the temporary title table definition
		create table #TEMP_TITLES ( TitleID int, BibID varchar(10), RowNumber int );
		
		-- Get the total counts
		select @total_items=COUNT(*), @total_titles=COUNT(distinct fk_TitleID)
		from #TEMP_ITEMS; 
		
		-- Now, calculate the actual ending row, based on the ration, page information,
		-- and the lookahead factor
		
		-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
		declare @computed_value int;
		select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
		
		-- Compute the minimum value.  This cannot be less than @minpagelookahead.
		declare @floored_value int;
		select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
		
		-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
		declare @actual_pages int;
		select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead)); 

		-- Set the final row again then
		set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1; 		
				  
		-- Create saved select across titles for row numbers
		with TITLES_SELECT AS
			(	select GroupID, G.BibID, 
					ROW_NUMBER() OVER (order by case when @sort=1 THEN G.SortTitle end ASC,											
												case when @sort=2 THEN BibID end ASC,
											    case when @sort=3 THEN BibID end DESC) as RowNumber
				from #TEMP_ITEMS I, SobekCM_Item_Group G
				where I.fk_TitleID = G.GroupID
				group by G.GroupID, G.BibID, G.SortTitle )

		-- Insert the correct rows into the temp title table	
		insert into #TEMP_TITLES ( TitleID, BibID, RowNumber )
		select GroupID, BibID, RowNumber
		from TITLES_SELECT
		where RowNumber >= @rowstart
		  and RowNumber <= @rowend;
	
		-- Return the title information for this page
		select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from #TEMP_TITLES T, SobekCM_Item_Group G
		where ( T.TitleID = G.GroupID )
		order by RowNumber ASC;
		
		-- Get the item id's for the items related to these titles
		insert into @TEMP_PAGED_ITEMS
		select ItemID, RowNumber
		from #TEMP_TITLES T, SobekCM_Item I
		where ( T.TitleID = I.GroupID );			  
			
		-- Return the basic system required item information for this page of results
		select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
		from SobekCM_Item I, @TEMP_PAGED_ITEMS T
		where ( T.ItemID = I.ItemID )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
								
		-- Return the aggregation-specific display values for all the items in this page of results
		execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 	
		
		-- drop the temporary table
		drop table #TEMP_TITLES;
	end;
	
	if ( @sort >= 10 )
	begin	
		-- Since these sorts make each item paired with a single title row,
		-- number of items and titles are equal
		select @total_items=COUNT(*), @total_titles=COUNT(*)
		from #TEMP_ITEMS; 
		
		-- In addition, always return the max lookahead pages
		set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
		
		-- Create saved select across items for row numbers
		with ITEMS_SELECT AS
		 (	select I.ItemID, 
				ROW_NUMBER() OVER (order by case when @sort=10 THEN SortDate end ASC,
											case when @sort=11 THEN SortDate end DESC) as RowNumber
				from #TEMP_ITEMS I
				group by I.ItemID, SortDate )
					  
		-- Insert the correct rows into the temp item table	
		insert into @TEMP_PAGED_ITEMS ( ItemID, RowNumber )
		select ItemID, RowNumber
		from ITEMS_SELECT
		where RowNumber >= @rowstart
		  and RowNumber <= @rowend;
		  
		-- Return the title information for this page
		select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from @TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
		where ( T.ItemID = I.ItemID )
		  and ( I.GroupID = G.GroupID )
		order by RowNumber ASC;
		
		-- Return the basic system required item information for this page of results
		select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
		from SobekCM_Item I, @TEMP_PAGED_ITEMS T
		where ( T.ItemID = I.ItemID )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
			
		-- Return the aggregation-specific display values for all the items in this page of results
		execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 
	end;
	
	-- Return the facets if asked for
	if ( @include_facets = 'true' )
	begin	
		-- Only return the aggregation codes if this was a search across all collections	
		if (( LEN( isnull( @aggregationcode, '')) = 0 ) or ( @aggregationcode='all'))
		begin
			-- Build the aggregation list
			select A.Code, A.ShortName, Metadata_Count=Count(*)
			from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
			where ( T.ItemID = I.ItemID )
			  and ( I.ItemID = L.ItemID )
			  and ( L.AggregationID = A.AggregationID )
			  and ( A.Hidden = 'false' )
			  and ( A.isActive = 'true' )
			  and ( A.Include_In_Collection_Facet = 'true' )
			group by A.Code, A.ShortName
			order by Metadata_Count DESC, ShortName ASC;	
		end;	
		
		-- Return the FIRST facet
		if ( @facettype1 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype1 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the SECOND facet
		if ( @facettype2 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype2 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the THIRD facet
		if ( @facettype3 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype3 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;	
		
		-- Return the FOURTH facet
		if ( @facettype4 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype4 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the FIFTH facet
		if ( @facettype5 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype5 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the SIXTH facet
		if ( @facettype6 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype6 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the SEVENTH facet
		if ( @facettype7 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype7 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the EIGHTH facet
		if ( @facettype8 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype8 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
	end;

	-- drop the temporary tables
	drop table #TEMP_ITEMS;
	drop table #TEMPZERO;
	drop table #TEMPSUBZERO;

END;
GO


-- Return a single browse for a collection or group
-- Written by Mark Sullivan ( December 2006 )
CREATE PROCEDURE [dbo].[SobekCM_Get_Aggregation_Browse_Paged2]
	@code varchar(20),
	@date varchar(10),
	@include_private bit,
	@pagesize int, 
	@pagenumber int,
	@sort int,	
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@item_count_to_use_cached int,
	@total_items int output,
	@total_titles int output
AS
begin

	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;
		
	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 
	
	-- Ensure there is a date value
	select @date=ISNULL(@date,'1/1/1900');

	-- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;

	-- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = ( select ISNULL(AggregationID,-1) from SobekCM_Item_Aggregation where Code=@code );
	
	-- Get the sql which will be used to return the aggregation-specific display values for all the items in this page of results
	declare @item_display_sql nvarchar(max);
	if ( @aggregationid < 0 )
	begin
		set @item_display_sql = 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID order by T.RowNumber;';
	end
	else
	begin
		select @item_display_sql=coalesce(Browse_Results_Display_SQL, 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID order by T.RowNumber;')
		from SobekCM_Item_Aggregation
		where AggregationID=@aggregationid;
	end;
	
	-- Create the temporary item table variable for paging purposes
	declare @TEMP_PAGED_ITEMS TempPagedItemsTableType;

	-- There are essentially two major paths of execution, depending on whether this should
	-- be grouped as items within the page requested titles ( sorting by title or the standard 
	-- create date sort which still lumps them this way ) or whether each item should be
	-- returned by itself, such as sorting by individual publication dates, etc..	
	if ( @sort < 10 )
	begin	
		-- Create temporary title table variale
		declare @TEMP_TITLES table ( TitleID int, BibID varchar(10), RowNumber int );		
		
		-- Return the total counts, if requested
		select @total_items=COUNT(distinct I.ItemID), @total_titles=COUNT(distinct I.GroupID)
		from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
		where ( CL.ItemID = I.ItemID )
		  and ( CL.AggregationID = @aggregationid )
		  and ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.CreateDate >= @date )
		  and ( I.Dark = 'false' );
		  
		-- Now, calculate the actual ending row, based on the ration, page information,
		-- and the lookahead factor		
		-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
		if (( @total_items > 0 ) and ( @total_titles > 0 ))
		begin
			declare @computed_value int;
			select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
			
			-- Compute the minimum value.  This cannot be less than @minpagelookahead.
			declare @floored_value int;
			select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
			
			-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
			declare @actual_pages int;
			select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead));

			-- Set the final row again then
			set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1;  
		end;	

		-- Create saved select across titles for row numbers
		with TITLES_SELECT AS
		 (	select G.GroupID, G.BibID, 
				ROW_NUMBER() OVER (order by case when @sort=1 THEN G.SortTitle end,
											case when @sort=0 THEN Max(I.CreateDate) end DESC,
											case when @sort=2 THEN BibID end ASC,
											case when @sort=3 THEN BibID end DESC) as RowNumber
				from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item_Group G
				where ( CL.ItemID = I.ItemID )
				  and ( CL.AggregationID = @aggregationid )
				  and ( I.GroupID = G.GroupID )
				  and ( I.IP_Restriction_Mask >= @lower_mask )
				  and ( I.CreateDate >= @date )
				  and ( I.Dark = 'false' )
				group by G.GroupID, G.BibID, G.SortTitle )
				  
		-- Insert the correct rows into the temp title table	
		insert into @TEMP_TITLES ( TitleID, BibID, RowNumber )
		select GroupID, BibID, RowNumber
		from TITLES_SELECT
		where RowNumber >= @rowstart
		  and RowNumber <= @rowend;
		
		-- Return the title information for this page
		select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number as OPAC_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from @TEMP_TITLES T, SobekCM_Item_Group G
		where ( T.TitleID = G.GroupID )
		order by RowNumber ASC;
		
		-- Get the item id's for the items related to these titles
		insert into @TEMP_PAGED_ITEMS
		select I.ItemID, RowNumber
		from @TEMP_TITLES T, SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
		where ( T.TitleID = I.GroupID )
		  and ( CL.ItemID = I.ItemID )
		  and ( CL.AggregationID = @aggregationid )
		  and ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.CreateDate >= @date )		
		  and ( I.Dark = 'false' );	  
		
		-- Return the basic system required item information for this page of results
		select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
		from SobekCM_Item I, @TEMP_PAGED_ITEMS T
		where ( T.ItemID = I.ItemID )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
							
		-- Return the aggregation-specific display values for all the items in this page of results
		execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 
	end
	else
	begin
				
		-- Return the total counts, if requested
		select @total_items=COUNT(distinct I.ItemID)
		from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
		where ( CL.ItemID = I.ItemID )
		  and ( CL.AggregationID = @aggregationid )
		  and ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.CreateDate >= @date )
		  and ( I.Dark = 'false' );
		  
		-- Since these sorts make each item paired with a single title row,
		-- number of items and titles are equal
		set @total_titles = @total_items;
		
		-- In addition, always return the max lookahead pages
		set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
		
		if ( @sort < 12 )
		begin		
			-- Create saved select across items for row numbers
			with ITEMS_SELECT AS
			 (	select I.ItemID, 
					ROW_NUMBER() OVER (order by case when @sort=10 THEN isnull(SortDate,9223372036854775807)  end ASC,
												case when @sort=11 THEN isnull(SortDate,-1) end DESC ) as RowNumber
					from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
					where ( CL.ItemID = I.ItemID )
					  and ( CL.AggregationID = @aggregationid )
					  and ( I.IP_Restriction_Mask >= @lower_mask )
					  and ( I.CreateDate >= @date )
					  and ( I.Dark = 'false' )
					group by I.ItemID, SortDate )
						  
			-- Insert the correct rows into the temp item table	
			insert into @TEMP_PAGED_ITEMS ( ItemID, RowNumber )
			select ItemID, RowNumber
			from ITEMS_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
		end
		else
		begin
			-- Create saved select across items for row numbers
			with ITEMS_SELECT AS
			 (	select I.ItemID, 
					ROW_NUMBER() OVER (order by case when @sort=12 THEN I.CreateDate end DESC ) as RowNumber
					from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
					where ( CL.ItemID = I.ItemID )
					  and ( CL.AggregationID = @aggregationid )
					  and ( I.IP_Restriction_Mask >= @lower_mask )
					  and ( I.CreateDate >= @date )
					  and ( I.Dark = 'false' )
					group by I.ItemID, I.CreateDate )
						  
			-- Insert the correct rows into the temp item table	
			insert into @TEMP_PAGED_ITEMS ( ItemID, RowNumber )
			select ItemID, RowNumber
			from ITEMS_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
		end		
		  
		-- Return the title information for this page
		select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number as OPAC_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from @TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
		where ( T.ItemID = I.ItemID )
		  and ( I.GroupID = G.GroupID )
		order by RowNumber ASC;
		
		-- Return the basic system required item information for this page of results
		select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
		from SobekCM_Item I, @TEMP_PAGED_ITEMS T
		where ( T.ItemID = I.ItemID )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
			
		-- Return the aggregation-specific display values for all the items in this page of results
		execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 
	end
	
	-- Return the facets if asked for
	if ( @include_facets = 'true' )
	begin			
	
		-- Since this is an aggregation browse, can possibly use the cached
		-- metadata links to the aggregation for the facets.  Only do if this is
		-- over the value provided though
		if (( @total_items >= @item_count_to_use_cached ) and ( @date <= '1/1/1990' ))
		begin
				
			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype1 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype2 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype3 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;	
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype4 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype5 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype6 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype7 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype8 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
		
		end
		else
		begin
		
			-- Get the list of all item ids 
			select distinct(I.ItemID) as ItemID
			into #TEMP_ITEMS_FACETS
			from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
			where ( CL.ItemID = I.ItemID )
			  and ( CL.AggregationID = @aggregationid )
			  and ( I.IP_Restriction_Mask >= @lower_mask )
			  and ( I.CreateDate >= @date );

			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype1 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype2 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype3 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;	
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype4 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype5 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype6 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype7 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype8 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- drop this temporary table
			drop table #TEMP_ITEMS_FACETS;
		end;
	end;
	
	SET NOCOUNT ON;
end;
GO


-- Return a single browse for the entire digital library
-- Written by Mark Sullivan ( September 2011 )
CREATE PROCEDURE [dbo].[SobekCM_Get_All_Browse_Paged2]
	@date varchar(10),
	@include_private bit,
	@pagesize int, 
	@pagenumber int,
	@sort int,	
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@item_count_to_use_cached int,
	@total_items int output,
	@total_titles int output
AS
begin 

	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;
	
	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int;
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 
	
	-- Ensure there is a date value
	select @date=ISNULL(@date,'1/1/1900');

	-- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;
	
	-- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = ( select ISNULL(AggregationID,-1) from SobekCM_Item_Aggregation where Code='all' );

	-- Get the sql which will be used to return the aggregation-specific display values for all the items in this page of results
	declare @item_display_sql nvarchar(max);
	if ( @aggregationid < 0 )
	begin
		set @item_display_sql = 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID order by T.RowNumber;';
	end
	else
	begin
		select @item_display_sql=coalesce(Browse_Results_Display_SQL, 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID order by T.RowNumber;')
		from SobekCM_Item_Aggregation
		where AggregationID=@aggregationid;
	end;
	
	-- Create the temporary item table variable for paging purposes
	declare @TEMP_PAGED_ITEMS TempPagedItemsTableType;

	-- There are essentially two major paths of execution, depending on whether this should
	-- be grouped as items within the page requested titles ( sorting by title or the standard 
	-- create date sort which still lumps them this way ) or whether each item should be
	-- returned by itself, such as sorting by individual publication dates, etc..	
	if ( @sort < 10 )
	begin	
		-- Create temporary title table variable
		declare @TEMP_TITLES table ( TitleID int primary key, BibID varchar(10), RowNumber int );		
		
		-- Return the total counts, if requested
		select @total_items=COUNT(distinct I.ItemID), @total_titles=COUNT(distinct I.GroupID)
		from SobekCM_Item I
		where ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.CreateDate >= @date );
		  
		-- Now, calculate the actual ending row, based on the ration, page information,
		-- and the lookahead factor		
		-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
		if (( @total_items > 0 ) and ( @total_titles > 0 ))
		begin
			declare @computed_value int;
			select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
			
			-- Compute the minimum value.  This cannot be less than @minpagelookahead.
			declare @floored_value int;
			select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
			
			-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
			declare @actual_pages int;
			select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead));

			-- Set the final row again then
			set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1;  
		end;	

		-- Create saved select across titles for row numbers
		with TITLES_SELECT AS
		 (	select G.GroupID, G.BibID, 
				ROW_NUMBER() OVER (order by case when @sort=1 THEN G.SortTitle end,
											case when @sort=0 THEN Max(I.CreateDate) end DESC,
											case when @sort=2 THEN BibID end ASC,
											case when @sort=3 THEN BibID end DESC) as RowNumber
				from SobekCM_Item I, SobekCM_Item_Group G
				where ( I.GroupID = G.GroupID )
				  and ( I.IP_Restriction_Mask >= @lower_mask )
				  and ( I.CreateDate >= @date )
				group by G.GroupID, G.BibID, G.SortTitle )
				  
		-- Insert the correct rows into the temp title table	
		insert into @TEMP_TITLES ( TitleID, BibID, RowNumber )
		select GroupID, BibID, RowNumber
		from TITLES_SELECT
		where RowNumber >= @rowstart
		  and RowNumber <= @rowend;
		
		-- Return the title information for this page
		select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number as OPAC_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from @TEMP_TITLES T, SobekCM_Item_Group G
		where ( T.TitleID = G.GroupID )
		order by RowNumber ASC;
				
		-- Get the item id's for the items related to these titles
		insert into @TEMP_PAGED_ITEMS
		select I.ItemID, RowNumber
		from @TEMP_TITLES T, SobekCM_Item I
		where ( T.TitleID = I.GroupID )
		  and ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.CreateDate >= @date )		
		  and ( I.Dark = 'false' );	  
		
		-- Return the basic system required item information for this page of results
		select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
		from SobekCM_Item I, @TEMP_PAGED_ITEMS T
		where ( T.ItemID = I.ItemID )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
							
		-- Return the aggregation-specific display values for all the items in this page of results
		execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 
	end
	else
	begin
				
		-- Return the total counts, if requested
		select @total_items=COUNT(distinct I.ItemID)
		from SobekCM_Item I
		where ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.CreateDate >= @date );
		  
		-- Since these sorts make each item paired with a single title row,
		-- number of items and titles are equal
		set @total_titles = @total_items;
		
		-- In addition, always return the max lookahead pages
		set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
		
		-- Create saved select across items for row numbers
		with ITEMS_SELECT AS
		 (	select I.ItemID, 
				ROW_NUMBER() OVER (order by case when @sort=10 THEN isnull(SortDate,9223372036854775807)  end ASC,
											case when @sort=11 THEN isnull(SortDate,-1) end DESC) as RowNumber
				from SobekCM_Item I
				where ( I.IP_Restriction_Mask >= @lower_mask )
				  and ( I.CreateDate >= @date )
				group by I.ItemID, SortDate )
					  
		-- Insert the correct rows into the temp item table	
		insert into @TEMP_PAGED_ITEMS ( ItemID, RowNumber )
		select ItemID, RowNumber
		from ITEMS_SELECT
		where RowNumber >= @rowstart
		  and RowNumber <= @rowend;
		  
		-- Return the title information for this page
		select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number as OPAC_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from @TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
		where ( T.ItemID = I.ItemID )
		  and ( I.GroupID = G.GroupID )
		order by RowNumber ASC;
		
		-- Return the basic system required item information for this page of results
		select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
		from SobekCM_Item I, @TEMP_PAGED_ITEMS T
		where ( T.ItemID = I.ItemID )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
			
		-- Return the aggregation-specific display values for all the items in this page of results
		execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 
	end;
	
	-- Return the facets if asked for
	if ( @include_facets = 'true' )
	begin	
	
		-- Since this is an aggregation browse, can possibly use the cached
		-- metadata links to the aggregation for the facets.  Only do if this is
		-- over the value provided though
		if (( @total_items >= @item_count_to_use_cached ) and ( @date <= '1/1/1990' ))
		begin
		
			-- Build the aggregation list
			if ( @date > '1/1/1900' )
			begin
				-- Since this was for NEW items, will need to calculate the counts here.
				-- This takes a LONG time, so we avoid it in as many cases as possible
				with AGGREGATION_COUNTS AS 
					(	select distinct( AggregationID ), COUNT(*) as Item_Count
						from SobekCM_Item_Aggregation_Item_Link as L inner join
							 SobekCM_Item as I on L.ItemID = I.ItemID
						where ( I.IP_Restriction_Mask >= @lower_mask )
						  and ( I.CreateDate >= @date )
						group by AggregationID )			
				select top 100 A.Code, A.ShortName, Item_Count as Metadata_Count
				from SobekCM_Item_Aggregation A, AGGREGATION_COUNTS L
				where ( L.AggregationID = A.AggregationID )
				  and ( A.Include_In_Collection_Facet = 'true' )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )	
				order by Metadata_Count DESC, ShortName ASC;
			end
			else
			begin
				-- Just use the last current item count
				select top 100 A.Code, A.ShortName, Current_Item_Count as Metadata_Count
				from SobekCM_Item_Aggregation A
				where ( A.Include_In_Collection_Facet = 'true' )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				  and ( Current_Item_Count > 0 )
				order by Metadata_Count DESC, ShortName ASC;			
			end;
			
			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype1 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype2 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype3 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype4 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype5 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype6 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype7 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype8 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
		
		end
		else
		begin
		
			-- Get the list of all item ids 
			select distinct(I.ItemID) as ItemID
			into #TEMP_ITEMS_FACETS
			from SobekCM_Item I
			where ( I.IP_Restriction_Mask >= @lower_mask )
			  and ( I.CreateDate >= @date );	  
			  			  
			-- Build the aggregation list
			if ( @date > '1/1/1900' )
			begin
				-- Since this was for NEW items, will need to calculate the counts here.
				-- This takes a LONG time, so we avoid it in as many cases as possible
				select top 100 A.Code, A.ShortName, Metadata_Count=Count(*)
				from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link CL, #TEMP_ITEMS_FACETS T
				where ( CL.ItemID = T.ItemID )
				  and ( CL.AggregationID = A.AggregationID )
				  and ( A.Include_In_Collection_Facet = 'true' )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				group by A.Code, A.ShortName
				order by Metadata_Count DESC, ShortName ASC;
			end
			else
			begin
				-- Just use the last current item count
				select top 100 A.Code, A.ShortName, Current_Item_Count as Metadata_Count
				from SobekCM_Item_Aggregation A
				where ( A.Include_In_Collection_Facet = 'true' )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				  and ( Current_Item_Count > 0 )
				order by Metadata_Count DESC, ShortName ASC;			
			end;
			

			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype1 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype2 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype3 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype4 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype5 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype6 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype7 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype8 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- drop this temporary table
			drop table #TEMP_ITEMS_FACETS;
		end;
	end;
	
	SET NOCOUNT ON;
end;
GO

CREATE PROCEDURE [dbo].[SobekCM_Metadata_Basic_Search_Paged2] 
	@searchcondition nvarchar(4000),
	@include_private bit,
	@aggregationcode varchar(20),
	@daterange_start bigint,
	@daterange_end bigint,
	@pagesize int, 
	@pagenumber int,
	@sort int,
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output,
	@all_collections_items int output,
	@all_collections_titles int output												
AS
BEGIN
	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	-- Create the temporary tables first
	-- Create the temporary table to hold all the item id's
	create table #TEMP_ITEMS ( ItemID int primary key, fk_TitleID int, Hit_Count int, SortDate bigint );

	-- Determine the start and end rows
	declare @rowstart int; 
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + ( @pagesize * @minpagelookahead ) - 1; 

	-- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;
		
	-- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = coalesce( (select AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode), -1 );
	
	-- Get the sql which will be used to return the aggregation-specific display values for all the items in this page of results
	declare @item_display_sql nvarchar(max);
	if ( @aggregationid < 0 )
	begin
		select @item_display_sql=coalesce(Browse_Results_Display_SQL, 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID order by T.RowNumber;')
		from SobekCM_Item_Aggregation
		where Code='all';
	end
	else
	begin
		select @item_display_sql=coalesce(Browse_Results_Display_SQL, 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID order by T.RowNumber;')
		from SobekCM_Item_Aggregation
		where AggregationID=@aggregationid;
	end;
	
	-- Perform the actual metadata search differently, depending on whether an aggregation was 
	-- included to limit this search
	if (( @daterange_end < 0 ) and ( @daterange_start < 0 ))
	begin
		if ( @aggregationid > 0 )
		begin		  
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID, Hit_Count, SortDate )
			select CL.ItemID, I.GroupID, KEY_TBL.RANK, SortDate
			from SobekCM_Item AS I inner join
				 SobekCM_Item_Aggregation_Item_Link AS CL ON CL.ItemID = I.ItemID inner join
				 CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @SearchCondition ) AS KEY_TBL on KEY_TBL.[KEY] = I.ItemID
			where ( I.Deleted = 'false' )
			  and ( CL.AggregationID = @aggregationid )
			  and ( I.IP_Restriction_Mask >= @lower_mask )
			  and ( I.Dark = 'false' );
		end
		else
		begin	
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID, Hit_Count, SortDate )
			select I.ItemID, I.GroupID, KEY_TBL.RANK, SortDate
			from SobekCM_Item AS I inner join
				 CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @SearchCondition ) AS KEY_TBL on KEY_TBL.[KEY] = I.ItemID
			where ( I.Deleted = 'false' )
			  and ( I.IP_Restriction_Mask >= @lower_mask ) 
			  and ( I.IncludeInAll = 'true' )
			  and ( I.Dark = 'false' );
		end;
	end
	else
	begin
		if ( @aggregationid > 0 )
		begin		  
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID, Hit_Count, SortDate )
			select CL.ItemID, I.GroupID, KEY_TBL.RANK, SortDate
			from SobekCM_Item AS I inner join
				 SobekCM_Item_Aggregation_Item_Link AS CL ON CL.ItemID = I.ItemID inner join
				 CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @SearchCondition ) AS KEY_TBL on KEY_TBL.[KEY] = I.ItemID
			where ( I.Deleted = 'false' )
			  and ( CL.AggregationID = @aggregationid )
			  and ( I.IP_Restriction_Mask >= @lower_mask )
			  and ( I.Dark = 'false' )
			  and ( I.SortDate >= @daterange_start )
			  and ( I.SortDate <= @daterange_end );
		end
		else
		begin	
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID, Hit_Count, SortDate )
			select I.ItemID, I.GroupID, KEY_TBL.RANK, SortDate
			from SobekCM_Item AS I inner join
				 CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @SearchCondition ) AS KEY_TBL on KEY_TBL.[KEY] = I.ItemID
			where ( I.Deleted = 'false' )
			  and ( I.IP_Restriction_Mask >= @lower_mask ) 
			  and ( I.IncludeInAll = 'true' )
			  and ( I.Dark = 'false' )			  
			  and ( I.SortDate >= @daterange_start )
			  and ( I.SortDate <= @daterange_end );
		end;	
	end;
	
	-- If there were no results at all, check the count in the entire library
	if ( ( select COUNT(*) from #TEMP_ITEMS ) = 0 )
	begin
		-- Set the items and titles correctly
		set @total_items = 0;
		set @total_titles = 0;
		
		-- If there was an aggregation id, just return the counts for the whole library
		if ( @aggregationid > 0 )	
		begin
			-- Get all items in the entire library then
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID, Hit_Count, SortDate )
			select I.ItemID, I.GroupID, KEY_TBL.RANK, SortDate
			from SobekCM_Item AS I inner join
				CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @SearchCondition ) AS KEY_TBL on KEY_TBL.[KEY] = I.ItemID
			where ( I.Deleted = 'false' )
			  and ( I.IP_Restriction_Mask >= @lower_mask ) 
			  and ( I.IncludeInAll = 'true' )
			  and ( I.Dark = 'false' );
			  
			-- Return these counts
			select @all_collections_items=COUNT(*), @all_collections_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS;		
		end;
	end
	else
	begin	
		-- Create the temporary item table variable for paging purposes
		declare @TEMP_PAGED_ITEMS TempPagedItemsTableType;
			
		-- There are essentially two major paths of execution, depending on whether this should
		-- be grouped as items within the page requested titles ( sorting by title or the basic
		-- sorting by rank, which ranks this way ) or whether each item should be
		-- returned by itself, such as sorting by individual publication dates, etc..	
		if ( @sort < 10 )
		begin	
			-- create the temporary title table definition
			declare @TEMP_TITLES table ( TitleID int, BibID varchar(10), RowNumber int);		
			
			-- Get the total counts
			select @total_items=COUNT(*), @total_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS;
			
			-- Now, calculate the actual ending row, based on the ration, page information,
			-- and the lookahead factor
			if (( @total_items > 0 ) and ( @total_titles > 0 ))
			begin	
				-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
				declare @computed_value int;
				select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
				
				-- Compute the minimum value.  This cannot be less than @minpagelookahead.
				declare @floored_value int;
				select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
				
				-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
				declare @actual_pages int;
				select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead)); 

				-- Set the final row again then
				set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1; 
			end;
					  
			-- Create saved select across titles for row numbers
			with TITLES_SELECT AS
				(	select GroupID, G.BibID, 
						ROW_NUMBER() OVER (order by case when @sort=0 THEN (SUM(Hit_COunt)/COUNT(*)) end DESC,
													case when @sort=1 THEN G.SortTitle end ASC,												
													case when @sort=2 THEN BibID end ASC,
													case when @sort=3 THEN BibID end DESC) as RowNumber
					from #TEMP_ITEMS I, SobekCM_Item_Group G
					where I.fk_TitleID = G.GroupID
					group by G.GroupID, G.BibID, G.SortTitle )

			-- Insert the correct rows into the temp title table	
			insert into @TEMP_TITLES ( TitleID, BibID, RowNumber )
			select GroupID, BibID, RowNumber
			from TITLES_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
			
			-- Return the title information for this page
			select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number as OPAC_Number, G.OCLC_Number, coalesce(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], coalesce(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, coalesce(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_TITLES T, SobekCM_Item_Group G
			where ( T.TitleID = G.GroupID )
			order by RowNumber ASC;
			
			-- Get the item id's for the items related to these titles
			insert into @TEMP_PAGED_ITEMS
			select ItemID, RowNumber
			from @TEMP_TITLES T, SobekCM_Item I
			where ( T.TitleID = I.GroupID );			  
			
			-- Return the basic system required item information for this page of results
			select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
			from SobekCM_Item I, @TEMP_PAGED_ITEMS T
			where ( T.ItemID = I.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
								
			-- Return the aggregation-specific display values for all the items in this page of results
			execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 		
		end
		else
		begin
		
			-- Since these sorts make each item paired with a single title row,
			-- number of items and titles are equal
			select @total_items=COUNT(*), @total_titles=COUNT(*)
			from #TEMP_ITEMS;
			
			-- In addition, always return the max lookahead pages
			set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 

			-- Create saved select across items for row numbers
			with ITEMS_SELECT AS
			 (	select I.ItemID, 
					ROW_NUMBER() OVER (order by case when @sort=10 THEN coalesce(SortDate,9223372036854775807)  end ASC,
												case when @sort=11 THEN coalesce(SortDate,-1) end DESC) as RowNumber
					from #TEMP_ITEMS I
					group by I.ItemID, SortDate )
						  
			-- Insert the correct rows into the temp item table	
			insert into @TEMP_PAGED_ITEMS ( ItemID, RowNumber )
			select ItemID, RowNumber
			from ITEMS_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
			  
			-- Return the title information for this page
			select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number as OPAC_Number, G.OCLC_Number, coalesce(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], coalesce(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, coalesce(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
			where ( T.ItemID = I.ItemID )
			  and ( I.GroupID = G.GroupID )
			order by RowNumber ASC;
			
			-- Return the basic system required item information for this page of results
			select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
			from SobekCM_Item I, @TEMP_PAGED_ITEMS T
			where ( T.ItemID = I.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
			
			-- Return the aggregation-specific display values for all the items in this page of results
			execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 
		end

		-- Return the facets if asked for
		if ( @include_facets = 'true' )
		begin	
			-- Build the aggregation list
			if ( LEN( isnull( @aggregationcode, '')) = 0 )
			begin
				select A.Code, A.ShortName, Metadata_Count=Count(*)
				from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
				where ( T.ItemID = I.ItemID )
				  and ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				  and ( A.Include_In_Collection_Facet = 'true' )
				group by A.Code, A.ShortName
				order by Metadata_Count DESC, ShortName ASC;
			end;
			
			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype1 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype2 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype3 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype4 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype5 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype6 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype7 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype8 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
		end; -- End overall FACET block
	end; -- End else statement entered if there are any results to return
	
	-- Drop the temporary table
	drop table #TEMP_ITEMS;
	
	SET NOCOUNT OFF;
END;
GO


-- Perform an EXACT match type of search against one field of metadata
CREATE PROCEDURE [dbo].[SobekCM_Metadata_Exact_Search_Paged2] 
	@term1 nvarchar(512),
	@field1 int,
	@include_private bit,
	@aggregationcode varchar(20),
	@daterange_start bigint,
	@daterange_end bigint,
	@pagesize int, 
	@pagenumber int,
	@sort int,
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output,
	@all_collections_items int output,
	@all_collections_titles int output			
AS
BEGIN

	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	-- Create the temporary table variable first
	-- Create the temporary table variable to hold all the item id's
	create table #TEMP_ITEMS ( ItemID int primary key, fk_TitleID int, Hit_Count int, SortDate bigint );

	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 

	-- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;

	-- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = coalesce(( select AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode ), -1);
	
	-- Get the sql which will be used to return the aggregation-specific display values for all the items in this page of results
	declare @item_display_sql nvarchar(max);
	if ( @aggregationid < 0 )
	begin
		select @item_display_sql=coalesce(Browse_Results_Display_SQL, 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID order by T.RowNumber;')
		from SobekCM_Item_Aggregation
		where Code='all';
	end
	else
	begin
		select @item_display_sql=coalesce(Browse_Results_Display_SQL, 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID order by T.RowNumber;')
		from SobekCM_Item_Aggregation
		where AggregationID=@aggregationid;
	end;
	
	-- Determine the first 100 of the search term
	declare @term_start varchar(100);
	set @term_start = SUBSTRING(@term1, 1, 100);
	
	-- Perform the actual metadata search differently, depending on whether an aggregation was 
	-- included to limit this search
	if (( @daterange_end < 0 ) and ( @daterange_start < 0 ))
	begin
		if ( @aggregationid > 0 )
		begin 
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate )
			select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1)
			from SobekCM_Item AS I inner join
				 SobekCM_Item_Aggregation_Item_Link AS CL ON CL.ItemID = I.ItemID inner join
				 SobekCM_Metadata_Unique_Link ML on ML.ItemID = I.ItemID inner join
				 SobekCM_Metadata_Unique_Search_Table M ON M.MetadataID = ML.MetadataID and MetadataTypeID = @field1 and M.MetadataValueStart = @term_start and M.MetadataValue = @term1
			where ( I.Deleted = 'false' )
			  and ( CL.AggregationID = @aggregationid )
			  and ( I.IP_Restriction_Mask >= @lower_mask )
			  and ( I.Dark = 'false' );
		end
		else
		begin	
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate )
			select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1)
			from SobekCM_Item AS I inner join
				 SobekCM_Metadata_Unique_Link ML on ML.ItemID = I.ItemID inner join
				 SobekCM_Metadata_Unique_Search_Table M ON M.MetadataID = ML.MetadataID and MetadataTypeID = @field1 and M.MetadataValueStart = @term_start and M.MetadataValue = @term1
			where ( I.Deleted = 'false' )
			  and ( I.IP_Restriction_Mask >= @lower_mask )	
			  and ( I.IncludeInAll = 'true' )
			  and ( I.Dark = 'false' );
		end;
	end
	else
	begin
		if ( @aggregationid > 0 )
		begin 
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate )
			select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1)
			from SobekCM_Item AS I inner join
				 SobekCM_Item_Aggregation_Item_Link AS CL ON CL.ItemID = I.ItemID inner join
				 SobekCM_Metadata_Unique_Link ML on ML.ItemID = I.ItemID inner join
				 SobekCM_Metadata_Unique_Search_Table M ON M.MetadataID = ML.MetadataID and MetadataTypeID = @field1 and M.MetadataValueStart = @term_start and M.MetadataValue = @term1
			where ( I.Deleted = 'false' )
			  and ( CL.AggregationID = @aggregationid )
			  and ( I.IP_Restriction_Mask >= @lower_mask )
			  and ( I.Dark = 'false' )			  
			  and ( I.SortDate >= @daterange_start )
			  and ( I.SortDate <= @daterange_end );
		end
		else
		begin	
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate )
			select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1)
			from SobekCM_Item AS I inner join
				 SobekCM_Metadata_Unique_Link ML on ML.ItemID = I.ItemID inner join
				 SobekCM_Metadata_Unique_Search_Table M ON M.MetadataID = ML.MetadataID and MetadataTypeID = @field1 and M.MetadataValueStart = @term_start and M.MetadataValue = @term1
			where ( I.Deleted = 'false' )
			  and ( I.IP_Restriction_Mask >= @lower_mask )	
			  and ( I.IncludeInAll = 'true' )
			  and ( I.Dark = 'false' )
			  and ( I.SortDate >= @daterange_start )
			  and ( I.SortDate <= @daterange_end );
		end;
	end;

	-- If there were no results at all, check the count in the entire library
	if ( ( select COUNT(*) from #TEMP_ITEMS ) = 0 )
	begin
		-- Set the items and titles correctly
		set @total_items = 0;
		set @total_titles = 0;
		
		-- If there was an aggregation id, just return the counts for the whole library
		if ( @aggregationid > 0 )	
		begin
			-- Get all items in the entire library then		  
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate )
			select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1)
			from SobekCM_Item AS I inner join
				 SobekCM_Metadata_Unique_Link ML on ML.ItemID = I.ItemID inner join
				 SobekCM_Metadata_Unique_Search_Table M ON M.MetadataID = ML.MetadataID and MetadataTypeID = @field1 and M.MetadataValueStart = @term_start and M.MetadataValue = @term1
			where ( I.Deleted = 'false' )
			  and ( I.IP_Restriction_Mask >= @lower_mask )	
			  and ( I.IncludeInAll = 'true' );	 
			  
			-- Return these counts
			select @all_collections_items=COUNT(*), @all_collections_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS; 		
		end;
	end
	else
	begin	
		-- Create the temporary item table variable for paging purposes
		declare @TEMP_PAGED_ITEMS TempPagedItemsTableType;
		
		-- There are essentially two major paths of execution, depending on whether this should
		-- be grouped as items within the page requested titles ( sorting by title or the basic
		-- sorting by rank, which ranks this way ) or whether each item should be
		-- returned by itself, such as sorting by individual publication dates, etc..		
		if ( @sort < 10 )
		begin	
			-- create the temporary title table definition
			declare @TEMP_TITLES table ( TitleID int, BibID varchar(10), RowNumber int);	
			
			-- Get the total counts
			select @total_items=COUNT(*), @total_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS;
			
			-- Now, calculate the actual ending row, based on the ration, page information,
			-- and the lookahead factor
			if (( @total_items > 0 ) and ( @total_titles > 0 ))
			begin	
				-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
				declare @computed_value int;
				select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
				
				-- Compute the minimum value.  This cannot be less than @minpagelookahead.
				declare @floored_value int;
				select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
				
				-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
				declare @actual_pages int;
				select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead)); 

				-- Set the final row again then
				set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1; 
			end;
					  
			-- Create saved select across titles for row numbers for selecting correct page(s) of results
			with TITLES_SELECT AS
				(	select GroupID, G.BibID, 
						ROW_NUMBER() OVER (order by case when @sort=0 THEN G.SortTitle end ASC,	
													case when @sort=1 THEN G.SortTitle end ASC,												
													case when @sort=2 THEN BibID end ASC,
													case when @sort=3 THEN BibID end DESC) as RowNumber
					from #TEMP_ITEMS I, SobekCM_Item_Group G
					where I.fk_TitleID = G.GroupID
					group by G.GroupID, G.BibID, G.SortTitle )

			-- Insert the correct rows into the temp title table	
			insert into @TEMP_TITLES ( TitleID, BibID, RowNumber )
			select GroupID, BibID, RowNumber
			from TITLES_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
			
			-- Return the title information for this page of results
			select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number as OPAC_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_TITLES T, SobekCM_Item_Group G
			where ( T.TitleID = G.GroupID )
			order by RowNumber ASC;
			
			-- Get the item id's for the items related to these titles
			insert into @TEMP_PAGED_ITEMS
			select ItemID, RowNumber
			from @TEMP_TITLES T, SobekCM_Item I
			where ( T.TitleID = I.GroupID );			  
			
			-- Return the basic system required item information for this page of results
			select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
			from SobekCM_Item I, @TEMP_PAGED_ITEMS T
			where ( T.ItemID = I.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;		
								
			-- Return the aggregation-specific display values for all the items in this page of results
			execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 
		end
		else
		begin		
			-- Since these sorts make each item paired with a single title row,
			-- number of items and titles are equal
			select @total_items=COUNT(*), @total_titles=COUNT(*)
			from #TEMP_ITEMS;
			
			-- In addition, always return the max lookahead pages
			set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
			
			-- Create saved select across items for row numbers
			with ITEMS_SELECT AS
			 (	select I.ItemID, 
					ROW_NUMBER() OVER (order by case when @sort=10 THEN isnull(SortDate,9223372036854775807)  end ASC,
												case when @sort=11 THEN isnull(SortDate,-1) end DESC) as RowNumber
					from #TEMP_ITEMS I
					group by I.ItemID, SortDate )
						  
			-- Insert the correct rows into the temp item table	
			insert into @TEMP_PAGED_ITEMS ( ItemID, RowNumber )
			select ItemID, RowNumber
			from ITEMS_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
			  
			-- Return the title information for this page
			select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number as OPAC_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
			where ( T.ItemID = I.ItemID )
			  and ( I.GroupID = G.GroupID )
			order by RowNumber ASC;
			
			-- Return the basic system required item information for this page of results
			select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
			from SobekCM_Item I, @TEMP_PAGED_ITEMS T
			where ( T.ItemID = I.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
			
			-- Return the aggregation-specific display values for all the items in this page of results
			execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 
		end
		
		-- Return the facets if asked for
		if ( @include_facets = 'true' )
		begin	
			-- Build the aggregation list
			if ( LEN( isnull( @aggregationcode, '')) = 0 )
			begin
				select A.Code, A.ShortName, Metadata_Count=Count(*)
				from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
				where ( T.ItemID = I.ItemID )
				  and ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				  and ( A.Include_In_Collection_Facet = 'true' )
				group by A.Code, A.ShortName
				order by Metadata_Count DESC, ShortName ASC;		
			end;
			
			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype1 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype2 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype3 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype4 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype5 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype6 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype7 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype8 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
		end; -- End overall FACET block
	end; -- End else statement entered if there are any results to return
	
	-- Drop the temporary table
	drop table #TEMP_ITEMS;
			
    Set NoCount OFF;	
END;
GO


-- Perform metadata search 
ALTER PROCEDURE [dbo].[SobekCM_Metadata_Search_Paged2]
	@term1 nvarchar(255),
	@field1 int,
	@link2 int,
	@term2 nvarchar(255),
	@field2 int,
	@link3 int,
	@term3 nvarchar(255),
	@field3 int,
	@link4 int,
	@term4 nvarchar(255),
	@field4 int,
	@link5 int,
	@term5 nvarchar(255),
	@field5 int,
	@link6 int,
	@term6 nvarchar(255),
	@field6 int,
	@link7 int,
	@term7 nvarchar(255),
	@field7 int,
	@link8 int,
	@term8 nvarchar(255),
	@field8 int,
	@link9 int,
	@term9 nvarchar(255),
	@field9 int,
	@link10 int,
	@term10 nvarchar(255),
	@field10 int,
	@include_private bit,
	@aggregationcode varchar(20),	
	@daterange_start bigint,
	@daterange_end bigint,
	@pagesize int, 
	@pagenumber int,
	@sort int,
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output,
	@all_collections_items int output,
	@all_collections_titles int output	
AS
BEGIN
	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;
	
	-- Field#'s indicate which metadata field (if any).  These are numbers from the 
	-- SobekCM_Metadata_Types table.  A field# of -1, means all fields are included.
	
	-- Link#'s indicate if this is an AND-joiner ( intersect ) or an OR-joiner ( union )
	-- 0 = AND, 1 = OR, 2 = AND NOT
	
	-- Examples of using this procedure are:
	-- exec SobekCM_Metadata_Search 'haiti',1,0,'kesse',4,0,'',0
	-- This searches for materials which have haiti in the title AND kesse in the creator
	
	-- exec SobekCM_Metadata_Search 'haiti',1,1,'kesse',-1,0,'',0
	-- This searches for materials which have haiti in the title OR kesse anywhere
	
	-- Create the temporary table variables first
	-- Create the temporary table to hold all the item id's
	create table #TEMPZERO ( ItemID int primary key );
	create table #TEMP_ITEMS ( ItemID int primary key, fk_TitleID int, Hit_Count int, SortDate bigint );
		    
	-- declare both the sql query and the parameter definitions
	declare @SQLQuery AS nvarchar(max);
	declare @rankselection AS nvarchar(1000);
    declare @ParamDefinition AS NVarchar(2000);
		
    -- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = coalesce(( select AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode ), -1);
	
	-- Get the sql which will be used to return the aggregation-specific display values for all the items in this page of results
	declare @item_display_sql nvarchar(max);
	if ( @aggregationid < 0 )
	begin
		select @item_display_sql=coalesce(Browse_Results_Display_SQL, 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID;')
		from SobekCM_Item_Aggregation
		where Code='all';
	end
	else
	begin
		select @item_display_sql=coalesce(Browse_Results_Display_SQL, 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID;')
		from SobekCM_Item_Aggregation
		where AggregationID=@aggregationid;
	end;
	
    -- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;
	    
    -- Start to build the main bulk of the query   
	set @SQLQuery = '';
	
	-- Start with the date range information, if this includes a date range search
	if ( @daterange_end > 0 )
	begin
		set @SQLQuery = 'L.SortDate > ' + cast(@daterange_start as nvarchar(12)) + ' and L.SortDate < ' +  cast(@daterange_end as nvarchar(12)) + ' and ';	
	end;
    
    -- Was a field listed?
    if (( @field1 > 0 ) and ( @field1 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
    begin
		-- Get the name of this column then
		declare @field1_name varchar(100);
		set @field1_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field1 );

		-- Add this search then
		set @SQLQuery = @SQLQuery + ' contains ( ' + @field1_name + ', @innerterm1 )';
	end
	else
	begin
		-- Search the full citation then
		set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm1 )';	
	end;
            
    -- Start to build the query which will do ranking over the results which match this search
    set @rankselection = @term1;

	-- Add the second term, if there is one
	if (( LEN( ISNULL(@term2,'')) > 0 ) and (( @link2 = 0 ) or ( @link2 = 1 ) or ( @link2 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link2 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link2 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link2 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field2 > 0 ) and ( @field2 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field2_name varchar(100);
			set @field2_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field2 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field2_name + ', @innerterm2 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm2 )';	
		end;			
		
		-- Build the ranking query
		if ( @link2 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term2;	
		end
	end;    
	
	-- Add the third term, if there is one
	if (( LEN( ISNULL(@term3,'')) > 0 ) and (( @link3 = 0 ) or ( @link3 = 1 ) or ( @link3 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link3 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link3 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link3 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field3 > 0 ) and ( @field3 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field3_name varchar(100);
			set @field3_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field3 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field3_name + ', @innerterm3 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm3 )';	
		end;	
		
		-- Build the ranking query
		if ( @link3 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term3;		
		end
	end;   
	
	-- Add the fourth term, if there is one
	if (( LEN( ISNULL(@term4,'')) > 0 ) and (( @link4 = 0 ) or ( @link4 = 1 ) or ( @link4 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link4 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link4 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link4 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field4 > 0 ) and ( @field4 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field4_name varchar(100);
			set @field4_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field4 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field4_name + ', @innerterm4 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm4 )';	
		end;	
			
		-- Build the ranking query
		if ( @link4 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term4;		
		end
	end;
	
	-- Add the fifth term, if there is one
	if (( LEN( ISNULL(@term5,'')) > 0 ) and (( @link5 = 0 ) or ( @link5 = 1 ) or ( @link5 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link5 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link5 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link5 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field5 > 0 ) and ( @field5 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field5_name varchar(100);
			set @field5_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field5 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field5_name + ', @innerterm5 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm5 )';	
		end;
			
		-- Build the ranking query
		if ( @link5 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term5;		
		end
	end;
	
	-- Add the sixth term, if there is one
	if (( LEN( ISNULL(@term6,'')) > 0 ) and (( @link6 = 0 ) or ( @link6 = 1 ) or ( @link6 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link6 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link6 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link6 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field6 > 0 ) and ( @field6 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field6_name varchar(100);
			set @field6_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field6 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field6_name + ', @innerterm6 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm6 )';	
		end;
		
		-- Build the ranking query
		if ( @link6 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term6;		
		end
	end; 
	
	-- Add the seventh term, if there is one
	if (( LEN( ISNULL(@term7,'')) > 0 ) and (( @link7 = 0 ) or ( @link7 = 1 ) or ( @link7 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link7 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link7 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link7 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field7 > 0 ) and ( @field7 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field7_name varchar(100);
			set @field7_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field7 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field7_name + ', @innerterm7 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm7 )';	
		end;
		
		-- Build the ranking query
		if ( @link7 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term7;		
		end
	end;
	
	-- Add the eighth term, if there is one
	if (( LEN( ISNULL(@term8,'')) > 0 ) and (( @link8 = 0 ) or ( @link8 = 1 ) or ( @link8 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link8 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link8 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link8 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field8 > 0 ) and ( @field8 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field8_name varchar(100);
			set @field8_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field8 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field8_name + ', @innerterm8 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm8 )';	
		end;
		
		-- Build the ranking query
		if ( @link8 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term8;		
		end
	end;
	
	-- Add the ninth term, if there is one
	if (( LEN( ISNULL(@term9,'')) > 0 ) and (( @link9 = 0 ) or ( @link9 = 1 ) or ( @link9 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link9 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link9 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link9 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field9 > 0 ) and ( @field9 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field9_name varchar(100);
			set @field9_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field9 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field9_name + ', @innerterm9 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm9 )';	
		end;
		
		-- Build the ranking query
		if ( @link9 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term9;		
		end
	end;
	
	-- Add the tenth term, if there is one
	if (( LEN( ISNULL(@term10,'')) > 0 ) and (( @link10 = 0 ) or ( @link10 = 1 ) or ( @link10 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link10 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link10 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link10 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field10 > 0 ) and ( @field10 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field10_name varchar(100);
			set @field10_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field10 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field10_name + ', @innerterm10 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm10 )';	
		end;
		
		-- Build the ranking query
		if ( @link10 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term10;		
		end		
	end;
	
	-- Add the recompile option
	--set @SQLQuery = @SQLQuery + ' option (RECOMPILE)';

    -- Add the first term and start to build the query which will provide the items which match the search
    declare @mainquery nvarchar(max);
    set @mainquery = 'select L.Itemid from SobekCM_Metadata_Basic_Search_Table AS L';
    
    -- Do we need to limit by aggregation id as well?
    if ( @aggregationid > 0 )
    begin
		set @mainquery = @mainquery + ' join SobekCM_Item_Aggregation_Item_Link AS A ON ( A.ItemID = L.ItemID ) and ( A.AggregationID = ' + CAST( @aggregationid as varchar(5) ) + ')';   
    end    
    
    -- Add the full text search portion here
    set @mainquery = @mainquery + ' where ' + @SQLQuery;
	
	-- Set the parameter definition
	set @ParamDefinition = ' @innerterm1 nvarchar(255), @innerterm2 nvarchar(255), @innerterm3 nvarchar(255), @innerterm4 nvarchar(255), @innerterm5 nvarchar(255), @innerterm6 nvarchar(255), @innerterm7 nvarchar(255), @innerterm8 nvarchar(255), @innerterm9 nvarchar(255), @innerterm10 nvarchar(255)';
		
	-- Execute this stored procedure
	insert #TEMPZERO execute sp_Executesql @mainquery, @ParamDefinition, @term1, @term2, @term3, @term4, @term5, @term6, @term7, @term8, @term9, @term10;
			
	-- Perform ranking against the items and insert into another temporary table 
	-- with all the possible data elements needed for applying the user's sort
	insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate, Hit_Count )
	select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1), isnull(KEY_TBL.RANK, 0 )
	from SobekCM_Item I, #TEMPZERO AS T1 join
	  CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @rankselection ) AS KEY_TBL on KEY_TBL.[KEY] = T1.ItemID
	where ( T1.ItemID = I.ItemID )
	  and ( I.Deleted = 'false' )
      and ( I.IP_Restriction_Mask >= @lower_mask )	
      and ( I.IncludeInAll = 'true' );    

	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int;
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 
	
	-- If there were no results at all, check the count in the entire library
	if ( ( select COUNT(*) from #TEMP_ITEMS ) = 0 )
	begin
		-- Set the items and titles correctly
		set @total_items = 0;
		set @total_titles = 0;
		
		-- If there was an aggregation id, just return the counts for the whole library
		if ( @aggregationid > 0 )	
		begin
		
			-- Truncate the table and repull the data
			truncate table #TEMPZERO;
			
			-- Query against ALL aggregations this time
			declare @allquery nvarchar(max);
			set @allquery = 'select L.Itemid from SobekCM_Metadata_Basic_Search_Table AS L where ' + @SQLQuery;
			
			-- Execute this stored procedure
			insert #TEMPZERO execute sp_Executesql @allquery, @ParamDefinition, @term1, @term2, @term3, @term4, @term5, @term6, @term7, @term8, @term9, @term10;
			
			-- Get all items in the entire library then		  
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID )
			select I.ItemID, I.GroupID
			from #TEMPZERO T1, SobekCM_Item I
			where ( T1.ItemID = I.ItemID )
			  and ( I.Deleted = 'false' )
			  and ( I.IP_Restriction_Mask >= @lower_mask )	
			  and ( I.IncludeInAll = 'true' );  
			  
			-- Return these counts
			select @all_collections_items=COUNT(*), @all_collections_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS;
		end;
		
		-- Drop the big temporary table
		drop table #TEMPZERO;
	
	end
	else
	begin	
	
		-- Drop the big temporary table
		drop table #TEMPZERO;	
		
		-- Create the temporary item table variable for paging purposes
		declare @TEMP_PAGED_ITEMS TempPagedItemsTableType;
		  
		-- There are essentially two major paths of execution, depending on whether this should
		-- be grouped as items within the page requested titles ( sorting by title or the basic
		-- sorting by rank, which ranks this way ) or whether each item should be
		-- returned by itself, such as sorting by individual publication dates, etc..
		
		if ( @sort < 10 )
		begin	
			-- create the temporary title table definition
			declare @TEMP_TITLES table ( TitleID int, BibID varchar(10), RowNumber int );	
			
			-- Return these counts
			select @total_items=COUNT(*), @total_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS;
			
			-- Now, calculate the actual ending row, based on the ration, page information,
			-- and the lookahead factor
			if (( @total_items > 0 ) and ( @total_titles > 0 ))
			begin		
				-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
				declare @computed_value int;
				select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
				
				-- Compute the minimum value.  This cannot be less than @minpagelookahead.
				declare @floored_value int;
				select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
				
				-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
				declare @actual_pages int;
				select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead));

				-- Set the final row again then
				set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1; 
			end;	
					  
			-- Create saved select across titles for row numbers
			with TITLES_SELECT AS
				(	select GroupID, G.BibID, 
						ROW_NUMBER() OVER (order by case when @sort=0 THEN (SUM(Hit_COunt)/COUNT(*)) end DESC,
													case when @sort=1 THEN G.SortTitle end ASC,												
													case when @sort=2 THEN BibID end ASC,
													case when @sort=3 THEN BibID end DESC) as RowNumber
					from #TEMP_ITEMS I, SobekCM_Item_Group G
					where I.fk_TitleID = G.GroupID
					group by G.GroupID, G.BibID, G.SortTitle )

			-- Insert the correct rows into the temp title table	
			insert into @TEMP_TITLES ( TitleID, BibID, RowNumber )
			select GroupID, BibID, RowNumber
			from TITLES_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
		
			-- Return the title information for this page
			select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number as OPAC_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_TITLES T, SobekCM_Item_Group G
			where ( T.TitleID = G.GroupID )
			order by RowNumber ASC;
			
			-- Get the item id's for the items related to these titles
			insert into @TEMP_PAGED_ITEMS
			select ItemID, RowNumber
			from @TEMP_TITLES T, SobekCM_Item I
			where ( T.TitleID = I.GroupID );			  
			
			-- Return the basic system required item information for this page of results
			select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
			from SobekCM_Item I, @TEMP_PAGED_ITEMS T
			where ( T.ItemID = I.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
								
			-- Return the aggregation-specific display values for all the items in this page of results
			execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 		

		end
		else
		begin		
			-- Since these sorts make each item paired with a single title row,
			-- number of items and titles are equal
			select @total_items=COUNT(*), @total_titles=COUNT(*)
			from #TEMP_ITEMS; 
			
			-- In addition, always return the max lookahead pages
			set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
			
			-- Create saved select across items for row numbers
			with ITEMS_SELECT AS
			 (	select I.ItemID, 
					ROW_NUMBER() OVER (order by case when @sort=10 THEN isnull(SortDate,9223372036854775807)  end ASC,
												case when @sort=11 THEN isnull(SortDate,-1) end DESC) as RowNumber
					from #TEMP_ITEMS I
					group by I.ItemID, SortDate )
						  
			-- Insert the correct rows into the temp item table	
			insert into @TEMP_PAGED_ITEMS ( ItemID, RowNumber )
			select ItemID, RowNumber
			from ITEMS_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
			  
			-- Return the title information for this page
			select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number as OPAC_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
			where ( T.ItemID = I.ItemID )
			  and ( I.GroupID = G.GroupID )
			order by RowNumber ASC;
			
			-- Return the basic system required item information for this page of results
			select T.RowNumber as fk_TitleID, I.ItemID, VID, Title, IP_Restriction_Mask, coalesce(I.MainThumbnail,'') as MainThumbnail, coalesce(I.Level1_Index, -1) as Level1_Index, coalesce(I.Level1_Text,'') as Level1_Text, coalesce(I.Level2_Index, -1) as Level2_Index, coalesce(I.Level2_Text,'') as Level2_Text, coalesce(I.Level3_Index,-1) as Level3_Index, coalesce(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], coalesce(I.Link,'') as Link, coalesce( Spatial_KML, '') as Spatial_KML, coalesce(COinS_OpenURL, '') as COinS_OpenURL		
			from SobekCM_Item I, @TEMP_PAGED_ITEMS T
			where ( T.ItemID = I.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;			
			
			-- Return the aggregation-specific display values for all the items in this page of results
			execute sp_Executesql @item_display_sql, N' @itemtable TempPagedItemsTableType READONLY', @TEMP_PAGED_ITEMS; 

		end;

		-- Return the facets if asked for
		if ( @include_facets = 'true' )
		begin	
			if (( LEN( isnull( @aggregationcode, '')) = 0 ) or ( @aggregationcode = 'all' ))
			begin
				-- Build the aggregation list
				select A.Code, A.ShortName, Metadata_Count=Count(*)
				from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
				where ( T.ItemID = I.ItemID )
				  and ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				  and ( A.Include_In_Collection_Facet = 'true' )
				group by A.Code, A.ShortName
				order by Metadata_Count DESC, ShortName ASC;	
			end;
			
			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype1 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype2 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype3 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype4 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype5 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype6 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype7 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype8 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
		end; -- End overall FACET block
	end; -- End else statement entered if there are any results to return
	
	-- return the query string as well, for debuggins
	select Query = @mainquery, RankSelection = @rankselection;
	
	-- drop the temporary tables
	drop table #TEMP_ITEMS;
	
	Set NoCount OFF;
			
	If @@ERROR <> 0 GoTo ErrorHandler;
    Return(0);
  
ErrorHandler:
    Return(@@ERROR);
	
END;
GO


GRANT EXECUTE ON SobekCM_Get_Aggregation_Browse_Paged2 TO sobek_user;
GRANT EXECUTE ON SobekCM_Get_All_Browse_Paged2 TO sobek_user;
GRANT EXECUTE ON SobekCM_Metadata_Basic_Search_Paged2 TO sobek_user;
GRANT EXECUTE ON SobekCM_Metadata_Exact_Search_Paged2 TO sobek_user;
GRANT EXECUTE ON SobekCM_Metadata_Search_Paged2 TO sobek_user;
GO





