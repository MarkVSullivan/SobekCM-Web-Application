
	
		-- Get the id for the ALL aggregation
		declare @all_id int;
		set @all_id = coalesce(( select AggregationID from SObekCM_Item_Aggregation where Code='all'), -1);
		
		declare @Aggregation_List TABLE
		(
		  AggregationID int,
		  Code varchar(20),
		  ChildCode varchar(20),
		  Child2Code varchar(20),
		  Name nvarchar(255),
		  ShortName nvarchar(100),
		  [Type] varchar(50),
		  isActive bit
		);
		
		-- Insert the list of items linked to ALL or linked to NONE (include ALL)
		insert into @Aggregation_List ( AggregationID, Code, ChildCode, Child2Code, Name, ShortName, [Type], isActive )
		select AggregationID, Code, '', '', Name, ShortName, [Type], isActive
		from SobekCM_Item_Aggregation A
		where ( [Type] not like 'Institut%' )
		  and ( Deleted='false' )
		  and exists ( select * from SobekCM_Item_Aggregation_Hierarchy where ChildID=A.AggregationID and ParentID=@all_id);
		  
		-- Insert the children under those top-level collections
		insert into @Aggregation_List ( AggregationID, Code, ChildCode, Child2Code, Name, ShortName, [Type], isActive )
		select A2.AggregationID, T.Code, A2.Code, '', A2.Name, A2.SHortName, A2.[Type], A2.isActive
		from @Aggregation_List T, SobekCM_Item_Aggregation A2, SobekCM_Item_Aggregation_Hierarchy H
		where ( A2.[Type] not like 'Institut%' )
		  and ( T.AggregationID = H.ParentID )
		  and ( A2.AggregationID = H.ChildID )
		  and ( Deleted='false' );
		  
		-- Insert the grand-children under those child collections
		insert into @Aggregation_List ( AggregationID, Code, ChildCode, Child2Code, Name, ShortName, [Type], isActive )
		select A2.AggregationID, T.Code, T.ChildCode, A2.Code, A2.Name, A2.SHortName, A2.[Type], A2.isActive
		from @Aggregation_List T, SobekCM_Item_Aggregation A2, SobekCM_Item_Aggregation_Hierarchy H
		where ( A2.[Type] not like 'Institut%' )
		  and ( T.AggregationID = H.ParentID )
		  and ( A2.AggregationID = H.ChildID )
		  and ( Deleted='false' )
		  and ( ChildCode <> '' );

declare @date1 datetime;
set @date1 = '07/01/2013'

		-- Start to build the return set of values
		select code1 = Code, 
		       code2 = ChildCode,
		       code3 = Child2Code,
		    [Name], 
		    C.isActive AS Active,
			title_count = ( select count(distinct(GroupID)) from Statistics_Item_Aggregation_Link_View T where T.AggregationID = C.AggregationID ),
			item_count = ( select count(distinct(ItemID)) from Statistics_Item_Aggregation_Link_View T where T.AggregationID = C.AggregationID ), 
			page_count = coalesce(( select sum( PageCount ) from Statistics_Item_Aggregation_Link_View T where T.AggregationID = C.AggregationID ), 0),
			title_count_date1 = ( select count(distinct(GroupID)) from Statistics_Item_Aggregation_Link_View T where T.AggregationID = C.AggregationID and Milestone_OnlineComplete is not null and Milestone_OnlineComplete <= @date1),
			item_count_date1 = ( select count(distinct(ItemID)) from Statistics_Item_Aggregation_Link_View T where T.AggregationID = C.AggregationID and Milestone_OnlineComplete is not null and Milestone_OnlineComplete <= @date1 ), 
			page_count_date1 = coalesce(( select sum( [PageCount] ) from Statistics_Item_Aggregation_Link_View T where T.AggregationID = C.AggregationID and Milestone_OnlineComplete is not null and Milestone_OnlineComplete <= @date1 ), 0)
		from @Aggregation_List C
		where ( C.Code <> 'TESTCOL' ) AND ( C.Code <> 'TESTG' )
		order by code, code2, code3;
		
				-- Get total item count
		declare @total_item_count int
		select @total_item_count =  ( select count(*) from SobekCM_Item where Deleted = 0 );

		-- Get total title count
		declare @total_title_count int
		select @total_title_count =  ( select count(*) from SobekCM_Item_Group where Deleted = 0 );

		-- Get total title count
		declare @total_page_count int
		select @total_page_count =  isnull(( select sum( PageCount ) from SobekCM_Item where Deleted = 0 ), 0 );

		-- get the list of items from date1
		select I.ItemID, GroupID, PageCount
		into #TEMP_DATE1
		from SobekCM_Item I
		where ( I.Deleted = 'false' )
		  and ( Milestone_OnlineComplete is not null )
		  and ( Milestone_OnlineComplete <= @date1 );

		-- Get total item count
		declare @total_item_count_date1 int
		select @total_item_count_date1 =  ( select count(distinct(ItemID)) from #TEMP_DATE1  );

		-- Get total title count
		declare @total_title_count_date1 int
		select @total_title_count_date1 =  ( select count(distinct(GroupID)) from #TEMP_DATE1  );

		-- Get total title count
		declare @total_page_count_date1 int
		select @total_page_count_date1 =  isnull(( select sum( PageCount ) from #TEMP_DATE1  ),0);

		-- Add one row with the total item count
		insert into #TEMP_COLLECTIONS
		values ('z', 'ZZZ', '', '', 'Total Count', 1, @total_title_count, @total_item_count, @total_page_count, 
			@total_title_count_date1, @total_item_count_date1, @total_page_count_date1, -1, 0 );