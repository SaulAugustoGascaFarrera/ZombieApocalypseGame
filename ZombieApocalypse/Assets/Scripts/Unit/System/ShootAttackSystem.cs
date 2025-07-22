using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach((RefRW<ShootAttack> shootAttack,RefRW<Target> target,RefRW<LocalTransform> localTransform,RefRW<UnitMover> unitMover,Entity entity) in SystemAPI.Query<RefRW<ShootAttack>,RefRW<Target>,RefRW<LocalTransform>,RefRW<UnitMover>>().WithDisabled<MoveOverride>().WithEntityAccess())
        {
            if(!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                continue;
            }

            

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);


            if(math.distance(localTransform.ValueRO.Position,targetLocalTransform.Position) > shootAttack.ValueRO.shootAttackDistance)
            {
                //too far ,move closer
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                continue;
            }
            else
            {
                //close enough, stop moving and attack
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            }

            float3 aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;

            aimDirection = math.normalize(aimDirection);

            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, quaternion.LookRotation(aimDirection, math.up()), unitMover.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);
            


            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if(shootAttack.ValueRO.timer > 0.0f)
            {
                continue;
            }

            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;


            if(SystemAPI.HasComponent<TargetOverride>(target.ValueRO.targetEntity))
            {
                RefRW<TargetOverride> enemyTargetOverride = SystemAPI.GetComponentRW<TargetOverride>(target.ValueRO.targetEntity);

                if(!SystemAPI.Exists(enemyTargetOverride.ValueRO.targetEntity))
                {
                    enemyTargetOverride.ValueRW.targetEntity = entity;
                }
            }

            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletEntity);
            float3 bulletSpawnPoint = localTransform.ValueRW.TransformPoint(shootAttack.ValueRO.shootSpawnPoint);
            SystemAPI.SetComponent(bulletEntity,LocalTransform.FromPosition(bulletSpawnPoint));


            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;


        }

    }

    
}
