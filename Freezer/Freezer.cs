using KSerialization;
using UnityEngine;

namespace Psyko.Freezer
{
    public class Freezer : KMonoBehaviour, IUserControlledCapacity
    {
        [MyCmpGet] 
        private Storage storage;
        [MyCmpGet] 
        private Operational operational;
        [MyCmpGet] 
        private LogicPorts ports;
        [Serialize] 
        private float userMaxCapacity = float.PositiveInfinity;
        private FilteredStorage filteredStorage;

        private static readonly EventSystem.IntraObjectHandler<Freezer> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Freezer>((component, data) => component.OnCopySettings(data));

        private static readonly EventSystem.IntraObjectHandler<Freezer> UpdateLogicCircuitCBDelegate = new EventSystem.IntraObjectHandler<Freezer>((component, data) => component.UpdateLogicCircuitCB(data));
        
        private static readonly EventSystem.IntraObjectHandler<Freezer> UpdateIciclesDelegate = new EventSystem.IntraObjectHandler<Freezer>((component, data) => component.UpdateIcicles());
        
        protected override void OnPrefabInit() => this.filteredStorage = new FilteredStorage(
            this,
            null, 
            new Tag[1]
            {
                GameTags.Compostable
            }, 
            this, 
            true, 
            Db.Get().ChoreTypes.FoodFetch); 

        protected override void OnSpawn()
        {
            Debug.Log("1");
            this.GetComponent<KAnimControllerBase>().Play((HashedString) "off");
            Debug.Log("2");
            this.filteredStorage.FilterChanged();
            Debug.Log("3");
            this.UpdateLogicCircuit();
            Debug.Log("4");
            this.Subscribe((int) GameHashes.CopySettings, OnCopySettingsDelegate);
            this.Subscribe((int) GameHashes.OnStorageChange, UpdateLogicCircuitCBDelegate);
            this.Subscribe((int) GameHashes.OperationalChanged, UpdateLogicCircuitCBDelegate);
            this.Subscribe((int) GameHashes.OperationalChanged, UpdateIciclesDelegate);
            Debug.Log("5");
            this.UpdateIcicles();
            Debug.Log("6");
        }
 
        protected override void OnCleanUp() => this.filteredStorage.CleanUp();

        public bool IsActive() => this.operational.IsActive;

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject) data;
            if (gameObject == null)
                return;
            Freezer freezerComponent = gameObject.GetComponent<Freezer>();
            if (freezerComponent == null)
                return;
            this.UserMaxCapacity = freezerComponent.UserMaxCapacity;
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
        
        protected void UpdateIcicles()
        {
            KBatchedAnimController component = this.GetComponent<KBatchedAnimController>();
            // Add all icicles symbols from anim
            component.SetSymbolVisiblity((KAnimHashedString) "ice", this.operational.IsOperational); 
            component.SetSymbolVisiblity((KAnimHashedString) "ice_b", this.operational.IsOperational); 
            component.SetSymbolVisiblity((KAnimHashedString) "gasframes", this.operational.IsOperational); 
        }
    }
}