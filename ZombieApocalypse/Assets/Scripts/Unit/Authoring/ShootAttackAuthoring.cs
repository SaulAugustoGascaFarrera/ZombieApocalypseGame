using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ShootAttackAuthoring : MonoBehaviour
{

    public float timerMax;
    public Transform shootSpawnPointTransform;
    public float shootAttackDistance;
    public class Baker : Baker<ShootAttackAuthoring>
    {
        public override void Bake(ShootAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ShootAttack
            {
                timerMax = authoring.timerMax,
                shootSpawnPoint = authoring.shootSpawnPointTransform.localPosition,
                shootAttackDistance = authoring.shootAttackDistance,

            });
        }
    }
}

public struct ShootAttack : IComponentData
{
    //public int shootDamageAmount;
    public float timer;
    public float timerMax;
    public float3 shootSpawnPoint;
    public float shootAttackDistance;
}
