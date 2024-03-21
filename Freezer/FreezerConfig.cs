﻿using System.Collections.Generic;
using UnityEngine;
using PeterHan.PLib.Core;
using TUNING;

namespace Psyko.Freezer
{
    public class FreezerConfig: IBuildingConfig
    {
        public const string ID = "Freezer";
        public const float CAPACITY_KG = 200f;
        public const int ENERGY_SAVER_POWER = 40;
        public const float ENERGY_ACTIVE_POWER = 480f;
        private const float COOLING_HEAT_KW = 0.375f;
        private const float STEADY_HEAT_KW = 0.0f;
        private const float SIMULATED_INTERNAL_TEMPERATUR_KELVIN = 254.15f;
        private static readonly string KANIM = "freezer_kanim";
    
        public override BuildingDef CreateBuildingDef()
        {
            PGameUtils.CopySoundsToAnim(KANIM, "refrigerator_kanim");
            SoundEventVolumeCache.instance.AddVolume(KANIM, "Refrigerator_open", TUNING.NOISE_POLLUTION.NOISY.TIER1);
            SoundEventVolumeCache.instance.AddVolume(KANIM, "Refrigerator_close", TUNING.NOISE_POLLUTION.NOISY.TIER1);
            string[] materials = {
                MATERIALS.REFINED_METAL,
                MATERIALS.PLASTIC
            };
            float[] materialMasses = {
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER4[0],
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
            };
            BuildingDef def = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 2,
                height: 2,
                anim: KANIM,
                hitpoints: BUILDINGS.HITPOINTS.TIER0,
                construction_time: 20f,
                construction_mass: materialMasses,
                construction_materials: materials,
                melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER0,
                build_location_rule: BuildLocationRule.OnFloor,
                decor: BUILDINGS.DECOR.BONUS.TIER1,
                noise: NOISE_POLLUTION.NONE
            );
            def.name = STRINGS.BUILDINGS.PREFABS.FREEZER.NAME;
            def.RequiresPowerInput = true;
            def.EnergyConsumptionWhenActive = FreezerOptions.Instance.EnergyActivePower;
            def.SelfHeatKilowattsWhenActive = COOLING_HEAT_KW;
            def.ExhaustKilowattsWhenActive = 0.0f;
            def.AudioCategory = "Metal";
            def.Entombable = true;
            def.Floodable = true;
            def.LogicInputPorts = new List<LogicPorts.Port>();
            def.LogicOutputPorts = new List<LogicPorts.Port>
            {
                LogicPorts.Port.OutputPort(
                    FilteredStorage.FULL_PORT_ID,
                    new CellOffset(0, 1),
                    STRINGS.BUILDINGS.PREFABS.FREEZER.LOGIC_PORT,
                    STRINGS.BUILDINGS.PREFABS.FREEZER.LOGIC_PORT_ACTIVE,
                    STRINGS.BUILDINGS.PREFABS.FREEZER.LOGIC_PORT_INACTIVE)
            };
            def.ObjectLayer = ObjectLayer.Building;
            def.OverheatTemperature = 75.0f + Constants.CELSIUS2KELVIN;
            def.ViewMode = OverlayModes.Power.ID;
            def.SceneLayer = Grid.SceneLayer.Building;
            return def;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            base.ConfigureBuildingTemplate(go, prefab_tag);
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.Refrigerator);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.showDescriptor = true;
            storage.storageFilters = TUNING.STORAGEFILTERS.FOOD;
            storage.allowItemRemoval = true;
            storage.capacityKg = FreezerOptions.Instance.Capacity;
            storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            storage.showCapacityStatusItem = true;
            Prioritizable.AddRef(go);
            go.AddOrGet<TreeFilterable>();
            go.AddOrGet<FoodStorage>();
            go.AddOrGet<Refrigerator>();
            go.AddOrGet<Freezer>();
            RefrigeratorController.Def def = go.AddOrGetDef<RefrigeratorController.Def>();
            def.powerSaverEnergyUsage = FreezerOptions.Instance.EnergySaverPower;
            def.coolingHeatKW = COOLING_HEAT_KW;
            def.steadyHeatKW = STEADY_HEAT_KW;
            def.simulatedInternalTemperature = SIMULATED_INTERNAL_TEMPERATUR_KELVIN;
            go.AddOrGet<UserNameable>();
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;
            go.AddOrGetDef<StorageController.Def>();
            PPatchTools.GetTypeSafe("PeterHan.NoWasteWant.NoWasteWantPatches")?.GetMethodSafe("AddFreshnessControl", true, typeof(GameObject))?.Invoke(null, new object[] { go });
        }
    }
}
