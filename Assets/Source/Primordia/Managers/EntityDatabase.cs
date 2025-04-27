using System;
using Primordia.Core;
using Primordia.Primordia.Components;
using UnityEngine;

namespace Primordia.Primordia.Managers
{
    public class EntityDatabase : Singleton<EntityDatabase>
    {
        public readonly DynamicArray<BoundsC> boundsComponents;
        public readonly DynamicArray<Entity> entities;
        public readonly DynamicArray<MoverC> movers;
        public readonly DynamicArray<RendererC> renderers;
        public readonly DynamicArray<GenerateResourceC> resourceGenerators;
        public readonly DynamicArray<TransformC> transforms;

        public EntityDatabase(int capacity = 2000)
        {
            entities = new DynamicArray<Entity>(capacity);
            transforms = new DynamicArray<TransformC>(capacity);
            movers = new DynamicArray<MoverC>(capacity);
            renderers = new DynamicArray<RendererC>(capacity);
            boundsComponents = new DynamicArray<BoundsC>(capacity);
            resourceGenerators = new DynamicArray<GenerateResourceC>(capacity);
        }

        public EntityDatabase() : this(2000)
        {
        }

        public DynamicArray<Entity> EntitiesWithComponents(in DynamicArray<Entity> toFill, Span<EnumComponentType> types)
        {
            toFill.Clear();
            foreach (Entity entity in entities.AsSpan())
            {
                int index = entity;
                if (index < 0) continue;
                var componentsFound = 0;
                foreach (EnumComponentType type in types)
                    switch (type)
                    {
                        case EnumComponentType.TransformComponent:
                            if (transforms.data[index] != null) componentsFound++;
                            break;
                        case EnumComponentType.MoverComponent:
                            if (movers.data[index] != null) componentsFound++;
                            break;
                        case EnumComponentType.BoundsComponent:
                            if (boundsComponents.data[index] != null) componentsFound++;
                            break;
                        case EnumComponentType.GenerateResourceComponent:
                            if (resourceGenerators.data[index] != null) componentsFound++;
                            break;
                    }

                if (componentsFound == types.Length) toFill.Add(entities.data[index]);
            }

            return toFill;
        }


        public void Add(Entity toAdd,
            TransformC transform = null,
            MoverC mover = null,
            RendererC renderer = null,
            BoundsC bounds = null,
            GenerateResourceC resourceGenerator = null)
        {
            entities.Add(toAdd);
            transforms.Add(transform);
            movers.Add(mover);
            renderers.Add(renderer);
            boundsComponents.Add(bounds);
            resourceGenerators.Add(resourceGenerator);
        }

        public void Remove(Entity toRemove)
        {
            int index = Array.IndexOf(entities.data, toRemove);
            entities.RemoveAt(index);
            transforms.RemoveAt(index);
            movers.RemoveAt(index);
            renderers.RemoveAt(index);
            boundsComponents.RemoveAt(index);
            resourceGenerators.RemoveAt(index);
        }

        public void Remove(int indexToRemove)
        {
            entities.RemoveAt(indexToRemove);
            transforms.RemoveAt(indexToRemove);
            movers.RemoveAt(indexToRemove);
            renderers.RemoveAt(indexToRemove);
            boundsComponents.RemoveAt(indexToRemove);
            resourceGenerators.RemoveAt(indexToRemove);
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetInstance()
        {
            Instance.entities.Clear();
            Instance.transforms.Clear();
            Instance.movers.Clear();
            Instance.renderers.Clear();
            Instance.boundsComponents.Clear();
            Instance.resourceGenerators.Clear();
        }
    }
}