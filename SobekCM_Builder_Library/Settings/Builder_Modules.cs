using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Builder_Library.Modules.Folders;
using SobekCM.Builder_Library.Modules.Items;
using SobekCM.Builder_Library.Modules.PostProcess;
using SobekCM.Builder_Library.Modules.PreProcess;
using SobekCM.Core.Settings;

namespace SobekCM.Builder_Library.Settings
{
    /// <summary> Collection of builder modules to run for an instance of SobekCM / builder </summary>
    public class Builder_Modules : Builder_Settings
    {
        private readonly List<iPreProcessModule> preProcessModules;
        private readonly List<iSubmissionPackageModule> processItemModules;
        private readonly List<iSubmissionPackageModule> deleteItemModules;
        private readonly List<iPostProcessModule> postProcessModules;

        private readonly List<iFolderModule> allFolderModules;
        private readonly Dictionary<string, iFolderModule> assemblyClassToModule;


        /// <summary> Constructor for a new instance of the Builder_Modules class </summary>
        public Builder_Modules() : base()
        {
            preProcessModules = new List<iPreProcessModule>();
            processItemModules = new List<iSubmissionPackageModule>();
            deleteItemModules = new List<iSubmissionPackageModule>();
            postProcessModules = new List<iPostProcessModule>();

            allFolderModules = new List<iFolderModule>();
            assemblyClassToModule = new Dictionary<string, iFolderModule>();
        }

        /// <summary> Clear all the settings and the list of modules </summary>
        public override void Clear()
        {
            base.Clear();

            preProcessModules.Clear();
            processItemModules.Clear();
            deleteItemModules.Clear();
            postProcessModules.Clear();
        }

        /// <summary> Build the modules for the non-folder specific builder modules </summary>
        /// <param name="Settings"> Settings indicate which modules to build </param>
        /// <returns> Either null, or a list of errors encountered </returns>
        public List<string> Builder_Modules_From_Settings( )
        {
            // Build the return value
            List<string> errors = new List<string>();
            string errorMessage;

            // Clear existing modules
            preProcessModules.Clear();
            processItemModules.Clear();
            deleteItemModules.Clear();
            postProcessModules.Clear();
            allFolderModules.Clear();
            assemblyClassToModule.Clear();

            // Create all the pre-process modules
            foreach (Builder_Module_Setting preSetting in PreProcessModulesSettings)
            {
                // Look for the standard 
                if (String.IsNullOrEmpty(preSetting.Assembly))
                {
                    switch (preSetting.Class)
                    {
                        case "SobekCM.Builder_Library.Modules.PreProcess.ProcessPendingFdaReportsModule":
                            iPreProcessModule thisModule = new ProcessPendingFdaReportsModule();
                            if ((!String.IsNullOrEmpty(preSetting.Argument1)) || (!String.IsNullOrEmpty(preSetting.Argument2)) || (!String.IsNullOrEmpty(preSetting.Argument3)))
                            {
                                if(thisModule.Arguments == null)
                                     thisModule.Arguments = new List<string>();
                                thisModule.Arguments.Add(String.IsNullOrEmpty(preSetting.Argument1) ? String.Empty : preSetting.Argument1);
                                thisModule.Arguments.Add(String.IsNullOrEmpty(preSetting.Argument2) ? String.Empty : preSetting.Argument2);
                                thisModule.Arguments.Add(String.IsNullOrEmpty(preSetting.Argument3) ? String.Empty : preSetting.Argument3);
                            }
                            preProcessModules.Add(thisModule);
                            continue;
                    }
                }

                object preAsObj = Get_Module(preSetting, out errorMessage);
                if ((preAsObj == null) && (errorMessage.Length > 0))
                {
                    errors.Add(errorMessage);
                }
                else
                {
                    iPreProcessModule preAsPre = preAsObj as iPreProcessModule;
                    if (preAsPre == null)
                    {
                        errors.Add(preSetting.Class + " loaded from assembly but does not implement the IPreProcessModules interface!");
                    }
                    else
                    {
                        if ((!String.IsNullOrEmpty(preSetting.Argument1)) || (!String.IsNullOrEmpty(preSetting.Argument2)) || (!String.IsNullOrEmpty(preSetting.Argument3)))
                        {
                            if (preAsPre.Arguments == null)
                                preAsPre.Arguments = new List<string>();
                            preAsPre.Arguments.Add(String.IsNullOrEmpty(preSetting.Argument1) ? String.Empty : preSetting.Argument1);
                            preAsPre.Arguments.Add(String.IsNullOrEmpty(preSetting.Argument2) ? String.Empty : preSetting.Argument2);
                            preAsPre.Arguments.Add(String.IsNullOrEmpty(preSetting.Argument3) ? String.Empty : preSetting.Argument3);
                        }

                        preProcessModules.Add(preAsPre);
                    }
                }
            }

            // Create all the post-process modules
            foreach (Builder_Module_Setting postSetting in PostProcessModulesSettings)
            {
                // Look for the standard 
                if (String.IsNullOrEmpty(postSetting.Assembly))
                {
                    switch (postSetting.Class)
                    {
                        case "SobekCM.Builder_Library.Modules.PostProcess.BuildAggregationBrowsesModule":
                            iPostProcessModule thisModule = new BuildAggregationBrowsesModule();
                            if ((!String.IsNullOrEmpty(postSetting.Argument1)) || (!String.IsNullOrEmpty(postSetting.Argument2)) || (!String.IsNullOrEmpty(postSetting.Argument3)))
                            {
                                if (thisModule.Arguments == null)
                                    thisModule.Arguments = new List<string>();
                                thisModule.Arguments.Add(String.IsNullOrEmpty(postSetting.Argument1) ? String.Empty : postSetting.Argument1);
                                thisModule.Arguments.Add(String.IsNullOrEmpty(postSetting.Argument2) ? String.Empty : postSetting.Argument2);
                                thisModule.Arguments.Add(String.IsNullOrEmpty(postSetting.Argument3) ? String.Empty : postSetting.Argument3);
                            }
                            postProcessModules.Add(thisModule);
                            continue;
                    }
                }

                object postAsObj = Get_Module(postSetting, out errorMessage);
                if ((postAsObj == null) && (errorMessage.Length > 0))
                {
                    errors.Add(errorMessage);
                }
                else
                {
                    iPostProcessModule postAsPost = postAsObj as iPostProcessModule;
                    if (postAsPost == null)
                    {
                        errors.Add(postSetting.Class + " loaded from assembly but does not implement the IPostProcessModules interface!");
                    }
                    else
                    {
                        if ((!String.IsNullOrEmpty(postSetting.Argument1)) || (!String.IsNullOrEmpty(postSetting.Argument2)) || (!String.IsNullOrEmpty(postSetting.Argument3)))
                        {
                            if (postAsPost.Arguments == null)
                                postAsPost.Arguments = new List<string>();
                            postAsPost.Arguments.Add(String.IsNullOrEmpty(postSetting.Argument1) ? String.Empty : postSetting.Argument1);
                            postAsPost.Arguments.Add(String.IsNullOrEmpty(postSetting.Argument2) ? String.Empty : postSetting.Argument2);
                            postAsPost.Arguments.Add(String.IsNullOrEmpty(postSetting.Argument3) ? String.Empty : postSetting.Argument3);
                        }

                        postProcessModules.Add(postAsPost);
                    }
                }
            }

            // Create all the item processing modules (for new or updated item)
            foreach (Builder_Module_Setting itemSetting in ItemProcessModulesSettings)
            {
                iSubmissionPackageModule itemModule = Get_Submission_Module(itemSetting, out errorMessage);
                if ((itemModule == null) && (!String.IsNullOrEmpty(errorMessage)))
                    errors.Add(errorMessage);
                else
                    processItemModules.Add(itemModule);
            }

            // Create all the item processing modules (for deleting items)
            foreach (Builder_Module_Setting itemSetting in ItemDeleteModulesSettings)
            {
                iSubmissionPackageModule itemModule = Get_Submission_Module(itemSetting, out errorMessage);
                if ((itemModule == null) && (!String.IsNullOrEmpty(errorMessage)))
                    errors.Add(errorMessage);
                else
                    deleteItemModules.Add(itemModule);
            }

            // Create the folder modules - look at every folder
            foreach (Builder_Source_Folder thisFolder in IncomingFolders)
            {
                // If not linked to a module set, do nothing
                if (thisFolder.Builder_Module_Settings == null)
                {
                    errors.Add("Folder has no module set, so no processing will occur ( " + thisFolder.Folder_Name + " )");
                    continue;
                }

                // Step through all the folder builer modules and if it hasn't been built yet, do so now
                foreach (Builder_Module_Setting folderSetting in thisFolder.Builder_Module_Settings)
                {
                   
                    string key = folderSetting.Key;
                    iFolderModule thisModule = null;

                    // Does this already exist?
                    if (!assemblyClassToModule.ContainsKey(key))
                    {
                        // Look for the standard options
                        if (String.IsNullOrEmpty(folderSetting.Assembly))
                        {
                            switch (folderSetting.Class)
                            {
                                case "SobekCM.Builder_Library.Modules.Folders.MoveAgedPackagesToProcessModule":
                                    thisModule = new MoveAgedPackagesToProcessModule();
                                    break;

                                case "SobekCM.Builder_Library.Modules.Folders.ApplyBibIdRestrictionModule":
                                    thisModule = new ApplyBibIdRestrictionModule();
                                    break;

                                case "SobekCM.Builder_Library.Modules.Folders.ValidateAndClassifyModule":
                                    thisModule = new ValidateAndClassifyModule();
                                    break;

                                case "SobekCM.Builder_Library.Modules.Folders.WolfsonianProcessorModule":
                                    thisModule = new WolfsonianProcessorModule();
                                    break;

                                case "SobekCM.Builder_Library.Modules.Folders.UpdateNonBibFolders":
                                    thisModule = new UpdateNonBibFolders();
                                    break;


                            }

                            if (thisModule != null)
                            {
                                if ((!String.IsNullOrEmpty(folderSetting.Argument1)) || (!String.IsNullOrEmpty(folderSetting.Argument2)) || (!String.IsNullOrEmpty(folderSetting.Argument3)))
                                {
                                    if (thisModule.Arguments == null)
                                        thisModule.Arguments = new List<string>();
                                    thisModule.Arguments.Add(String.IsNullOrEmpty(folderSetting.Argument1) ? String.Empty : folderSetting.Argument1);
                                    thisModule.Arguments.Add(String.IsNullOrEmpty(folderSetting.Argument2) ? String.Empty : folderSetting.Argument2);
                                    thisModule.Arguments.Add(String.IsNullOrEmpty(folderSetting.Argument3) ? String.Empty : folderSetting.Argument3);
                                }
                                allFolderModules.Add(thisModule);
                                assemblyClassToModule[folderSetting.Key] = thisModule;
                                continue;
                            }
                        }

                        object folderAsObj = Get_Module(folderSetting, out errorMessage);
                        if ((folderAsObj == null) && (errorMessage.Length > 0))
                        {
                            errors.Add(errorMessage);
                        }
                        else
                        {
                            iFolderModule folderAsFolder = folderAsObj as iFolderModule;
                            if (folderAsFolder == null)
                            {
                                errors.Add(folderSetting.Class + " loaded from assembly but does not implement the IFolderModule interface!");
                            }
                            else
                            {
                                if ((!String.IsNullOrEmpty(folderSetting.Argument1)) || (!String.IsNullOrEmpty(folderSetting.Argument2)) || (!String.IsNullOrEmpty(folderSetting.Argument3)))
                                {
                                    if (folderAsFolder.Arguments == null)
                                        folderAsFolder.Arguments = new List<string>();
                                    folderAsFolder.Arguments.Add(String.IsNullOrEmpty(folderSetting.Argument1) ? String.Empty : folderSetting.Argument1);
                                    folderAsFolder.Arguments.Add(String.IsNullOrEmpty(folderSetting.Argument2) ? String.Empty : folderSetting.Argument2);
                                    folderAsFolder.Arguments.Add(String.IsNullOrEmpty(folderSetting.Argument3) ? String.Empty : folderSetting.Argument3);
                                }

                                allFolderModules.Add(folderAsFolder);
                                assemblyClassToModule[folderSetting.Key] = folderAsFolder;
                            }
                        }
                    }
                }
            }


            return errors;
        }

        private iSubmissionPackageModule Get_Submission_Module(Builder_Module_Setting itemSetting, out string ErrorMessage)
        {
            ErrorMessage = String.Empty;

            // Look for the standard 
            if (String.IsNullOrEmpty(itemSetting.Assembly))
            {
                iSubmissionPackageModule thisModule = null;
                switch (itemSetting.Class)
                {
                    case "SobekCM.Builder_Library.Modules.Items.ConvertOfficeFilesToPdfModule":
                        thisModule = new ConvertOfficeFilesToPdfModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.ExtractTextFromPdfModule":
                        thisModule = new ExtractTextFromPdfModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.CreatePdfThumbnailModule":
                        thisModule = new CreatePdfThumbnailModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.ExtractTextFromHtmlModule":
                        thisModule = new ExtractTextFromHtmlModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.ExtractTextFromXmlModule":
                        thisModule = new ExtractTextFromXmlModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.OcrTiffsModule":
                        thisModule = new OcrTiffsModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.CleanDirtyOcrModule":
                        thisModule = new CleanDirtyOcrModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.CheckForSsnModule":
                        thisModule = new CheckForSsnModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.CreateImageDerivativesModule":
                        thisModule = new CreateImageDerivativesModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.CopyToArchiveFolderModule":
                        thisModule = new CopyToArchiveFolderModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.MoveFilesToImageServerModule":
                        thisModule = new MoveFilesToImageServerModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.ReloadMetsAndBasicDbInfoModule":
                        thisModule = new ReloadMetsAndBasicDbInfoModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.UpdateJpegAttributesModule":
                        thisModule = new UpdateJpegAttributesModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.AttachAllNonImageFilesModule":
                        thisModule = new AttachAllNonImageFilesModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.AddNewImagesAndViewsModule":
                        thisModule = new AddNewImagesAndViewsModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.AttachImagesAllModule":
                        thisModule = new AttachImagesAllModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.EnsureMainThumbnailModule":
                        thisModule = new EnsureMainThumbnailModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.GetPageCountFromPdfModule":
                        thisModule = new GetPageCountFromPdfModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.UpdateWebConfigModule":
                        thisModule = new UpdateWebConfigModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.SaveServiceMetsModule":
                        thisModule = new SaveServiceMetsModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.SaveMarcXmlModule":
                        thisModule = new SaveMarcXmlModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.SaveToDatabaseModule":
                        thisModule = new SaveToDatabaseModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.SaveToSolrLuceneModule":
                        thisModule = new SaveToSolrLuceneModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.CleanWebResourceFolderModule":
                        thisModule = new CleanWebResourceFolderModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.CreateStaticVersionModule":
                        thisModule = new CreateStaticVersionModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.AddTrackingWorkflowModule":
                        thisModule = new AddTrackingWorkflowModule();
                        break;

                    case "SobekCM.Builder_Library.Modules.Items.DeleteItemModule":
                        thisModule = new DeleteItemModule();
                        break;
                }

                if (thisModule != null)
                {
                    if ((!String.IsNullOrEmpty(itemSetting.Argument1)) || (!String.IsNullOrEmpty(itemSetting.Argument2)) || (!String.IsNullOrEmpty(itemSetting.Argument3)))
                    {
                        if (thisModule.Arguments == null)
                            thisModule.Arguments = new List<string>();
                        thisModule.Arguments.Add(String.IsNullOrEmpty(itemSetting.Argument1) ? String.Empty : itemSetting.Argument1);
                        thisModule.Arguments.Add(String.IsNullOrEmpty(itemSetting.Argument2) ? String.Empty : itemSetting.Argument2);
                        thisModule.Arguments.Add(String.IsNullOrEmpty(itemSetting.Argument3) ? String.Empty : itemSetting.Argument3);
                    }
                    return thisModule;
                }
            }

            object itemAsObj = Get_Module(itemSetting, out ErrorMessage);
            if ((itemAsObj == null) && (ErrorMessage.Length > 0))
            {
                return null;
            }
            else
            {
                iSubmissionPackageModule itemAsItem = itemAsObj as iSubmissionPackageModule;
                if (itemAsItem == null)
                {
                    ErrorMessage = itemSetting.Class + " loaded from assembly but does not implement the ISubmissionPackageModules interface!";
                    return null;
                }
                else
                {
                    if ((!String.IsNullOrEmpty(itemSetting.Argument1)) || (!String.IsNullOrEmpty(itemSetting.Argument2)) || (!String.IsNullOrEmpty(itemSetting.Argument3)))
                    {
                        if (itemAsItem.Arguments == null)
                            itemAsItem.Arguments = new List<string>();
                        itemAsItem.Arguments.Add(String.IsNullOrEmpty(itemSetting.Argument1) ? String.Empty : itemSetting.Argument1);
                        itemAsItem.Arguments.Add(String.IsNullOrEmpty(itemSetting.Argument2) ? String.Empty : itemSetting.Argument2);
                        itemAsItem.Arguments.Add(String.IsNullOrEmpty(itemSetting.Argument3) ? String.Empty : itemSetting.Argument3);
                    }

                    return itemAsItem;
                }
            }
        }

        private object Get_Module(Builder_Module_Setting Settings, out string ErrorMessage )
        {
            ErrorMessage = String.Empty;

            try
            {
                // Using reflection, create an object from the class namespace/name 
                Assembly dllAssembly = Assembly.GetExecutingAssembly();
                if (!String.IsNullOrEmpty(Settings.Assembly))
                {
                    dllAssembly = Assembly.LoadFrom(Settings.Assembly);
                }
                
                Type readerWriterType = dllAssembly.GetType(Settings.Class);
                return Activator.CreateInstance(readerWriterType); 
            }
            catch (Exception ee)
            {
                ErrorMessage = "Unable to load class from assembly. ( " + Settings.Class + " ) : " + ee.Message;
                return null;
            }
        }

        /// <summary> Get the list of pre-process module objects to use for pre-processing during a SobekCM builder execution </summary>
        public ReadOnlyCollection<iPreProcessModule> PreProcessModules { get { return new ReadOnlyCollection<iPreProcessModule>(preProcessModules); }}

        /// <summary> Get the list of item processing module objects to use for processing a new item or update an existing item during a SobekCM builder execution </summary>
        public ReadOnlyCollection<iSubmissionPackageModule> ItemProcessModules { get { return new ReadOnlyCollection<iSubmissionPackageModule>(processItemModules); } }

        /// <summary> Get the list of item delete modules objects to use when deleting an object during a SobekCM builder execution </summary>
        public ReadOnlyCollection<iSubmissionPackageModule> DeleteItemModules { get { return new ReadOnlyCollection<iSubmissionPackageModule>(deleteItemModules); }}

        /// <summary> Get the list of post-process module objects to use for post-processing during a SobekCM builder execution </summary>
        public ReadOnlyCollection<iPostProcessModule> PostProcessModules { get { return new ReadOnlyCollection<iPostProcessModule>(postProcessModules); }}


        public ReadOnlyCollection<iFolderModule> AllFolderModules { get { return new ReadOnlyCollection<iFolderModule>(allFolderModules); }}

        public iFolderModule Get_Folder_Module_By_Key(string Key)
        {
            return assemblyClassToModule[Key];
        }
    }
}
