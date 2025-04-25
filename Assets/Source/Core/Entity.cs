using System;
using Spectral.Autonation.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Spectral.Core
{
    public class Entity : IEquatable<Entity>
    {
        public readonly int index;
        public readonly GameObject linkedGameObject;
        public readonly Transform linkedTransform;

        public Entity(int index)
        {
            this.index = index;
            linkedGameObject = new GameObject("Entity " + index);
            linkedTransform = linkedGameObject.transform;
        }

        public Entity(GameObject linkedGameObject)
        {
            index = EntityDatabase.Instance.entities.length;
            this.linkedGameObject = linkedGameObject;
            linkedTransform = linkedGameObject.transform;
            linkedGameObject.name = linkedGameObject.name + index;
        }

        public bool Equals(Entity other)
        {
            return index == other?.index;
        }

        ~Entity()
        {
            if (linkedGameObject != null) Object.Destroy(linkedGameObject);
        }

        public static implicit operator int(Entity entity)
        {
            if (entity == null) return -1;
            return entity.index;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return index;
        }
    }
}