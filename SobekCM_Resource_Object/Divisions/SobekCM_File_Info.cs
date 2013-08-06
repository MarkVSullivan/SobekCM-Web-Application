#region Using directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using SobekCM.Resource_Object.Behaviors;

#endregion

namespace SobekCM.Resource_Object.Divisions
{
    /// <summary> Enumeration is used to keep track of what type of file reference this is (i.e., 
    /// referenced on the same file system or a reference to an external URL ) </summary>
    public enum SobekCM_File_Info_Type_Enum : byte
    {
        /// <summary> File is referenced as a file on the system (or network) file structure </summary>
        SYSTEM = 0,

        /// <summary> File is referenced as an external URL </summary>
        URL
    }

    /// <summary> Stores the information about single file associated with a digital resource </summary>
    /// <remarks>Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class SobekCM_File_Info : MetadataDescribableBase
    {
        private string checksum, checksumtype;
        private SobekCM_File_Info_Type_Enum filetype;
        private ushort height;
        private long size;
        private string systemname;
        private ushort width;
        private string id, group_number;

        #region Constructors

        /// <summary> Constructor creates an empty instance of the SobekCM_File_Info class </summary>
        public SobekCM_File_Info()
        {
            // Set integers to default of -1
            size = -1;
            width = 0;
            height = 0;
            filetype = SobekCM_File_Info_Type_Enum.SYSTEM;
        }

        /// <summary> Constructor creates a new instance of the SobekCM_File_Info class </summary>
        /// <param name="System_Name">System name for the new file</param>
        public SobekCM_File_Info(string System_Name)
        {
            // Save the parameters
            this.System_Name = System_Name;

            // Set integers to default of -1
            size = -1;
            width = 0;
            height = 0;
        }

        /// <summary> Constructor creates a new instance of the SobekCM_File_Info class </summary>
        /// <param name="System_Name">System name for the new file</param>
        /// <param name="Width"> Width of this image file </param>
        /// <param name="Height"> Height of this image file </param>
        public SobekCM_File_Info(string System_Name, ushort Width, ushort Height)
        {
            // Save the parameters
            this.System_Name = System_Name;
            size = -1;
            width = Width;
            height = Height;
        }

        #endregion

        #region Simple Public Properties and Property-Setting Methods

        /// <summary> Gets and sets the ID of this file within the context of a larger METS file </summary>
        /// <remarks> This is not READ or used except during the METS writing process </remarks>
        internal string ID
        {
            get { return id ?? String.Empty; }
            set { id = value; }
        }

        /// <summary> Gets and sets the Group Number of this file within the context of a larger METS file </summary>
        /// <remarks> This is not READ or used except during the METS writing process </remarks>
        internal string Group_Number
        {
            get { return group_number ?? String.Empty; }
            set { group_number = value; }
        }

        /// <summary> Gets or sets the width of this file, if this is an image file </summary>
        public ushort Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary> Gets or sets the height of this file, if this is an image file </summary>
        public ushort Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary> Gets or sets the size of this file </summary>
        public long Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary> Gets or sets the system name of this file </summary>
        public string System_Name
        {
            get { return systemname ?? String.Empty; }
            set
            {
                systemname = value;
                if (systemname.IndexOf("http://", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    filetype = SobekCM_File_Info_Type_Enum.URL;
                }
                else
                {
                    filetype = SobekCM_File_Info_Type_Enum.SYSTEM;
                }
            }
        }

        /// <summary> Gets how this file is referenced in the METS file (i.e., 
        /// referenced on the same file system or a reference to an external URL ) </summary>
        public SobekCM_File_Info_Type_Enum METS_LocType
        {
            get { return filetype; }
        }

        /// <summary> Gets or sets the checksum assoicated with this file </summary>
        /// <remarks>Used to verify FTP was successful and complete</remarks>
        public string Checksum
        {
            get { return checksum ?? String.Empty; }
            set { checksum = value; }
        }

        /// <summary> Gets or sets the checksum type for the checksum associated with this file </summary>
        public string Checksum_Type
        {
            get { return checksumtype ?? String.Empty; }
            set { checksumtype = value; }
        }

        /// <summary> Gets the last extension for this file </summary>
        /// <remarks> This is computed from the System_Name and is always returned upper case </remarks>
        public string File_Extension
        {
            get
            {
                try
                {

                    if (String.IsNullOrEmpty(systemname))
                        return string.Empty;
                    int last_slash_index = Math.Max(systemname.LastIndexOfAny("\\/".ToCharArray()), 0);
                    int last_period_index = systemname.LastIndexOf('.', systemname.Length - 1, systemname.Length - last_slash_index);
                    if (last_period_index < 0)
                        return String.Empty;
                    if (last_period_index + 1 >= systemname.Length)
                        return String.Empty;
                    return systemname.Substring(last_period_index + 1).ToUpper();
                }
                catch (Exception ee)
                {
                    throw ee;
                }
            }
        }

        /// <summary> Gets the name of this file, without any extensions </summary>
        /// <remarks> This is computed from the System_Name and is always returned upper case </remarks>
        public string File_Name_Sans_Extension
        {
            get
            {
                if (String.IsNullOrEmpty(systemname))
                    return string.Empty;

                string name_upper = systemname.ToUpper();

                // If this is a URL type file, don't keep the directory / subdirectory information here
                if (METS_LocType == SobekCM_File_Info_Type_Enum.URL)
                {
                    string[] split = name_upper.Split("\\/".ToCharArray());
                    name_upper = split[split.Length - 1];
                }


                int last_slash_index = Math.Max(name_upper.LastIndexOfAny("\\/".ToCharArray()), 0);
                int first_period_index = name_upper.IndexOf('.', last_slash_index);

                if (first_period_index < 0)
                    return name_upper;
                if (first_period_index + 1 >= name_upper.Length)
                    return name_upper.Substring(0, systemname.Length - 1);

                if ((name_upper.IndexOf("THM.GIF") > 0) || (name_upper.IndexOf("THM.JPG") > 0))
                    return name_upper.Replace("THM.GIF", "").Replace("THM.JPG", "");
                return name_upper.Substring(0, first_period_index);
            }
        }

        /// <summary> Sets the technical details for this image file </summary>
        /// <param name="New_Width"> Width of this image file </param>
        /// <param name="New_Height"> Height of this image file </param>
        public void Set_Technical_Details(ushort New_Width, ushort New_Height)
        {
            width = New_Width;
            height = New_Height;
        }

        #endregion

        /// <summary> Gets the view object related to this file type, or NULL </summary>
        /// <returns> SobekCM view object, or NULL </returns>
        public View_Object Get_Viewer()
        {
            string mimetype = MIME_Type(File_Extension);

            if (String.IsNullOrEmpty(mimetype))
                return null;

            switch (mimetype)
            {
                case "text/plain":
                    return new View_Object(View_Enum.TEXT, "Text", string.Empty, System_Name);

                case "image/jpeg":
                    // Special code to exclude thumbnails from the jpegs that
                    // get an item viewer
                    if (System_Name.ToLower().IndexOf("thm.jpg") > 0)
                        return null;
                    return new View_Object(View_Enum.JPEG, "Page Image", "WIDTH=" + width + ";HEIGHT=" + height, System_Name);

                case "image/jp2":
                    return new View_Object(View_Enum.JPEG2000, "Zoomable Image", "WIDTH=" + width + ";HEIGHT=" + height, System_Name); //Service_Copy);
            }

            return null;
        }

        #region Section to compute the MIME TYPE from the extension

        /// <summary> Gets the MIME Type of this file </summary>
        /// <param name="Extenstion"> File extension </param>
        /// <remarks> This is computed from the provided System_Name's final extension </remarks>
        public string MIME_Type(string Extenstion)
        {
            if (Extenstion.Length == 0)
                return String.Empty;

            string Resource_Type = String.Empty;

            // Handle the most common cases first, to avoid the long switch/case for speed optimization
            if ((Extenstion == "TIF") || (Extenstion == "TIFF"))
                return "image/tiff";

            if ((Extenstion == "JPG") || (Extenstion == "JPEG"))
                return "image/jpeg";

            if ((Extenstion == "JP2") || (Extenstion == "JPEG2") || (Extenstion == "JPX"))
                return "image/jp2";

            if ((Extenstion == "TXT") || (Extenstion == "TEXT"))
                return "text/plain";

            if (Extenstion == "PRO")
                return "text/x-pro";

            switch (Extenstion)
            {
                case "3DM":
                    return "x-world/x-3dmf";
                case "3DMF":
                    return "x-world/x-3dmf";
                case "A":
                    return "application/octet-stream";
                case "AAB":
                    return "application/x-authorware-bin";
                case "AAM":
                    return "application/x-authorware-map";
                case "AAS":
                    return "application/x-authorware-seg";
                case "ABC":
                    return "text/vnd.abc";
                case "ACGI":
                    return "text/html";
                case "AFL":
                    return "video/animaflex";
                case "AI":
                    return "application/postscript";
                case "AIF":
                    return "audio/aiff";
                case "AIFC":
                    return "audio/aiff";
                case "AIFF":
                    return "audio/aiff";
                case "AIM":
                    return "application/x-aim";
                case "AIP":
                    return "text/x-audiosoft-intra";
                case "ANI":
                    return "application/x-navi-animation";
                case "AOS":
                    return "application/x-nokia-9000-communicator-add-on-software";
                case "APS":
                    return "application/mime";
                case "ARC":
                    return "application/octet-stream";
                case "ARJ":
                    return "application/arj";
                case "ART":
                    return "image/x-jg";
                case "ASF":
                    return "video/x-ms-asf";
                case "ASM":
                    return "text/x-asm";
                case "ASP":
                    return "text/asp";
                case "ASPX":
                    return "text/aspx";
                case "ASX":
                    return "application/x-mplayer2";
                case "AU":
                    return "audio/basic";
                case "AVI":
                    return "video/avi";
                case "AVS":
                    return "video/avs-video";
                case "BCPIO":
                    return "application/x-bcpio";
                case "BIN":
                    return "application/octet-stream";
                case "BM":
                    return "image/bmp";
                case "BMP":
                    return "image/bmp";
                case "BOO":
                    return "application/book";
                case "BOOK":
                    return "application/book";
                case "BOZ":
                    return "application/x-bzip2";
                case "BSH":
                    return "application/x-bsh";
                case "BZ":
                    return "application/x-bzip";
                case "BZ2":
                    return "application/x-bzip2";
                case "C":
                    return "text/x-c";
                case "C++":
                    return "text/x-c";
                case "CAT":
                    return "application/vnd.ms-pki.seccat";
                case "CC":
                    return "text/plain";
                case "CCAD":
                    return "application/clariscad";
                case "CCO":
                    return "application/x-cocoa";
                case "CDF":
                    return "application/cdf";
                case "CER":
                    return "application/x-x509-ca-cert";
                case "CHA":
                    return "application/x-chat";
                case "CHAT":
                    return "application/x-chat";
                case "CLASS":
                    return "application/java";
                case "COM":
                    return "application/octet-stream";
                case "CONF":
                    return "text/plain";
                case "CPIO":
                    return "application/x-cpio";
                case "CPP":
                    return "text/x-c";
                case "CPT":
                    return "application/x-compactpro";
                case "CRL":
                    return "application/pkcs-crl";
                case "CRT":
                    return "application/x-x509-ca-cert";
                case "CSH":
                    return "text/x-script.csh";
                case "CS":
                    return "text/x-csharp-source";
                case "CSS":
                    return "text/css";
                case "CXX":
                    return "text/plain";
                case "DCR":
                    return "application/x-director";
                case "DEEPV":
                    return "application/x-deepv";
                case "DEF":
                    return "text/plain";
                case "DER":
                    return "application/x-x509-ca-cert";
                case "DIF":
                    return "video/x-dv";
                case "DIR":
                    return "application/x-director";
                case "DL":
                    return "video/dl";
                case "DOC":
                    return "application/msword";
                case "DOT":
                    return "application/msword";
                case "DP":
                    return "application/commonground";
                case "DRW":
                    return "application/drafting";
                case "DUMP":
                    return "application/octet-stream";
                case "DV":
                    return "video/x-dv";
                case "DVI":
                    return "application/x-dvi";
                case "DWF":
                    return "model/vnd.dwf";
                case "DWG":
                    return "image/vnd.dwg";
                case "DXF":
                    return "application/dxf";
                case "DXR":
                    return "application/x-director";
                case "EL":
                    return "text/x-script.elisp";
                case "ELC":
                    return "application/x-elc";
                case "ENV":
                    return "application/x-envoy";
                case "EPS":
                    return "application/postscript";
                case "ES":
                    return "application/x-esrehber";
                case "ETX":
                    return "text/x-setext";
                case "EVY":
                    return "application/envoy";
                case "EXE":
                    return "application/octet-stream";
                case "F":
                    return "text/x-fortran";
                case "F77":
                    return "text/x-fortran";
                case "F90":
                    return "text/x-fortran";
                case "FDF":
                    return "application/vnd.fdf";
                case "FIF":
                    return "application/fractals";
                case "FLI":
                    return "video/fli";
                case "FLO":
                    return "image/florian";
                case "FLX":
                    return "text/vnd.fmi.flexstor";
                case "FMF":
                    return "video/x-atomic3d-feature";
                case "FOR":
                    return "text/x-fortran";
                case "FPX":
                    return "image/vnd.fpx";
                case "FRL":
                    return "application/freeloader";
                case "FUNK":
                    return "audio/make";
                case "G":
                    return "text/plain";
                case "G3":
                    return "image/g3fax";
                case "GIF":
                    return "image/gif";
                case "GL":
                    return "video/gl";
                case "GSD":
                    return "audio/x-gsm";
                case "GSM":
                    return "audio/x-gsm";
                case "GSP":
                    return "application/x-gsp";
                case "GSS":
                    return "application/x-gss";
                case "GTAR":
                    return "application/x-gtar";
                case "GZ":
                    return "application/x-gzip";
                case "GZIP":
                    return "application/x-gzip";
                case "H":
                    return "text/x-h";
                case "HDF":
                    return "application/x-hdf";
                case "HELP":
                    return "application/x-helpfile";
                case "HGL":
                    return "application/vnd.hp-hpgl";
                case "HH":
                    return "text/plain";
                case "HLB":
                    return "text/x-script";
                case "HLP":
                    return "application/hlp";
                case "HPG":
                    return "application/vnd.hp-hpgl";
                case "HPGL":
                    return "application/vnd.hp-hpgl";
                case "HQX":
                    return "application/binhex";
                case "HTA":
                    return "application/hta";
                case "HTC":
                    return "text/x-component";
                case "HTM":
                    return "text/html";
                case "HTML":
                    return "text/html";
                case "HTMLS":
                    return "text/html";
                case "HTT":
                    return "text/webviewhtml";
                case "HTX":
                    return "text/html";
                case "ICE":
                    return "x-conference/x-cooltalk";
                case "ICO":
                    return "image/x-icon";
                case "IDC":
                    return "text/plain";
                case "IEF":
                    return "image/ief";
                case "IEFS":
                    return "image/ief";
                case "IGES":
                    return "model/iges";
                case "IGS":
                    return "model/iges";
                case "IMA":
                    return "application/x-ima";
                case "IMAP":
                    return "application/x-httpd-imap";
                case "INF":
                    return "application/inf";
                case "INS":
                    return "application/x-internett-signup";
                case "IP":
                    return "application/x-ip2";
                case "ISU":
                    return "video/x-isvideo";
                case "IT":
                    return "audio/it";
                case "IV":
                    return "application/x-inventor";
                case "IVR":
                    return "i-world/i-vrml";
                case "IVY":
                    return "application/x-livescreen";
                case "JAM":
                    return "audio/x-jam";
                case "JAV":
                    return "text/x-java-source";
                case "JAVA":
                    return "text/x-java-source";
                case "JCM":
                    return "application/x-java-commerce";
                case "JFIF":
                    return "image/jpeg";
                case "JFIF-TBNL":
                    return "image/jpeg";
                case "JPE":
                    return "image/jpeg";
                case "JPEG":
                    return "image/jpeg";
                case "JPG":
                    return "image/jpeg";
                case "JPS":
                    return "image/x-jps";
                case "JS":
                    return "application/x-javascript";
                case "JUT":
                    return "image/jutvision";
                case "KAR":
                    return "audio/midi";
                case "KSH":
                    return "application/x-ksh";
                case "LA":
                    return "audio/nspaudio";
                case "LAM":
                    return "audio/x-liveaudio";
                case "LATEX":
                    return "application/x-latex";
                case "LHA":
                    return "application/octet-stream";
                case "LHX":
                    return "application/octet-stream";
                case "LIST":
                    return "text/plain";
                case "LMA":
                    return "audio/nspaudio";
                case "LOG":
                    return "text/plain";
                case "LSP":
                    return "application/x-lisp";
                case "LST":
                    return "text/plain";
                case "LTX":
                    return "application/x-latex";
                case "LZH":
                    return "application/x-lzh";
                case "LZX":
                    return "application/lzx";
                case "M":
                    return "text/plain";
                case "M1V":
                    return "video/mpeg";
                case "M2A":
                    return "audio/mpeg";
                case "M2V":
                    return "video/mpeg";
                case "M3U":
                    return "audio/x-mpequrl";
                case "MAN":
                    return "application/x-troff-man";
                case "MAP":
                    return "application/x-navimap";
                case "MAR":
                    return "text/plain";
                case "MBD":
                    return "application/mbedlet";
                case "MC$":
                    return "application/x-magic-cap-package-1.0";
                case "MCD":
                    return "application/mcad";
                case "MCF":
                    return "text/mcf";
                case "MCP":
                    return "application/netmc";
                case "ME":
                    return "application/x-troff-me";
                case "MHT":
                    return "message/rfc822";
                case "MHTML":
                    return "message/rfc822";
                case "MID":
                    return "audio/midi";
                case "MIDI":
                    return "audio/midi";
                case "MIF":
                    return "application/x-mif";
                case "MIME":
                    return "message/rfc822";
                case "MJF":
                    return "audio/x-vnd.audioexplosion.mjuicemediafile";
                case "MJPG":
                    return "video/x-motion-jpeg";
                case "MM":
                    return "application/x-meme";
                case "MME":
                    return "application/base64";
                case "MOD":
                    return "audio/mod";
                case "MOOV":
                    return "video/quicktime";
                case "MOV":
                    return "video/quicktime";
                case "MOVIE":
                    return "video/x-sgi-movie";
                case "MP2":
                case "MPG":
                case "MPA":
                    return Resource_Type.ToUpper().IndexOf("VIDEO") < 0 ? "audio/mpeg" : "video/mpeg";
                case "MP3":
                    return Resource_Type.ToUpper().IndexOf("VIDEO") < 0 ? "audio/mpeg3" : "video/mpeg";
                case "MPC":
                    return "application/x-project";
                case "MPE":
                    return "video/mpeg";
                case "MPEG":
                    return "video/mpeg";
                case "MPGA":
                    return "audio/mpeg";
                case "MPP":
                    return "application/vnd.ms-project";
                case "MPT":
                    return "application/x-project";
                case "MPV":
                    return "application/x-project";
                case "MPX":
                    return "application/x-project";
                case "MRC":
                    return "application/marc";
                case "MS":
                    return "application/x-troff-ms";
                case "MV":
                    return "video/x-sgi-movie";
                case "MY":
                    return "audio/make";
                case "MZZ":
                    return "application/x-vnd.audioexplosion.mzz";
                case "NAP":
                    return "image/naplps";
                case "NAPLPS":
                    return "image/naplps";
                case "NC":
                    return "application/x-netcdf";
                case "NCM":
                    return "application/vnd.nokia.configuration-message";
                case "NIF":
                    return "image/x-niff";
                case "NIFF":
                    return "image/x-niff";
                case "NIX":
                    return "application/x-mix-transfer";
                case "NSC":
                    return "application/x-conference";
                case "NVD":
                    return "application/x-navidoc";
                case "O":
                    return "application/octet-stream";
                case "ODA":
                    return "application/oda";
                case "OMC":
                    return "application/x-omc";
                case "OMCD":
                    return "application/x-omcdatamaker";
                case "OMCR":
                    return "application/x-omcregerator";
                case "P":
                    return "text/x-pascal";
                case "P10":
                    return "application/pkcs10";
                case "P12":
                    return "application/pkcs-12";
                case "P7A":
                    return "application/x-pkcs7-signature";
                case "P7C":
                    return "application/pkcs7-mime";
                case "P7M":
                    return "application/pkcs7-mime";
                case "P7R":
                    return "application/x-pkcs7-certreqresp";
                case "P7S":
                    return "application/pkcs7-signature";
                case "PART":
                    return "application/pro_eng";
                case "PAS":
                    return "text/pascal";
                case "PBM":
                    return "image/x-portable-bitmap";
                case "PCL":
                    return "application/x-pcl";
                case "PCT":
                    return "image/x-pict";
                case "PCX":
                    return "image/x-pcx";
                case "PDB":
                    return "chemical/x-pdb";
                case "PDF":
                    return "application/pdf";
                case "PFUNK":
                    return "audio/make";
                case "PGM":
                    return "image/x-portable-graymap";
                case "PIC":
                    return "image/pict";
                case "PICT":
                    return "image/pict";
                case "PKG":
                    return "application/x-newton-compatible-pkg";
                case "PKO":
                    return "application/vnd.ms-pki.pko";
                case "PL":
                    return "text/x-script.perl";
                case "PLX":
                    return "application/x-pixclscript";
                case "PM":
                    return "text/x-script.perl-module";
                case "PM4":
                    return "application/x-pagemaker";
                case "PM5":
                    return "application/x-pagemaker";
                case "PNG":
                    return "image/png";
                case "PNM":
                    return "application/x-portable-anymap";
                case "POT":
                    return "application/mspowerpoint";
                case "POV":
                    return "model/x-pov";
                case "PPA":
                    return "application/vnd.ms-powerpoint";
                case "PPM":
                    return "image/x-portable-pixmap";
                case "PPS":
                    return "application/mspowerpoint";
                case "PPT":
                    return "application/mspowerpoint";
                case "PPZ":
                    return "application/mspowerpoint";
                case "PRE":
                    return "application/x-freelance";
                case "PRT":
                    return "application/pro_eng";
                case "PS":
                    return "application/postscript";
                case "PSD":
                    return "application/octet-stream";
                case "PVU":
                    return "paleovu/x-pv";
                case "PWZ":
                    return "application/vnd.ms-powerpoint";
                case "PY":
                    return "text/x-script.phyton";
                case "PYC":
                    return "applicaiton/x-bytecode.python";
                case "QCP":
                    return "audio/vnd.qcelp";
                case "QD3":
                    return "x-world/x-3dmf";
                case "QD3D":
                    return "x-world/x-3dmf";
                case "QIF":
                    return "image/x-quicktime";
                case "QT":
                    return "video/quicktime";
                case "QTC":
                    return "video/x-qtc";
                case "QTI":
                    return "image/x-quicktime";
                case "QTIF":
                    return "image/x-quicktime";
                case "RA":
                    return "audio/x-realaudio";
                case "RAM":
                    return "audio/x-pn-realaudio";
                case "RAS":
                    return "image/cmu-raster";
                case "RAST":
                    return "image/cmu-raster";
                case "REXX":
                    return "text/x-script.rexx";
                case "RF":
                    return "image/vnd.rn-realflash";
                case "RGB":
                    return "image/x-rgb";
                case "RM":
                    return "audio/x-pn-realaudio";
                case "RMI":
                    return "audio/mid";
                case "RMM":
                    return "audio/x-pn-realaudio";
                case "RMP":
                    return "audio/x-pn-realaudio";
                case "RNG":
                    return "application/ringing-tones";
                case "RNX":
                    return "application/vnd.rn-realplayer";
                case "ROFF":
                    return "application/x-troff";
                case "RP":
                    return "image/vnd.rn-realpix";
                case "RPM":
                    return "audio/x-pn-realaudio-plugin";
                case "RT":
                    return "text/richtext";
                case "RTF":
                    return "text/richtext";
                case "RTX":
                    return "text/richtext";
                case "RV":
                    return "video/vnd.rn-realvideo";
                case "S":
                    return "text/x-asm";
                case "S3M":
                    return "audio/s3m";
                case "SAVEME":
                    return "application/octet-stream";
                case "SBK":
                    return "application/x-tbook";
                case "SCM":
                    return "text/x-script.scheme";
                case "SDML":
                    return "text/plain";
                case "SDP":
                    return "application/sdp";
                case "SDR":
                    return "application/sounder";
                case "SEA":
                    return "application/sea";
                case "SET":
                    return "application/set";
                case "SGM":
                    return "text/sgml";
                case "SGML":
                    return "text/sgml";
                case "SH":
                    return "text/x-script.sh";
                case "SHAR":
                    return "application/x-shar";
                case "SHTML":
                    return "text/x-server-parsed-html";
                case "SID":
                    return "audio/x-psid";
                case "SIT":
                    return "application/x-stuffit";
                case "SKD":
                    return "application/x-koan";
                case "SKM":
                    return "application/x-koan";
                case "SKP":
                    return "application/x-koan";
                case "SKT":
                    return "application/x-koan";
                case "SL":
                    return "application/x-seelogo";
                case "SMI":
                    return "application/smil";
                case "SMIL":
                    return "application/smil";
                case "SND":
                    return "audio/basic";
                case "SOL":
                    return "application/solids";
                case "SPC":
                    return "application/x-pkcs7-certificates";
                case "SPL":
                    return "application/futuresplash";
                case "SPR":
                    return "application/x-sprite";
                case "SPRITE":
                    return "application/x-sprite";
                case "SRC":
                    return "application/x-wais-source";
                case "SSI":
                    return "text/x-server-parsed-html";
                case "SSM":
                    return "application/streamingmedia";
                case "SST":
                    return "application/vnd.ms-pki.certstore";
                case "STEP":
                    return "application/step";
                case "STL":
                    return "application/sla";
                case "STP":
                    return "application/step";
                case "SV4CPIO":
                    return "application/x-sv4cpio";
                case "SV4CRC":
                    return "application/x-sv4crc";
                case "SVF":
                    return "image/vnd.dwg";
                case "SVR":
                    return "application/x-world";
                case "SWF":
                    return "application/x-shockwave-flash";
                case "T":
                    return "application/x-troff";
                case "TALK":
                    return "text/x-speech";
                case "TAR":
                    return "application/x-tar";
                case "TBK":
                    return "application/toolbook";
                case "TCL":
                    return "application/x-tcl";
                case "TCSH":
                    return "text/x-script.tcsh";
                case "TEX":
                    return "application/x-tex";
                case "TEXI":
                    return "application/x-texinfo";
                case "TEXINFO":
                    return "application/x-texinfo";
                case "TEXT":
                    return "text/plain";
                case "TGZ":
                    return "application/gnutar";
                case "TIF":
                    return "image/tiff";
                case "TIFF":
                    return "image/tiff";
                case "TR":
                    return "application/x-troff";
                case "TSI":
                    return "audio/tsp-audio";
                case "TSP":
                    return "audio/tsplayer";
                case "TSV":
                    return "text/tab-separated-values";
                case "TURBOT":
                    return "image/florian";
                case "TXT":
                    return "text/plain";
                case "UIL":
                    return "text/x-uil";
                case "UNI":
                    return "text/uri-list";
                case "UNIS":
                    return "text/uri-list";
                case "UNV":
                    return "application/i-deas";
                case "URI":
                    return "text/uri-list";
                case "URIS":
                    return "text/uri-list";
                case "USTAR":
                    return "application/x-ustar";
                case "UU":
                    return "text/x-uuencode";
                case "UUE":
                    return "text/x-uuencode";
                case "VCD":
                    return "application/x-cdlink";
                case "VCS":
                    return "text/x-vcalendar";
                case "VDA":
                    return "application/vda";
                case "VDO":
                    return "video/vdo";
                case "VEW":
                    return "application/groupwise";
                case "VIV":
                    return "video/vivo";
                case "VIVO":
                    return "video/vivo";
                case "VMD":
                    return "application/vocaltec-media-desc";
                case "VMF":
                    return "application/vocaltec-media-file";
                case "VOC":
                    return "audio/voc";
                case "VOS":
                    return "video/vosaic";
                case "VOX":
                    return "audio/voxware";
                case "VQE":
                    return "audio/x-twinvq-plugin";
                case "VQF":
                    return "audio/x-twinvq";
                case "VQL":
                    return "audio/x-twinvq-plugin";
                case "VRML":
                    return "model/vrml";
                case "VRT":
                    return "x-world/x-vrt";
                case "VSD":
                    return "application/x-visio";
                case "VST":
                    return "application/x-visio";
                case "VSW":
                    return "application/x-visio";
                case "W60":
                    return "application/wordperfect6.0";
                case "W61":
                    return "application/wordperfect6.1";
                case "W6W":
                    return "application/msword";
                case "WAV":
                    return "audio/wav";
                case "WB1":
                    return "application/x-qpro";
                case "WBMP":
                    return "image/vnd.wap.wbmp";
                case "WEB":
                    return "application/vnd.xara";
                case "WIZ":
                    return "application/msword";
                case "WK1":
                    return "application/x-123";
                case "WMF":
                    return "windows/metafile";
                case "WML":
                    return "text/vnd.wap.wml";
                case "WMLC":
                    return "application/vnd.wap.wmlc";
                case "WMLS":
                    return "text/vnd.wap.wmlscript";
                case "WMLSC":
                    return "application/vnd.wap.wmlscriptc";
                case "WORD":
                    return "application/msword";
                case "WP":
                    return "application/wordperfect";
                case "WP5":
                    return "application/wordperfect";
                case "WP6":
                    return "application/wordperfect";
                case "WPD":
                    return "application/wordperfect";
                case "WQ1":
                    return "application/x-lotus";
                case "WRI":
                    return "application/mswrite";
                case "WRL":
                    return "model/vrml";
                case "WRZ":
                    return "model/vrml";
                case "WSC":
                    return "text/scriplet";
                case "WSRC":
                    return "application/x-wais-source";
                case "WTK":
                    return "application/x-wintalk";
                case "XBM":
                    return "image/xbm";
                case "XDR":
                    return "video/x-amt-demorun";
                case "XGZ":
                    return "xgl/drawing";
                case "XIF":
                    return "image/vnd.xiff";
                case "XL":
                    return "application/excel";
                case "XLA":
                    return "application/excel";
                case "XLB":
                    return "application/excel";
                case "XLC":
                    return "application/excel";
                case "XLD":
                    return "application/excel";
                case "XLK":
                    return "application/excel";
                case "XLL":
                    return "application/excel";
                case "XLM":
                    return "application/excel";
                case "XLS":
                    return "application/excel";
                case "XLT":
                    return "application/excel";
                case "XLV":
                    return "application/excel";
                case "XLW":
                    return "application/excel";
                case "XM":
                    return "audio/xm";
                case "XML":
                    return "text/xml";
                case "XMZ":
                    return "xgl/movie";
                case "XPIX":
                    return "application/x-vnd.ls-xpix";
                case "XPM":
                    return "image/xpm";
                case "X-PNG":
                    return "image/png";
                case "XSR":
                    return "video/x-amt-showrun";
                case "XWD":
                    return "image/x-xwindowdump";
                case "XYZ":
                    return "chemical/x-pdb";
                case "Z":
                    return "application/x-compressed";
                case "ZIP":
                    return "application/zip";
                case "ZOO":
                    return "application/octet-stream";
                case "ZSH":
                    return "text/x-script.zsh";
                case "XLSX":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "XLTX":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                case "POTX":
                    return "application/vnd.openxmlformats-officedocument.presentationml.template";
                case "PPSX":
                    return "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
                case "PPTX":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case "SLDX":
                    return "application/vnd.openxmlformats-officedocument.presentationml.slide";
                case "DOCX":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case "DOTX":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                case "XLAM":
                    return "application/vnd.ms-excel.addin.macroEnabled.12";
                case "XLSB":
                    return "application/vnd.ms-excel.sheet.binary.macroEnabled.12";


                default:
                    return "unknown/x-" + Extenstion.ToLower();
            }
        }

        #endregion

        #region Code to compute the technical specifications for this file

        /// <summary> Determine the width and height of a JPEG file by reading the file into a temporary <see cref="System.Drawing.Bitmap" /> object.  </summary>
        /// <param name="File_Location"> Current location for this file </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        /// <remarks> After this method is run, the width and height should now be available by calling the 
        /// respective properties on this file object </remarks>
        public bool Compute_Jpeg_Attributes(string File_Location)
        {
            // Does this file exist?
            if ((filetype == SobekCM_File_Info_Type_Enum.SYSTEM) && (File.Exists(File_Location + "/" + System_Name)))
            {
                try
                {
                    // Get the height and width of this JPEG file
                    Bitmap image = (Bitmap) Image.FromFile(File_Location + "/" + System_Name);
                    Width = (ushort) image.Width;
                    Height = (ushort) image.Height;
                    image.Dispose();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary> Determine the width and height of a JPEG2000 file by reading the file into a temporary <see cref="System.Drawing.Bitmap" /> object.  </summary>
        /// <param name="File_Location"> Current location for this file </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        /// <remarks> After this method is run, the width and height should now be available by calling the 
        /// respective properties on this file object </remarks>
        public bool Compute_Jpeg2000_Attributes(string File_Location)
        {
            // Does this file exist?
            if ((filetype == SobekCM_File_Info_Type_Enum.SYSTEM) && (File.Exists(File_Location + "/" + System_Name)))
            {
                return get_attributes_from_jpeg2000(File_Location + "/" + System_Name);
            }

            return false;
        }

        private bool get_attributes_from_jpeg2000(string file)
        {
            try
            {
                // Get the height and width of this JPEG file
                FileStream reader = new FileStream(file, FileMode.Open, FileAccess.Read);
                int[] previousValues = new int[] {0, 0, 0, 0};
                int bytevalue = reader.ReadByte();
                int count = 1;
                while (bytevalue != -1)
                {
                    // Move this value into the array
                    previousValues[0] = previousValues[1];
                    previousValues[1] = previousValues[2];
                    previousValues[2] = previousValues[3];
                    previousValues[3] = bytevalue;

                    // Is this IHDR?
                    if ((previousValues[0] == 105) && (previousValues[1] == 104) &&
                        (previousValues[2] == 100) && (previousValues[3] == 114))
                    {
                        break;
                    }
                    else
                    {
                        // Is this the first four bytes and does it match the output from Kakadu 3-2?
                        if ((count == 4) && (previousValues[0] == 255) && (previousValues[1] == 79) &&
                            (previousValues[2] == 255) && (previousValues[3] == 81))
                        {
                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadByte();
                            break;
                        }
                        else
                        {
                            // Read the next byte
                            bytevalue = reader.ReadByte();
                            count++;
                        }
                    }
                }

                // Now, read ahead for the height and width
                Height = (ushort) ((((((reader.ReadByte()*256) + reader.ReadByte())*256) + reader.ReadByte())*256) + reader.ReadByte());
                Width = (ushort) ((((((reader.ReadByte()*256) + reader.ReadByte())*256) + reader.ReadByte())*256) + reader.ReadByte());
                reader.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}