using System;
using Primordia.Primordia.PlanetGeneration.Configuration;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Primordia.Primordia.PlanetGeneration
{
    [Serializable]
    [GenerateHLSL(PackingRules.Exact, false, sourcePath: "Assets/Shaders/Compute/NoiseSettings")]
    public struct NoiseSettings
    {
        [Title("Base Noise")] [Range(0, 8)] public int octaves;
        public float persistence;
        public float lacunarity;
        public float initialFrequency;
        public float initialAmplitude;
        public float strength;
        public float baseMinValue;
        [Title("Detail Noise")] 
        [Range(0, 10)] public int detailOctaves;
        public float detailPersistence;
        public float detailLacunarity;
        public float detailInitialFrequency;
        public float detailInitialAmplitude;
        public float detailStrength;
        public float detailMinValue;
        [Title("Ridges")] [Range(0, 8)] public int ridgeOctaves;
        public float ridgeLacunarity;
        public float ridgeGain;
        public float ridgeOffset;
        public float ridgeInitialFrequency;
        public float ridgeInitialAmplitude;
        public float ridgeStrength;
        public float ridgeMinValue;
        [Title("Extra")] public float minValue;

        public NoiseSettings(ShapeInfoConfiguration shapeInfoConfiguration)
        {
            this = shapeInfoConfiguration.noiseSettings;
        }
    }
}