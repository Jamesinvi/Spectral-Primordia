using Spectral.Core;
using UnityEngine;
using Component = Spectral.Core.Component;

namespace Spectral.Autonation.Components
{
    public class TransformC : Component
    {
        public EnumComponentType ComponentType => EnumComponentType.TransformComponent;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformC(uint componentId, int entityIndex, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.scale = scale;
            this.rotation = rotation;
            this.position = position;
            this.componentId = componentId;
            this.entityIndex = entityIndex;
        }
        
        public void Translate(Vector3 translation)
        {
            position += translation;
        }
    }
}