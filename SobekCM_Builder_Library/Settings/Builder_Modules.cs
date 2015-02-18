using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        /// <summary> Constructor for a new instance of the Builder_Modules class </summary>
        public Builder_Modules()
        {
            preProcessModules = new List<iPreProcessModule>();
            processItemModules = new List<iSubmissionPackageModule>();
            deleteItemModules = new List<iSubmissionPackageModule>();
            postProcessModules = new List<iPostProcessModule>();
        }

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
        public List<string> Builder_Modules_From_Settings()
        {
            // Build the return value
            List<string> errors = new List<string>();

            // Clear existing modules
            preProcessModules.Clear();
            processItemModules.Clear();
            deleteItemModules.Clear();
            postProcessModules.Clear();


            return errors;
        }

        /// <summary> Get the list of pre-process module objects to use for pre-processing during a SobekCM builder execution </summary>
        public ReadOnlyCollection<iPreProcessModule> PreProcessModules { get { return new ReadOnlyCollection<iPreProcessModule>(preProcessModules); }}

        /// <summary> Get the list of item processing module objects to use for processing a new item or update an existing item during a SobekCM builder execution </summary>
        public ReadOnlyCollection<iSubmissionPackageModule> ProcessItemModules { get { return new ReadOnlyCollection<iSubmissionPackageModule>(processItemModules); }}

        /// <summary> Get the list of item delete modules objects to use when deleting an object during a SobekCM builder execution </summary>
        public ReadOnlyCollection<iSubmissionPackageModule> DeleteItemModules { get { return new ReadOnlyCollection<iSubmissionPackageModule>(deleteItemModules); }}

        /// <summary> Get the list of post-process module objects to use for post-processing during a SobekCM builder execution </summary>
        public ReadOnlyCollection<iPostProcessModule> PostProcessModules { get { return new ReadOnlyCollection<iPostProcessModule>(postProcessModules); }}
    }
}
