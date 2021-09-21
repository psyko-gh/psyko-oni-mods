using KSerialization;
using STRINGS;
using UnityEngine;

namespace JouleSensor
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class JouleSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
    {
        [Serialize]
        public float thresholdPower;
        [Serialize]
        public bool activateOnHigherThan;
        [Serialize]
        public bool dirty = true;

        private float currentPower;
        private bool wasOn;

        private readonly float minPower = 1000f;
        private readonly float maxPower = 100000f;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.OnToggle += new System.Action<bool>(this.OnSwitchToggled);
            this.UpdateVisualState(true);
            this.UpdateLogicCircuit();
            this.wasOn = this.switchedOn;
        }

        public void Sim200ms(float dt)
        {
            ushort circuitID = Game.Instance.circuitManager.GetCircuitID(Grid.PosToCell((KMonoBehaviour) this));
            if (circuitID < 0 || !Game.Instance.circuitManager.HasBatteries(circuitID))
            {
                if (this.IsSwitchedOn)
                    this.Toggle();
                return;
            }
            this.currentPower = Game.Instance.circuitManager.GetJoulesAvailableOnCircuit(circuitID);
            this.currentPower = Mathf.Max(0.0f, Mathf.Round(this.currentPower));

            if (this.activateOnHigherThan)
            {
                if (((double) this.currentPower <= (double) this.thresholdPower || this.IsSwitchedOn) 
                    && ((double) this.currentPower > (double) this.thresholdPower || !this.IsSwitchedOn))
                    return;
                this.Toggle();
            }
            else
            {
                if (((double) this.currentPower < (double) this.thresholdPower || !this.IsSwitchedOn) 
                    && ((double) this.currentPower >= (double) this.thresholdPower || this.IsSwitchedOn))
                    return;
                this.Toggle();
            }
        }

        public float GetPowerStored() => this.currentPower;

        private void OnSwitchToggled(bool toggled_on)
        {
            this.UpdateVisualState();
            this.UpdateLogicCircuit();
        }

        private void UpdateLogicCircuit() => this.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0);

        private void UpdateVisualState(bool force = false)
        {
            if (!(this.wasOn != this.switchedOn | force))
                return;
            this.wasOn = this.switchedOn;
            KBatchedAnimController component = this.GetComponent<KBatchedAnimController>();
            component.Play((HashedString)(this.switchedOn ? "on_pre" : "on_pst"));
            component.Queue((HashedString)(this.switchedOn ? "on" : "off"));
        }

        protected override void UpdateSwitchStatus()
        {
            StatusItem status_item = this.switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive;
            this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
        }

        public float Threshold
        {
            get => this.thresholdPower;
            set
            {
                this.thresholdPower = value;
                this.dirty = true;
            }
        }

        public bool ActivateAboveThreshold
        {
            get => this.activateOnHigherThan;
            set
            {
                this.activateOnHigherThan = value;
                this.dirty = true;
            }
        }

        public float CurrentValue => this.GetPowerStored();

        public float RangeMin => this.minPower;

        public float RangeMax => this.maxPower;

        public float GetRangeMinInputField() => this.minPower;

        public float GetRangeMaxInputField() => this.maxPower;

        public LocString ThresholdValueUnits() => $"";

        public string Format(float value, bool units) => GameUtil.GetFormattedRoundedJoules(value);

        public float ProcessedSliderValue(float input) => Mathf.Round(input);

        public float ProcessedInputValue(float input) => input;

        public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

        public int IncrementScale => 10;

        public LocString Title => STRINGS.BUILDINGS.PREFABS.JOULESENSOR.TITLE;

        public LocString ThresholdValueName => STRINGS.THRESHOLD_SWITCH_SIDESCREEN.POWER;

        public string AboveToolTip => (string)  STRINGS.THRESHOLD_SWITCH_SIDESCREEN.POWER_TOOLTIP_ABOVE;

        public string BelowToolTip => (string)  STRINGS.THRESHOLD_SWITCH_SIDESCREEN.POWER_TOOLTIP_BELOW;

        public NonLinearSlider.Range[] GetRanges => new NonLinearSlider.Range[3]
        {
            new NonLinearSlider.Range(10f, 1000f),
            new NonLinearSlider.Range(50f, 50000f),
            new NonLinearSlider.Range(40f, this.maxPower)
        };
    }
}
