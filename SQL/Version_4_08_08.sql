

-- Ensure the stored procedure exists
-- THIS GOES FROM 4_08_07 to 4_08_08


IF object_id('SobekCM_Random_Item') IS NULL EXEC ('create procedure dbo.SobekCM_Random_Item as select 1;');
GO


-- Choose a random item from the entire digital library
-- that is public
ALTER PROCEDURE SobekCM_Random_Item AS
BEGIN

	-- Get the minimum and maximum ids
	declare @minid int;
	declare @maxid int;
	set @minid = ( select MIN(GroupID) from SobekCM_Item_Group where Deleted = 'false' );
	set @maxid = ( select MAX(GroupID) from SobekCM_Item_Group where Deleted = 'false' );

	-- Pick a random if
	declare @randomid int;
	set @randomid = -1;
	declare @attempt int;
	set @attempt = 0;

	-- Loop here for about 20 times (since this is so relatively cheap)
	while (( @attempt <= 20 ) and ( @randomid < 0 ))
	begin

		set @randomid = @minid + ( RAND() * (@maxid - @minid ));

		if ( not exists ( select * from SobekCM_Item_Group G where Deleted='false' and GroupID = @randomid and exists ( select 1 from SobekCM_Item I where I.GroupID=@randomid and I.Deleted='false' and I.IP_Restriction_Mask = 0 and I.Dark = 'false')))
		begin
			set @randomid = -1;
		end;

	end;

	-- Sometimes, the process above does not generate any BibID, so use the brute force method
	if ( @randomid < 0 )
	begin

		-- Get a small sample of rows, and assign top value
		with sample_rows_ordered AS (
			select GroupID, newid() as randomid
			from SobekCM_Item_Group G
			where exists ( select 1 from SobekCM_Item I where I.GroupID=G.GroupID and I.Deleted='false' and I.IP_Restriction_Mask = 0 and I.Dark = 'false')
		)
		select @randomid = (select top 1 GroupID from sample_rows_ordered order by randomid);

	end;

	-- With the bibid in hand, now select a random vid
	select top 1 BibID, VID
	from SobekCM_Item I, SobekCM_Item_Group G
	where ( I.Deleted = 'false' )
	  and ( I.IP_Restriction_Mask = 0 )
	  and ( I.Dark = 'false' )
	  and ( G.GroupID = @randomid )
	  and ( G.GroupID = I.GroupID )
	order by newid();

END;
GO

GRANT EXECUTE ON SobekCM_Random_Item TO sobek_user;
GO

if (( select count(*) from SobekCM_Database_Version ) = 0 )
begin
	insert into SobekCM_Database_Version ( Major_Version, Minor_Version, Release_Phase )
	values ( 4, 8, '8' );
end
else
begin
	update SobekCM_Database_Version
	set Major_Version=4, Minor_Version=8, Release_Phase='8';
end;
GO


