-- THIS GOES FROM 4_08_08 to 4_08_09

-- Esure the SobekCM_Set_Additional_Work_Needed exists
IF object_id('SobekCM_Set_Additional_Work_Needed') IS NULL EXEC ('create procedure dbo.SobekCM_Set_Additional_Work_Needed as select 1;');
GO

-- Set the flag to have the builder reprocss this item
alter procedure SobekCM_Set_Additional_Work_Needed
	@bibid varchar(10),
	@vid varchar(5) = null
as
begin transaction

	-- Find the GroupID
	declare @groupid int;
	if ( exists ( select 1 from SobekCM_Item_Group where BibID=@bibid ))
	begin
		
		set @groupid = ( select GroupID from SobekCM_Item_Group where BibID=@bibid );

		-- Was a VID provided?
		if ( @vid is null )
		begin
			-- Mark all VIDs of the BibID to be reprocessed
			update SobekCM_Item set AdditionalWorkNeeded='true'
			where GroupID = @groupid;
		end
		else
		begin
			-- update the specific vid
			update SobekCM_Item set AdditionalWorkNeeded='true'
			where GroupID = @groupid
			  and VID = @vid;		
		end;
	end;

commit transaction;
GO

-- Ensure the module exists in the stanard set for adding ALL Images
if ( not exists ( select 1 from SobekCM_Builder_Module where Class='SobekCM.Builder_Library.Modules.Items.AttachImagesAllModule' ))
begin
	-- Ensure the more selective module is there
	if ( exists ( select 1 from SobekCM_Builder_Module where Class='SobekCM.Builder_Library.Modules.Items.AddNewImagesAndViewsModule' and ModuleSetID=3 ))
	begin
		-- Add the new module
		insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] )
		select 3, 'Attach ALL the images in the resource folder to the item', 'SobekCM.Builder_Library.Modules.Items.AttachImagesAllModule', 'true', [Order]
		from SobekCM_Builder_Module 
		where Class='SobekCM.Builder_Library.Modules.Items.AddNewImagesAndViewsModule' 
		and ModuleSetID=3;

		-- Disable the old, more selective, module
		update SobekCM_Builder_Module
		set [Enabled]='false'
		where Class='SobekCM.Builder_Library.Modules.Items.AddNewImagesAndViewsModule' 
		  and ModuleSetID=3;
	end;
end;
GO




IF object_id('SobekCM_Statistics_Save_TopLevel') IS NULL EXEC ('create procedure dbo.SobekCM_Statistics_Save_TopLevel as select 1;');
GO

-- Save the top-level statistics for the entire instance
ALTER PROCEDURE dbo.SobekCM_Statistics_Save_TopLevel
	@year smallint,
	@month smallint,
	@hits int,
	@sessions int,
	@robot_hits int,
	@xml_hits int,
	@oai_hits int,
	@json_hits int
as
begin

	-- Clear any existing one
	delete from SobekCM_Statistics where [Year]=@year and [Month]=@month;

	-- Add this
	insert into SobekCM_Statistics ( [Year], [Month], [Hits], [Sessions], Robot_Hits, XML_Hits, OAI_Hits, JSON_Hits )
	values ( @year, @Month, @hits, @sessions, @robot_hits, @xml_hits, @oai_hits, @json_hits);
end;
GO

IF object_id('SobekCM_Statistics_Save_Webcontent') IS NULL EXEC ('create procedure dbo.SobekCM_Statistics_Save_Webcontent as select 1;');
GO


-- Insert statistics for a top-level web content page
ALTER PROCEDURE dbo.SobekCM_Statistics_Save_Webcontent
	@year smallint,
	@month smallint,
	@hits int,
	@hits_complete int,
	@level1 varchar(100),
	@level2 varchar(100),
	@level3 varchar(100),
	@level4 varchar(100),
	@level5 varchar(100),
	@level6 varchar(100),
	@level7 varchar(100),
	@level8 varchar(100)
as
begin

	insert into SobekCM_Webcontent_Statistics ( Level1, Level2, Level3, Level4, Level5, Level6, Level7, Level8, [Year], [Month], [Hits], Hits_Complete ) 
	values ( @level1, @level2, @level3, @level4, @level5, @level6, @level7, @level8, @year, @month, @hits, @hits_complete );

end;
GO

IF object_id('SobekCM_Statistics_Save_Portal') IS NULL EXEC ('create procedure dbo.SobekCM_Statistics_Save_Portal as select 1;');
GO


-- Insert statistics for a URL portal
ALTER PROCEDURE dbo.SobekCM_Statistics_Save_Portal
	@year smallint,
	@month smallint,
	@hits int,
	@portalid int
as
begin

	insert into SobekCM_Portal_URL_Statistics ( PortalID, [Year], [Month], [Hits] )
	values ( @portalid, @year, @month, @hits );

end;
GO

IF object_id('SobekCM_Statistics_Save_Aggregation') IS NULL EXEC ('create procedure dbo.SobekCM_Statistics_Save_Aggregation as select 1;');
GO

-- Add item aggregation ( collection ) statistics
ALTER PROCEDURE dbo.SobekCM_Statistics_Save_Aggregation
	@aggregationid int,
	@year smallint,
	@month smallint,
	@hits int,
	@sessions int,
	@home_page_views int,
	@browse_views int,
	@advanced_search_views int,
	@search_results_views int
as
begin
	insert into SobekCM_Item_Aggregation_Statistics ( AggregationID, [Year], [Month], [Hits], [Sessions], Home_Page_Views, Browse_Views, Advanced_Search_Views, Search_Results_Views ) 
	values ( @aggregationid, @year, @month, @hits, @sessions, @home_page_views, @browse_views, @advanced_search_views, @search_results_views );
end;
GO

IF object_id('SobekCM_Statistics_Save_Item_Group') IS NULL EXEC ('create procedure dbo.SobekCM_Statistics_Save_Item_Group as select 1;');
GO

-- Insert statistics for an item group
ALTER PROCEDURE dbo.SobekCM_Statistics_Save_Item_Group
	@year smallint,
	@month smallint,
	@hits int,
	@sessions int,
	@groupid int
as
begin

	insert into SobekCM_Item_Group_Statistics ( GroupID, [Year], [Month], [Hits], [Sessions] ) 
	values ( @groupid, @year, @month, @hits, @sessions );

end;
GO

IF object_id('SobekCM_Statistics_Save_Item') IS NULL EXEC ('create procedure dbo.SobekCM_Statistics_Save_Item as select 1;');
GO

ALTER PROCEDURE dbo.SobekCM_Statistics_Save_Item
	@year smallint,
	@month smallint,
	@hits int,
	@sessions int,
	@itemid int,
	@jpeg_views int,
	@zoomable_views int,
	@citation_views int,
	@thumbnail_views int,
	@text_search_views int,
	@flash_views int,
	@google_map_views int,
	@download_views int,
	@static_views int
as
begin

	insert into SobekCM_Item_Statistics ( ItemID, [Year], [Month], [Hits], [Sessions], JPEG_Views, Zoomable_Views, Citation_Views,
		Thumbnail_Views, Text_Search_Views, Flash_Views, Google_Map_Views, Download_Views, Static_Views ) 
	values ( @itemid, @year, @month, @hits, @sessions, @jpeg_views, @zoomable_views, @citation_views,
		@thumbnail_views, @text_search_views, @flash_views, @google_map_views, @download_views, @static_views );

end;
GO

GRANT EXECUTE ON SobekCM_Set_Additional_Work_Needed to sobek_user;
GO
GRANT EXECUTE ON SobekCM_Statistics_Save_TopLevel to sobek_user;
GO
GRANT EXECUTE ON SobekCM_Statistics_Save_Webcontent to sobek_user;
GO
GRANT EXECUTE ON SobekCM_Statistics_Save_Portal to sobek_user;
GO
GRANT EXECUTE ON SobekCM_Statistics_Save_Aggregation to sobek_user;
GO
GRANT EXECUTE ON SobekCM_Statistics_Save_Item_Group to sobek_user;
GO
GRANT EXECUTE ON SobekCM_Statistics_Save_Item to sobek_user;
GO
GRANT EXECUTE ON SobekCM_Statistics_Aggregate to sobek_user;
GO
GRANT EXECUTE ON SobekCM_Set_Additional_Work_Needed to sobek_builder;
GO
GRANT EXECUTE ON SobekCM_Statistics_Save_TopLevel to sobek_builder;
GO
GRANT EXECUTE ON SobekCM_Statistics_Save_Webcontent to sobek_builder;
GO
GRANT EXECUTE ON SobekCM_Statistics_Save_Portal to sobek_builder;
GO
GRANT EXECUTE ON SobekCM_Statistics_Save_Aggregation to sobek_builder;
GO
GRANT EXECUTE ON SobekCM_Statistics_Save_Item_Group to sobek_builder;
GO
GRANT EXECUTE ON SobekCM_Statistics_Save_Item to sobek_builder;
GO
GRANT EXECUTE ON SobekCM_Statistics_Aggregate to sobek_builder;
GO



if (( select count(*) from SobekCM_Database_Version ) = 0 )
begin
	insert into SobekCM_Database_Version ( Major_Version, Minor_Version, Release_Phase )
	values ( 4, 8, '9' );
end
else
begin
	update SobekCM_Database_Version
	set Major_Version=4, Minor_Version=8, Release_Phase='9';
end;
GO


