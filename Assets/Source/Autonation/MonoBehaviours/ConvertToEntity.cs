using System;
using Spectral.Autonation.Components;
using Spectral.Autonation.Managers;
using Spectral.Core;
using UnityEngine;

namespace Spectral.Autonation.MonoBehaviours
{
    public class ConvertToEntity : MonoBehaviour
    {
        [NonSerialized] public int generatedEntityIndex = -1;

        private void Start()
        {
            var entity = new Entity(gameObject);
            generatedEntityIndex = entity.index;
            BoundsC bounds = null;
            GenerateResourceC generateResource = null;
            if (TryGetComponent(out BoundsAuthoring boundsAuthoring)) bounds = new BoundsC(boundsAuthoring.bounds);
            if (TryGetComponent(out ResourceGeneratorAuthoring resourceGeneratorAuthoring)) generateResource = new GenerateResourceC(resourceGeneratorAuthoring.oxygenPerSecond, resourceGeneratorAuthoring.hydrogenPerSecond);

            EntityDatabase.Instance.Add(entity, bounds: bounds, resourceGenerator: generateResource);
        }
    }
}