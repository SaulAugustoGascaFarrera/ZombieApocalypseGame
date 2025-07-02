using Unity.Entities;
using UnityEngine;

public class SelectedAuthoring : MonoBehaviour
{
    public class Baker : Baker<SelectedAuthoring>
    {
        public override void Bake(SelectedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Selected
            {
                onSelected = false,
                onDeselected = false,
            });

            SetComponentEnabled<Selected>(entity, false);
        }
    }
}

public struct Selected : IComponentData,IEnableableComponent
{
    public bool onSelected;
    public bool onDeselected;
}
