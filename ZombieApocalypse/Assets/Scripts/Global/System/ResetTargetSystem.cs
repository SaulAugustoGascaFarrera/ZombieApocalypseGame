using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


[UpdateInGroup(typeof(SimulationSystemGroup),OrderFirst = true)]
partial struct ResetTargetSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(RefRW<Target> target in SystemAPI.Query<RefRW<Target>>())
        {
            if(SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                if(!SystemAPI.Exists(target.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity))
                {
                    target.ValueRW.targetEntity = Entity.Null;
                }
            }
        }
    }

    
}
