﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCutter : MonoBehaviour {

    private Texture2D tex;
    private Sprite generateSprite;
    private SpriteRenderer sr;
    private List<Vector2> intersectionPoint;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        intersectionPoint = new List<Vector2>();
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



            Debug.Log(vertices[a] +", " + vertices[b]);
            //To see these you must view the game in the Scene tab while in Play mode
            Debug.DrawLine(currentPos + vertices[a], currentPos + vertices[b], Color.red, 1);
            Debug.DrawLine(currentPos + vertices[b], currentPos + vertices[c], Color.red, 1);
            Debug.DrawLine(currentPos + vertices[c], currentPos + vertices[a], Color.red, 1);
        }
    }

    public void FindIntersection(Vector2 p_point1, Vector2 p_point2)
    {
        intersectionPoint.Clear();
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

            Vector2 pointA = currentPos + vertices[a];
            Vector2 pointB = currentPos + vertices[b];
            Vector2 pointC = currentPos + vertices[c];


            AddIntersectPoint(pointA, pointB, p_point1, p_point2);
            AddIntersectPoint(pointB, pointC, p_point1, p_point2);
            AddIntersectPoint(pointC, pointC, p_point1, p_point2);
        }
    }

    private void AddIntersectPoint(Vector2 p_line1A, Vector2 p_line1B, Vector2 p_line2A, Vector2 p_line2B) {
        bool isIntersect = MathUtility.Line.doIntersect(p_line1A, p_line1B, p_line2A, p_line2B);
        if (isIntersect) {
            Vector2 intersectPoint = MathUtility.Line.lineLineIntersection(p_line1A, p_line1B, p_line2A, p_line2B);
            if (intersectPoint != Vector2.positiveInfinity) {
                intersectionPoint.Add(intersectPoint);
                Debug.Log("intersectPoint " + intersectPoint);
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


}