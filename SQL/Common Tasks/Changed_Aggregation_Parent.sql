

-- Will be a parameter to the stored procedure - code of aggregation to move
declare @aggr_to_move varchar(20);
set @aggr_to_move = 'GINASUB';

-- Will be a parameter to the stored procedure - new parent code
declare @new_parent varchar(20);
set @new_parent = 'sandhya_coll';

-- Ensure the parent exists
if ( not exists ( select 1 from SobekCM_Item_Aggregation where Code=@aggr_to_move ))
begin
	print 'Aggregation code to move is invalid';
	return;
end;

if ( not exists ( select 1 from SobekCM_Item_Aggregation where Code=@new_parent ))
begin
	print 'New parent aggregation code to move is invalid';
	return;
end;

-- Get the primary keys for the aggregations
declare @new_parent_id int;
declare @aggr_id int;
set @new_parent_id = ( select AggregationID from SobekCM_Item_Aggregation where Code=@new_parent );
set @aggr_id = ( select AggregationID from SobekCM_Item_Aggregation where Code=@aggr_to_move );

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

-- Get the count of affected items
select count(*) as AffectedItemCount from #AffectedItems;

-- Drop the temporary table
drop table #AffectedItems;