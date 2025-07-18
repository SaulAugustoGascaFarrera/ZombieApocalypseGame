using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct TestDeathSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<LocalTransform> localTransform,RefRW<Health> health,Entity entity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<Health>>().WithEntityAccess())
        {
            if(health.ValueRO.healthAmount <= 0)
            {
                entityCommandBuffer.DestroyEntity(entity);
            }
        }

    }

    
}
