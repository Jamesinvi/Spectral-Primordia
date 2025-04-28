using System;
using Primordia.Primordia.PlanetGeneration.Configuration;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Pool;

namespace Primordia.Primordia.PlanetGeneration
{
    public class PlanetMeshGenerator : MonoBehaviour
    {
        [OnValueChanged(nameof(CreatePlanet), true)] [InlineEditor]
        public ShapeInfoConfiguration shapeInfoConfiguration;

        public bool updateTextures;

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
            GraphicsFormat format = SystemInfo.GetCompatibleFormat(GraphicsFormat.R16_UNorm, GraphicsFormatUsage.Render);
            var textures = new RenderTexture[6];
            var texture2DArray = new Texture2DArray(shapeInfoConfiguration.resolution, shapeInfoConfiguration.resolution, textures.Length, format, TextureCreationFlags.None);
            texture2DArray.wrapMode = TextureWrapMode.Mirror;


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
                textures[i] = new RenderTexture(shapeInfoConfiguration.resolution, shapeInfoConfiguration.resolution, 0, format);
                textures[i].enableRandomWrite = true;
                if (_meshFilters[i] == null)
                {
                    var child = new GameObject("Face " + i, typeof(MeshFilter), typeof(MeshRenderer));
                    child.transform.SetParent(transform);
                    _meshFilters[i] = child.GetComponent<MeshFilter>();
                    _meshFilters[i].sharedMesh = new Mesh
                    {
                        name = gameObject.name + "Face " + i
                    };
                    _meshRenderers[i] = child.GetComponent<MeshRenderer>();
                    _meshRenderers[i].sharedMaterial = material;
                }

                SphereGenerator.GenerateCubeSphereFace(_meshFilters[i].sharedMesh, i, normals[i], textures[i], new ShapeInfo(shapeInfoConfiguration), new NoiseSettings(shapeInfoConfiguration), computeShader);
                Graphics.CopyTexture(textures[i].graphicsTexture, 0, texture2DArray.graphicsTexture, i);
            }

            if (updateTextures) AssetDatabase.CreateAsset(texture2DArray, "Assets/Textures/TEX_ARR2D_Planet.asset");

            for (var i = 0; i < textures.Length; i++) textures[i].Release();
        }
    }
}