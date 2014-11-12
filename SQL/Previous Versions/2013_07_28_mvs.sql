
-- Make the settings table be able to hold longer values
alter table SobekCM_Settings alter column Setting_Value varchar(max) not null; 
GO

-- Add setting for uploading file downloads
insert into SobekCM_Settings ( Setting_Key, Setting_Value )
values ( 'Upload File Types', '.aif,.aifc,.aiff,.au,.avi,.bz2,.c,.c++,.css,.dbf,.ddl,.doc,.docx,.dtd,.dvi,.flac,.gz,.htm,.html,.java,.jps,.js,.m4p,.mid,.midi,.mp2,.mp3,.mpg,.odp,.ogg,.pdf,.pgm,.ppt,.pptx,.ps,.ra,.ram,.rar,.rm,.rtf,.sgml,.swf,.sxi,.tbz2,.tgz,.wav,.wave,.wma,.wmv,.xls,.xlsx,.xml,.zip' );
GO

-- Add setting for uploading file downloads
insert into SobekCM_Settings ( Setting_Key, Setting_Value )
values ( 'Upload Image Types', '.txt,.tif,.jpg,.jp2,.pro' );
GO