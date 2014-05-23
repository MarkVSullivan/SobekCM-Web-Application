
CREATE TABLE [dbo].[SobekCM_Item_Viewer_Priority](
	[ItemViewPriorityID] [int] IDENTITY(1,1) NOT NULL,
	[ViewType] [nvarchar](255) NOT NULL,
	[Priority] [int] NOT NULL,
 CONSTRAINT [PK_SobekCM_Item_View_Priority] PRIMARY KEY CLUSTERED 
(
	[ItemViewPriorityID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('DATASET_CODEBOOK',1);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('DATASET_REPORTS',2);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('DATASET_VIEWDATA',3);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('EAD_CONTAINER_LIST',4);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('EAD_DESCRIPTION',5);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('YOUTUBE_VIDEO',6);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('EMBEDDED_VIDEO',7);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('FLASH',8);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('JPEG',9);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('JPEG_TEXT_TWO_UP',10);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('JPEG2000',11);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('PDF',12);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('DOWNLOADS',13);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('TEXT',14);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('CITATION',15);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('FEATURES',16);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('GOOGLE_MAP',17);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('HTML',18);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('PAGE_TURNER',19);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('RELATED_IMAGES',20);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('SEARCH',21);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('SIMPLE_HTML_LINK',22);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('STREETS',23);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('TEI',24);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('TOC',25);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('ALL_VOLUMES',26);
GO

CREATE PROCEDURE dbo.SobekCM_Get_Viewer_Priority
AS
BEGIN
	select * 
	from SobekCM_Item_Viewer_Priority
	order by [Priority] ASC;
END;
GO

GRANT EXECUTE on dbo.SobekCM_Get_Viewer_Priority to sobek_user;
GO
