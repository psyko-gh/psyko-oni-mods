using System;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace Psyko.Freezer
{
    [Serializable][RestartRequired]
    public class FreezerOptions: SingletonOptions<FreezerOptions>
    {
        [JsonProperty]
        [Option("Capocity (kg)", "Determines the capacity of the freezer in kg.", Format = "F1")]
        [Limit(10f, 10000f)]
        public float Capacity { get; set; }

        [JsonProperty]
        [Option("Energy saver power (watts)", "Determines the power consumed in Watts when the freezer is in power saver mode.", Format = "F1")]
        [Limit(10f, 1000f)]
        public float EnergySaverPower { get; set; }

        [JsonProperty]
        [Option("Energy active power (watts)", "Determines the power consumed in Watts when the freezer is in active cooling mode.", Format = "F1")]
        [Limit(100f, 1000f)]
        public float EnergyActivePower { get; set; }
        
        public FreezerOptions()
        {
            Capacity = FreezerConfig.CAPACITY_KG;
            EnergySaverPower = FreezerConfig.ENERGY_SAVER_POWER;
            EnergyActivePower = FreezerConfig.ENERGY_ACTIVE_POWER;
        }
    }
}