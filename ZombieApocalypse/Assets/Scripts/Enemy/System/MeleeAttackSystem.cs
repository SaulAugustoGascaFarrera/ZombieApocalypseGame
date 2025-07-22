using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MeleeAttackSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRW<LocalTransform> localTransform,RefRW<Target> target,RefRW<MeleeAttack> meleeAttack,RefRW<UnitMover> unitMover) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<Target>,RefRW<MeleeAttack>,RefRW<UnitMover>>())
        {
            if(!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            float3 moveDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;

            moveDirection = math.normalize(moveDirection);

            if(math.distancesq(targetLocalTransform.Position,localTransform.ValueRO.Position) > 2.0f)
            {
                //localTransform.ValueRW.Position += moveDirection * unitMover.ValueRO.movementSpeed * SystemAPI.Time.DeltaTime;

                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;

                //localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, quaternion.LookRotation(moveDirection, math.up()), unitMover.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);

                //continue;
            }
            else
            {
                unitMover.ValueRW.targetPosition = localTransform.ValueRW.Position;


                meleeAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

                if(meleeAttack.ValueRO.timer > 0.0f)
                {
                    continue;
                }

                meleeAttack.ValueRW.timer = meleeAttack.ValueRW.timerMax;


                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.onHealthChanged = true;
                targetHealth.ValueRW.healthAmount -= meleeAttack.ValueRO.damageAmount;

                
            }

        }
    }

}
