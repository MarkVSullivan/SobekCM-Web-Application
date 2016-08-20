

-- Variables hold the year and month to clear the statistics from
declare @Year int;
declare @Month int;

-- Set the month and year from which to clear the stats
set @Year = 2016;
set @Month = 7;

-- Clear all the stats
delete from SobekCM_Browse_Info_Statistics where ( [Year]=@Year and [Month] >= @Month ) or ( [Year] > @Year );
delete from SobekCM_Item_Aggregation_Statistics where ( [Year]=@Year and [Month] >= @Month ) or ( [Year] > @Year );
delete from SobekCM_Item_Group_Statistics where ( [Year]=@Year and [Month] >= @Month ) or ( [Year] > @Year );
delete from SobekCM_Item_Statistics where ( [Year]=@Year and [Month] >= @Month ) or ( [Year] > @Year );
delete from SobekCM_Portal_URL_Statistics where ( [Year]=@Year and [Month] >= @Month ) or ( [Year] > @Year );
delete from SobekCM_WebContent_Statistics where ( [Year]=@Year and [Month] >= @Month ) or ( [Year] > @Year );
delete from SobekCM_Statistics where ( [Year]=@Year and [Month] >= @Month ) or ( [Year] > @Year );
GO
