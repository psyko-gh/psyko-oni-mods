using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Psyko.MomentarySwitch
{
    public class MomentarySwitchConfig: IBuildingConfig
    {
        public static string ID = "MomentarySwitch";
        private static readonly string KANIM = "momentaryswitch_kanim";
        
        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 1,
                anim: KANIM,
                hitpoints: BUILDINGS.HITPOINTS.TIER0,
                construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
                construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER0,
                construction_materials: MATERIALS.REFINED_METALS,
                melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER1,
                build_location_rule: BuildLocationRule.Anywhere,
                decor: BUILDINGS.DECOR.NONE,
                noise: NOISE_POLLUTION.NONE
            );
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.AlwaysOperational = true;
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.OutputPort(
                    LogicSwitch.PORT_ID,
                    new CellOffset(0, 0),
                    STRINGS.BUILDINGS.PREFABS.MOMENTARYSWITCH.LOGIC_PORT,
                    STRINGS.BUILDINGS.PREFABS.MOMENTARYSWITCH.LOGIC_PORT_ACTIVE,
                    STRINGS.BUILDINGS.PREFABS.MOMENTARYSWITCH.LOGIC_PORT_INACTIVE,
                    true
                )
            };
            SoundEventVolumeCache.instance.AddVolume("switchpower_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume("switchpower_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, LogicSwitchConfig.ID);
            return buildingDef;
        }
        
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            go.AddOrGet<MomentarySwitch>().manuallyControlled = false;
            go.AddOrGet<MomentarySwitch>().defaultState = false;
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
        } 

        public override void DoPostConfigureComplete(GameObject go)
        {
            
        }
    }
}