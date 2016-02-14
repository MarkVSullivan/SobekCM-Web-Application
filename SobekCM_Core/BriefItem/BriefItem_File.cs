#region Using directives

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.BriefItem
{
    /// <summary> Information about a single file within a digital resource </summary>
    [Serializable, DataContract, ProtoContract]
    public class BriefItem_File
    {
        /// <summary> Name for this file </summary>
        /// <remarks> If this is not in the resource folder, this may include a URL </remarks>
        [DataMember(Name = "name")]
        [XmlText]
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary> Width of the file (in pixels), if relevant and available </summary>
        /// <remarks> This could be the actual width, or the preferred viewing width </remarks>
        [DataMember(EmitDefaultValue = false, Name = "width")]
        [XmlIgnore]
        [ProtoMember(2)]
        public int? Width { get; set; }

        /// <summary> Width of the file (in pixels), if relevant and available </summary>
        /// <remarks> This is used for XML serialization primarily. &lt;br /&gt;
        /// This could be the actual width, or the preferred viewing width </remarks>
        [IgnoreDataMember]
        [XmlAttribute("width")]
        public string Width_AsString
        {
            get { return Width.HasValue ? Width.ToString() : null; }
            set
            {
                int temp;
                if (Int32.TryParse(value, out temp))
                    Width = temp;
            }
        }

        /// <summary> Height of the file (in pixels), if relevant and available </summary>
        /// <remarks> This could be the actual height, or the preferred viewing height </remarks>
        [DataMember(EmitDefaultValue = false, Name = "height")]
        [XmlIgnore]
        [ProtoMember(3)]
        public int? Height { get; set; }

        /// <summary> Height of the file (in pixels), if relevant and available </summary>
        /// <remarks> This is used for XML serialization primarily. &lt;br /&gt;
        /// This could be the actual height, or the preferred viewing height </remarks>
        [IgnoreDataMember]
        [XmlAttribute("height")]
        public string Height_AsString
        {
            get { return Height.HasValue ? Height.ToString() : null; }
            set
            {
                int temp;
                if (Int32.TryParse(value, out temp))
                    Height = temp;
            }
        }

        /// <summary> Other attributes associated with this file, that may be needed for display purposes </summary>
        [DataMember(EmitDefaultValue = false, Name = "attributes")]
        [XmlAttribute("attributes")]
        [ProtoMember(4)]
        public string Attributes { get; set; }

        /// <summary> File extension, extrapolated from the file name </summary>
        [XmlIgnore]
        public string File_Extension
        {
            get { return Path.GetExtension(Name) ?? String.Empty; }
        }
        
        /// <summary> Constructor for a new instance of the BriefItem_File class </summary>
        public BriefItem_File()
        {
            // Does nothing - needed for deserialization
        }

        /// <summary> Constructor for a new instance of the BriefItem_File class </summary>
        /// <param name="Name"> Name for this file </param>
        public BriefItem_File( string Name )
        {
            this.Name = Name;
        }


        #region Section to compute the MIME TYPE from the extension

        /// <summary> Gets the MIME Type of this file </summary>
        /// <remarks> This is computed from the provided System_Name's final extension </remarks>
        public string MIME_Type
        {
            get
            {
                string fileExt = File_Extension.Replace(".", "").ToUpper();

                if (fileExt.Length == 0)
                    return String.Empty;

                string resourceType = String.Empty;

                // Handle the most common cases first, to avoid the long switch/case for speed optimization
                if ((fileExt == "TIF") || (fileExt == "TIFF"))
                    return "image/tiff";

                if ((fileExt == "JPG") || (fileExt == "JPEG"))
                    return "image/jpeg";

                if ((fileExt == "JP2") || (fileExt == "JPEG2") || (fileExt == "JPX"))
                    return "image/jp2";

                if ((fileExt == "TXT") || (fileExt == "TEXT"))
                    return "text/plain";

                if (fileExt == "PRO")
                    return "text/x-pro";

                switch (fileExt)
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
                        return resourceType.ToUpper().IndexOf("VIDEO") < 0 ? "audio/mpeg" : "video/mpeg";
                    case "MP3":
                        return resourceType.ToUpper().IndexOf("VIDEO") < 0 ? "audio/mpeg3" : "video/mpeg";
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
                        return "unknown/x-" + fileExt.ToLower();
                }
            }
        }

        #endregion

    }
}
