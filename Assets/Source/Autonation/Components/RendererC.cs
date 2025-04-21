using Spectral.Autonation.Managers;
using Spectral.Core;
using UnityEngine;
using Component = Spectral.Core.Component;

namespace Spectral.Autonation.Components
{
    public class RendererC : Component
    {
        private readonly Material _material;
        private readonly Mesh _mesh;
        private uint _componentId;

        public RendererC(uint componentId, int entityIndex, Mesh mesh, Material material)
        {
            _componentId = componentId;
            this.entityIndex = entityIndex;
            _mesh = mesh;
            _material = material;
        }

        public EnumComponentType ComponentType => EnumComponentType.RendererComponent;

        public void LinkToEntityGameObject()
        {
            EntityDatabase.Instance.Entities.data[entityIndex].linkedGameObject.AddComponent<MeshFilter>().sharedMesh = _mesh;
            EntityDatabase.Instance.Entities.data[entityIndex].linkedGameObject.AddComponent<MeshRenderer>().sharedMaterial = _material;
        }
    }
}