using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

namespace Cork
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct CorkTriMesh {
        public readonly int TrianglesCount;
        public readonly int VerticesCount;
        public readonly IntPtr TrianglesPointer;
        public readonly IntPtr VerticesPointer;

        public Mesh ToUnityMesh()
        {
            #if CORK_WRAPPER_CLOCK_DEBUG
                var stopwatch = new Stopwatch();
                stopwatch.Start();
            #endif
            var result = new Mesh{indexFormat = IndexFormat.UInt32};;
        
            var rawVertices = new float[VerticesCount * 3];
            Marshal.Copy(VerticesPointer, rawVertices, 0, rawVertices.Length);
        
            var unityVertices = new Vector3[VerticesCount];

            for (var i = 0; i < VerticesCount; ++i)
            {
                var mulIndex = i * 3;
                unityVertices[i] = new Vector3(rawVertices[mulIndex], rawVertices[mulIndex + 1], rawVertices[mulIndex + 2]);
            }
            
            result.vertices = unityVertices;

            var unityTrianglesArrayLength = TrianglesCount * 3;
        
            var unityTriangles = new int[unityTrianglesArrayLength];
            Marshal.Copy(TrianglesPointer, unityTriangles, 0, unityTrianglesArrayLength);
            
            result.triangles = unityTriangles;
            result.Optimize();

            #if CORK_WRAPPER_CLOCK_DEBUG
                stopwatch.Stop();
                Debug.Log("ToUnityMesh finished in " + stopwatch.ElapsedMilliseconds + " ms for corkTriMesh:\n" + "Vertices: {" + VerticesCount + "} Triangles: {" + TrianglesCount + "}");
            #endif 
            return result;
        }
    }
}
