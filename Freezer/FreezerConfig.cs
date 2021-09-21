using System.Collections.Generic;
using TUNING;
using UnityEngine;
using STRINGS;

namespace Freezer
{
    public class FreezerConfig: IBuildingConfig
    {
        public const string ID = "Freezer";
        private const int ENERGY_SAVER_POWER = 40;
        private readonly string KANIM = "freezer_kanim";
        public override BuildingDef CreateBuildingDef()
        {
              float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
              string[] rawMinerals = MATERIALS.RAW_MINERALS;
              EffectorValues tieR0 = TUNING.NOISE_POLLUTION.NOISY.TIER0;
              EffectorValues tieR1 = TUNING.BUILDINGS.DECOR.BONUS.TIER1;
              EffectorValues noise = tieR0;
              BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                  FreezerConfig.ID, 
                  2, 
                  2, 
                  KANIM, 
                  30, 
                  10f, 
                  tieR4, 
                  MATERIALS.REFINED_METALS,
        800f,
                  BuildLocationRule.OnFloor,
                  tieR1,
                  noise);
              buildingDef.RequiresPowerInput = true;
              buildingDef.EnergyConsumptionWhenActive = 480f;
              buildingDef.SelfHeatKilowattsWhenActive = 0.250f;
              buildingDef.ExhaustKilowattsWhenActive = 0.0f;
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
              storage.capacityKg = 200f;
              storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
              storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
              storage.showCapacityStatusItem = true;
              Prioritizable.AddRef(go);
              go.AddOrGet<TreeFilterable>();
              go.AddOrGet<Freezer>();
              RefrigeratorController.Def def = go.AddOrGetDef<RefrigeratorController.Def>();
              def.powerSaverEnergyUsage = ENERGY_SAVER_POWER;
              def.coolingHeatKW = 0.375f;
              def.steadyHeatKW = 0.0f;
              def.simulatedInternalTemperature = 254.15f;
              go.AddOrGet<UserNameable>();
              go.AddOrGet<DropAllWorkable>();
              go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;
              go.AddOrGetDef<StorageController.Def>();
        }
    }
}