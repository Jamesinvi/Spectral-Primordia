using Primordia.Core;

namespace Primordia.Components
{
    public record GenerateResourceC : Component
    {
        public int hydrogenPerSecond;
        public int oxygenPerSecond;

        public GenerateResourceC(int entityIndex, int oxygenPerSecond, int hydrogenPerSecond) : base(entityIndex)
        {
            this.oxygenPerSecond = oxygenPerSecond;
            this.hydrogenPerSecond = hydrogenPerSecond;
        }
    }
}