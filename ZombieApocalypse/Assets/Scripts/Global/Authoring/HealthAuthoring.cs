using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{

    public int healthAmount;
    public int healthmountMax;
    public class Baker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Health
            {
                healthAmount = authoring.healthAmount,
                healthmountMax = authoring.healthmountMax
            });
        }
    }
}

public struct Health : IComponentData
{
    public bool onHealthChanged;
    public int healthAmount;
    public int healthmountMax;
}
