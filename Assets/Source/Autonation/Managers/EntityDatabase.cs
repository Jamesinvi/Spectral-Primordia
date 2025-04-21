using System;
using Spectral.Autonation.Components;
using Spectral.Core;

namespace Spectral.Autonation.Managers
{
    public class EntityDatabase : Singleton<EntityDatabase>
    {
        public DynamicArray<Entity> Entities;
        public DynamicArray<MoverC> Movers;
        public DynamicArray<RendererC> Renderers;
        public DynamicArray<TransformC> Transforms;

        public EntityDatabase(int capacity = 2000)
        {
            Entities = new DynamicArray<Entity>(capacity);
            Transforms = new DynamicArray<TransformC>(capacity);
            Movers = new DynamicArray<MoverC>(capacity);
            Renderers = new DynamicArray<RendererC>(capacity);
        }

        public EntityDatabase() : this(2000)
        {
        }

        public DynamicArray<Entity> EntitiesWithComponents(in DynamicArray<Entity> toFill, Span<EnumComponentType> types)
        {
            toFill.Clear();
            foreach (Entity entity in Entities.AsSpan())
            {
                int index = entity.entityID;
                var added = false;
                foreach (EnumComponentType type in types)
                    switch (type)
                    {
                        case EnumComponentType.TransformComponent:
                            if (Movers.data[index] != null && !added)
                            {
                                toFill.Add(Entities.data[index]);
                                added = true;
                            }

                            break;
                        case EnumComponentType.MoverComponent:
                            if (Movers.data[index] != null && !added)
                            {
                                toFill.Add(Entities.data[index]);
                                added = true;
                            }

                            break;
                    }
            }

            return toFill;
        }


        public void Add(Entity toAdd, TransformC transform, MoverC mover, RendererC renderer)
        {
            Entities.Add(toAdd);
            Transforms.Add(transform);
            Movers.Add(mover);
            Renderers.Add(renderer);
        }

        public void Remove(Entity toRemove)
        {
            int index = Array.IndexOf(Entities.data, toRemove);
            Entities.RemoveAt(index);
            Transforms.RemoveAt(index);
            Movers.RemoveAt(index);
            Renderers.RemoveAt(index);
        }
    }
}