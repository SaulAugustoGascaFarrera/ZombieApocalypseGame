using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletMoverSystem : ISystem
{
   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<LocalTransform> localTransform,RefRW<Target> target,RefRO<Bullet> bullet,Entity entity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<Target>,RefRO<Bullet>>().WithEntityAccess())
        {
            if(!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            //UnityEngine.Debug.Log("Entre a Bullet mover");

            LocalTransform targetLocalTranform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            ShootVictim shootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);

            float3 targetLocation =  targetLocalTranform.TransformPoint(shootVictim.hitLocation);

            float distanceSQBefore = math.distancesq(localTransform.ValueRO.Position, targetLocation);

            float3 moveDirection = targetLocation - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.movementSpeed * SystemAPI.Time.DeltaTime;

            float distanceSQAfter = math.distancesq(localTransform.ValueRO.Position, targetLocation);

            if(distanceSQAfter > distanceSQBefore)
            {
                localTransform.ValueRW.Position = targetLocalTranform.Position;
            }

            if(math.distancesq(localTransform.ValueRO.Position, targetLocation) < UnitMoverSystem.UNIT_MOVER_REACHED_DISTANCE_SQ)
            {
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.onHealthChanged = true;
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;

                entityCommandBuffer.DestroyEntity(entity);
            }

        }
    }

    
}
