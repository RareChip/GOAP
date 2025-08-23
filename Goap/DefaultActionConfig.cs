using RareChip.Serialization;
using UnityEngine;

namespace RareChip.Goap
{
    [CreateAssetMenu(fileName = "Action Config", menuName = "Scriptable Objects/GOAP/Action Configs/DefaultActionConfig")]
    public class DefaultActionConfig : ActionConfig
    {
        [SerializeReference] public IActionStrategy actionStrategy;
        protected override IActionStrategy GetStrategy() => actionStrategy;
    }
}