using UnityEngine;
using Component = Spectral.Core.Component;

namespace Spectral.Autonation.Components
{
    public record BoundsC : Component
    {
        public Bounds bounds;

        public BoundsC(int entityIndex, Bounds bounds = default) : base(entityIndex)
        {
            this.bounds = bounds;
        }
    }
}