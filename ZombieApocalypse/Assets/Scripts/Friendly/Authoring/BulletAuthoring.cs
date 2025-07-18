using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{

    public float movementSpeed;
    public int damageAmount;
    public class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Bullet
            {
                movementSpeed = authoring.movementSpeed,
                damageAmount = authoring.damageAmount,
            });
        }
    }
}

public struct Bullet : IComponentData
{
    public float movementSpeed;
    public int damageAmount;


}
