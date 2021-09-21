using KSerialization;
using STRINGS;
using UnityEngine;

namespace Freezer
{
    public class Freezer : KMonoBehaviour, IUserControlledCapacity
    {
        [MyCmpGet] private Storage storage;
        [MyCmpGet] private Operational operational;
        [MyCmpGet] private LogicPorts ports;
        [Serialize] private float userMaxCapacity = float.PositiveInfinity;
        private FilteredStorage filteredStorage;

        private static readonly EventSystem.IntraObjectHandler<Freezer> OnCopySettingsDelegate =
            new EventSystem.IntraObjectHandler<Freezer>(
                (System.Action<Freezer, object>) ((component, data) => component.OnCopySettings(data)));

        private static readonly EventSystem.IntraObjectHandler<Freezer> UpdateLogicCircuitCBDelegate =
            new EventSystem.IntraObjectHandler<Freezer>(
                (System.Action<Freezer, object>) ((component, data) => component.UpdateLogicCircuitCB(data)));

        protected override void OnPrefabInit() => this.filteredStorage = new FilteredStorage((KMonoBehaviour) this,
            (Tag[]) null, new Tag[1]
            {
                GameTags.Compostable
            }, (IUserControlledCapacity) this, true, Db.Get().ChoreTypes.FoodFetch);

        protected override void OnSpawn()
        {
            this.GetComponent<KAnimControllerBase>().Play((HashedString) "off");
            this.filteredStorage.FilterChanged();
            this.UpdateLogicCircuit();
            this.Subscribe<Freezer>((int) GameHashes.CopySettings, Freezer.OnCopySettingsDelegate);
            this.Subscribe<Freezer>((int) GameHashes.OnStorageChange, Freezer.UpdateLogicCircuitCBDelegate);
            this.Subscribe<Freezer>((int) GameHashes.OperationalChanged, Freezer.UpdateLogicCircuitCBDelegate);
        }

        protected override void OnCleanUp() => this.filteredStorage.CleanUp();

        public bool IsActive() => this.operational.IsActive;

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject) data;
            if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
                return;
            Freezer component = gameObject.GetComponent<Freezer>();
            if ((UnityEngine.Object) component == (UnityEngine.Object) null)
                return;
            this.UserMaxCapacity = component.UserMaxCapacity;
        }

        public float UserMaxCapacity
        {
            get => Mathf.Min(this.userMaxCapacity, this.storage.capacityKg);
            set
            {
                this.userMaxCapacity = value;
                this.filteredStorage.FilterChanged();
                this.UpdateLogicCircuit();
            }
        }

        public float AmountStored => this.storage.MassStored();

        public float MinCapacity => 0.0f;

        public float MaxCapacity => this.storage.capacityKg;

        public bool WholeValues => false;

        public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

        private void UpdateLogicCircuitCB(object data) => this.UpdateLogicCircuit();

        private void UpdateLogicCircuit()
        {
            bool on = this.filteredStorage.IsFull() & this.operational.IsOperational;
            this.ports.SendSignal(FilteredStorage.FULL_PORT_ID, on ? 1 : 0);
            this.filteredStorage.SetLogicMeter(on);
        }
    }
}