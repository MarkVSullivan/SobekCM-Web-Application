/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP 1000 [MetadataTypeID]
      ,[MetadataName]
      ,[SobekCode]
      ,[SolrCode]
      ,[DisplayTerm]
      ,[FacetTerm]
      ,[CustomField]
      ,[canFacetBrowse]
      ,[DefaultAdvancedSearch]
  FROM [wolfsonian].[dbo].[SobekCM_Metadata_Types]



CREATE PROCEDURE Wolfsonian_Library_Item_By_Identifier 
	@Identifier varchar(20)
AS 
BEGIN
  select G.BibID, I.VID, I.Title, L.ItemID, S.MetadataValue
  from SobekCM_Metadata_Unique_Search_Table S, 
       SobekCM_Metadata_Unique_Link L, 
	   SobekCM_Item I, 
	   SobekCM_Item_Group G,
	   SobekCM_Item_Aggregation_Item_Link X,
	   SobekCM_Item_Aggregation A
  where S.MetadataID = L.MetadataID
    and S.MetadataTypeID=17
	and I.ItemID = L.ItemID
	and I.GroupID = G.GroupID
	and I.ItemID = X.ItemID
	and X.AggregationID = A.AggregationID
	and A.Code = 'library'
	and S.MetadataValue=@Identifier;
END;

GRANT EXECUTE on Wolfsonian_Library_Item_By_Identifier to sobek_builder;
GRANT EXECUTE on Wolfsonian_Library_Item_By_Identifier to sobek_user;
GO


CREATE PROCEDURE SobekCM_Get_Next_BibID
	@BibIdStart varchar(5)
AS
BEGIN

			declare @next_bibid_number int;

			-- Find the next bibid number
			select @next_bibid_number = isnull(CAST(REPLACE(MAX(BibID), @BibIdStart, '') as int) + 1,-1)
			from SobekCM_Item_Group
			where BibID like @BibIdStart + '%';
			
			-- If no matches to this BibID, just start at 0000001
			if ( @next_bibid_number < 0 )
			begin
				select @BibIdStart + RIGHT('00000001', 10-LEN(@BibIdStart)) as NextBibId;
			end
			else
			begin
				select @BibIdStart + RIGHT('00000000' + (CAST( @next_bibid_number as varchar(10))), 10-LEN(@BibIdStart)) as NextBibId;
			end;

END;
GO

GRANT EXECUTE on SobekCM_Get_Next_BibID to sobek_builder;
GRANT EXECUTE on SobekCM_Get_Next_BibID to sobek_user;
GO

exec SobekCM_Get_Next_BibID 'WOLF'