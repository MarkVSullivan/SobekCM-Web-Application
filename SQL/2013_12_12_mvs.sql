
CREATE PROCEDURE SobekCM_Get_Item_Restrictions
	@bibid varchar(10),
	@vid varchar(5)
AS
BEGIN
	select IP_Restriction_Mask, Dark
	from SObekCM_Item I, SobekCM_Item_Group G
	where ( I.VID = @vid )
	  and ( I.GroupID = G.GroupID )
	  and ( G.BibID=@bibid)
	  and ( I.Deleted = 'false' )
	  and ( G.Deleted = 'false' );
END
GO

GRANT EXECUTE ON SobekCM_Get_Item_Restrictions TO sobek_user;
GO