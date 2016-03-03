using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Tools;

namespace SobekCM.Core.Builder
{
    /// <summary> Latest status information for the builder, from the SobekCM database </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("builderStatus")]
    public class Builder_Status
    {
        private Dictionary<string, StringKeyValuePair> settingLookupTable;

            /// <summary> List of the latest builder status values from the settings table, where
        /// it is updated as each run of the builder completes </summary>
        [DataMember(Name = "settings")]
        [XmlArray("settings")]
        [XmlArrayItem("setting", typeof(StringKeyValuePair))]
        [ProtoMember(1)]
        public List<StringKeyValuePair> Settings { get; set; }

        /// <summary> List of all the scheduled tasks with their lastest update and basic information </summary>
        [DataMember(Name = "scheduledTasks")]
        [XmlArray("scheduledTasks")]
        [XmlArrayItem("task", typeof(Builder_Scheduled_Task_Status))]
        [ProtoMember(2)]
        public List<Builder_Scheduled_Task_Status> ScheduledTasks { get; set; }

        /// <summary> Constructor for a new instance of the builder status object </summary>
        public Builder_Status()
        {
            Settings = new List<StringKeyValuePair>();
            ScheduledTasks = new List<Builder_Scheduled_Task_Status>();
        }

        /// <summary> Add a new setting string key value pair </summary>
        /// <param name="Key"> Key for this setting value </param>
        /// <param name="Value"> Value for this setting value </param>\
        /// <remarks> If a setting for that Key already exists, it will be replaced with the new Value</remarks>
        public void Add_Setting(string Key, string Value)
        {
            // Is the dictionary built?
            if (settingLookupTable == null)
                settingLookupTable = new Dictionary<string, StringKeyValuePair>( StringComparer.OrdinalIgnoreCase);

            // Is the dictionary apparently current?
            if (settingLookupTable.Count != Settings.Count)
            {
                foreach (StringKeyValuePair pair in Settings)
                {
                    settingLookupTable[pair.Key] = pair;
                }
            }

            // Now, is the an existing pair for this key?
            if (settingLookupTable.ContainsKey(Key))
                settingLookupTable[Key].Value = Value;
            else
            {
                StringKeyValuePair pair = new StringKeyValuePair(Key, Value);
                Settings.Add(pair);
                settingLookupTable[Key] = pair;
            }
        }

        /// <summary> Gets the setting value, based on key  </summary>
        /// <param name="Key"> Key for this setting value </param>
        /// <returns> Indicated value or NULL if it does not exists </returns>
        public string Get_Setting(string Key)
        {
            // Is the dictionary built?
            if (settingLookupTable == null)
                settingLookupTable = new Dictionary<string, StringKeyValuePair>(StringComparer.OrdinalIgnoreCase);

            // Is the dictionary apparently current?
            if (settingLookupTable.Count != Settings.Count)
            {
                foreach (StringKeyValuePair pair in Settings)
                {
                    settingLookupTable[pair.Key] = pair;
                }
            }

            // Now, is the an existing pair for this key?
            if (settingLookupTable.ContainsKey(Key))
                return settingLookupTable[Key].Value;

            return null;
        }
    }
}
