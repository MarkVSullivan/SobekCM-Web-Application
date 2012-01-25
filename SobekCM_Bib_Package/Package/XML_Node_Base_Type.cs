using System;
using System.Collections.Generic;
using System.Text;

namespace SobekCM.Bib_Package
{
    /// <summary> Helper base class which keeps tracking id the XML ID and assists in formatting 
    /// the ID in special ways to signify that a field is user-entered </summary>
    /// <remarks> This class extends the <see cref="XML_Writing_Base_Type" /> class. </remarks>
    [Serializable]
    public class XML_Node_Base_Type : XML_Writing_Base_Type
    {
        #region Static portion of this class keeps track of the index for all the user submitted nodes

        private static int user_id_index;
        private static bool include_user_in_id;

        /// <summary> Static constructor for this class </summary>
        static XML_Node_Base_Type()
        {
            user_id_index = 1;
            include_user_in_id = true;
        }

        /// <summary> Resets the user-submitted ID index, used before writing the entire metadata file </summary>
        internal static void Reset_User_ID_Index()
        {
            user_id_index = 1;
        }

        /// <summary> Flag indicates whether to include the special user-flag portion in the ID for this XML node </summary>
        internal static bool Include_User_In_ID
        {
            set { include_user_in_id = value; }
        }

        #endregion

        /// <summary>  Identifier field for this XML node </summary>
        protected string id;
        private bool user_submitted;

        /// <summary> Constructor for a new instancee of the XML_Node_Base_Type class </summary>
        public XML_Node_Base_Type()
        {
            user_submitted = false;
        }

        /// <summary> Gets and sets the flag that indicates this node was user-submitted </summary>
        public bool User_Submitted
        {
            get { return user_submitted; }
            set { user_submitted = value; }
        }

        /// <summary> Returns the actual id for this node, excluding the special user indicator portion </summary>
        public string Actual_ID
        {
            get { return id ?? String.Empty; }
        }

        /// <summary> Gets and sets the ID for this XML node </summary>
        public string ID
        {
            get
            {
                if ((include_user_in_id) && (user_submitted))
                {
                    if (!String.IsNullOrEmpty(id))
                    {
                        string returnVal = "USER" + user_id_index.ToString().PadLeft(3, '0') + "_" + id;
                        user_id_index++;
                        return returnVal;
                    }
                    else
                    {
                        string returnVal = "USER" + user_id_index.ToString().PadLeft(3, '0');
                        user_id_index++;
                        return returnVal;
                    }
                }
                else
                {
                    return id ?? String.Empty;
                }
            }
            set
            {
                if (value.IndexOf("USER") >= 0)
                {
                    user_submitted = true;
                    include_user_in_id = true;

                    int user_index = value.IndexOf("USER");
                    if (value.Length == user_index + 4)
                    {
                        id = value.Replace("USER", "");
                    }
                    else
                    {
                        int user_length = 4;
                        if (Char.IsNumber(value[user_index + user_length]))
                            user_length++;
                        if ((value.Length > user_index + user_length) && (Char.IsNumber(value[user_index + user_length])))
                            user_length++;
                        if ((value.Length > user_index + user_length) && (Char.IsNumber(value[user_index + user_length])))
                            user_length++;
                        if ((value.Length > user_index + user_length) && (Char.IsNumber(value[user_index + user_length])))
                            user_length++;
                        if ((value.Length > user_index + user_length) && (Char.IsNumber(value[user_index + user_length])))
                            user_length++;
                        if ((value.Length > user_index + user_length) && (value[user_index + user_length] == '_' ))
                            user_length++;
                        if (value.Length == user_length)
                        {
                            id = String.Empty;
                        }
                        else
                        {
                            if (value.Length == user_index + user_length)
                            {
                                // Ends in USER###...
                                id = value.Substring(0, user_index );
                            }
                            else
                            {
                                // Hopefully this starts with USER####
                                id = value.Substring(user_length);
                            }
                        }
                    }
                }
                else
                {
                    id = value;
                }
            }
        }

        /// <summary> Add the XML-formatted ID string to the output stream </summary>
        /// <param name="results"> Stream to write to </param>
        internal void Add_ID(System.IO.TextWriter results)
        {
            if ((!String.IsNullOrEmpty(id)) || ((include_user_in_id) && (user_submitted)))
            {
                results.Write(" ID=\"" + ID + "\"");
                return;
            }

            return;
        }
    }
}
