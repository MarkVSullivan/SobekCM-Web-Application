#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Enumeration of the different resource types generally allowed by MODS </summary>
    public enum TypeOfResource_MODS_Enum : byte
    {
        UNKNOWN,
        Text,
        Cartographic,
        Notated_Music,
        Sound_Recording,
        Sound_Recording_Musical,
        Sound_Recording_Nonmusical,
        Still_Image,
        Moving_Image,
        Three_Dimensional_Object,
        Sofware_Multimedia,
        Mixed_Material
    }

    /// <summary> Enumeration of the different resource types recognized by SobekCM web application </summary>
    public enum TypeOfResource_SobekCM_Enum : byte
    {
        UNKNOWN,
        Aerial,
        Archival,
        Artifact,
        Audio,
        Book,
        Learning_Object,
        Map,
        Mixed_Material,
        Multivolume,
        Newspaper,
        Notated_Music,
        Photograph,
        Project,
        Serial,
        Software_Multimedia,
        Video
    }


    /// <summary> A term that specifies the characteristics and general type of content of the resource </summary>
    [Serializable]
    public class TypeOfResource_Info : XML_Writing_Base_Type
    {
        private bool collection;
        private bool manuscript;
        private TypeOfResource_MODS_Enum modsType;
        private List<string> uncontrolledTypes;

        /// <summary> Constructor for a new type of resource object </summary>
        public TypeOfResource_Info()
        {
            modsType = TypeOfResource_MODS_Enum.UNKNOWN;
            collection = false;
            manuscript = false;
        }

        /// <summary> Gets and sets the flag indicating this is a collection type </summary>
        public bool Collection
        {
            get { return collection; }
            set { collection = value; }
        }

        /// <summary> Gets and sets the flag indicating this is a manuscript type </summary>
        public bool Manuscript
        {
            get { return manuscript; }
            set { manuscript = value; }
        }

        /// <summary> Gets and sets the controlled MODS type of resource value </summary>
        public TypeOfResource_MODS_Enum MODS_Type
        {
            get { return modsType; }
            set { modsType = value; }
        }

        public string MODS_Type_String
        {
            get
            {
                switch (modsType)
                {
                    case TypeOfResource_MODS_Enum.Text:
                        return "text";

                    case TypeOfResource_MODS_Enum.Cartographic:
                        return "cartographic";

                    case TypeOfResource_MODS_Enum.Notated_Music:
                        return "notated music";

                    case TypeOfResource_MODS_Enum.Sound_Recording:
                        return "sound recording";

                    case TypeOfResource_MODS_Enum.Sound_Recording_Musical:
                        return "sound recording-musical";

                    case TypeOfResource_MODS_Enum.Sound_Recording_Nonmusical:
                        return "sound recording-nonmusical";

                    case TypeOfResource_MODS_Enum.Still_Image:
                        return "still image";

                    case TypeOfResource_MODS_Enum.Moving_Image:
                        return "moving image";

                    case TypeOfResource_MODS_Enum.Three_Dimensional_Object:
                        return "three dimensional object";

                    case TypeOfResource_MODS_Enum.Sofware_Multimedia:
                        return "software, multimedia";

                    case TypeOfResource_MODS_Enum.Mixed_Material:
                        return "mixed material";
                }
                return String.Empty;
            }
        }

        public int Uncontrolled_Types_Count
        {
            get
            {
                if (uncontrolledTypes == null)
                    return 0;
                else
                    return uncontrolledTypes.Count;
            }
        }

        public ReadOnlyCollection<string> Uncontrolled_Types
        {
            get
            {
                if (uncontrolledTypes != null)
                {
                    return new ReadOnlyCollection<string>(uncontrolledTypes);
                }
                else
                {
                    return new ReadOnlyCollection<string>(new List<string>());
                }
            }
        }

        /// <summary> Clear all the data associated with this type </summary>
        public void Clear()
        {
            modsType = TypeOfResource_MODS_Enum.UNKNOWN;
            collection = false;
            manuscript = false;
        }

        public void Clear_Uncontrolled_Types()
        {
            if (uncontrolledTypes != null)
                uncontrolledTypes.Clear();
        }

        public void Add_Uncontrolled_Type(string Uncontrolled_Type)
        {
            switch (Uncontrolled_Type.ToUpper().Replace(" ", "").Replace(",", "").Replace("-", ""))
            {
                case "TEXT":
                    modsType = TypeOfResource_MODS_Enum.Text;
                    break;

                case "CARTOGRAPHIC":
                    modsType = TypeOfResource_MODS_Enum.Cartographic;
                    break;

                case "NOTATEDMUSIC":
                    modsType = TypeOfResource_MODS_Enum.Notated_Music;
                    break;

                case "SOUNDRECORDING":
                    modsType = TypeOfResource_MODS_Enum.Sound_Recording;
                    break;

                case "SOUNDRECORDINGMUSICAL":
                    modsType = TypeOfResource_MODS_Enum.Sound_Recording_Musical;
                    break;

                case "SOUNDRECORDINGNONMUSICAL":
                    modsType = TypeOfResource_MODS_Enum.Sound_Recording_Nonmusical;
                    break;

                case "STILLIMAGE":
                    modsType = TypeOfResource_MODS_Enum.Still_Image;
                    break;

                case "MOVINGIMAGE":
                    modsType = TypeOfResource_MODS_Enum.Moving_Image;
                    break;

                case "THREEDIMENSIONALOBJECT":
                    modsType = TypeOfResource_MODS_Enum.Three_Dimensional_Object;
                    break;

                case "SOFTWAREMULTIMEDIA":
                    modsType = TypeOfResource_MODS_Enum.Sofware_Multimedia;
                    break;

                case "MIXEDMATERIALS":
                case "MIXEDMATERIAL":
                    modsType = TypeOfResource_MODS_Enum.Mixed_Material;
                    break;

                default:
                    if (uncontrolledTypes == null)
                        uncontrolledTypes = new List<string>();
                    uncontrolledTypes.Add(Uncontrolled_Type);
                    break;
            }
        }


        ///// <summary> Gets and sets the type for this material </summary>
        //public string Type
        //{
        //    get { return type; }
        //    set
        //    {
        //        switch (value.ToUpper())
        //        {
        //            case "AERIAL":
        //            case "IMAGEAERIAL":
        //                type = "Aerial";
        //                break;

        //            case "ARTIFACT":
        //            case "THREE DIMENSIONAL OBJECT":
        //                type = "Artifact";
        //                break;

        //            case "BOOK":
        //            case "MONOGRAPH":
        //            case "TEXT":
        //                type = "Book";
        //                break;

        //            case "IMAGEMAP":
        //            case "CARTOGRAPHIC":
        //                type = "Map";
        //                break;

        //            case "PHOTOGRAPH":
        //            case "IMAGE":
        //            case "POSTCARD":
        //            case "STILL IMAGE":
        //                type = "Photograph";
        //                break;

        //            case "SERIAL":
        //                type = "Serial";
        //                break;

        //            case "AUDIO":
        //            case "SOUND RECORDING":
        //            case "SOUND RECORDING-MUSICAL":
        //            case "SOUND RECORDING-NONMUSICAL":
        //                type = "Audio";
        //                break;

        //            case "VIDEO":
        //            case "MOVING IMAGE":
        //                type = "Video";
        //                break;

        //            case "NEWSPAPER":
        //                type = "Newspaper";
        //                break;

        //            case "ARCHIVES":
        //            case "ARCHIVE":
        //            case "MIXED MATERIAL":
        //                type = "Archival";
        //                break;

        //            case "PROJECT":
        //                type = "Project";
        //                break;

        //            case "MULTIVOLUME":
        //                type = "MultiVolume";
        //                break;

        //            case "EXTERNAL LINK":
        //                type = "External Link";
        //                break;

        //            default:
        //                type = value;
        //                break;
        //        }
        //    }
        //}

        internal void Add_MODS_MODS(TextWriter results)
        {
            if (modsType == TypeOfResource_MODS_Enum.UNKNOWN)
                return;

            string modsTypeString = String.Empty;

            switch (modsType)
            {
                case TypeOfResource_MODS_Enum.Text:
                    modsTypeString = "text";
                    break;

                case TypeOfResource_MODS_Enum.Cartographic:
                    modsTypeString = "cartographic";
                    break;

                case TypeOfResource_MODS_Enum.Notated_Music:
                    modsTypeString = "notated music";
                    break;

                case TypeOfResource_MODS_Enum.Sound_Recording:
                    modsTypeString = "sound recording";
                    break;

                case TypeOfResource_MODS_Enum.Sound_Recording_Musical:
                    modsTypeString = "sound recording-musical";
                    break;

                case TypeOfResource_MODS_Enum.Sound_Recording_Nonmusical:
                    modsTypeString = "sound recording-nonmusical";
                    break;

                case TypeOfResource_MODS_Enum.Still_Image:
                    modsTypeString = "still image";
                    break;

                case TypeOfResource_MODS_Enum.Moving_Image:
                    modsTypeString = "moving image";
                    break;

                case TypeOfResource_MODS_Enum.Three_Dimensional_Object:
                    modsTypeString = "three dimensional object";
                    break;

                case TypeOfResource_MODS_Enum.Sofware_Multimedia:
                    modsTypeString = "software, multimedia";
                    break;

                case TypeOfResource_MODS_Enum.Mixed_Material:
                    modsTypeString = "mixed material";
                    break;
            }

            if (modsTypeString.Length > 0)
            {
                results.Write("<mods:typeOfResource");
                if (collection)
                    results.Write(" collection=\"yes\"");
                if (manuscript)
                    results.Write(" manuscript=\"yes\"");
                results.Write(">" + modsTypeString + "</mods:typeOfResource>\r\n");
            }
        }
    }
}