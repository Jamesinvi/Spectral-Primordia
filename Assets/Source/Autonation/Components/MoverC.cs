using UnityEngine;
using Component = Spectral.Core.Component;

namespace Spectral.Autonation.Components
{
    public class MoverC : Component
    {
        public Vector3 direction;
        public float speed;

        public MoverC(uint componentId, int entityIndex, float speed, Vector3 dir)
        {
            direction = dir;
            this.entityIndex = entityIndex;
            this.componentId = componentId;
            this.speed = speed;
        }
    }
}