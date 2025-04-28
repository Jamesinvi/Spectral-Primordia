using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Primordia.Primordia.PlanetGeneration
{
    [BurstCompile]
    public static class SphereGenerator
    {
        public static void GenerateCubeSphereFace(Mesh mesh, int faceIndex, Vector3 normal, RenderTexture texture, ShapeInfo shapeInfo, NoiseSettings noiseSettings, ComputeShader computeShader)
        {
            mesh.Clear();
            int vertexCount = shapeInfo.resolution * shapeInfo.resolution;
            int indexCount = (shapeInfo.resolution - 1) * (shapeInfo.resolution - 1) * 6;
            var vertices = new Vector3[vertexCount];
            var uvs = new Vector2[vertexCount];
            var uvs2 = new Vector2[vertexCount];
            var indices = new int[indexCount];
            var shapeInfos = new ShapeInfo[1] { shapeInfo };
            var noiseSettingsArr = new NoiseSettings[1] { noiseSettings };

            var vertexBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 3, ComputeBufferType.Structured);
            var uvsBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 2, ComputeBufferType.Structured);
            var uvs2Buffer = new ComputeBuffer(vertexCount, sizeof(float) * 2, ComputeBufferType.Structured);
            var indexBuffer = new ComputeBuffer(indexCount, sizeof(int), ComputeBufferType.Structured);
            var shapeInfoBuffer = new ComputeBuffer(shapeInfos.Length, UnsafeUtility.SizeOf(typeof(ShapeInfo)), ComputeBufferType.Structured);
            var noiseSettingsBuffer = new ComputeBuffer(noiseSettingsArr.Length, UnsafeUtility.SizeOf(typeof(NoiseSettings)), ComputeBufferType.Structured);

            int kernel = computeShader.FindKernel("GenerateSphereFace");

            vertexBuffer.SetData(vertices);
            uvsBuffer.SetData(uvs);
            uvs2Buffer.SetData(uvs2);
            indexBuffer.SetData(indices);
            shapeInfoBuffer.SetData(shapeInfos);
            noiseSettingsBuffer.SetData(noiseSettingsArr);
            computeShader.SetBuffer(kernel, "vertices", vertexBuffer);
            computeShader.SetBuffer(kernel, "uvs", uvsBuffer);
            computeShader.SetBuffer(kernel, "uvs2", uvs2Buffer);
            computeShader.SetBuffer(kernel, "indices", indexBuffer);
            computeShader.SetBuffer(kernel, "shapeInfos", shapeInfoBuffer);
            computeShader.SetBuffer(kernel, "noiseSettings", noiseSettingsBuffer);
            computeShader.SetTexture(kernel, "heightTex", texture);
            computeShader.SetVector("normal", new float4(normal, 1));
            computeShader.SetInt("faceIndex", faceIndex);


            var groupSize = 8;
            int groups = Mathf.CeilToInt(shapeInfo.resolution / (float)groupSize);
            computeShader.Dispatch(kernel, groups, groups, 1);

            vertexBuffer.GetData(vertices);
            indexBuffer.GetData(indices);
            uvsBuffer.GetData(uvs);
            uvs2Buffer.GetData(uvs2);
            mesh.indexFormat = vertices.Length > 65535
                ? IndexFormat.UInt32
                : IndexFormat.UInt16;
            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.SetUVs(0, uvs);
            mesh.SetUVs(1, uvs2);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            vertexBuffer.Dispose();
            uvsBuffer.Dispose();
            uvs2Buffer.Dispose();
            indexBuffer.Dispose();
            shapeInfoBuffer.Dispose();
            noiseSettingsBuffer.Dispose();
        }
    }
}