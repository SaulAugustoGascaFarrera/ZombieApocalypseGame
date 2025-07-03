using Unity.Entities;
using UnityEditor.Rendering;
using UnityEngine;

public class FindTargetAuthoring : MonoBehaviour
{
    public float timerMax;

    public FactionType targetFaction;
    public class Baker : Baker<FindTargetAuthoring>
    {
        public override void Bake(FindTargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new FindTarget
            {
                timerMax = authoring.timerMax,
                targetFaction = authoring.targetFaction,
            });
        }
    }
}

public struct FindTarget : IComponentData
{
    public float timer;
    public float timerMax;

    public FactionType targetFaction; 
}
