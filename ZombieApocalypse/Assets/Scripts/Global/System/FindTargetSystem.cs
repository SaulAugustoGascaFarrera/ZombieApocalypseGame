using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct FindTargetSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<GameAssets>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);

        foreach ((RefRW<LocalTransform> localTransform,RefRW<FindTarget> findTarget,RefRW<Target> target) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<FindTarget>,RefRW<Target>>())
        {

            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if(findTarget.ValueRO.timer > 0.0f)
            {
                continue;
            }
            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;

            distanceHitList.Clear();

            CollisionFilter collisionFilter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UNIT_LAYER,
                GroupIndex = 0
            };


            if(collisionWorld.OverlapSphere(localTransform.ValueRO.Position,findTarget.ValueRO.findDistanceRange,ref distanceHitList,collisionFilter))
            {
                foreach (DistanceHit distanceHit in distanceHitList)
                {

                    if(!SystemAPI.Exists(distanceHit.Entity) || !SystemAPI.HasComponent<Faction>(distanceHit.Entity))
                    {
                        continue;
                    }

                    Faction faction = SystemAPI.GetComponent<Faction>(distanceHit.Entity);

                    if (faction.factionType == findTarget.ValueRO.targetFaction)
                    {
                        target.ValueRW.targetEntity = distanceHit.Entity;
                    }

                    

                }
            }


        }
    }

    
}
