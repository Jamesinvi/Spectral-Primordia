using UnityEngine;

namespace Primordia.Primordia.PlanetGeneration.Configuration
{
    [CreateAssetMenu(menuName = "Autonation/Planet Generation/Configuration")]
    public class ShapeInfoConfiguration : ScriptableObject
    {
        [Range(2, 512)] public int resolution;
        [Range(2, 256)] public float radius;
        public NoiseSettings noiseSettings;
    }
}