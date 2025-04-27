using UnityEngine;
using Core_Component = Primordia.Core.Component;

namespace Primordia.Primordia.Components
{
    public record BoundsC : Core_Component
    {
        public Bounds bounds;

        public BoundsC(int entityIndex, Bounds bounds = default) : base(entityIndex)
        {
            this.bounds = bounds;
        }
    }
}