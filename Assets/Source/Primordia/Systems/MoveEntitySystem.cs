using System;
using Primordia.Components;
using Primordia.Core;
using Primordia.Managers;

namespace Primordia.Systems
{
    public class MoveEntitySystem : EntitySystem
    {
        private const UpdateType MoveSystemUpdateType = UpdateType.Update;
        private readonly DynamicArray<Entity> _toIterate;

        public MoveEntitySystem(string name) : base(name, MoveSystemUpdateType)
        {
            _toIterate = new DynamicArray<Entity>(1000);
            GameManager.Instance.AddSystemToUpdateList(this);
        }

        protected override void Tick(float dt)
        {
            Span<EnumComponentType> typesToQuery = stackalloc EnumComponentType[1] { EnumComponentType.MoverComponent };
            var toMove = EntityDatabase.Instance.EntitiesWithComponents(_toIterate, typesToQuery);
            foreach (Entity entity in toMove.AsSpan())
            {
                int index = entity.index;
                TransformC transformC = EntityDatabase.Instance.transforms.data[entity.index];
                transformC.Translate(EntityDatabase.Instance.movers.data[index].direction * (EntityDatabase.Instance.movers.data[index].speed * dt));
                entity.linkedTransform.SetLocalPositionAndRotation(transformC.position, transformC.rotation);
                entity.linkedTransform.localScale = transformC.scale;
            }
        }
    }
}