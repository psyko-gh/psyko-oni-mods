using System.Collections.Generic;
using TUNING;
using UnityEngine;
using STRINGS;

namespace Freezer
{
    public class FreezerConfig: IBuildingConfig
    {
        public const string ID = "Freezer";
        private const float CAPACITY_KG = 200f;
        private const int ENERGY_SAVER_POWER = 40;
        private const float ENERGY_ACTIVE_POWER = 480f;
        private const float COOLING_HEAT_KW = 0.375f;
        private const float STEADY_HEAT_KW = 0.0f;
        private const float SIMULATED_INTERNAL_TEMPERATUR_KELVIN = 254.15f;
        private readonly string KANIM = "freezer_kanim";
        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                FreezerConfig.ID, 
                2, 
                2, 
                KANIM, 
                30, 
                10f, 
                TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, 
                MATERIALS.REFINED_METALS,
                800f,
                BuildLocationRule.OnFloor,
                TUNING.BUILDINGS.DECOR.BONUS.TIER1,
                TUNING.NOISE_POLLUTION.NOISY.TIER0);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = ENERGY_ACTIVE_POWER;
            buildingDef.SelfHeatKilowattsWhenActive = COOLING_HEAT_KW;
            buildingDef.ExhaustKilowattsWhenActive = STEADY_HEAT_KW;
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), (string) STRINGS.BUILDINGS.PREFABS.FREEZER.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.FREEZER.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.FREEZER.LOGIC_PORT_INACTIVE)
            };
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            SoundEventVolumeCache.instance.AddVolume(KANIM, "Refrigerator_open", TUNING.NOISE_POLLUTION.NOISY.TIER1);
            SoundEventVolumeCache.instance.AddVolume(KANIM, "Refrigerator_close", TUNING.NOISE_POLLUTION.NOISY.TIER1);
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.FOOD;
            storage.allowItemRemoval = true;
            storage.capacityKg = CAPACITY_KG;
            storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            storage.showCapacityStatusItem = true;
            Prioritizable.AddRef(go);
            go.AddOrGet<TreeFilterable>();
            go.AddOrGet<Freezer>();
            RefrigeratorController.Def def = go.AddOrGetDef<RefrigeratorController.Def>();
            def.powerSaverEnergyUsage = ENERGY_SAVER_POWER;
            def.coolingHeatKW = COOLING_HEAT_KW;
            def.steadyHeatKW = STEADY_HEAT_KW;
            def.simulatedInternalTemperature = SIMULATED_INTERNAL_TEMPERATUR_KELVIN;
            go.AddOrGet<UserNameable>();
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;
            go.AddOrGetDef<StorageController.Def>();
        }
    }
}