
ALTER TABLE [SobekCM_Item_Aggregation] DROP CONSTRAINT [DF_SobekCM_Item_Aggregation_DefaultInterface];
GO

ALTER TABLE [SobekCM_Item_Aggregation] ADD CONSTRAINT [DF_SobekCM_Item_Aggregation_DefaultInterface] DEFAULT N'' FOR [DefaultInterface];
GO

-- Fix some standard translations (somewhat temporary)
update SobekCM_Metadata_Translation
set English='Citation', French='Citation', Spanish='Cita'
where English='CITATION';
GO

update SobekCM_Metadata_Translation
set English='Page Image', French='Image de Page', Spanish='Imagen de la Página'
where English='PAGE IMAGE';
GO

update SobekCM_Metadata_Translation
set English='Page Text', French='Texte de Page', Spanish='Texto de la Página'
where English='PAGE TEXT';
GO

update SobekCM_Metadata_Translation
set English='Zoomable', French='Zoomable', Spanish='Imagen Ampliable'
where English='ZOOMABLE';
GO

update SobekCM_Metadata_Translation
set English='Features', French='Fonctions', Spanish='Edificios'
where English='FEATURES';
GO

update SobekCM_Metadata_Translation
set English='Streets', French='Rue', Spanish='Calles'
where English='STREETS';
GO

update SobekCM_Metadata_Translation
set English='Thumbnails', French='Miniatures', Spanish='Miniaturas'
where English='THUMBNAILS';
GO

update SobekCM_Metadata_Translation
set English='Downloads', French='Téléchargements', Spanish='Descargas'
where English='DOWNLOADS';
GO

update SobekCM_Metadata_Translation
set English='All Issues', French='Éditions', Spanish='Ediciones'
where English='ALL ISSUES';
GO

update SobekCM_Metadata_Translation
set English='All Volumes', French='Volumes', Spanish='Volumenes'
where English='ALL VOLUMES';
GO

update SobekCM_Metadata_Translation
set English='Related Maps', French='Cartes y Afférentes', Spanish='Mapas Relacionados'
where English='RELATED MAPS';
GO

update SobekCM_Metadata_Translation
set English='Map Search', French='Carte', Spanish='Mapas de Búsqueda'
where English='MAP SEARCH';
GO

update SobekCM_Metadata_Translation
set English='Advanced Search', French='Avancé', Spanish='Búsqueda Avanzada'
where English='ADVANCED SEARCH';
GO

update SobekCM_Metadata_Translation
set English='Text Search', French='Recherche texte', Spanish='Búsqueda de Texto'
where English='TEXT SEARCH';
GO
