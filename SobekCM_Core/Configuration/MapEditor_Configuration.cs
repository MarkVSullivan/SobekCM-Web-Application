#region Using directives

using System;
using System.Collections.Generic;
using System.Xml;

#endregion

namespace SobekCM.Core.Configuration
{
    /// <summary> Instance-wide configuration information for the map editor </summary>
    public static class MapEditor_Configuration
    {
        //assign config file
        private static string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "/config/default/sobekcm_mapeditor.config";

        /// <summary> Gets all settings from config file </summary>
        /// <param name="IdsFromPage"> The IDs from the page </param>
        /// <returns> List of settings to use for that ID(?) </returns>
        public static List<string>[] GetSettings(List<string> IdsFromPage)
        {
            //get defaults as base
            List<string>[] settings = GetDefaultSettings();

            //determine if custom has settings
            if (HasCustomSettings(IdsFromPage))
            {
                List<string>[] newSettings =  GetCustomSettings(IdsFromPage);
                foreach (string settingName in newSettings[0])
                {
                    settings[0].Add(settingName);
                }
                foreach (string settingValue in newSettings[1])
                {
                    settings[1].Add(settingValue);
                }
            }
            
            return settings;
        }

        /// <summary> Determines if there are custom settings </summary>
        /// <param name="IdsFromPage">The ids from page.</param>
        /// <returns></returns>
        private static bool HasCustomSettings(List<string> IdsFromPage)
        {
            bool hasCustomSettings = false;
            foreach (string idFromPage in IdsFromPage)
            {
                foreach (string idFromConfig in GetIdsFromConfig())
                {
                    if (idFromPage.Replace(" ", "") == idFromConfig.Replace(" ", ""))
                        hasCustomSettings = true;
                }
            }
            
            return hasCustomSettings;
        }

        /// <summary> Get all the collection ids from the config file  </summary>
        /// <returns> List of ids from the config file </returns>
        private static List<string> GetIdsFromConfig()
        {
            //init IdsFromConfig
            List<string> idsFromConfig = new List<string>();

            //read the Ids from config file
            using (XmlReader reader = XmlReader.Create(configFilePath))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "collection")
                        {
                            //get all of the  ids
                            string attribute = reader.GetAttribute("id");
                            if (attribute != null)
                            {
                                string[] tempIds = attribute.Split(',');
                                idsFromConfig.AddRange(tempIds);
                            }
                        }
                    }
                }
            }
            return idsFromConfig;
        }

        /// <summary> Gets the default settings </summary>
        /// <returns></returns>
        private static List<string>[] GetDefaultSettings()
        {
            //init LoadParams
            List<string>[] settings = new List<string>[2];
            settings[0] = new List<string>();
            settings[1] = new List<string>();

            //read through load default params
            using (XmlReader reader = XmlReader.Create(configFilePath))
            {
                while (reader.Read())
                {
                    // Only detect start elements.
                    if (reader.IsStartElement())
                    {
                        // Get element name and switch on it.
                        switch (reader.Name)
                        {
                            case "collection":
                                if ((reader["id"] == "default"))
                                {
                                    while (reader.Read())
                                    {
                                        if (!reader.IsStartElement()) continue;
                                        if (reader.Name == "collection")
                                            break;
                                        settings[0].Add(reader.Name);
                                        if (reader.Read())
                                            settings[1].Add(string.IsNullOrEmpty(reader.Value) ? "\"\"" : reader.Value);
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            //return settings
            return settings;
        }

        /// <summary> Gets all the custom settigns as defined from page itself </summary>
        /// <param name="IdsFromPage"> The ids from page.</param>
        /// <returns></returns>
        private static List<string>[] GetCustomSettings(List<string> IdsFromPage)
        {
            //init LoadParams
            List<string>[] settings = new List<string>[2];
            settings[0] = new List<string>();
            settings[1] = new List<string>();

            //read through load default params
            using (XmlReader reader = XmlReader.Create(configFilePath))
            {
                while (reader.Read())
                {
                    // Only detect start elements.
                    if (reader.IsStartElement())
                    {
                        // Get element name and switch on it.
                        switch (reader.Name)
                        {
                            case "collection":
                                foreach (string id in IdsFromPage) 
                                {
                                    string s = reader["id"];
                                    if (s != null && (s.Contains(id)))
                                    {
                                        while (reader.Read())
                                        {
                                            if (!reader.IsStartElement()) continue;
                                            if (reader.Name == "collection")
                                                break;
                                            settings[0].Add(reader.Name);
                                            if (reader.Read())
                                                settings[1].Add(string.IsNullOrEmpty(reader.Value) ? "\"\"" : reader.Value);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            //return settings
            return settings;
        } 

    }
}
