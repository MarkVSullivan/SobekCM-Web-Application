
CREATE TABLE [dbo].[SobekCM_Extension](
	[ExtensionID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[CurrentVersion] [varchar](50) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[EnabledDate] [datetime] NULL,
	[LicenseKey] [nvarchar](max) NULL,
	[UpgradeUrl] [nvarchar](255) NULL,
	[LatestVersion] [varchar](50) NULL,
 CONSTRAINT [PK_SobekCM_Extension] PRIMARY KEY CLUSTERED 
(
	[ExtensionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

