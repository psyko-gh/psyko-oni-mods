using HarmonyLib;
using KMod;
using System;
using System.IO;
using System.Reflection;
using static Localization;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using Psyko.OniUtils;

namespace Psyko.Freezer
{
    public class FreezerPatch : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            LocString.CreateLocStringKeys(typeof(STRINGS));
            new PLocalization().Register();
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                Utils.AddBuildingToTech("FinerDining", FreezerConfig.ID);
                Utils.AddPlan("Food", FreezerConfig.ID, RefrigeratorConfig.ID);
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
                var modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string path = Path.Combine(modPath, "translations", GetLocale()?.Code + ".po");
                if (File.Exists(path))
                    OverloadStrings(LoadStringsFile(path, false));
            }
        }
        
    }
}