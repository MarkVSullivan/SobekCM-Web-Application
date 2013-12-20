
CREATE TABLE [dbo].[mySobek_User_Settings](
	[UserSettingID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[Setting_Key] [nvarchar](255) NOT NULL,
	[Setting_Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_mySobek_User_Settings] PRIMARY KEY CLUSTERED 
(
	[UserSettingID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO 


ALTER TABLE [dbo].[mySobek_User_Settings]  WITH CHECK ADD  CONSTRAINT [FK_User_Settings_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[mySobek_User] ([UserID])
GO


CREATE TABLE [dbo].[mySobek_User_Item_Permissions](
	[UserItemPermissionsID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[PermissionStatement] [varchar](255) NOT NULL,
 CONSTRAINT [PK_mySobek_User_Item_Permissions] PRIMARY KEY CLUSTERED 
(
	[UserItemPermissionsID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[mySobek_User_Item_Permissions]  WITH CHECK ADD  CONSTRAINT [FK_User_Item_Permissions_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[mySobek_User] ([UserID])
GO

ALTER TABLE [dbo].[mySobek_User_Item_Permissions]  WITH CHECK ADD  CONSTRAINT [FK_User_Item_Permissions_Item] FOREIGN KEY([ItemID])
REFERENCES [dbo].[SobekCM_Item] ([ItemID])
GO

CREATE TABLE [dbo].[SobekCM_Item_Aggregation_Settings](
	[AggregationSettingID] [bigint] IDENTITY(1,1) NOT NULL,
	[AggregationID] [int] NOT NULL,
	[Setting_Key] [nvarchar](255) NOT NULL,
	[Setting_Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_SobekCM_Item_Aggregation_Settings] PRIMARY KEY CLUSTERED 
(
	[AggregationSettingID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[SobekCM_Item_Aggregation_Settings]  WITH CHECK ADD  CONSTRAINT [FK_Aggregation_Settings_Aggregation] FOREIGN KEY([AggregationID])
REFERENCES [dbo].SobekCM_Item_Aggregation ([AggregationID])
GO