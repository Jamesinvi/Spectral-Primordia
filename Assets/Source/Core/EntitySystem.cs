using System;
using System.Runtime.CompilerServices;
using Unity.Profiling;

namespace Spectral.Core
{
    public abstract class EntitySystem : IEquatable<EntitySystem>
    {
        public readonly string name;
        public readonly UpdateType updateType;
        private ProfilerMarker _marker;
        public bool shouldUpdate;

        protected EntitySystem(string name, UpdateType updateType)
        {
            this.name = name;
            this.updateType = updateType;
            _marker = new ProfilerMarker(this.name);
        }

        public bool Equals(EntitySystem other)
        {
            if (other is null) return false;
            return name == other.name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(float dt)
        {
            using (_marker.Auto())
            {
                Tick(dt);
            }
        }

        protected abstract void Tick(float dt);

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EntitySystem)obj);
        }

        public override int GetHashCode()
        {
            return name != null ? name.GetHashCode() : 0;
        }
    }

    public enum UpdateType
    {
        Update, // Per-frame
        LateUpdate, // Per-frame but after all Updates
        FixedUpdate, // Fixed tick defined by Engine
        SlowUpdate, // Once every 1/10 of a second
        EverySecond // Once every second
    }
}