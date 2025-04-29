using Primordia.Core;
using UnityEngine;
using Core_Component = Primordia.Core.Component;

namespace Primordia.Components
{
    public record TransformC : Core_Component
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformC(uint componentId, int entityIndex, Vector3 position, Quaternion rotation, Vector3 scale) : base(entityIndex)
        {
            this.scale = scale;
            this.rotation = rotation;
            this.position = position;
            this.componentId = componentId;
            this.entityIndex = entityIndex;
        }

        public EnumComponentType ComponentType => EnumComponentType.TransformComponent;

        public void Translate(Vector3 translation)
        {
            position += translation;
        }
    }
}