using JetBrains.Annotations;
using Unity.Burst;
using Unity.Entities;


[UpdateInGroup(typeof(LateSimulationSystemGroup),OrderLast = true)]
partial struct ResetEventsSystem : ISystem
{
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new ResetHelathEventsJob().ScheduleParallel();
        new ResetSelectedEventsJob().ScheduleParallel();
    }

   
}



[BurstCompile]
public partial struct ResetHelathEventsJob : IJobEntity
{
    public void Execute(ref Health health)
    {
        health.onHealthChanged = false;
    }
}


[BurstCompile]
[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
public partial struct ResetSelectedEventsJob : IJobEntity
{
    public void Execute(ref Selected selected)
    {
        selected.onSelected = false;
        selected.onDeselected = false;
    }
}
