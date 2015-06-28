#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Enumeration of the different resource types generally allowed by MODS </summary>
    /// <remarks> Source of the remarks on each enumeration value are from the Library of 
	/// Congress MODS site ( http://www.loc.gov/standards/mods/userguide/typeofresource.html ) [2013] </remarks>
    public enum TypeOfResource_MODS_Enum : byte
    {
		/// <summary> Unknown, default MODS type </summary>
        UNKNOWN,

		/// <summary> Resources that are basically textual in nature </summary>
        Text,

		/// <summary> Includes maps, atlases, globes, digital maps, and other cartographic items </summary>
        Cartographic,

		/// <summary>  Graphic, non-realized representations of musical works, both in printed and digitized manifestations that represent the four components of musical sound: pitch, duration, timbre, and loudness </summary>
        Notated_Music,

		/// <summary> Used when a mixture of musical and nonmusical sound recordings occurs in a resource or when a user does not want to or cannot make a distinction between musical and nonmusical </summary>
        Sound_Recording,

		/// <summary> Used when a resource is predominately a musical sound recording </summary>
        Sound_Recording_Musical,

		/// <summary> Used when the sound recording is nonmusical in nature </summary>
        Sound_Recording_Nonmusical,

		/// <summary> Includes two-dimensional images and slides and transparencies </summary>
        Still_Image,

		/// <summary> Includes motion pictures and videorecordings, as well as television programs, digital video, and animated computer graphics—but not slides and transparencies. It does not include moving images that are primarily computer programs, such as computer games or computer-oriented multimedia; these are included in "software, multimedia" </summary>
        Moving_Image,

		/// <summary>  Includes man-made objects such as models, sculptures, clothing, and toys, as well as naturally occurring objects such as specimens mounted for viewing </summary>
        Three_Dimensional_Object,

		/// <summary> Appropriate for any electronic resource without a significant aspect that indicates one of the other typeOfResource categories. It includes: software, numeric data, computer-oriented multimedia, and online systems and services </summary>
        Sofware_Multimedia,

		/// <summary> Indicates that there are significant materials in two or more forms that are usually related by virtue of their having been accumulated by or about a person or body. Mixed material includes archival fonds and manuscript collections of mixed forms of materials, such as text, photographs, and sound recordings </summary>
        Mixed_Material
    }

    /// <summary> Enumeration of the different resource types recognized by SobekCM web application </summary>
    public enum TypeOfResource_SobekCM_Enum : byte
    {
        /// <summary> Unknown resource type (default) </summary>
        UNKNOWN,

        /// <summary> Aerial photography </summary>
        Aerial,

        /// <summary> Archival material </summary>
        Archival,

        /// <summary> Artifact type material </summary>
        Artifact,

        /// <summary> Audio material (musical or nonmusical)  </summary>
        Audio,

        /// <summary> Book type material   </summary>
        Book,

        /// <summary> DataSet or tabular data  </summary>
		Dataset,

        /// <summary> Encoded Archival Descriptor  </summary>
        EAD,

        /// <summary> Learning object </summary>
        Learning_Object,

        /// <summary> Cartographic material </summary>
        Map,

        /// <summary> Beta map material type? </summary>
        Map_Beta,

        /// <summary> Mixed material (often also classified as Archival) </summary>
        Mixed_Material,

        /// <summary> Multi-volume type </summary>
        Multivolume,

        /// <summary> Newspaper issue  </summary>
        Newspaper,

        /// <summary> Notated musical score </summary>
        Notated_Music,

        /// <summary> Photographic material  </summary>
        Photograph,

        /// <summary> Special SobekCM reserved type for PROJECT, or default metadata set </summary>
        Project,

        /// <summary> Serial type material </summary>
        Serial,

        /// <summary> Software or multimedia type material </summary>
        Software_Multimedia,

        /// <summary> Video </summary>
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

        /// <summary> Returns the type of resource, in the MODS ready string </summary>
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

        /// <summary> Gets the number of additional, uncontrolled types associated
        /// with this resource </summary>
        public int Uncontrolled_Types_Count
        {
            get {
	            return uncontrolledTypes == null ? 0 : uncontrolledTypes.Count;
            }
        }

        /// <summary> Gets the collection of uncontrolled types associated 
        /// with this resource </summary>
        public ReadOnlyCollection<string> Uncontrolled_Types
        {
            get {
	            return uncontrolledTypes != null ? new ReadOnlyCollection<string>(uncontrolledTypes) : new ReadOnlyCollection<string>(new List<string>());
            }
        }

        /// <summary> Clear all the data associated with this type </summary>
        public void Clear()
        {
            modsType = TypeOfResource_MODS_Enum.UNKNOWN;
            collection = false;
            manuscript = false;
        }

        /// <summary> Clears the list of all uncontrolled types associated with this  </summary>
        public void Clear_Uncontrolled_Types()
        {
            if (uncontrolledTypes != null)
                uncontrolledTypes.Clear();
        }

		/// <summary> Adds an uncontrolled type to this resource </summary>
		/// <param name="Uncontrolled_Type"> New uncontrolled type </param>
		/// <remarks> This type is still analyzed to see if it is actually controlled </remarks>
        public TypeOfResource_MODS_Enum Add_Uncontrolled_Type(string Uncontrolled_Type)
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
				case "IMAGE":
                    modsType = TypeOfResource_MODS_Enum.Still_Image;
                    break;

                case "MOVINGIMAGE":
                    modsType = TypeOfResource_MODS_Enum.Moving_Image;
                    break;

                case "THREEDIMENSIONALOBJECT":
                    modsType = TypeOfResource_MODS_Enum.Three_Dimensional_Object;
                    break;

                case "SOFTWAREMULTIMEDIA":
				case "DATASET":
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
                    return TypeOfResource_MODS_Enum.UNKNOWN;
            }

            return modsType;
        }


        internal void Add_MODS_MODS(TextWriter Results)
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
                Results.Write("<mods:typeOfResource");
                if (collection)
                    Results.Write(" collection=\"yes\"");
                if (manuscript)
                    Results.Write(" manuscript=\"yes\"");
                Results.Write(">" + modsTypeString + "</mods:typeOfResource>\r\n");
            }
        }
    }
}