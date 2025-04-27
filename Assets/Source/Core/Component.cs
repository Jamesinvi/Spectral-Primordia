namespace Primordia.Core
{
    public record Component
    {
        public uint componentId;
        public EnumComponentType componentType;
        public int entityIndex;

        protected Component(int entityIndex)
        {
            this.entityIndex = entityIndex;
        }
    }
}