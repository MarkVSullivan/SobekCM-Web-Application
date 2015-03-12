
ALTER PROCEDURE [dbo].[SobekCM_Get_Item_Aggregation_AllCodes]
AS
begin
	
	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- First, get the aggregations
	SELECT P.Code, P.[Type], P.Name, ShortName=coalesce(P.ShortName, P.Name), P.isActive, P.Hidden, P.AggregationID, 
	       [Description]=coalesce(P.[Description],''), ThematicHeadingID=coalesce(T.ThematicHeadingID, -1 ),
		   External_URL=coalesce(P.External_Link,''), P.DateAdded, P.LanguageVariants, T.ThemeName, 
		   F.ShortName as ParentShortName, F.Name as ParentName, F.Code as ParentCode
	FROM SobekCM_Item_Aggregation AS P left outer join
	     SobekCM_Thematic_Heading as T on P.ThematicHeadingID=T.ThematicHeadingID left outer join
		 SobekCM_Item_Aggregation_Hierarchy as H on H.ChildID=P.AggregationID left outer join
		 SobekCM_Item_Aggregation as F on F.AggregationID=H.ParentID
	WHERE P.Deleted = 'false'
	  and ( F.Deleted = 'false' or F.Deleted is null )
	order by P.Code;

end;
GO

