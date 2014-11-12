



/****** Object:  StoredProcedure [dbo].[[mySobek_Get_All_Projects_DefaultMetadatas]]    Script Date: 12/20/2013 05:43:35 ******/
-- Get the list of all templates and default metadata sets 
CREATE PROCEDURE [dbo].[mySobek_Get_All_Template_DefaultMetadatas]
AS
BEGIN
	
	select * 
	from mySobek_DefaultMetadata
	order by MetadataCode;

	select * 
	from mySobek_Template
	order by TemplateCode;

END;
GO


GRANT EXECUTE ON [dbo].[mySobek_Get_All_Templates_DefaultMetadatas] TO sobek_user;
GO


/****** Object:  StoredProcedure [dbo].[mySobek_Save_User2]    Script Date: 12/20/2013 05:43:36 ******/
-- Saves a user
ALTER PROCEDURE [dbo].[mySobek_Save_User]
	@userid int,
	@ufid char(8),
	@username nvarchar(100),
	@password nvarchar(100),
	@emailaddress nvarchar(100),
	@firstname nvarchar(100),
	@lastname nvarchar(100),
	@cansubmititems bit,
	@nickname nvarchar(100),
	@organization nvarchar(250),
	@college nvarchar(250),
	@department nvarchar(250),
	@unit nvarchar(250),
	@rights nvarchar(1000),
	@sendemail bit,
	@language nvarchar(50),
	@default_template varchar(50),
	@default_metadata varchar(50),
	@organization_code varchar(15),
	@receivestatsemail bit
AS
BEGIN

	if ( @userid < 0 )
	begin

		-- Add this into the user table first
		insert into mySobek_User ( UFID, UserName, [Password], EmailAddress, LastName, FirstName, DateCreated, LastActivity, isActive,  Note_Length, Can_Make_Folders_Public, isTemporary_Password, Can_Submit_Items, NickName, Organization, College, Department, Unit, Default_Rights, sendEmailOnSubmission, UI_Language, Internal_User, OrganizationCode, Receive_Stats_Emails, Include_Tracking_Standard_Forms )
		values ( @ufid, @username, @password, @emailaddress, @lastname, @firstname, getdate(), getDate(), 'true', 1000, 'true', 'false', @cansubmititems, @nickname, @organization, @college, @department, @unit, @rights, @sendemail, @language, 'false', @organization_code, @receivestatsemail, 'false' );

		-- Get the user is
		declare @newuserid int;
		set @newuserid = @@identity;

	end
	else
	begin

		-- Update this user
		update mySobek_User
		set UFID = @ufid, UserName = @username, EmailAddress=@emailAddress,
			Firstname = @firstname, Lastname = @lastname, Can_Submit_Items = @cansubmititems,
			NickName = @nickname, Organization=@organization, College=@college, Department=@department,
			Unit=@unit, Default_Rights=@rights, sendEmailOnSubmission = @sendemail, UI_Language=@language,
			OrganizationCode=@organization_code, Receive_Stats_Emails=@receivestatsemail
		where UserID = @userid;

		-- Set the default template
		if ( len( @default_template ) > 0 )
		begin
			-- Get the template id
			declare @templateid int;
			select @templateid = TemplateID from mySobek_Template where TemplateCode=@default_template;

			-- Clear the current default template
			update mySobek_User_Template_Link set DefaultTemplate = 'false' where UserID=@userid;

			-- Does this link already exist?
			if (( select count(*) from mySobek_User_Template_Link where UserID=@userid and TemplateID=@templateid ) > 0 )
			begin
				-- Update the link
				update mySobek_User_Template_Link set DefaultTemplate = 'true' where UserID=@userid and TemplateID=@templateid;
			end
			else
			begin
				-- Just add this link
				insert into mySobek_User_Template_Link ( UserID, TemplateID, DefaultTemplate ) values ( @userid, @templateid, 'true' );
			end;
		end;

		-- Set the default metadata
		if ( len( @default_metadata ) > 0 )
		begin
			-- Get the project id
			declare @projectid int;
			select @projectid = DefaultMetadataID from mySobek_DefaultMetadata where MetadataCode=@default_metadata;

			-- Clear the current default project
			update mySobek_User_DefaultMetadata_Link set CurrentlySelected = 'false' where UserID=@userid;

			-- Does this link already exist?
			if (( select count(*) from mySobek_User_DefaultMetadata_Link where UserID=@userid and DefaultMetadataID=@projectid ) > 0 )
			begin
				-- Update the link
				update mySobek_User_DefaultMetadata_Link set CurrentlySelected = 'true' where UserID=@userid and DefaultMetadataID=@projectid;
			end
			else
			begin
				-- Just add this link
				insert into mySobek_User_DefaultMetadata_Link ( UserID, DefaultMetadataID, CurrentlySelected ) values ( @userid, @projectid, 'true' );
			end;
		end;
	end;
END;
GO
