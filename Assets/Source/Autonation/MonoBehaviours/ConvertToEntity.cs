using System;
using Spectral.Autonation.Components;
using Spectral.Autonation.Managers;
using Spectral.Core;
using UnityEngine;

namespace Spectral.Autonation.MonoBehaviours
{
    public class ConvertToEntity : MonoBehaviour
    {
        [NonSerialized] public Entity generatedEntity;

        private void Start()
        {
            generatedEntity = new Entity(gameObject);
            BoundsC bounds = null;
            GenerateResourceC generateResource = null;
            var transformComponent = new TransformC(0, generatedEntity, transform.position, transform.rotation, transform.localScale);
            if (TryGetComponent(out BoundsAuthoring boundsAuthoring)) bounds = new BoundsC(generatedEntity, boundsAuthoring.bounds);
            if (TryGetComponent(out ResourceGeneratorAuthoring resourceGeneratorAuthoring)) generateResource = new GenerateResourceC(generatedEntity, resourceGeneratorAuthoring.oxygenPerSecond, resourceGeneratorAuthoring.hydrogenPerSecond);

            EntityDatabase.Instance.Add(generatedEntity, transformComponent, bounds: bounds, resourceGenerator: generateResource);
        }

        private void OnDestroy()
        {
            EntityDatabase.Instance.Remove(generatedEntity);
        }
    }
}