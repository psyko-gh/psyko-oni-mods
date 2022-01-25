using STRINGS;
using static STRINGS.UI;

namespace Psyko.MomentarySwitch
{
    public class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class MOMENTARYSWITCH
                {
                    public static LocString NAME = FormatAsLink("Momentary switch", MomentarySwitchConfig.ID);
                    public static LocString DESC = "Signal switches don't turn grids on and off like power switches, but add an extra signal.";
                    public static LocString EFFECT = "Send a logic pulse before going back to its initial state";
                    public static LocString LOGIC_PORT = "Signal output";
                    public static LocString LOGIC_PORT_ACTIVE = "Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when pressed, for a small duration";
                    public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby);
                    public static LocString SIDESCREEN_TITLE = "Momentary switch";
                    public static LocString SIDESCREEN_DURATION_TOOLTIP = "Will send " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " for <b>{0} seconds</b> when pressed";
                }
            }
        }
    }
}