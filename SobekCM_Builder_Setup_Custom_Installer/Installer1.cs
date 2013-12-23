using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Management;
using System.Management.Instrumentation;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;


namespace SobekCM_Builder_Setup_Custom_Installer
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            string install_directory = base.Context.Parameters["targetdir"];
            try
            {
                DirectoryInfo myDirectoryInfo = new DirectoryInfo(install_directory + "\\config");
                DirectorySecurity myDirectorySecurity = myDirectoryInfo.GetAccessControl();
                myDirectorySecurity.AddAccessRule(new FileSystemAccessRule("Everyone",  FileSystemRights.FullControl, AccessControlType.Allow));  
                myDirectoryInfo.SetAccessControl(myDirectorySecurity);

                // If the LOGS subdirectory does not exist, make it
                if (!Directory.Exists(install_directory + "\\logs"))
                    Directory.CreateDirectory(install_directory + "\\logs");

                DirectoryInfo myDirectoryInfo2 = new DirectoryInfo(install_directory + "\\logs");
                DirectorySecurity myDirectorySecurity2 = myDirectoryInfo2.GetAccessControl();
                myDirectorySecurity2.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                myDirectoryInfo2.SetAccessControl(myDirectorySecurity2);


            }
            catch { }


            if (!File.Exists(install_directory + "\\config\\sobekcm.config"))
			{
                string config_file = install_directory + "\\config\\SobekCM_Builder_Configuration.exe";
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = config_file;
                process.StartInfo.Arguments = "--installing";
                process.Start();
            }
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

    }
}
