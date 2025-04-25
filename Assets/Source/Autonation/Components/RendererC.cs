using Spectral.Autonation.Managers;
using Spectral.Core;
using UnityEngine;
using Component = Spectral.Core.Component;

namespace Spectral.Autonation.Components
{
    public record RendererC : Component
    {
        private readonly Material _material;
        private readonly Mesh _mesh;

        public RendererC(uint componentId, int entityIndex, Mesh mesh, Material material) : base(entityIndex)
        {
            this.componentId = componentId;
            _mesh = mesh;
            _material = material;
        }

        public EnumComponentType ComponentType => EnumComponentType.RendererComponent;

        public void LinkToEntityGameObject()
        {
            EntityDatabase.Instance.entities.data[entityIndex].linkedGameObject.AddComponent<MeshFilter>().sharedMesh = _mesh;
            EntityDatabase.Instance.entities.data[entityIndex].linkedGameObject.AddComponent<MeshRenderer>().sharedMaterial = _material;
        }
    }
}