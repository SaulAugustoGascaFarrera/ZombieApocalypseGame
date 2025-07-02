using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMoverSystem : ISystem
{
    public const float UNIT_MOVER_REACHED_DISTANCE_SQ = 0.2f;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRW<LocalTransform> localTransform,RefRW<UnitMover> unitMover,RefRW<PhysicsVelocity> physicsVelocity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<UnitMover>,RefRW<PhysicsVelocity>>())
        {
            UnitMoverJob unitMoverJob = new UnitMoverJob{
                deltaTime = SystemAPI.Time.DeltaTime
            };

            unitMoverJob.ScheduleParallel();
        }
    }

    
}


[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{

    public float deltaTime;
    public void Execute(ref LocalTransform localTransform,ref UnitMover unitMover,ref PhysicsVelocity physicsVelocity)
    {
        float3 moveDirection = (unitMover.targetPosition - localTransform.Position);

        moveDirection = math.normalize(moveDirection);

        if(math.distancesq(unitMover.targetPosition,localTransform.Position) > UnitMoverSystem.UNIT_MOVER_REACHED_DISTANCE_SQ)
        {
            localTransform.Rotation = math.slerp(localTransform.Rotation, quaternion.LookRotation(moveDirection, math.up()), unitMover.rotationSpeed * deltaTime);


            physicsVelocity.Linear = moveDirection * unitMover.movementSpeed;

            physicsVelocity.Angular = 0.0f;
        }
        else
        {
            physicsVelocity.Linear = 0.0f;
            physicsVelocity.Angular = 0.0f;
        }


    }
}
