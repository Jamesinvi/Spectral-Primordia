using System;
using UnityEngine;

namespace Spectral.Core
{
    public class Entity : IEquatable<Entity>
    {
        public readonly int entityID;
        public readonly GameObject linkedGameObject;
        public readonly Transform linkedTransform;

        public Entity(int entityID)
        {
            this.entityID = entityID;
            linkedGameObject = new GameObject("Entity " + entityID);
            linkedTransform = linkedGameObject.transform;
        }

        public bool Equals(Entity other)
        {
            return entityID == other?.entityID;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return entityID;
        }
    }
}