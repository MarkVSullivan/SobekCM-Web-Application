using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SobekCM.Library.Localization
{
    class Program
    {
	    private static void Main(string[] args)
	    {
			SobekCM_LocalizationInfo info = new SobekCM_LocalizationInfo();
		    info.Write_Localization_XML("test.xml");


		    StreamReader reader = new StreamReader("SobekCM_Localization.txt");
		    string line = reader.ReadLine();
		    line = reader.ReadLine();
		    Single_Class_Info current_class = null;
		    List<Single_Class_Info> classes_to_add = new List<Single_Class_Info>();
		    while (line != null)
		    {
			    if (line.Trim().Length > 0)
			    {
				    string[] split = line.Split("\t".ToCharArray());
				    if (split.Length > 4)
				    {
					    string sobekcm_class = split[0];
					    string property_name = split[2];
					    string english_term = split[2];
					    string remarks = split[4];

					    if ((current_class == null) || (sobekcm_class != current_class.ClassName))
					    {
						    current_class = new Single_Class_Info(sobekcm_class);
						    classes_to_add.Add(current_class);
					    }
					    if (property_name.Length > 0)
					    {
						    Property_Info newProperty = new Property_Info();
						    newProperty.Name = property_name;
						    newProperty.English_Term = english_term;
						    newProperty.Remarks = remarks;
						    current_class.Properties.Add(newProperty);
					    }
				    }
			    }

			    line = reader.ReadLine();
		    }
		    reader.Close();


		    StreamWriter overallwriter = new StreamWriter("OUTPUT\\SobekCM_LocalizationInfo.cs");
			overallwriter.WriteLine("using System;");
		    overallwriter.WriteLine("using System.IO;");
			overallwriter.WriteLine("using SobekCM.Library.Configuration;");
		    overallwriter.WriteLine("using SobekCM.Library.Localization.Classes;");
		    overallwriter.WriteLine();
		    overallwriter.WriteLine("namespace SobekCM.Library.Localization");
		    overallwriter.WriteLine("{");
		    overallwriter.WriteLine("    public class SobekCM_LocalizationInfo");
		    overallwriter.WriteLine("    {");

		    overallwriter.WriteLine("		/// <summary> Language of this localization information </summary>");
		    overallwriter.WriteLine("		public Web_Language_Enum Language { get; set; }");
			overallwriter.WriteLine();

		    overallwriter.WriteLine("		#region Private members that contain the localization strings for each class ");
		    overallwriter.WriteLine();
		    foreach (Single_Class_Info thisObject in classes_to_add)
		    {
			    if (thisObject.ClassName.IndexOf("n/a") < 0)
			    {
				    overallwriter.WriteLine("        /// <summary> Localization string information for the " + thisObject.ClassName + " class </summary>");
				    overallwriter.WriteLine("        public " + thisObject.ClassName + "_LocalizationInfo " + thisObject.ClassName + " { get ; private set; }");
				    overallwriter.WriteLine("");
			    }
		    }
		    overallwriter.WriteLine();
		    overallwriter.WriteLine("		#endregion");
		    overallwriter.WriteLine();

		    overallwriter.WriteLine("		#region Constructor that configures all strings to the default english");
		    overallwriter.WriteLine();

		    overallwriter.WriteLine("        /// <summary> Constructor for a new instance of the SobekCM_LocalizationInfo class </summary>");
		    overallwriter.WriteLine("        /// <remarks> This sets all the terms for localization to the system default, before any resource file is read </remarks>");
		    overallwriter.WriteLine("        public SobekCM_LocalizationInfo()");
		    overallwriter.WriteLine("        {");
		    overallwriter.WriteLine("            // Set a hardwired default language for this localization initially");
		    overallwriter.WriteLine("            // This will be replaced by the actual value");
		    overallwriter.WriteLine("            Language = Web_Language_Enum.English;");
			overallwriter.WriteLine();

		    overallwriter.WriteLine("            // Initialize all the child localization objects");
		    foreach (Single_Class_Info thisObject in classes_to_add)
		    {
			    overallwriter.WriteLine("            //Initialize the " + thisObject.ClassName + "_Localization class");
			    overallwriter.WriteLine("            " + thisObject.ClassName + " = new " + thisObject.ClassName + "_LocalizationInfo();");

			    foreach (Property_Info thisProperty in thisObject.Properties)
			    {
				    overallwriter.WriteLine("            " + thisObject.ClassName + ".Add_Localization_String( \"" + thisProperty.Name_To_Use_In_XML.Trim() + "\", \"" + thisProperty.English_Term.Replace("\"", "\\\"") + "\");");
			    }
			    overallwriter.WriteLine();
		    }
		    overallwriter.WriteLine("        }");

		    overallwriter.WriteLine();
		    overallwriter.WriteLine("		#endregion");
		    overallwriter.WriteLine();



		    overallwriter.WriteLine("        /// <summary> Write the localization XML source file from the data within this localization object </summary>");
		    overallwriter.WriteLine("        /// <param name=\"File\"> Filename for the resulting XML file </param>");
		    overallwriter.WriteLine("        /// <returns> TRUE if successful, otherise FALSE </returns>");
		    overallwriter.WriteLine("		public bool Write_Localization_XML(string File)");
		    overallwriter.WriteLine("		{");
		    overallwriter.WriteLine("			try");
		    overallwriter.WriteLine("			{");
		    overallwriter.WriteLine("				// Open the file and write to it");
		    overallwriter.WriteLine("				StreamWriter writer = new StreamWriter(File, false);");
		    overallwriter.WriteLine("				writer.WriteLine(\"<?xml version=\\\"1.0\\\" encoding=\\\"UTF-8\\\" standalone=\\\"yes\\\" ?>\");");
			overallwriter.WriteLine("				writer.WriteLine(\"<localization lang=\\\"\" + Web_Language_Enum_Converter.Enum_To_Name(Language) + \"\\\">\");");
		    overallwriter.WriteLine();
		    overallwriter.WriteLine("				// Add the inforamtion for each localization object");
			foreach (Single_Class_Info thisObject in classes_to_add)
			{
				overallwriter.WriteLine("				" + thisObject.ClassName + ".Write_Localization_XML(writer);");
			}
		    overallwriter.WriteLine();
		    overallwriter.WriteLine("				// Close the file");
		    overallwriter.WriteLine("				writer.WriteLine(\"</localization>\");");
		    overallwriter.WriteLine("				writer.Flush();");
		    overallwriter.WriteLine("				writer.Close();");
		    overallwriter.WriteLine();
		    overallwriter.WriteLine("				return true;");
		    overallwriter.WriteLine("			}");
		    overallwriter.WriteLine("			catch (Exception ee)");
		    overallwriter.WriteLine("			{");
		    overallwriter.WriteLine("				return false;");
		    overallwriter.WriteLine("			}");
		    overallwriter.WriteLine("		}");

		    overallwriter.WriteLine("    }");
		    overallwriter.WriteLine("}");
		    overallwriter.Flush();
		    overallwriter.Close();

		    // Now, write each class
		    foreach (Single_Class_Info thisObject in classes_to_add)
		    {
			    if (thisObject.ClassName.IndexOf("n/a") < 0)
			    {
				    StreamWriter classWriter = new StreamWriter("OUTPUT\\Classes\\" + thisObject.ClassName + "_LocalizationInfo.cs");
				    classWriter.WriteLine("namespace SobekCM.Library.Localization.Classes");
				    classWriter.WriteLine("{");
				    classWriter.WriteLine("    /// <summary> Localization class holds all the standard terms utilized by the " + thisObject.ClassName + " class </summary>");
				    classWriter.WriteLine("    public class " + thisObject.ClassName + "_LocalizationInfo : baseLocalizationInfo");
				    classWriter.WriteLine("    {");
				    classWriter.WriteLine("        /// <summary> Constructor for a new instance of the " + thisObject.ClassName + "_Localization class </summary>");
				    classWriter.WriteLine("        public " + thisObject.ClassName + "_LocalizationInfo()");
				    classWriter.WriteLine("        {");
				    classWriter.WriteLine("            // Set the source class name this localization file serves");
				    classWriter.WriteLine("            ClassName = \"" + thisObject.ClassName + "\";");
				    classWriter.WriteLine("        }");
				    classWriter.WriteLine();

				    classWriter.WriteLine("        /// <summary> Adds a localization string ( with key and value ) to this localization class </summary>");
				    classWriter.WriteLine("        /// <param name=\"Key\"> Key for the new localization string being saved </param>");
				    classWriter.WriteLine("        /// <param name=\"Value\"> Value for this localization string </param>");
				    classWriter.WriteLine("        /// <remarks> This overrides the base class's implementation </remarks>");
				    classWriter.WriteLine("        public override void Add_Localization_String(string Key, string Value)");
				    classWriter.WriteLine("        {");
				    classWriter.WriteLine("            // First, add to the localization string dictionary");
				    classWriter.WriteLine("            base.Add_Localization_String(Key, Value);");
				    classWriter.WriteLine();
				    classWriter.WriteLine("            // Assign to custom properties depending on the key");
				    classWriter.WriteLine("            switch (Key)");
				    classWriter.WriteLine("            {");

				    foreach (Property_Info thisProperty in thisObject.Properties)
				    {
					    classWriter.WriteLine("                case \"" + thisProperty.Name_To_Use_In_XML.Trim() + "\":");
					    classWriter.WriteLine("                    " + thisProperty.Name_To_Use_For_Property + " = Value;");
					    classWriter.WriteLine("                    break;");
					    classWriter.WriteLine();
				    }

				    classWriter.WriteLine("            }");
				    classWriter.WriteLine("        }");

				    // Now, add each property individually
				    foreach (Property_Info thisProperty in thisObject.Properties)
				    {
					    if (thisProperty.Remarks.Length > 0)
					    {
						    classWriter.WriteLine("        /// <remarks> " + thisProperty.Remarks + " </remarks>");
					    }
					    else
					    {
						    classWriter.WriteLine("        /// <remarks> '" + thisProperty.English_Term + "' localization string </remarks>");
					    }
					    classWriter.WriteLine("        public string " + thisProperty.Name_To_Use_For_Property + " { get; private set; }");
					    classWriter.WriteLine();
				    }


				    classWriter.WriteLine("    }");
				    classWriter.WriteLine("}");
				    classWriter.Flush();
				    classWriter.Close();
			    }
		    }

			

		    Console.WriteLine("COMPLETE");
		    Console.ReadLine();
	    }
    }
}
