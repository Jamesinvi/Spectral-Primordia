using System;
using Spectral.Autonation.Managers;
using Spectral.Core;

namespace Spectral.Autonation.Systems
{
    public class GenerateResourcesSystem : EntitySystem
    {
        public const UpdateType GenerateResourceSystemUpdateType = UpdateType.EverySecond;
        private readonly DynamicArray<Entity> _toIterate;

        public GenerateResourcesSystem(string name) : base(name, GenerateResourceSystemUpdateType)
        {
            _toIterate = new DynamicArray<Entity>(1000);
            GameManager.Instance.AddSystemToUpdateList(this);
        }

        protected override void Tick(float dt)
        {
            _toIterate.Clear();
            Span<EnumComponentType> typesToQuery = stackalloc EnumComponentType[1]
            {
                EnumComponentType.GenerateResourceComponent
            };
            EntityDatabase.Instance.EntitiesWithComponents(_toIterate, typesToQuery);

            foreach (Entity entity in _toIterate.AsSpan())
            {
                ResourcesManager.Instance.oxygen += EntityDatabase.Instance.resourceGenerators.data[entity].oxygenPerSecond;
                ResourcesManager.Instance.hydrogen += EntityDatabase.Instance.resourceGenerators.data[entity].hydrogenPerSecond;
            }
        }
    }
}