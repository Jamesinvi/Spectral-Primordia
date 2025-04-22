using Spectral.Core;

namespace Spectral.Autonation.Components
{
    public record GenerateResourceC : Component
    {
        public int hydrogenPerSecond;
        public int oxygenPerSecond;

        public GenerateResourceC(int oxygenPerSecond, int hydrogenPerSecond)
        {
            this.oxygenPerSecond = oxygenPerSecond;
            this.hydrogenPerSecond = hydrogenPerSecond;
        }
    }
}