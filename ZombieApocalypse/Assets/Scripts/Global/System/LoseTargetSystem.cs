using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct LoseTargetSystem : ISystem
{
   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRO<LocalTransform> localTransform,RefRW<LoseTarget> loseTarget,RefRW<Target> target) in SystemAPI.Query<RefRO<LocalTransform>,RefRW<LoseTarget>,RefRW<Target>>())
        {
            if(!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            float targetDistance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);

            if(targetDistance > loseTarget.ValueRO.loseTargetDistance)
            {
                //target is too far, reset it
                target.ValueRW.targetEntity = Entity.Null;
            }
        }
    }

   
}
