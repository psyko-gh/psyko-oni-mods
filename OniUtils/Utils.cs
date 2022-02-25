using System.Collections.Generic;
using KMod;
using System;
using System.IO;
using System.Reflection;
using static Localization;

namespace Psyko.OniUtils
{
    public static class Utils
    {
        public static void AddBuildingToTech(string tech, string idBuilding)
        {
            Db.Get().Techs.Get(tech).unlockedItemIDs.Add(idBuilding);
        }
        
        public static void AddPlan(HashedString category, string subcategory, string idBuilding, string addAfter = null) {
            Debug.Log("Adding " + idBuilding+ " to category " + category);
            foreach (PlanScreen.PlanInfo menu in TUNING.BUILDINGS.PLANORDER)
            {
                if (menu.category == category)
                {
                    AddPlanToCategory(menu, subcategory, idBuilding, addAfter);
                    return;
                }
            }

            Debug.Log($"Unknown build menu category: ${category}");
        }

        private static void AddPlanToCategory(PlanScreen.PlanInfo menu, string subcategory, string idBuilding, string addAfter = null)
        {
            List<KeyValuePair<string, string>> data = menu.buildingAndSubcategoryData;
            if (data != null)
            {
                if (addAfter == null)
                {
                    data.Add(new KeyValuePair<string, string>(idBuilding, subcategory));
                }
                else
                {
                    /*
                    foreach (KeyValuePair<string, string> k in data)
                    {
                        Debug.Log($"{k.Key}/{k.Value}");
                    }
                    */
                    int index = data.IndexOf(new KeyValuePair<string, string>(addAfter, subcategory));
                    if (index == -1)
                    {
                        Debug.Log($"Could not find building {subcategory}/{addAfter} to add {idBuilding} after. Adding at the end !");
                        data.Add(new KeyValuePair<string, string>(idBuilding, subcategory));                        
                        return;
                    }
                    data.Insert(index + 1, new KeyValuePair<string, string>(idBuilding, subcategory));
                }
            }
        }

        public static string GetModPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
        
        public static void Translate(Type root)
        {
            // Basic intended way to register strings, keeps namespace
            RegisterForTranslation(root);
 
            // Load user created translation files
            LoadStrings();
 
            // Register strings without namespace
            // because we already loaded user transltions, custom languages will overwrite these
            LocString.CreateLocStringKeys(root, null);
 
            // Creates template for users to edit
            GenerateStringsTemplate(root, Path.Combine(Manager.GetDirectory(), "strings_templates"));
        }
            
        private static void LoadStrings()
        {
            string path = Path.Combine(GetModPath(), "translations", GetLocale()?.Code + ".po");
            if (File.Exists(path))
                OverloadStrings(LoadStringsFile(path, false));
        }
    }
}