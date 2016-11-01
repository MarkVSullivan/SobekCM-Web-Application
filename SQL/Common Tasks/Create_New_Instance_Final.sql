
declare @institution_code varchar(20);
set @institution_code = 'SU';

declare @institution_name varchar(50);
set @institution_name = 'South University';

-- Add new web skin for code
if ( not exists ( select * from SobekCM_Web_Skin where WebSkinCode=@institution_code )) 
begin 
	insert into SobekCM_Web_Skin ( WebSkinCode, OverrideHeaderFooter, OverrideBanner, BaseWebSkin, Notes, Build_On_Launch, SuppressTopNavigation ) 
	values ( @institution_code, 'true', 'false', '', @institution_name + ' Web Skin', 'true', 'false' ); 
end;

-- Update portal for system code and system name
update SobekCM_Portal_URL 
set Abbreviation=@institution_code, Name=@institution_name + ' Portal' 
where len(Base_URL) = 0;

-- The portal should use the new web skin
update SobekCM_Portal_Web_Skin_Link 
set WebSkinID = ( select WebSkinID from SobekCM_Web_Skin where WebSkinCode=@institution_code )  
where PortalID = ( select PortalID from SobekCM_Portal_URL where len(Base_URL) = 0);

-- Set the system abbreviation in main settings also
update SobekCM_Settings 
set Setting_Value=@institution_code 
where Setting_Key='System Base Abbreviation' and Setting_Value='Sobek';

-- Set the system name in main settings also
update SobekCM_Settings 
set Setting_Value=@institution_name 
where Setting_Key='System Base Name' and Setting_Value='Sobek';

-- Add the default institution aggregation
declare @institution_aggregation_code varchar(20);
set @institution_aggregation_code = 'i' + @institution_code;
declare @institution_aggregation_name varchar(60);
set @institution_aggregation_name = @institution_name + ' Institution';
exec SobekCM_Save_Item_Aggregation -1, @institution_aggregation_code, @institution_aggregation_name, @institution_name, 'Default institution page', -1, 'Institution', 'false', 'true', 'BAFI', 0, 0, 'false', '', '', '', '', 1, 'Installer', '', -1;

-- Add my email for the error emails
update SobekCM_Settings 
set Setting_Value='Mark.V.Sullivan@sobekdigital.com' 
where Setting_Key='System Email' or Setting_Key='System Error Email';