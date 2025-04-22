using UnityEngine;
using Component = Spectral.Core.Component;

namespace Spectral.Autonation.Components
{
    public record BoundsC : Component
    {
        public Bounds bounds;

        public BoundsC(Bounds bounds = default)
        {
            this.bounds = bounds;
        }
    }
}