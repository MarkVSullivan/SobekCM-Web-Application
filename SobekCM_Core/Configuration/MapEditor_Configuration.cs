#region Using directives

using System;
using System.Collections.Generic;
using System.Xml;

#endregion

namespace SobekCM.Core.Configuration
{
    public static class MapEditor_Configuration
    {
        //assign config file
        private static string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "/config/default/sobekcm_mapeditor.config";
        
        //gets all settings from config file
        public static List<string>[] getSettings(List<string> IdsFromPage)
        {
            //get defaults as base
            List<string>[] settings = getDefaultSettings();

            //determine if custom has settings
            if (hasCustomSettings(IdsFromPage))
            {
                List<string>[] newSettings =  getCustomSettings(IdsFromPage);
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

        //determines if there are custom settings
        private static bool hasCustomSettings(List<string> IdsFromPage)
        {
            bool hasCustomSettings = false;
            foreach (string IdFromPage in IdsFromPage)
            {
                foreach (string IdFromConfig in getIdsFromConfig())
                {
                    if (IdFromPage.Replace(" ", "") == IdFromConfig.Replace(" ", ""))
                        hasCustomSettings = true;
                }
            }
            
            return hasCustomSettings;
        }
        
        //get all the collection ids from the config file
        private static List<string> getIdsFromConfig()
        {
            //init IdsFromConfig
            List<string> IdsFromConfig = new List<string>();

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
                            string[] tempIds = reader.GetAttribute("id").Split(',');
                            foreach (string tempId in tempIds)
                            {
                                IdsFromConfig.Add(tempId);
                            }
                        }
                    }
                }
            }
            return IdsFromConfig;
        } 

        //gets the default settings
        private static List<string>[] getDefaultSettings()
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

        //gets all the custom settigns as defined from page itself
        private static List<string>[] getCustomSettings(List<string> IdsFromPage)
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
                                foreach (string Id in IdsFromPage)
                                {
                                    if ((reader["id"].Contains(Id)))
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
