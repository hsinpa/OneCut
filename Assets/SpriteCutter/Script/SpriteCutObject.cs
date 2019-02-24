using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SC.Trig;

namespace SC.Main
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteCutObject : MonoBehaviour
    {

        #region Parameter
        public SpriteRenderer sr {
            get {
                return _sr;
            }
        }
        private SpriteRenderer _sr;

        public List<Triangle> triangles
        {
            get
            {
                return _triangles;
            }
        }
        private List<Triangle> _triangles;

        private Sprite backupSprite;
        #endregion

        void Start()
        {
            _sr = GetComponent<SpriteRenderer>();

            AssignSprite(_sr.sprite);
        }

        public Sprite CopySprite(Sprite p_sprite) {
            var pix = p_sprite.texture.GetPixels32();

            // Copy the reversed image data to a new texture.
            Texture2D tex = new Texture2D(_sr.sprite.texture.width, _sr.sprite.texture.height);
            tex.SetPixels32(pix);
            tex.Apply();

            return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        public void AssignSprite(Sprite p_sprite) {
            backupSprite = CopySprite(p_sprite);
            _sr.sprite = CopySprite(p_sprite);

            _triangles = GenerateTriangle(_sr.sprite);
        }

        public void AssignMesh(Vector2[] p_vertices, ushort[] p_triangles) {
            _sr.sprite.OverrideGeometry(p_vertices, p_triangles);
        }

        /// <summary>
        /// Create Triangle class by given sprite
        /// </summary>
        /// <param name="p_sprite"></param>
        /// <returns></returns>
        private List<Triangle> GenerateTriangle(Sprite p_sprite)
        {
            List<Triangle> triangles = new List<Triangle>();
            ushort[] raw_triangles = p_sprite.triangles;
            Vector2[] vertices = p_sprite.vertices;

            int a, b, c;
            for (int i = 0; i < raw_triangles.Length; i = i + 3)
            {
                a = raw_triangles[i];
                b = raw_triangles[i + 1];
                c = raw_triangles[i + 2];

                List<Vector2> nodes = new List<Vector2> { vertices[a], vertices[b], vertices[c] };
                List<TrigPairs> pairs = new List<TrigPairs> { new TrigPairs(vertices[a], vertices[b]),
                                                        new TrigPairs(vertices[b], vertices[c]),
                                                        new TrigPairs(vertices[c], vertices[a])
                                                    };

                triangles.Add(new Triangle(nodes, pairs));
            }

            return triangles;
        }

        public void ChangeSpriteMesh(List<Triangle> p_triangles, ushort[] p_meshTrig, Vector2[] p_meshVert)
        {
            this._triangles = p_triangles;
            Vector2[] spriteVertices = new Vector2[p_meshVert.Length];

            for (int i = 0; i < p_meshVert.Length; i++)
            {
                spriteVertices[i] = VerticesToWorldPos(p_meshVert[i], _sr.sprite);
            }

            //Override the geometry with the new vertices
            AssignMesh(spriteVertices, p_meshTrig);
        }

        private Vector2 VerticesToWorldPos(Vector2 vertices, Sprite sprite)
        {
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

        public void Restore()
        {
            if (backupSprite != null) {
                AssignMesh(backupSprite.vertices, backupSprite.triangles);
                _triangles = GenerateTriangle(backupSprite);
            }
        }

        #region Debug
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

        void DrawTriangle(List<Triangle> p_triangle)
        {
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
        #endregion


    }
}