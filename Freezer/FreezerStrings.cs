using static STRINGS.UI;

namespace Psyko.Freezer
{
    public class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class FREEZER
                {
                    public static LocString NAME = FormatAsLink("Freezer", FreezerConfig.ID);
                    public static LocString DESC = "Food will be kept nice and very cold.";
                    public static LocString EFFECT = "Stores " + FormatAsLink("Food", "FOOD") + " at a very cold " + FormatAsLink("Temperature", "HEAT") + " to stop spoilage.";
                    public static LocString LOGIC_PORT = "Full/Not Full";
                    public static LocString LOGIC_PORT_ACTIVE = "Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when full";
                    public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby);
                }
            }
        }
        
        public static class BUILDING
        {
            public static class STATUSITEMS
            {
                public static class FRIDGECOOLING
                {
                    public static LocString NAME = (LocString) "Cooling Contents: {UsedPower}";
                    public static LocString TOOLTIP = (LocString) "{UsedPower} of {MaxPower} are being used to cool the contents of this food storage";
                }
            
                public static class FRIDGESTEADY
                {
                    public static LocString NAME = (LocString) "Energy Saver: {UsedPower}";
                    public static LocString TOOLTIP = (LocString) "The contents of this food storage are at refrigeration temperatures\n\nEnergy Saver mode has been automatically activated using only {UsedPower} of {MaxPower}";
                }
            }
        }
    }
}

