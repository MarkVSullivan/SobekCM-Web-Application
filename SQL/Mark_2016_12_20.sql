-- There may be a dependency on the sort title (default value)?
DECLARE @var0 nvarchar(128);

SELECT @var0 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.SobekCM_Item_Group')
AND col_name(parent_object_id, parent_column_id) = 'SortTitle';

IF ( @var0 IS NOT NULL )
begin
    EXECUTE('ALTER TABLE [dbo].[SobekCM_Item_Group] DROP CONSTRAINT [' + @var0 + ']');
end;
GO

alter table SobekCM_Item_Group alter column SortTitle nvarchar(1000) not null;
GO

ALTER TABLE [dbo].[SobekCM_Item_Group] ADD  DEFAULT ('') FOR [SortTitle];
GO

update SobekCM_Settings set Heading='Server Settings' where Setting_Key='Document Solr Index URL';
GO