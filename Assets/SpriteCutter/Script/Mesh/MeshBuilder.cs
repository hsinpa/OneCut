using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SC.Math;
using SC.Trig;

public class MeshBuilder {

    List<Triangle> _triangles;
    VerticesIndex verticesIndex;

    public ushort[] meshTrig {
        get {
            return _meshTrig;
        }
    }

    public Vector2[] meshVertices {
        get {
            return _meshVertices;
        }
    }

    private ushort[] _meshTrig;
    private Vector2[] _meshVertices;

    public MeshBuilder(List<Triangle> p_triangles) {
        this._triangles = p_triangles;
        verticesIndex = BuildVerticesIndex(this._triangles);

        _meshTrig = GetTrigStructure(this._triangles, verticesIndex);
        _meshVertices = verticesIndex.vertices;
    }

    /// <summary>
    /// Build up ushort[] for mesh trig
    /// </summary>
    /// <param name="p_triangles"></param>
    /// <param name="p_verticeIndex"></param>
    /// <returns></returns>
    private ushort[] GetTrigStructure(List<Triangle> p_triangles, VerticesIndex p_verticeIndex) {
        ushort[] newMeshTrig = new ushort[p_triangles.Count * 3];

        for (int i = 0; i < p_triangles.Count; i++)
        {
            try {
                ushort a = (ushort) p_verticeIndex.GetIndex(p_triangles[i].nodes[0]);
                ushort b = (ushort)p_verticeIndex.GetIndex(p_triangles[i].nodes[1]);
                ushort c = (ushort)p_verticeIndex.GetIndex(p_triangles[i].nodes[2]);


                int trigIndex = i * 3;

                newMeshTrig[trigIndex] = a;
                newMeshTrig[trigIndex + 1] = b;
                newMeshTrig[trigIndex + 2] = c;
            }
            catch {
                Debug.LogError("p_verticeIndex.VectorToIndex Eror");
            }
        }

        return newMeshTrig;
    }

    /// <summary>
    /// Build up vertices index table
    /// </summary>
    /// <param name="p_triangles"></param>
    /// <returns></returns>
    private VerticesIndex BuildVerticesIndex(List<Triangle> p_triangles)
    {
        VerticesIndex verticesIndex = new VerticesIndex();

        for (int i = 0; i < p_triangles.Count; i++)
        {
            for (int n = 0; n < p_triangles[i].nodes.Count; n++)
            {
                verticesIndex.AddVertices(p_triangles[i].nodes[n]);
            }
        }

        return verticesIndex;
    }

    private class VerticesIndex {
        private Dictionary<string, int> VectorToIndex;

        public Vector2[] vertices {
            get {
                return _vertices.ToArray();
            }
        }
        private List<Vector2> _vertices;

        public VerticesIndex()
        {
            _vertices = new List<Vector2>();
            VectorToIndex = new Dictionary<string, int>();
        }

        public int GetIndex(Vector2 p_vertices) {
            string verticeString = VectorToString(p_vertices);

            if (VectorToIndex.ContainsKey(verticeString))
                return VectorToIndex[verticeString];

            return -1;
        }

        public void AddVertices(Vector2 p_vertices) {
            if (!_vertices.Contains(p_vertices)) {

                string verticeString =  VectorToString(p_vertices);
                if (!VectorToIndex.ContainsKey(verticeString)) {
                    _vertices.Add(p_vertices);
                    int c_index = _vertices.Count - 1;

                    VectorToIndex.Add(verticeString, c_index);
                }
            }
        }

        private string VectorToString(Vector2 vector) {
            return vector.x + ", " + vector.y;
        }
    }
}
