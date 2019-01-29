using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCutter : MonoBehaviour {

    private Texture2D tex;
    private Sprite generateSprite;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
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
