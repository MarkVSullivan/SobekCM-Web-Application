
CREATE TABLE [dbo].[SobekCM_Item_OAI](
	[ItemID] [int] NOT NULL,
	[OAI_Data] [nvarchar](max) NOT NULL,
	[Locked] [bit] NOT NULL DEFAULT ('false'),
	[OAI_Date] [date] NULL,
	[Data_Code] [varchar](20) NOT NULL DEFAULT ('oai_dc'),
 CONSTRAINT [PK_SobekCM_Item_OAI] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[SobekCM_Item_OAI]  WITH CHECK ADD  CONSTRAINT [FK_SobekCM_Item_OAI_SobekCM_Item] FOREIGN KEY([ItemID])
REFERENCES [dbo].[SobekCM_Item] ([ItemID])
GO

ALTER TABLE [dbo].[SobekCM_Item_OAI] CHECK CONSTRAINT [FK_SobekCM_Item_OAI_SobekCM_Item]
GO


/****** Object:  StoredProcedure [dbo].[SobekCM_Add_OAI_PMH_Data]    Script Date: 12/20/2013 05:43:36 ******/
-- Add some OAI-PMH data to an item.  Included will be the data (usually in XML format)
-- and the OAI-PMH code for that data type.  The XML information is saved as nvarchar, rather
-- than XML, since this data is never sub-queried.  It is just returned while serving OAI.
ALTER PROCEDURE [dbo].[SobekCM_Add_OAI_PMH_Data]
	@itemid int,
	@data_code nvarchar(20),
	@oai_data nvarchar(max)
AS
begin

	-- Does this already exists?
	if (( select COUNT(*) from SobekCM_Item_OAI where ItemID=@itemid ) = 0 )
	begin
		insert into SobekCM_Item_OAI ( ItemID, OAI_Data, OAI_Date, Data_Code )
		values ( @itemid, @oai_data, GETDATE(), @data_code );
	end
	else
	begin
		update SobekCM_Item_OAI
		set OAI_Data=@oai_data, OAI_Date=GETDATE(), Data_Code=@data_code
		where ItemID=@itemid and Locked='false';
	end;
end;

GO


