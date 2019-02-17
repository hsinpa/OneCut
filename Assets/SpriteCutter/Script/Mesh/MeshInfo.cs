using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MeshInfo {

    ushort[] triangles;
    Vector2[] vertices;

    public MeshInfo(ushort[] p_trig, Vector2[] p_vertices ) {
        this.triangles = p_trig;
        this.vertices = p_vertices;
    }

}
