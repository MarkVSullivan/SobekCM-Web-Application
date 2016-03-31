

-- Procedure will change the parent aggregation for an aggregation
-- Usage: 
--         First argument is the code for the aggregation to move
--         Second argument is the code the new parent aggregation for the aggregation
--             specified in the first argument.
CREATE PROCEDURE SobekCM_Aggregation_Change_Parent
	@aggr_to_move varchar(20),
	@new_parent varchar(20)
AS
BEGIN 

	SET NOCOUNT ON;
	
	-- Declare the message object to return
	declare @returnMessage varchar(255);

	-- Ensure the parent exists
	if ( not exists ( select 1 from SobekCM_Item_Aggregation where Code=@aggr_to_move ))
	begin
		print 'INVALID: Aggregation code to move is invalid';
		select 'INVALID: Aggregation code to move is invalid' as [Message], -1 as AffectedItems;
		return;
	end;

	if ( not exists ( select 1 from SobekCM_Item_Aggregation where Code=@new_parent ))
	begin
		print 'INVALID: New parent aggregation code is invalid';
		select 'INVALID: New parent aggregation code is invalid' as [Message], -1 as AffectedItems;
		return;
	end;

	-- Get the primary keys for the aggregations
	declare @new_parent_id int;
	declare @aggr_id int;
	set @new_parent_id = ( select AggregationID from SobekCM_Item_Aggregation where Code=@new_parent );
	set @aggr_id = ( select AggregationID from SobekCM_Item_Aggregation where Code=@aggr_to_move );
	
	-- Ensure a circular aggregation hierarchy loop is not being created

	-- Build list of all the parents, grandparents, etc..  (up to ten levels)
	select ParentID, 0 as [Level] into #ParentCheck from SobekCM_Item_Aggregation_Hierarchy where ChildID=@new_parent_id;	
	insert into #ParentCheck (ParentID, [Level]) select H.ParentID, 1 from SobekCM_Item_Aggregation_Hierarchy H, #ParentCheck P where H.ChildID=P.ParentID;
	insert into #ParentCheck (ParentID, [Level]) select H.ParentID, 2 from SobekCM_Item_Aggregation_Hierarchy H, #ParentCheck P where H.ChildID=P.ParentID and [Level]=1;
	insert into #ParentCheck (ParentID, [Level]) select H.ParentID, 3 from SobekCM_Item_Aggregation_Hierarchy H, #ParentCheck P where H.ChildID=P.ParentID and [Level]=2;
	insert into #ParentCheck (ParentID, [Level]) select H.ParentID, 4 from SobekCM_Item_Aggregation_Hierarchy H, #ParentCheck P where H.ChildID=P.ParentID and [Level]=3;
	insert into #ParentCheck (ParentID, [Level]) select H.ParentID, 5 from SobekCM_Item_Aggregation_Hierarchy H, #ParentCheck P where H.ChildID=P.ParentID and [Level]=4;
	insert into #ParentCheck (ParentID, [Level]) select H.ParentID, 6 from SobekCM_Item_Aggregation_Hierarchy H, #ParentCheck P where H.ChildID=P.ParentID and [Level]=5;
	insert into #ParentCheck (ParentID, [Level]) select H.ParentID, 7 from SobekCM_Item_Aggregation_Hierarchy H, #ParentCheck P where H.ChildID=P.ParentID and [Level]=6;
	insert into #ParentCheck (ParentID, [Level]) select H.ParentID, 8 from SobekCM_Item_Aggregation_Hierarchy H, #ParentCheck P where H.ChildID=P.ParentID and [Level]=7;
	insert into #ParentCheck (ParentID, [Level]) select H.ParentID, 9 from SobekCM_Item_Aggregation_Hierarchy H, #ParentCheck P where H.ChildID=P.ParentID and [Level]=8;

	-- Ensure the aggregation id is not listed in the temp table, which would indicate
	-- that the aggregation is actually a parent of what would be the parent (i.e., circular)
	if ( exists ( select 1 from #ParentCheck where ParentID=@aggr_id ))
	begin
		print 'INVALID: Provided parent aggregation is in the child hierarchy of the aggregation.  Circular aggregation hierarchy is invalid.';
		select 'INVALID: Provided parent aggregation is in the child hierarchy of the aggregation.  Circular aggregation hierarchy is invalid.' as [Message], -1 as AffectedItems;
		return;
	end;

	-- drop the temporary table now 
	drop table #ParentCheck;

	-- The rest of this work should be done in a transaction
	begin transaction;

		-- Clear the parent(s) for the aggregation to move
		delete from SobekCM_Item_Aggregation_Hierarchy 
		where ChildID = @aggr_id;

		-- Add the new link
		insert into SobekCM_Item_Aggregation_Hierarchy ( ParentID, ChildID )
		values ( @new_parent_id, @aggr_id );
	
		-- Get the list of affected children
		select ItemID
		into #AffectedItems
		from SobekCM_Item_Aggregation_Item_Link 
		where AggregationID = @aggr_id;

		-- Remove previously implied links from the affected items
		delete from SobekCM_Item_Aggregation_Item_Link
		where ( impliedLink = 'true' )
		  and ( ItemID in ( select ItemID from #AffectedItems ));

		-- Rebuild the implied links

		-- Add back the first implied links
		insert into SobekCM_Item_Aggregation_Item_Link (ItemID, AggregationID, impliedLink )
		select distinct L.ItemID, H.ParentID, 'true'
		from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation_Hierarchy H, SobekCM_Item_Aggregation A, #AffectedItems I
		where ( L.AggregationID = H.ChildID )
			and ( L.AggregationID = A.AggregationID )
			and ( I.ItemID = L.ItemID )
			and not exists ( select * from SobekCM_Item_Aggregation_Item_Link T where T.ItemID = L.ItemID and T.AggregationID = H.ParentID );

		-- Add back the second level of implied links
		insert into SobekCM_Item_Aggregation_Item_Link (ItemID, AggregationID, impliedLink )
		select distinct L.ItemID, H.ParentID, 'true'
		from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation_Hierarchy H, SobekCM_Item_Aggregation A, #AffectedItems I
		where ( L.AggregationID = H.ChildID )
			and ( L.ImpliedLink = 'true' )
			and ( L.AggregationID = A.AggregationID )
			and ( I.ItemID = L.ItemID )
			and not exists ( select * from SobekCM_Item_Aggregation_Item_Link T where T.ItemID = L.ItemID and T.AggregationID = H.ParentID );

		-- Add back the third level of implied links
		insert into SobekCM_Item_Aggregation_Item_Link (ItemID, AggregationID, impliedLink )
		select distinct L.ItemID, H.ParentID, 'true'
		from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation_Hierarchy H, SobekCM_Item_Aggregation A, #AffectedItems I
		where ( L.AggregationID = H.ChildID )
			and ( L.ImpliedLink = 'true' )
			and ( L.AggregationID = A.AggregationID )
			and ( I.ItemID = L.ItemID )
			and not exists ( select * from SobekCM_Item_Aggregation_Item_Link T where T.ItemID = L.ItemID and T.AggregationID = H.ParentID );

		-- Add back the fourth level of implied links
		insert into SobekCM_Item_Aggregation_Item_Link (ItemID, AggregationID, impliedLink )
		select distinct L.ItemID, H.ParentID, 'true'
		from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation_Hierarchy H, SobekCM_Item_Aggregation A, #AffectedItems I
		where ( L.AggregationID = H.ChildID )
			and ( L.ImpliedLink = 'true' )
			and ( L.AggregationID = A.AggregationID )
			and ( I.ItemID = L.ItemID )
			and not exists ( select * from SobekCM_Item_Aggregation_Item_Link T where T.ItemID = L.ItemID and T.AggregationID = H.ParentID );

		-- Add back the fifth level of implied links
		insert into SobekCM_Item_Aggregation_Item_Link (ItemID, AggregationID, impliedLink )
		select distinct L.ItemID, H.ParentID, 'true'
		from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation_Hierarchy H, SobekCM_Item_Aggregation A, #AffectedItems I
		where ( L.AggregationID = H.ChildID )
			and ( L.ImpliedLink = 'true' )
			and ( L.AggregationID = A.AggregationID )
			and ( I.ItemID = L.ItemID )
			and not exists ( select * from SobekCM_Item_Aggregation_Item_Link T where T.ItemID = L.ItemID and T.AggregationID = H.ParentID );

		-- Also, mark all affected items to be reprocessed
		update SobekCM_Item
		set AdditionalWorkNeeded='true'
		where ItemID in ( select ItemID from #AffectedItems );

	commit transaction;

	-- Get the count of affected items and return a success message
	print 'Aggregation parent changed successfully';
	select 'Aggregation parent changed successfully' as [Message], count(*) as AffectedItems
	from #AffectedItems;

	-- Drop the temporary table
	drop table #AffectedItems;

END;
GO

GRANT EXECUTE ON SobekCM_Aggregation_Change_Parent to sobek_user;
GO
