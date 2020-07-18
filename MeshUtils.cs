using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

namespace Gnom6584
{
    public static class MeshUtils
    {
        private static string PropertyInfo(string propertyName, object propertyValue)
        {
            return new StringBuilder().Append(propertyName).Append(": ").Append("{").Append(propertyValue).Append("}").ToString();
        }
        public static string ShortMeshInfo(Mesh mesh)
        {
            return new StringBuilder().
                Append(PropertyInfo("Mesh name", mesh.name)).Append(" ").
                Append(PropertyInfo("Vertices count", mesh.vertices.Length)).Append(" ").
                Append(PropertyInfo("Triangles count count", mesh.triangles.Length)).Append(" ").
                ToString();
        }
    
        public static Mesh SeparateAllTriangles(Mesh mesh)
        {
            #if CORK_WRAPPER_CLOCK_DEBUG
                var stopwatch = new Stopwatch();
                stopwatch.Start();
            #endif

            var result = new Mesh {indexFormat = IndexFormat.UInt32};
            var vertices = new List<Vector3>();
            var triangles = new int[mesh.triangles.Length];
            for(var i = 0; i < mesh.triangles.Length; i++)
            {
                triangles[i] = vertices.Count;
                vertices.Add(mesh.vertices[mesh.triangles[i]]);
            }
            result.SetVertices(vertices);
            result.triangles = triangles;
        
            #if CORK_WRAPPER_CLOCK_DEBUG
                stopwatch.Stop();
                Debug.Log("SeparateAllTriangles finished in " + stopwatch.ElapsedMilliseconds + " ms for mesh:\n" + ShortMeshInfo(mesh));
            #endif
        
            return result;
        }

        public static Mesh TransformMesh(Mesh mesh, Transform transform)
        {
            #if CORK_WRAPPER_CLOCK_DEBUG
                var stopwatch = new Stopwatch();
                stopwatch.Start();
            #endif
            
            var result = new Mesh();
            
            var tempVertices = new Vector3[mesh.vertices.Length];
            for (var i = 0; i < tempVertices.Length; ++i)
            {
                tempVertices[i] = transform.TransformPoint(mesh.vertices[i]);
            }
            result.vertices = tempVertices;

            var tempTriangles = new int[mesh.triangles.Length];
            System.Array.Copy(mesh.triangles, tempTriangles, mesh.triangles.Length);
            result.triangles = tempTriangles;

            #if CORK_WRAPPER_CLOCK_DEBUG
                stopwatch.Stop();
                Debug.Log("TransformMesh finished in " + stopwatch.ElapsedMilliseconds + " ms for mesh:\n" + ShortMeshInfo(mesh));
            #endif
            
            return result;
        }    
    }
}