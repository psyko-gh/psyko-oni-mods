using HarmonyLib;
using static SkyLib.OniUtils;
using KMod;
using System;
using System.IO;
using static Localization;

namespace Freezer
{
    public class FreezerPatch
    {
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Prefix()
            {
            }

            public static void Postfix()
            {
                Debug.Log("I execute after Db.Initialize!");
                AddBuildingToBuildMenu("Food", FreezerConfig.ID);
                AddBuildingToTech("FinerDining", FreezerConfig.ID);
                // AddStatusItem("FRIDGECOOLING", "NAME", "Cooling Contents: {UsedPower}", "BUILDING");
                // AddStatusItem("FRIDGECOOLING", "TOOLTIP", "{UsedPower} of {MaxPower} are being used to cool the contents of this food storage", "BUILDING");
                // AddStatusItem("FRIDGESTEADY", "NAME", "Energy Saver: {UsedPower}", "BUILDING");
                // AddStatusItem("FRIDGESTEADY", "TOOLTIP", "The contents of this food storage are at refrigeration temperatures\n\nEnergy Saver mode has been automatically activated using only {UsedPower} of {MaxPower}", "BUILDING");
            }
        }

        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class Localization_Initialize_Patch
        {
            public static void Postfix() => Translate(typeof(STRINGS));
 
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
                // string path = Path.Combine(ModPath, "translations", GetLocale()?.Code + ".po");
                // if (File.Exists(path))
                //     OverloadStrings(LoadStringsFile(path, false));
            }
        }
    }
}