using System;
using Spectral.Autonation.Components;
using Spectral.Autonation.Managers;
using Spectral.Core;

namespace Spectral.Autonation.Systems
{
    public class MoveEntitySystem : EntitySystem
    {
        private readonly DynamicArray<Entity> _toIterate;

        public MoveEntitySystem(string name, UpdateType update) : base(name, update)
        {
            _toIterate = new DynamicArray<Entity>(1000);
            GameManager.Instance.AddSystemToUpdateList(this);
        }

        protected override void Tick(float dt)
        {
            Span<EnumComponentType> typesToQuery = stackalloc EnumComponentType[1] { EnumComponentType.MoverComponent };
            var toMove = EntityDatabase.Instance.EntitiesWithComponents(_toIterate, typesToQuery);
            foreach (var entity in toMove.AsSpan())
            {
                var index = entity.entityID;
                TransformC transformC = EntityDatabase.Instance.transforms.data[entity.entityID];
                transformC.Translate(EntityDatabase.Instance.movers.data[index].direction * (EntityDatabase.Instance.movers.data[index].speed * dt));
                entity.linkedTransform.SetLocalPositionAndRotation(transformC.position, transformC.rotation);
                entity.linkedTransform.localScale = transformC.scale;
            }
        }
    }
}