using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Spectral.Autonation.MonoBehaviours
{
    public class BoundsAuthoring : MonoBehaviour
    {
        public Bounds bounds;


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Selection.activeGameObject == gameObject && !Application.isPlaying)
            {
                var renderers = GetComponentsInChildren<MeshRenderer>();
                bounds = renderers[0].bounds;
                foreach (MeshRenderer meshRenderer in renderers) bounds.Encapsulate(meshRenderer.bounds);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
        }
#endif
    }
}