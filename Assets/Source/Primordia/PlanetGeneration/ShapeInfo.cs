using Primordia.Primordia.PlanetGeneration.Configuration;
using UnityEngine.Rendering;

namespace Primordia.Primordia.PlanetGeneration
{
    [GenerateHLSL(PackingRules.Exact, false, sourcePath: "Assets/Shaders/Compute/ShapeInfo")]
    public struct ShapeInfo
    {
        public readonly int resolution;
        public readonly float radius;

        public ShapeInfo(ShapeInfoConfiguration shapeInfoConfiguration)
        {
            resolution = shapeInfoConfiguration.resolution;
            radius = shapeInfoConfiguration.radius;
        }
    }
}