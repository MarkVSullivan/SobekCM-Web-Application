using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Saxon.Api;


namespace SobekCM.Core.XSLT
{
    public static class XSLT_Transformer
    {
        /// <summary> Use the version-appropriate library to transform a source XML file into an output file with the XSLT file </summary>
        /// <param name="SourceFile"> Source XML file, to be transformed </param>
        /// <param name="XSLT_File"> XSLT file to perform the transform </param>
        /// <param name="OutputFile"> Resulting output file to write </param>
        /// <returns> XSLT return arguments including a flag for success, and possibly an exception </returns>
        public static XSLT_Transformer_ReturnArgs Transform(string SourceFile, string XSLT_File, string OutputFile)
        {
            // Ensure the XSLT file exists
            if (!File.Exists(XSLT_File))
            {
                XSLT_Transformer_ReturnArgs returnArgs = new XSLT_Transformer_ReturnArgs();
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = "Indicated XSLT file ( " + XSLT_File + " ) does not exist.";
                return returnArgs;
            }

            // Get the XSLT version number, since that determines which library to use
            int xslt_version = get_xslt_version(XSLT_File);

            if (xslt_version == 2)
            {
                // Start with the Saxon transformer
                XSLT_Transformer_ReturnArgs returnArgs = Saxon_Transform(SourceFile, XSLT_File, OutputFile);

                // If that failed, try the other
                if ((!returnArgs.Successful) && (returnArgs.InnerException != null))
                {
                    returnArgs = Native_Transform(SourceFile, XSLT_File, OutputFile);
                }

                returnArgs.XSLT_Version = xslt_version;
                return returnArgs;
            }
            else
            {
                XSLT_Transformer_ReturnArgs returnArgs = Native_Transform(SourceFile, XSLT_File, OutputFile);

                // If that failed, try the other
                if ((!returnArgs.Successful) && (returnArgs.InnerException != null))
                {
                    returnArgs = Saxon_Transform(SourceFile, XSLT_File, OutputFile);
                }

                returnArgs.XSLT_Version = xslt_version;
                return returnArgs;
            }
        }

        /// <summary> Use the version-appropriate library to transform a source XML file with the XSLT file and return the string </summary>
        /// <param name="SourceFile"> Source XML file, to be transformed </param>
        /// <param name="XSLT_File"> XSLT file to perform the transform </param>
        /// <returns> XSLT return arguments including a flag for success, and possibly an exception </returns>
        public static XSLT_Transformer_ReturnArgs Transform(string SourceFile, string XSLT_File)
        {
            // Ensure the XSLT file exists
            if (!File.Exists(XSLT_File))
            {
                XSLT_Transformer_ReturnArgs returnArgs = new XSLT_Transformer_ReturnArgs();
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = "Indicated XSLT file ( " + XSLT_File + " ) does not exist.";
                return returnArgs;
            }

            // Get the XSLT version number, since that determines which library to use
            int xslt_version = get_xslt_version(XSLT_File);

            if (xslt_version == 2)
            {
                // Start with the Saxon transformer
                XSLT_Transformer_ReturnArgs returnArgs = Saxon_Transform(SourceFile, XSLT_File);

                // If that failed, try the other
                if ((!returnArgs.Successful) && (returnArgs.InnerException != null))
                {
                    returnArgs = Native_Transform(SourceFile, XSLT_File);
                }

                returnArgs.XSLT_Version = xslt_version;
                return returnArgs;
            }
            else
            {
                XSLT_Transformer_ReturnArgs returnArgs = Native_Transform(SourceFile, XSLT_File);

                // If that failed, try the other
                if ((!returnArgs.Successful) && (returnArgs.InnerException != null))
                {
                    returnArgs = Saxon_Transform(SourceFile, XSLT_File);
                }

                returnArgs.XSLT_Version = xslt_version;
                return returnArgs;
            }
        }

        /// <summary> Gets the XSLT version from the specified XSLT file  </summary>
        /// <param name="XSLT_File"> XSLT file to check for version string </param>
        /// <returns> Integer for the version 1,2, or 3 (default is 2) </returns>
        private static int get_xslt_version(string XSLT_File)
        {
            int version = 2;

            try
            {
                using (Stream readerStream = new FileStream(XSLT_File, FileMode.Open, FileAccess.Read))
                using (XmlTextReader readerXml = new XmlTextReader(readerStream))
                {
                    // Step through this configuration file
                    while (readerXml.Read())
                    {
                        if (readerXml.NodeType == XmlNodeType.Element)
                        {
                            string nodename = readerXml.Name;
                            if (readerXml.Name.ToLower() == "xsl:stylesheet")
                            {
                                if (readerXml.MoveToAttribute("version"))
                                {
                                    float version_float;
                                    if (float.TryParse(readerXml.Value, out version_float))
                                        version = (int) Math.Round(version_float);
                                }
                            }

                            break;
                        }
                    }
                }
            }
            catch
            {
                version = 2;
            }

            return version;
        }

        /// <summary> Use the version-appropriate library to transform a source XML file into an output file with the XSLT file </summary>
        /// <param name="SourceFile"> Source XML file, to be transformed </param>
        /// <param name="XSLT_File"> XSLT file to perform the transform </param>
        /// <param name="XSLT_Version"> Version of the XSLT ( either 1,2, or 3) </param>
        /// <param name="OutputFile"> Resulting output file to write </param>
        /// <returns> XSLT return arguments including a flag for success, and possibly an exception </returns>
        public static XSLT_Transformer_ReturnArgs Transform(string SourceFile, string XSLT_File, int XSLT_Version, string OutputFile)
        {
            if (XSLT_Version == 2)
            {
                // Start with the Saxon transformer
                XSLT_Transformer_ReturnArgs returnArgs = Saxon_Transform(SourceFile, XSLT_File, OutputFile);

                // If that failed, try the other
                if ((!returnArgs.Successful) && (returnArgs.InnerException != null))
                {
                    returnArgs = Native_Transform(SourceFile, XSLT_File, OutputFile);
                }

                return returnArgs;
            }
            else
            {
                XSLT_Transformer_ReturnArgs returnArgs = Native_Transform(SourceFile, XSLT_File, OutputFile);

                // If that failed, try the other
                if ((!returnArgs.Successful) && (returnArgs.InnerException != null))
                {
                    returnArgs = Saxon_Transform(SourceFile, XSLT_File, OutputFile);
                }

                return returnArgs;
            }
        }

        /// <summary> Use the version-appropriate library to transform a source XML file with the XSLT file and return the string </summary>
        /// <param name="SourceFile"> Source XML file, to be transformed </param>
        /// <param name="XSLT_File"> XSLT file to perform the transform </param>
        /// <param name="XSLT_Version"> Version of the XSLT ( either 1,2, or 3) </param>
        /// <returns> XSLT return arguments including a flag for success, and possibly an exception </returns>
        public static XSLT_Transformer_ReturnArgs Transform(string SourceFile, string XSLT_File, int XSLT_Version)
        {
            if (XSLT_Version == 2)
            {
                // Start with the Saxon transformer
                XSLT_Transformer_ReturnArgs returnArgs = Saxon_Transform(SourceFile, XSLT_File);

                // If that failed, try the other
                if ((!returnArgs.Successful) && (returnArgs.InnerException != null))
                {
                    returnArgs = Native_Transform(SourceFile, XSLT_File);
                }

                return returnArgs;
            }
            else
            {
                XSLT_Transformer_ReturnArgs returnArgs = Native_Transform(SourceFile, XSLT_File);

                // If that failed, try the other
                if ((!returnArgs.Successful) && (returnArgs.InnerException != null))
                {
                    returnArgs = Saxon_Transform(SourceFile, XSLT_File);
                }

                return returnArgs;
            }
        }

        /// <summary> Use the Saxon-HE library to transform a source XML file into an output file with the XSLT file </summary>
        /// <param name="SourceFile"> Source XML file, to be transformed </param>
        /// <param name="XSLT_File"> XSLT file to perform the transform </param>
        /// <param name="OutputFile"> Resulting output file to write </param>
        /// <returns> XSLT return arguments including a flag for success, and possibly an exception </returns>
        public static XSLT_Transformer_ReturnArgs Saxon_Transform(string SourceFile, string XSLT_File, string OutputFile)
        {
            // Keep track of the start time
            DateTime starTime = DateTime.Now;

            // Create the return object
            XSLT_Transformer_ReturnArgs returnArgs = new XSLT_Transformer_ReturnArgs();
            returnArgs.Engine = XSLT_Transformer_Engine_Enum.Saxon;

            // Ensure the XSLT file exists
            if (!File.Exists(XSLT_File))
            {
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = "Indicated XSLT file ( " + XSLT_File + " ) does not exist.";
                return returnArgs;
            }

            // Ensure the source file exists
            if (!File.Exists(SourceFile))
            {
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = "Indicated source file ( " + SourceFile + " ) does not exist.";
                return returnArgs;
            }

            FileInfo input = new FileInfo(SourceFile);

            try
            {

                // Compile stylesheet
                var processor = new Processor();
                var compiler = processor.NewXsltCompiler();
                var executable = compiler.Compile(new Uri(XSLT_File));

                // Do transformation to a destination
                var destination = new DomDestination();
                using (var inputStream = input.OpenRead())
                {
                    var transformer = executable.Load();
                    transformer.SetInputStream(inputStream, new Uri(input.DirectoryName));
                    transformer.Run(destination);
                }

                // Save result to a file (or whatever else you wanna do)
                destination.XmlDocument.Save(OutputFile);

                returnArgs.Successful = true;
            }
            catch (Exception ee)
            {
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = ee.Message;
                returnArgs.InnerException = ee;
            }

            // Determine the elapsed time, in milliseconds
            returnArgs.Milliseconds = DateTime.Now.Subtract(starTime).TotalMilliseconds;

            return returnArgs;
        }

        /// <summary> Use the Saxon-HE library to transform a source XML file with the XSLT file and return the string </summary>
        /// <param name="SourceFile"> Source XML file, to be transformed </param>
        /// <param name="XSLT_File"> XSLT file to perform the transform </param>
        /// <returns> XSLT return arguments including a flag for success, and possibly an exception </returns>
        public static XSLT_Transformer_ReturnArgs Saxon_Transform(string SourceFile, string XSLT_File)
        {
            // Keep track of the start time
            DateTime starTime = DateTime.Now;

            // Create the return object
            XSLT_Transformer_ReturnArgs returnArgs = new XSLT_Transformer_ReturnArgs();
            returnArgs.Engine = XSLT_Transformer_Engine_Enum.Saxon;

            // Ensure the XSLT file exists
            if (!File.Exists(XSLT_File))
            {
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = "Indicated XSLT file ( " + XSLT_File + " ) does not exist.";
                return returnArgs;
            }

            // Ensure the source file exists
            if (!File.Exists(SourceFile))
            {
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = "Indicated source file ( " + SourceFile + " ) does not exist.";
                return returnArgs;
            }

            FileInfo input = new FileInfo(SourceFile);

            try
            {

                // Compile stylesheet
                var processor = new Processor();
                var compiler = processor.NewXsltCompiler();
                var executable = compiler.Compile(new Uri(XSLT_File));

                // Do transformation to a destination
                var destination = new DomDestination();
                using (var inputStream = input.OpenRead())
                {
                    var transformer = executable.Load();
                    transformer.SetInputStream(inputStream, new Uri(input.DirectoryName));
                    transformer.Run(destination);
                }

                // Save result to a file (or whatever else you wanna do)
                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    destination.XmlDocument.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    returnArgs.TransformedString = stringWriter.GetStringBuilder().ToString();
                }

                returnArgs.Successful = true;
            }
            catch (Exception ee)
            {
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = ee.Message;
                returnArgs.InnerException = ee;
            }

            // Determine the elapsed time, in milliseconds
            returnArgs.Milliseconds = DateTime.Now.Subtract(starTime).TotalMilliseconds;

            return returnArgs;
        }

        /// <summary> Use the native .NET library to transform a source XML file into an output file with the XSLT file </summary>
        /// <param name="SourceFile"> Source XML file, to be transformed </param>
        /// <param name="XSLT_File"> XSLT file to perform the transform </param>
        /// <param name="OutputFile"> Resulting output file to write </param>
        /// <returns> XSLT return arguments including a flag for success, and possibly an exception </returns>
        public static XSLT_Transformer_ReturnArgs Native_Transform(string SourceFile, string XSLT_File, string OutputFile)
        {
            // Keep track of the start time
            DateTime starTime = DateTime.Now;

            // Create the return object
            XSLT_Transformer_ReturnArgs returnArgs = new XSLT_Transformer_ReturnArgs();
            returnArgs.Engine = XSLT_Transformer_Engine_Enum.Native_dotNet;

            // Ensure the XSLT file exists
            if (!File.Exists(XSLT_File))
            {
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = "Indicated XSLT file ( " + XSLT_File + " ) does not exist.";
                return returnArgs;
            }

            // Ensure the source file exists
            if (!File.Exists(SourceFile))
            {
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = "Indicated source file ( " + SourceFile + " ) does not exist.";
                return returnArgs;
            }

            try
            {
                // Create the XsltSettings object with script enabled.
                XsltSettings xslt_settings = new XsltSettings(true, true);

                // Create the transform and load the XSL indicated
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XSLT_File, xslt_settings, new XmlUrlResolver());
                
                // Apply the transform to convert the XML into HTML
                StringWriter results = new StringWriter();
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse
                };
                using (XmlReader transformreader = XmlReader.Create(SourceFile, settings))
                {
                    transform.Transform(transformreader, null, results);
                }

                // Write the transformed string
                StreamWriter writer = new StreamWriter(OutputFile);
                writer.WriteLine(results.ToString());
                writer.Flush();
                writer.Close();

                returnArgs.Successful = true;
            }
            catch (Exception ee)
            {
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = ee.Message;
                returnArgs.InnerException = ee;
            }

            // Determine the elapsed time, in milliseconds
            returnArgs.Milliseconds = DateTime.Now.Subtract(starTime).TotalMilliseconds;

            return returnArgs;
        }

        /// <summary> Use the native .NET library to transform a source XML file with the XSLT file and return the string </summary>
        /// <param name="SourceFile"> Source XML file, to be transformed </param>
        /// <param name="XSLT_File"> XSLT file to perform the transform </param>
        /// <returns> XSLT return arguments including a flag for success, and possibly an exception </returns>
        public static XSLT_Transformer_ReturnArgs Native_Transform(string SourceFile, string XSLT_File)
        {
            // Keep track of the start time
            DateTime starTime = DateTime.Now;

            // Create the return object
            XSLT_Transformer_ReturnArgs returnArgs = new XSLT_Transformer_ReturnArgs();
            returnArgs.Engine = XSLT_Transformer_Engine_Enum.Native_dotNet;

            // Ensure the XSLT file exists
            if (!File.Exists(XSLT_File))
            {
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = "Indicated XSLT file ( " + XSLT_File + " ) does not exist.";
                return returnArgs;
            }

            // Ensure the source file exists
            if (!File.Exists(SourceFile))
            {
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = "Indicated source file ( " + SourceFile + " ) does not exist.";
                return returnArgs;
            }

            try
            {
                // Create the XsltSettings object with script enabled.
                XsltSettings xslt_settings = new XsltSettings(true, true);

                // Create the transform and load the XSL indicated
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XSLT_File, xslt_settings, new XmlUrlResolver());

                // Apply the transform to convert the XML into HTML
                StringWriter results = new StringWriter();
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse
                };
                using (XmlReader transformreader = XmlReader.Create(SourceFile, settings))
                {
                    transform.Transform(transformreader, null, results);
                }

                // Write the transformed string
                returnArgs.TransformedString = results.ToString();

                returnArgs.Successful = true;
            }
            catch (Exception ee)
            {
                returnArgs.Successful = false;
                returnArgs.ErrorMessage = ee.Message;
                returnArgs.InnerException = ee;
            }

            // Determine the elapsed time, in milliseconds
            returnArgs.Milliseconds = DateTime.Now.Subtract(starTime).TotalMilliseconds;

            return returnArgs;
        }
    }
}
