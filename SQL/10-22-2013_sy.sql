
USE [SobekDB]
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Get_Users_Scanning_Processing]    Script Date: 10/22/2013 11:52:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Get_Users_Scanning_Processing]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT UserName,EmailAddress,FirstName,LastName,ScanningTechnician, ProcessingTechnician 
	FROM mySobek_User
	WHERE ScanningTechnician=1 OR ProcessingTechnician=1
END



/*Create [Tracking_Get_Scanners_List] Stored Procedure*/


/****** Object:  StoredProcedure [dbo].[Tracking_Get_Scanners_List]    Script Date: 10/22/2013 12:04:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Tracking_Get_Scanners_List]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ScanningEquipment, Notes, Location,EquipmentType 
	FROM Tracking_ScanningEquipment
	WHERE isActive=1
END