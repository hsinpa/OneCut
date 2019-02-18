using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SC.Math;

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

        Debug.Log("MeshTrig Number " + _meshTrig.Length);
        Debug.Log("Vertices Number " + verticesIndex.vertices.Length);
    }

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

                //Debug.Log("RE_Trig " + a + " Vertices " + p_verticeIndex.vertices[a] + ", Trig " + b + " Vertices " + p_verticeIndex.vertices[b] + ", Trig " + c + " Vertices " + p_verticeIndex.vertices[c]);

                //int _short = p_verticeIndex.GetIndex(p_triangles[i].nodes[n]);
                //Debug.Log("Int " + _short + ", Short " + (ushort)_short);
                //Debug.Log("Index " + _short + ", Nodes (" + p_triangles[i].nodes[n].x + "," + p_triangles[i].nodes[n].y + ")");

                //newMeshTrig[i + n] = (ushort)_short;
            }
            catch {
                Debug.LogError("p_verticeIndex.VectorToIndex Eror");
            }
        }

        return newMeshTrig;
    }

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
