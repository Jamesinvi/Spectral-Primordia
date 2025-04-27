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
        [Title("Ridges")] [Range(0, 8)] public int ridgeOctaves;
        public float ridgeLacunarity;
        public float ridgeGain;
        public float ridgeOffset;
        public float ridgeInitialFrequency;
        public float ridgeInitialAmplitude;
        public float ridgeStrength;
        [Title("Extra")] public float minValue;

        public NoiseSettings(ShapeInfoConfiguration shapeInfoConfiguration)
        {
            octaves = shapeInfoConfiguration.noiseSettings.octaves;
            persistence = shapeInfoConfiguration.noiseSettings.persistence;
            lacunarity = shapeInfoConfiguration.noiseSettings.lacunarity;
            initialFrequency = shapeInfoConfiguration.noiseSettings.initialFrequency;
            initialAmplitude = shapeInfoConfiguration.noiseSettings.initialAmplitude;
            strength = shapeInfoConfiguration.noiseSettings.strength;
            ridgeOctaves = shapeInfoConfiguration.noiseSettings.ridgeOctaves;
            ridgeLacunarity = shapeInfoConfiguration.noiseSettings.ridgeLacunarity;
            ridgeGain = shapeInfoConfiguration.noiseSettings.ridgeGain;
            ridgeOffset = shapeInfoConfiguration.noiseSettings.ridgeOffset;
            ridgeInitialFrequency = shapeInfoConfiguration.noiseSettings.ridgeInitialFrequency;
            ridgeInitialAmplitude = shapeInfoConfiguration.noiseSettings.ridgeInitialAmplitude;
            minValue = shapeInfoConfiguration.noiseSettings.minValue;
            ridgeStrength = shapeInfoConfiguration.noiseSettings.ridgeStrength;
        }
    }
}