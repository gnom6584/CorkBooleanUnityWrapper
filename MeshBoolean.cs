using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Gnom6584;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Cork
{
    public static class MeshBoolean
    {
        public const string CORK_DLL_NAME = "libCorkWrapper";

        #region IMPORT
        
        [DllImport(CORK_DLL_NAME, EntryPoint = "isSolid")]
        private static extern bool ImportIsSolid(Vector3[] vertices0, int numVertices0, int[] triangles0, int numTriangles0);  
        
        [DllImport(CORK_DLL_NAME, EntryPoint = "computeUnion")]
        private static extern CorkTriMesh ImportComputeUnion(
            Vector3[] vertices0, int numVertices0, int[] triangles0, int numTriangles0,
            Vector3[] vertices1, int numVertices1, int[] triangles1, int numTriangles1);        
        
        [DllImport(CORK_DLL_NAME, EntryPoint = "computeDifference")]
        private static extern CorkTriMesh ImportComputeDifference(
            Vector3[] vertices0, int numVertices0, int[] triangles0, int numTriangles0,
            Vector3[] vertices1, int numVertices1, int[] triangles1, int numTriangles1);
        
        [DllImport(CORK_DLL_NAME, EntryPoint = "computeIntersection")]
        private static extern CorkTriMesh ImportComputeIntersection(
            Vector3[] vertices0, int numVertices0, int[] triangles0, int numTriangles0,
            Vector3[] vertices1, int numVertices1, int[] triangles1, int numTriangles1);    
        
        [DllImport(CORK_DLL_NAME, EntryPoint = "computeSymmetricDifference")]
        private static extern CorkTriMesh ImportComputeSymmetricDifference(
            Vector3[] vertices0, int numVertices0, int[] triangles0, int numTriangles0,
            Vector3[] vertices1, int numVertices1, int[] triangles1, int numTriangles1);    
        
        [DllImport(CORK_DLL_NAME, EntryPoint = "resolveIntersections")]
        private static extern CorkTriMesh ImportResolveIntersections(
            Vector3[] vertices0, int numVertices0, int[] triangles0, int numTriangles0,
            Vector3[] vertices1, int numVertices1, int[] triangles1, int numTriangles1);
        
        [DllImport(CORK_DLL_NAME, EntryPoint = "freeCorkTriMesh")]
        private static extern void ImportFreeCorkTriMesh(IntPtr vPtr, IntPtr tPtr);
        
        #endregion
        
        public enum Operation
        {
            Union,
            Difference,
            Intersection,
            SymmetricDifference
        }
        public static Mesh ComputeOperation(Mesh first, Mesh second, Operation operation)
        {
            #if CORK_WRAPPER_CLOCK_DEBUG
                var stopwatch = new Stopwatch();
                stopwatch.Start();
            #endif

            CorkTriMesh corkResult;

            switch (operation)
            {
                case Operation.Union:
                    corkResult = ImportComputeUnion(first.vertices, first.vertices.Length, first.triangles,
                        first.triangles.Length,
                        second.vertices, second.vertices.Length, second.triangles, second.triangles.Length);
                    break;
                case Operation.Difference:
                    corkResult = ImportComputeDifference(first.vertices, first.vertices.Length, first.triangles,
                        first.triangles.Length,
                        second.vertices, second.vertices.Length, second.triangles, second.triangles.Length);
                    break;
                case Operation.Intersection:
                    corkResult = ImportComputeIntersection(first.vertices, first.vertices.Length, first.triangles,
                        first.triangles.Length,
                        second.vertices, second.vertices.Length, second.triangles, second.triangles.Length);
                    break;
                case Operation.SymmetricDifference:
                    corkResult = ImportComputeSymmetricDifference(first.vertices, first.vertices.Length, first.triangles,
                        first.triangles.Length,
                        second.vertices, second.vertices.Length, second.triangles, second.triangles.Length);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
            
            var result = corkResult.ToUnityMesh();

            ImportFreeCorkTriMesh(corkResult.VerticesPointer, corkResult.TrianglesPointer);

            #if CORK_WRAPPER_CLOCK_DEBUG
                stopwatch.Stop();
                Debug.Log("Mesh operation: {" + operation + "} finished in " + stopwatch.ElapsedMilliseconds + " ms for meshes:\n" + 
                MeshUtils.ShortMeshInfo(first) + " and " + MeshUtils.ShortMeshInfo(second));
            #endif

            return result;
        }

        public static Mesh ComputeUnion(Mesh first, Mesh second)
        {
            return ComputeOperation(first, second, Operation.Union);
        }
        
        public static Mesh ComputeDifference(Mesh first, Mesh second)
        {
            return ComputeOperation(first, second, Operation.Difference);
        }
        
        public static Mesh ComputeIntersection(Mesh first, Mesh second)
        {
            return ComputeOperation(first, second, Operation.Intersection);
        }
        
        public static Mesh ComputeSymmetricDifference(Mesh first, Mesh second)
        {
            return ComputeOperation(first, second, Operation.SymmetricDifference);
        }
        
        public static Mesh ResolveIntersections(Mesh first, Mesh second)
        {
            #if CORK_WRAPPER_CLOCK_DEBUG
                var stopwatch = new Stopwatch();
                stopwatch.Start();
            #endif

            var corkResult = ImportResolveIntersections(first.vertices, first.vertices.Length, first.triangles, 
                second.triangles.Length, second.vertices, second.vertices.Length, second.triangles, second.triangles.Length);

            var result = corkResult.ToUnityMesh();

            ImportFreeCorkTriMesh(corkResult.VerticesPointer, corkResult.TrianglesPointer);

            #if CORK_WRAPPER_CLOCK_DEBUG
                stopwatch.Stop();
                Debug.Log("Mesh ResolveIntersections finished in " + stopwatch.ElapsedMilliseconds + " ms for meshes:\n" + 
                MeshUtils.ShortMeshInfo(first) + " and " + MeshUtils.ShortMeshInfo(second));
            #endif

            return result;
        }
        
        [Obsolete("IsSolid may not work, does not work in c++ either")]
        public static bool IsSolid(Mesh mesh)
        {
            #if CORK_WRAPPER_CLOCK_DEBUG
                var stopwatch = new Stopwatch();
                stopwatch.Start();
            #endif
            
            var result = ImportIsSolid(mesh.vertices, mesh.vertices.Length, mesh.triangles, mesh.triangles.Length);
            
            #if CORK_WRAPPER_CLOCK_DEBUG    
                Debug.Log("Mesh IsSolid finished in " + stopwatch.ElapsedMilliseconds + " ms for meshes:\n" + 
                    MeshUtils.ShortMeshInfo(mesh));
            #endif
            return result;
        }
    }
}

