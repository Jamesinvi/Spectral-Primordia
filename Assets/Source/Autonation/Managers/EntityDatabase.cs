using System;
using Spectral.Autonation.Components;
using Spectral.Core;
using UnityEngine;

namespace Spectral.Autonation.Managers
{
    public class EntityDatabase : Singleton<EntityDatabase>
    {
        public readonly DynamicArray<Entity> entities;
        public readonly DynamicArray<MoverC> movers;
        public readonly DynamicArray<RendererC> renderers;
        public readonly DynamicArray<TransformC> transforms;

        public EntityDatabase(int capacity = 2000)
        {
            entities = new DynamicArray<Entity>(capacity);
            transforms = new DynamicArray<TransformC>(capacity);
            movers = new DynamicArray<MoverC>(capacity);
            renderers = new DynamicArray<RendererC>(capacity);
        }
        public EntityDatabase() : this(2000)
        {
        }

        public DynamicArray<Entity> EntitiesWithComponents(in DynamicArray<Entity> toFill, Span<EnumComponentType> types)
        {
            toFill.Clear();
            foreach (Entity entity in entities.AsSpan())
            {
                int index = entity.entityID;
                int componentsFound = 0;
                foreach (EnumComponentType type in types)
                    switch (type)
                    {
                        case EnumComponentType.TransformComponent:
                            if (transforms.data[index] != null)
                            {
                                componentsFound++;
                            }

                            break;
                        case EnumComponentType.MoverComponent:
                            if (movers.data[index] != null)
                            {
                                componentsFound++;
                            }
                            break;
                    }

                if (componentsFound == types.Length)
                {
                    toFill.Add(entities.data[index]);
                }
            }

            return toFill;
        }


        public void Add(Entity toAdd, TransformC transform, MoverC mover, RendererC renderer)
        {
            entities.Add(toAdd);
            transforms.Add(transform);
            movers.Add(mover);
            renderers.Add(renderer);
        }

        public void Remove(Entity toRemove)
        {
            int index = Array.IndexOf(entities.data, toRemove);
            entities.RemoveAt(index);
            transforms.RemoveAt(index);
            movers.RemoveAt(index);
            renderers.RemoveAt(index);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetInstance()
        {
            Instance.entities.Clear();
            Instance.transforms.Clear();
            Instance.movers.Clear();
            Instance.renderers.Clear();
        }
    }
}