using System.Collections.Generic;
using TUNING;
using UnityEngine;
using KMod;

namespace JouleSensor
{
    internal class JouleSensorConfig : IBuildingConfig
    {
        public const string ID = "JouleSensor";
        public const string DisplayName = "Joule Sensor";
        public const string Description = "Joule Sensor";
        public const string Effect = "Sends a Green Signal Sensor when available power available is less than <b>Low Threshold</b> until <b>High Threshold</b> is reached again";
        public const string LogicPort = "Stored power";
        public const string LogicPortActive = "Sends a Green Signal Sensor when available power is less than <b>Low Threshold</b> until <b>High Threshold</b> is reached again";
        public const string LogicPortInactive = "Sends a Red Signal when available power is <b>High Threshold</b>, until <b>Low Threshold</b> is reached again";
        public static float[] MASS = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
        private static readonly string kanim = "wattage_sensor_kanim";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(
                ID,
                1,
                1,
                kanim,
                BUILDINGS.HITPOINTS.TIER2,
                30f,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER0,
                MATERIALS.REFINED_METALS,
                1600f,
                BuildLocationRule.Anywhere,
                BUILDINGS.DECOR.PENALTY.TIER0,
                NOISE_POLLUTION.NONE
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
                  LogicPort,
                  LogicPortActive,
                  LogicPortInactive,
                  true)
            };
            
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, JouleSensorConfig.ID);
            SoundEventVolumeCache.instance.AddVolume(JouleSensorConfig.kanim, "PowerSwitch_on", TUNING.NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume(JouleSensorConfig.kanim, "PowerSwitch_off", TUNING.NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, LogicWattageSensorConfig.ID);
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            BuildingTemplates.DoPostConfigure(go);
            JouleSensor jouleSensor = go.AddOrGet<JouleSensor>();
            jouleSensor.manuallyControlled = false;
            jouleSensor.activateOnHigherThan = true;
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
        }
    }
}