using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{

    public event EventHandler OnSelectionStart;
    public event EventHandler OnSelectionEnd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            OnSelectionStart?.Invoke(this, EventArgs.Empty);
        }

        if(Input.GetMouseButtonUp(0))
        {
            OnSelectionEnd?.Invoke(this, EventArgs.Empty);


            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected,Unit>().Build(entityManager);


            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

            NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

            for(int i=0;i<entityArray.Length;i++)
            {
                Selected selected = selectedArray[i];
                
                selected.onSelected = false;
                selected.onDeselected = true;

                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
                entityManager.SetComponentData(entityArray[i], selected);
            }


       

            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform,Unit>().WithPresent<Selected>().Build(entityManager);

            entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));

            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();

            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

            UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastInput raycastInput = new RaycastInput
            {
                Start = cameraRay.GetPoint(0.0f),
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.UNIT_LAYER,
                    GroupIndex = 0
                },
                End = cameraRay.GetPoint(9999.0f)
            };   


            if(collisionWorld.CastRay(raycastInput,out Unity.Physics.RaycastHit raycastHit))
            {

                entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                Selected  selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                selected.onSelected = true;
                selected.onDeselected = false;
                entityManager.SetComponentData(raycastHit.Entity, selected);

            }


        }

        if(Input.GetMouseButton(1))
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<MoveOverride>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<MoveOverride> moveOverrideArray = entityQuery.ToComponentDataArray<MoveOverride>(Allocator.Temp);


            Vector3 mousePosition = MouseManager.Instance.GetMousePosition();

            for(int i=0;i<entityArray.Length;i++)
            {
                MoveOverride moveOverride = moveOverrideArray[i];

                moveOverride.targetPosition = mousePosition;

                moveOverrideArray[i] = moveOverride;

                entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], true);
            }

            entityQuery.CopyFromComponentDataArray(moveOverrideArray);

        }
    }
}
