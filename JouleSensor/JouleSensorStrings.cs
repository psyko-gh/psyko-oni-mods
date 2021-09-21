using static STRINGS.UI;

namespace JouleSensor
{
    public class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class JOULESENSOR
                {
                    public static LocString NAME = FormatAsLink("Joule Sensor", JouleSensorConfig.ID);
                    public static LocString DESC = "A Joule Sensor.";
                    public static LocString EFFECT = $"Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " or a " + FormatAsAutomationState("Red Signal", AutomationState.Standby) + " when " + FormatAsLink("Power", "POWER") + " available enters the chosen range.";
                    public static LocString TITLE = $"Power storage";
                }
            }
        }

        public class THRESHOLD_SWITCH_SIDESCREEN
        {
            public static LocString POWER = (LocString) "power reading";
            public static LocString POWER_TOOLTIP_ABOVE = (LocString) ("Will send a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " if the " + PRE_KEYWORD + "Power" + PST_KEYWORD + " available is above <b>{0}</b>");
            public static LocString POWER_TOOLTIP_BELOW = (LocString) ("Will send a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " if the " + PRE_KEYWORD + "Power" + PST_KEYWORD + " available is below <b>{0}</b>");

        }
    }
}