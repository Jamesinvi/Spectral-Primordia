using System;

namespace Spectral.Core
{
    public abstract class Component : IEquatable<Component>
    {
        public uint componentId;
        public EnumComponentType componentType;
        public int entityIndex;

        public bool Equals(Component other)
        {
            if (other is null) return false;
            return componentId == other.componentId && entityIndex == other.entityIndex && componentType == other.componentType;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Component)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(componentId, entityIndex, (int)componentType);
        }
    }
}