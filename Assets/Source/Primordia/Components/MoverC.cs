using UnityEngine;
using Core_Component = Primordia.Core.Component;

namespace Primordia.Primordia.Components
{
    public record MoverC : Core_Component
    {
        public Vector3 direction;
        public float speed;

        public MoverC(uint componentId, int entityIndex, float speed, Vector3 dir) : base(entityIndex)
        {
            direction = dir;
            this.componentId = componentId;
            this.speed = speed;
        }
    }
}