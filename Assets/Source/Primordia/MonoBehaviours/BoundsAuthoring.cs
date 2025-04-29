using UnityEditor;
using UnityEngine;

namespace Primordia.MonoBehaviours
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
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
#endif
    }
}