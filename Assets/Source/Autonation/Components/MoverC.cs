using UnityEngine;
using Component = Spectral.Core.Component;

namespace Spectral.Autonation.Components
{
    public record MoverC : Component
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