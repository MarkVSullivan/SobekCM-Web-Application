
CREATE PROCEDURE [dbo].[SobekCM_QC_Get_Errors]
	-- Add the parameters for the stored procedure here
	@itemID int

	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT * FROM SobekCM_QC_Errors WHERE ItemID=@itemID; 
     	
END

GO


GRANT EXECUTE ON [dbo].[SobekCM_QC_Get_Errors] to ufdc_user;
GRANT EXECUTE ON [dbo].[SobekCM_QC_Get_Errors] to sobek_user;
GO
