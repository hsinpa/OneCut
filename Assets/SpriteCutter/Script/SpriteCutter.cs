using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SC.Math;

public class SpriteCutter : MonoBehaviour {

    private Texture2D tex;
    private Sprite generateSprite;
    private Sprite backupSprite;

    private SpriteRenderer sr;
    private List<Vector2> intersectionPoints;
    private List<Vector2> verticeSegmentOne;
    private List<Vector2> verticeSegmentTwo;

    private List<Triangle> triangles = new List<Triangle>();
    private List<Triangle> segmentTrigA = new List<Triangle>();
    private List<Triangle> segmentTrigB = new List<Triangle>();
    private MeshBuilder meshBuilder;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        intersectionPoints = new List<Vector2>();
        var pix = sr.sprite.texture.GetPixels32();
        //System.Array.Reverse(pix);

        // Copy the reversed image data to a new texture.
        tex = new Texture2D(sr.sprite.texture.width, sr.sprite.texture.height);
        tex.SetPixels32(pix);
        tex.Apply();

        generateSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        backupSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

        sr.sprite = generateSprite;

        this.triangles = FormTriangle(sr.sprite);
    }

    private List<Triangle> FormTriangle(Sprite p_sprite) {

        List<Triangle> triangles = new List<Triangle>();
        ushort[] raw_triangles = p_sprite.triangles;
        Vector2[] vertices = p_sprite.vertices;

        //Debug.Log("Triangles " + raw_triangles.Length);
        //Debug.Log("Vertices " + vertices.Length);

        int a, b, c;
        for (int i = 0; i < raw_triangles.Length; i = i + 3)
        {
            a = raw_triangles[i];
            b = raw_triangles[i + 1];
            c = raw_triangles[i + 2];

            //Debug.Log("Trig " + a + " Vertices " + vertices[a] + ", Trig " + b + " Vertices " + vertices[b] + ", Trig " + c + " Vertices " + vertices[c]);

            List<Vector2> nodes = new List<Vector2> { vertices[a], vertices[b], vertices[c] };
            List<TrigPairs> pairs = new List<TrigPairs> { new TrigPairs(vertices[a], vertices[b]),
                                                        new TrigPairs(vertices[b], vertices[c]),
                                                        new TrigPairs(vertices[c], vertices[a])
                                                    };

            triangles.Add(new Triangle(nodes, pairs));
        }

        return triangles;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Rebuild"))
        {
            //ChangeSprite();
            //DrawDebug(meshBuilder);
            //DrawTriangle(this.triangles);

            ChangeSpriteMesh(sr.sprite, backupSprite.vertices, backupSprite.triangles);
        }
    }

    void DrawTriangle(List<Triangle> p_triangle) {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        //Debug.Log(triangles.Count);
        for (int i = 0; i < p_triangle.Count; i++)
        {
            //To see these you must view the game in the Scene tab while in Play mode
            Debug.DrawLine(currentPos + p_triangle[i].nodes[0], currentPos + p_triangle[i].nodes[1], Color.red, 1);
            Debug.DrawLine(currentPos + p_triangle[i].nodes[1], currentPos + p_triangle[i].nodes[2], Color.red, 1);
            Debug.DrawLine(currentPos + p_triangle[i].nodes[2], currentPos + p_triangle[i].nodes[0], Color.red, 1);
        }
    }

    void DrawTriangle(Vector2[] p_vertices, ushort[] p_triangles)
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        int a, b, c;
        for (int i = 0; i < p_triangles.Length; i = i + 3)
        {
            a = p_triangles[i];
            b = p_triangles[i + 1];
            c = p_triangles[i + 2];

            //To see these you must view the game in the Scene tab while in Play mode
            Debug.DrawLine(currentPos + p_vertices[a], currentPos + p_vertices[b], Color.red, 1);
            Debug.DrawLine(currentPos + p_vertices[b], currentPos + p_vertices[c], Color.red, 1);
            Debug.DrawLine(currentPos + p_vertices[c], currentPos + p_vertices[a], Color.red, 1);
        }
    }

    void DrawDebug(MeshBuilder p_meshBuilder)
    {
        Sprite sprite = sr.sprite;

        ushort[] triangles = sprite.triangles;
        Vector2[] vertices = sprite.vertices;
        int a, b, c;
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        // draw the triangles using grabbed vertices
        for (int i = 0; i < triangles.Length; i = i + 3)
        {
            a = triangles[i];
            b = triangles[i + 1];
            c = triangles[i + 2];

            //To see these you must view the game in the Scene tab while in Play mode
            Debug.DrawLine(currentPos + vertices[a], currentPos + vertices[b], Color.red, 1);
            Debug.DrawLine(currentPos + vertices[b], currentPos + vertices[c], Color.red, 1);
            Debug.DrawLine(currentPos + vertices[c], currentPos + vertices[a], Color.red, 1);
        }
    }

    public void FindIntersection(Vector2 p_point1, Vector2 p_point2)
    {
        intersectionPoints.Clear();
        List<Triangle> exp_trig = new List<Triangle>(this.triangles);

        Sprite sprite = sr.sprite;
        Vector2[] vertices = sprite.vertices;
        VerticesSegmentor verticesSegmentor = new VerticesSegmentor(p_point1, p_point2);

        for (int i = 0; i < exp_trig.Count; i++)
        {
            if (exp_trig[i].pairs.Count == 3)
            {
                HandleTrigIntersection(exp_trig[i], verticesSegmentor, p_point1, p_point2);
            }
        }

        intersectionPoints = SortIntersectionPoint(intersectionPoints, (p_point2 - p_point1).normalized);

        //List<Triangle> newTrigCol = TrigBuilder.Build(exp_trig);

        //ResegmentTriangle(newTrigCol, verticesSegmentor);

        //meshBuilder = new MeshBuilder(newTrigCol);

       // DrawTriangle(segmentTrigA);
        //DrawTriangle(segmentTrigB);

        //DrawTriangle(meshBuilder.meshVertices, meshBuilder.meshTrig);

        //Debug.Log("intersectionPointsLength " + intersectionPoints.Count + ", TriangleLength " + exp_trig.Count + "newTrigColLength " + newTrigCol.Count);
        //Debug.Log("triangles " + sprite.triangles.Length + ", vertices " + sprite.vertices.Length);
        //Debug.Log("MeshBuilder triangles " + meshBuilder.meshTrig.Length + ", MeshBuilder vertices " + meshBuilder.meshVertices.Length);

        //ChangeSpriteMesh(sprite, meshBuilder.meshVertices, meshBuilder.meshTrig);

        ResegmentVertices(vertices, verticesSegmentor);

    }

    void OnDrawGizmosSelected() {
        // Draw a yellow sphere at the transform's position
        if (intersectionPoints != null) {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < intersectionPoints.Count; i++)
            {
                Gizmos.DrawSphere(intersectionPoints[i], 0.02f);
            }
        }

        if (verticeSegmentOne != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < verticeSegmentOne.Count; i++)
            {
                Gizmos.DrawSphere(verticeSegmentOne[i], 0.03f);
            }
        }

        if (verticeSegmentTwo != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < verticeSegmentTwo.Count; i++)
            {
                Gizmos.DrawSphere(verticeSegmentTwo[i], 0.03f);
            }
        }

        int a, b, c;

        if (meshBuilder != null) {
            Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
            for (int i = 0; i < meshBuilder.meshTrig.Length; i = i + 3)
            {
                Gizmos.color = Color.black;

                a = meshBuilder.meshTrig[i];
                b = meshBuilder.meshTrig[i + 1];
                c = meshBuilder.meshTrig[i + 2];

                Gizmos.DrawSphere(currentPos + meshBuilder.meshVertices[a], 0.01f);
                Gizmos.DrawSphere(currentPos + meshBuilder.meshVertices[b], 0.01f);
                Gizmos.DrawSphere(currentPos + meshBuilder.meshVertices[c], 0.01f);
            }

        }

    }

    private List<Triangle> ResegmentTriangle(List<Triangle> p_triangles, VerticesSegmentor verticesSegmentor)
    {
        List<Triangle> segmentOne = new List<Triangle>();
        List<Triangle> segmentTwo = new List<Triangle>();

        float segmentA_area = 0, segmentB_area = 0;


        for (int i = 0; i < p_triangles.Count; i++) {
            if (verticesSegmentor.CompareInputWithAverageLine(p_triangles[i].center))
            {
                p_triangles[i].segmentID = "A";
                segmentA_area += p_triangles[i].area;
                segmentOne.Add(p_triangles[i]);
            }
            else {
                p_triangles[i].segmentID = "B";
                segmentB_area += p_triangles[i].area;
                segmentTwo.Add(p_triangles[i]);
            }
        }

        Debug.Log("segmentA_area " + segmentA_area + ", segmentB_area " + segmentB_area);

        segmentTrigA = segmentOne;
        segmentTrigB = segmentTwo;

        return (segmentA_area > segmentB_area) ? segmentOne : segmentTwo;
    }

    private void ResegmentVertices(Vector2[] p_vertices, VerticesSegmentor verticesSegmentor)
    {
        List<Vector2> segmentOne = new List<Vector2>();
        List<Vector2> segmentTwo = new List<Vector2>();

        for (int i = 0; i < p_vertices.Length; i++)
        {
            if (verticesSegmentor.CompareInputWithAverageLine(p_vertices[i]))
            {
                segmentOne.Add(p_vertices[i]);
            }
            else
            {
                segmentTwo.Add(p_vertices[i]);
            }
        }

        verticeSegmentOne = segmentOne;
        verticeSegmentTwo = segmentTwo;
    }


    private List<Vector2> SortIntersectionPoint(List<Vector2> unsortPoints, Vector2 cut_direction) {
        cut_direction = cut_direction * cut_direction;
        bool isSortByX = (cut_direction.x > cut_direction.y);

        return unsortPoints.OrderBy(x => (isSortByX) ? x.x : x.y).ToList();
    }

    private void HandleTrigIntersection(Triangle triangle, VerticesSegmentor verticesSegmentor, Vector2 p_pointA, Vector2 p_pointB) {
        int pairNum = triangle.pairs.Count;

        for (int i = pairNum - 1; i >= 0; i--)
        {
            TrigPairs pair = triangle.pairs[i];

            Vector2 point = AddIntersectPoint(
                pair.nodeA,
                pair.nodeB,
                p_pointA, p_pointB
            );
            
            //Intersection has occur
            if (!point.Equals( Vector2.positiveInfinity)) {
                Triangle.Fragment fragmentA = FindFragment(verticesSegmentor, pair.nodeA);
                Triangle.Fragment fragmentB = FindFragment(verticesSegmentor, pair.nodeB);
                Triangle.Fragment fragmentC = new Triangle.Fragment(point, "", Triangle.Fragment.Type.Cutted);

                triangle.AddFragment(new Triangle.Fragment[] {fragmentA, fragmentB, fragmentC });
            }
        }
    }

    private Triangle.Fragment FindFragment(VerticesSegmentor verticesSegmentor, Vector2 vertices) {
        //Triangle.Fragment fragment = new Triangle.Fragment(vertices,);
        string segmentID = (verticesSegmentor.CompareInputWithAverageLine(vertices)) ? "A" : "B";
        Triangle.Fragment fragment = new Triangle.Fragment(vertices, segmentID, Triangle.Fragment.Type.Original);

        return fragment;
    }

    private Vector2 AddIntersectPoint(Vector2 p_line1A, Vector2 p_line1B, Vector2 p_line2A, Vector2 p_line2B) {
        bool isIntersect = MathUtility.Line.doIntersect(p_line1A, p_line1B, p_line2A, p_line2B);

        if (isIntersect) {
            Vector2 intersectPoint = MathUtility.Line.lineLineIntersection(p_line1A, p_line1B, p_line2A, p_line2B);
            if (!intersectPoint.Equals(Vector2.positiveInfinity)) {

                if (!intersectionPoints.Contains(intersectPoint))
                    intersectionPoints.Add(intersectPoint);

                return intersectPoint;
            }
        }

        return Vector2.positiveInfinity;
    }

    // Edit the vertices obtained from the sprite.  Use OverrideGeometry to
    // submit the changes.
    void ChangeSprite()
    {
        //Fetch the Sprite and vertices from the SpriteRenderer
        Sprite sprite = sr.sprite;
        Vector2[] spriteVertices = sprite.vertices;

        for (int i = 0; i < spriteVertices.Length; i++)
        {

            spriteVertices[i] = VerticesToWorldPos(spriteVertices[i], sprite);

            spriteVertices[i].x = Mathf.Clamp(spriteVertices[i].x * 0.995f, 0.0f, sprite.rect.width);
            spriteVertices[i].y = Mathf.Clamp(spriteVertices[i].y * 0.995f, 0.0f, sprite.rect.height);
        }

        //Override the geometry with the new vertices
        sprite.OverrideGeometry(spriteVertices, sprite.triangles);
    }

    private void ChangeSpriteMesh(Sprite p_sprite, Vector2[] p_vertices, ushort[] p_triangles) {
        Sprite sprite = p_sprite;
        Vector2[] spriteVertices = new Vector2[p_vertices.Length];

        for (int i = 0; i < p_vertices.Length; i++)
        {
            spriteVertices[i] = VerticesToWorldPos(p_vertices[i], sprite);
        }

        //Override the geometry with the new vertices
        sprite.OverrideGeometry(spriteVertices, p_triangles);
    }

    private Vector2 VerticesToWorldPos(Vector2 vertices, Sprite sprite) {
        Vector2 worldPos = Vector2.zero;

        worldPos.x = Mathf.Clamp(
                    (vertices.x - sprite.bounds.center.x -
                        (sprite.textureRectOffset.x / sprite.texture.width) + sprite.bounds.extents.x) /
                    (2.0f * sprite.bounds.extents.x) * sprite.rect.width,
                    0.0f, sprite.rect.width);

        worldPos.y = Mathf.Clamp(
                (vertices.y - sprite.bounds.center.y -
                    (sprite.textureRectOffset.y / sprite.texture.height) + sprite.bounds.extents.y) /
                (2.0f * sprite.bounds.extents.y) * sprite.rect.height,
                0.0f, sprite.rect.height);

        return worldPos;
    }

    IEnumerator DoSomethingAfterDelay(float time, System.Action callback) {
        yield return new WaitForSeconds(time);
        if (callback != null)
            callback();

    }


}
