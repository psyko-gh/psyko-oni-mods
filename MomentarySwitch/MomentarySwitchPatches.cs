using HarmonyLib;
using KMod;
using PeterHan.PLib.UI;
using Psyko.OniUtils;
using UnityEngine;

namespace Psyko.MomentarySwitch
{
    public class MomentarySwitchPatches : UserMod2
    {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            LocString.CreateLocStringKeys(typeof(STRINGS));
        }
        
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                Utils.AddBuildingToTech(TechCategory.LogicControl, MomentarySwitchConfig.ID);
                Utils.AddPlan(PlanCategory.Automation, "switches", MomentarySwitchConfig.ID, LogicSwitchConfig.ID);
            }
        }
        
        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class DetailsScreen_OnPrefabInit_Patch {
            internal static void Postfix() {
                PUIUtils.AddSideScreenContent<MomentarySwitchSideScreen>();
            }
        }

        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class Localization_Initialize_Patch
        {
            public static void Postfix() => Utils.Translate(typeof(STRINGS));
        }
        
        // Disable the default sidescreen
        [HarmonyPatch(typeof(PlayerControlledToggleSideScreen), "IsValidForTarget")]
        public class PlayerControlledToggleSideScreen_IsValidForTarget_Patch
        {
            public static bool Prefix(GameObject target, ref bool __result)
            {
                if (target.GetComponent<MomentarySwitch>() != null)
                {
                    __result = false;
                    return false;
                }

                return true;
            }
        }

    }
}