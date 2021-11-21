using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using Psyko.OniUtils;

namespace Psyko.Freezer
{
    public class FreezerPatch : UserMod2
    {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            POptions pOptions = new POptions();
            pOptions.RegisterOptions(this, typeof(FreezerOptions));
            LocString.CreateLocStringKeys(typeof(STRINGS));
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                Utils.AddBuildingToTech(TechCategory.FinerDining, FreezerConfig.ID);
                Utils.AddPlan(PlanCategory.Food, FreezerConfig.ID, RefrigeratorConfig.ID);
            }
        }

        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class Localization_Initialize_Patch
        {
            public static void Postfix() => Utils.Translate(typeof(STRINGS));

        }
        
    }
}