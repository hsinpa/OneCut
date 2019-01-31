using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpriteCutter : MonoBehaviour {

    private Texture2D tex;
    private Sprite generateSprite;
    private SpriteRenderer sr;
    private List<Vector2> intersectionPoints;

    private List<Vector2> verticeSegmentOne;
    private List<Vector2> verticeSegmentTwo;

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

        sr.sprite = generateSprite;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Deform"))
        {
            ChangeSprite();
            DrawDebug();
        }
    }

    void DrawDebug()
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



            Debug.Log(vertices[a] + ", " + vertices[b]);
            //To see these you must view the game in the Scene tab while in Play mode
            Debug.DrawLine(currentPos + vertices[a], currentPos + vertices[b], Color.red, 1);
            Debug.DrawLine(currentPos + vertices[b], currentPos + vertices[c], Color.red, 1);
            Debug.DrawLine(currentPos + vertices[c], currentPos + vertices[a], Color.red, 1);
        }
    }

    public void FindIntersection(Vector2 p_point1, Vector2 p_point2)
    {
        intersectionPoints.Clear();
        Sprite sprite = sr.sprite;
        VerticesSegmentor verticesSegmentor = new VerticesSegmentor(p_point1, p_point2);

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

            vertices[a] = currentPos + vertices[a];
            vertices[b] = currentPos + vertices[b];
            vertices[c] = currentPos + vertices[c];


            AddIntersectPoint(vertices[a], vertices[b], p_point1, p_point2);
            AddIntersectPoint(vertices[b], vertices[c], p_point1, p_point2);
            AddIntersectPoint(vertices[c], vertices[a], p_point1, p_point2);
        }

        intersectionPoints = SortIntersectionPoint(intersectionPoints, (p_point2 - p_point1).normalized);
        ResegmentVertices(vertices, verticesSegmentor);

        //for (int i = 0; i < intersectionPoints.Count; i++)
        //{
        //    Debug.Log(intersectionPoints[i]);
        //}
    }

    void OnDrawGizmosSelected() {
        // Draw a yellow sphere at the transform's position
        if (intersectionPoints != null) {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < intersectionPoints.Count; i++)
            {
                Gizmos.DrawSphere(intersectionPoints[i], 0.05f);
            }
        }

        if (verticeSegmentOne != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < verticeSegmentOne.Count; i++)
            {
                Gizmos.DrawSphere(verticeSegmentOne[i], 0.05f);
            }
        }

        if (verticeSegmentTwo != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < verticeSegmentTwo.Count; i++)
            {
                Gizmos.DrawSphere(verticeSegmentTwo[i], 0.05f);
            }
        }

    }

    private void ResegmentVertices(Vector2[] p_vertices, VerticesSegmentor verticesSegmentor)
    {
        List<Vector2> segmentOne = new List<Vector2>();
        List<Vector2> segmentTwo = new List<Vector2>();

        for (int i = 0; i < p_vertices.Length; i++) {
            if (verticesSegmentor.CompareInputWithAverageLine(p_vertices[i]))
            {
                    segmentOne.Add(p_vertices[i]);
            }
            else {
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

    private void AddIntersectPoint(Vector2 p_line1A, Vector2 p_line1B, Vector2 p_line2A, Vector2 p_line2B) {
        bool isIntersect = MathUtility.Line.doIntersect(p_line1A, p_line1B, p_line2A, p_line2B);
        if (isIntersect) {
            Vector2 intersectPoint = MathUtility.Line.lineLineIntersection(p_line1A, p_line1B, p_line2A, p_line2B);
            if (intersectPoint != Vector2.positiveInfinity && !intersectionPoints.Contains(intersectPoint)) {
                intersectionPoints.Add(intersectPoint);
                //Debug.Log("intersectPoint " + intersectPoint);
            }
        }
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

            spriteVertices[i].x = Mathf.Clamp(
                (sprite.vertices[i].x - sprite.bounds.center.x -
                    (sprite.textureRectOffset.x / sprite.texture.width) + sprite.bounds.extents.x) /
                (2.0f * sprite.bounds.extents.x) * sprite.rect.width,
                0.0f, sprite.rect.width);

            spriteVertices[i].y = Mathf.Clamp(
                (sprite.vertices[i].y - sprite.bounds.center.y -
                    (sprite.textureRectOffset.y / sprite.texture.height) + sprite.bounds.extents.y) /
                (2.0f * sprite.bounds.extents.y) * sprite.rect.height,
                0.0f, sprite.rect.height);

            spriteVertices[i].x = Mathf.Clamp(spriteVertices[i].x * 0.995f, 0.0f, sprite.rect.width);
            spriteVertices[i].y = Mathf.Clamp(spriteVertices[i].y * 0.995f, 0.0f, sprite.rect.height);


        }
        //Override the geometry with the new vertices
        sprite.OverrideGeometry(spriteVertices, sprite.triangles);
    }

    IEnumerator DoSomethingAfterDelay(float time, System.Action callback) {
        yield return new WaitForSeconds(time);
        if (callback != null)
            callback();

    }


}
