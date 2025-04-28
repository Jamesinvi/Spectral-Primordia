using System;
using Primordia.Primordia.PlanetGeneration.Configuration;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace Primordia.Primordia.PlanetGeneration
{
    public class PlanetMeshGenerator : MonoBehaviour
    {
        [OnValueChanged(nameof(CreatePlanet), true)] [InlineEditor]
        public ShapeInfoConfiguration shapeInfoConfiguration;

        public Material material;
        public bool showNormals;

        [SerializeField] private MeshFilter[] _meshFilters = new MeshFilter[6];
        [SerializeField] private MeshRenderer[] _meshRenderers = new MeshRenderer[6];
        public ComputeShader computeShader;

        private void OnDrawGizmosSelected()
        {
            if (!showNormals || _meshFilters == null) return;

            var vertices = ListPool<Vector3>.Get();
            var normals = ListPool<Vector3>.Get();
            foreach (MeshFilter filter in _meshFilters)
            {
                filter.sharedMesh.GetVertices(vertices);
                filter.sharedMesh.GetNormals(normals);
                for (var i = 0; i < normals.Count; i++) Gizmos.DrawRay(vertices[i], normals[i]);
                vertices.Clear();
                normals.Clear();
            }

            ListPool<Vector3>.Release(vertices);
            ListPool<Vector3>.Release(normals);
        }

        [Button]
        private void ClearChildren()
        {
            int childCount = transform.childCount;
            for (var i = 0; i < childCount; i++) DestroyImmediate(transform.GetChild(0).gameObject);
        }

        [Button]
        private void CreatePlanet()
        {
            Span<Vector3> normals = stackalloc Vector3[6]
            {
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 1),
                new Vector3(-1, 0, 0),
                new Vector3(0, -1, 0),
                new Vector3(0, 0, -1)
            };
            for (var i = 0; i < 6; i++)
            {
                if (_meshFilters[i] == null)
                {
                    var child = new GameObject("Face " + i, typeof(MeshFilter), typeof(MeshRenderer));
                    child.transform.SetParent(transform);
                    _meshFilters[i] = child.GetComponent<MeshFilter>();
                    _meshFilters[i].sharedMesh = new Mesh
                    {
                        name = "Face " + i
                    };
                    _meshRenderers[i] = child.GetComponent<MeshRenderer>();
                }

                SphereGenerator.GenerateCubeSphereFace(_meshFilters[i].sharedMesh, normals[i], new ShapeInfo(shapeInfoConfiguration), new NoiseSettings(shapeInfoConfiguration), computeShader);
                
            }
        }
    }
}