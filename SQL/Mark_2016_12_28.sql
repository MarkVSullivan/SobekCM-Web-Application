
-- Since the constraint wasn't there before, there MAY be (incorrect) duplication
if ( exists ( select ViewType from SobekCM_Item_Viewer_Types group by ViewType having count(*) > 1 ))
begin
	-- Need to fix this then
	declare @viewtype varchar(50);
	declare @minViewTypeId int;
	declare @otherViewTypeId int;

	declare dupe_cursor cursor for
	select ViewType 
	from SobekCM_Item_Viewer_Types 
	group by ViewType having count(*) > 1;

	open dupe_cursor;

	-- Get the first viewtype with duplication
	fetch next from dupe_cursor   
	into @viewtype;

	while @@FETCH_STATUS = 0  
	begin 

		-- Get the MINIMUM id that matched ( we will keep that one )
		set @minViewTypeId = ( select MIN(ItemViewTypeID) from SobekCM_Item_Viewer_Types where ViewType=@viewtype );

		-- Using another cursor here actually seems like the best performance
		-- as it results in updating/scanning the least number of rows
		declare dupe2_cursor cursor for
		select ItemViewTypeID  
		from SobekCM_Item_Viewer_Types 
		where ViewType=@viewType  
		  and ItemViewTypeID != @minViewTypeId;

		open dupe2_cursor;

		-- Get the first view type id to remove
		fetch next from dupe2_cursor   
		into @otherViewTypeId;

		while @@FETCH_STATUS = 0  
		begin 
			-- Any items linked to this will be moved to the one we are keeping
			update SobekCM_Item_Viewers 
			set ItemViewTypeID=@minViewTypeId
			where ItemViewTypeID=@otherViewTypeId
			  and not exists ( select 1 
			                   from SobekCM_Item_Viewers V2 
			                   where V2.ItemID = SobekCM_Item_Viewers.ItemID 
							     and V2.ItemViewTypeID=@minViewTypeId );

			-- Any remaining links between the item and this view type are because they
			-- are already linked to the view type we are keeping
			delete from SobekCM_Item_Viewers 
			where ItemViewTypeID=@otherViewTypeId;

			-- Get the next matching view type id to remove (in case
			-- there were more than two dupes with the same viewtype)
			fetch next from dupe2_cursor   
			into @otherViewTypeId;
		end;

		close dupe2_cursor;  
		deallocate dupe2_cursor;  

		-- Now that all items are linked correctly, we can remove the extraneous views
		delete from SobekCM_Item_Viewer_Types 
		where ViewType=@viewtype
		  and ItemViewTypeID != @minViewTypeId;

		-- Next viewtype with duplication
		fetch next from dupe_cursor   
		into @viewtype;
	end;

	close dupe_cursor;  
	deallocate dupe_cursor;  
end;
GO

if ( not exists ( select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME='SobekCM_Item_Viewer_Types_Viewer_Unique' ))
begin
	ALTER TABLE SobekCM_Item_Viewer_Types  
	ADD CONSTRAINT SobekCM_Item_Viewer_Types_Viewer_Unique UNIQUE (ViewType);   
end;
GO