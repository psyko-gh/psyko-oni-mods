using KSerialization;
using UnityEngine;
using System.Runtime.Serialization;

namespace Psyko.Freezer
{
    public class Freezer : KMonoBehaviour
    {
        [MyCmpGet]
        private Operational operational;

        [Serialize]
        private float userMaxCapacity = float.NegativeInfinity;

        private static readonly EventSystem.IntraObjectHandler<Freezer> UpdateIciclesDelegate = new EventSystem.IntraObjectHandler<Freezer>((component, data) => component.UpdateIcicles());

        protected override void OnSpawn()
        {
            this.Subscribe((int) GameHashes.OperationalChanged, UpdateIciclesDelegate);
            this.UpdateIcicles();
        }

        protected void UpdateIcicles()
        {
            KBatchedAnimController component = this.GetComponent<KBatchedAnimController>();
            // Add all icicles symbols from anim
            component.SetSymbolVisiblity((KAnimHashedString) "ice", this.operational.IsOperational); 
            component.SetSymbolVisiblity((KAnimHashedString) "ice_b", this.operational.IsOperational); 
            component.SetSymbolVisiblity((KAnimHashedString) "gasframes", this.operational.IsOperational); 
        }

        [OnDeserialized]
        public void OnDeserialize()
        {
            if( userMaxCapacity == float.NegativeInfinity )
                return;
            // Backwards compatibility. Read from old version that copy&pasted Refrigerator.
            GetComponent< Refrigerator >().UserMaxCapacity = userMaxCapacity;
            userMaxCapacity = float.NegativeInfinity;
        }
    }
}
