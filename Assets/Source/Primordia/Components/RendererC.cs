using Primordia.Core;
using Primordia.Managers;
using UnityEngine;
using Core_Component = Primordia.Core.Component;

namespace Primordia.Components
{
    public record RendererC : Core_Component
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