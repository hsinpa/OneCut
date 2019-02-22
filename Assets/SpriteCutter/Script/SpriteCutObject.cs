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

        public void Restore()
        {
            if (backupSprite != null)
                AssignMesh(backupSprite.vertices, backupSprite.triangles);
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

    }
}