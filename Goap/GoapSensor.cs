using UnityEngine;

namespace RareChip.Goap
{
    public abstract class GoapSensor : MonoBehaviour
    {
        public abstract void InitializeConditions(GoapAgent agent);
    }
}
