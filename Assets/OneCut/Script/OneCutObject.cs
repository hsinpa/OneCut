using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OC.Trig;

namespace OC.Main
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class OneCutObject : MonoBehaviour
    {
        #region Parameter
        public SpriteRenderer sr {
            get {
                return _sr;
            }
        }
        private SpriteRenderer _sr;

        [HideInInspector]
        public Sprite sprite;

        public List<Triangle> triangles
        {
            get
            {
                return _triangles;
            }
        }
        private List<Triangle> _triangles;


        public ushort[] meshTrig
        {
            get
            {
                return _meshTrig;
            }
        }
        private ushort[] _meshTrig;


        public Vector2[] meshVert
        {
            get
            {
                return _meshVert;
            }
        }
        private Vector2[] _meshVert;

        private Sprite backupSprite;

        //For debug purpose
        private List<Vector2> intersectionPoints = new List<Vector2>();
        private Vector2[] verticeSegmentOne;
        private Vector2[] verticeSegmentTwo;

        #endregion

        void Start()
        {
            _sr = GetComponent<SpriteRenderer>();

            AssignSprite(_sr.sprite);
        }

        private Sprite CopySprite(Sprite p_sprite) {
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
            sprite = _sr.sprite;

            _triangles = GenerateTriangle(_sr.sprite);

            AssignMesh(p_sprite.vertices, p_sprite.triangles, false);
        }

        public void AssignMesh(Vector2[] p_vertices, ushort[] p_triangles, bool overrideGeometry = true) {
            _meshVert = p_vertices;
            _meshTrig = p_triangles;

            if (overrideGeometry)
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
                spriteVertices[i] = VerticesToGeometryPos(p_meshVert[i], _sr.sprite);
            }

            //Override the geometry with the new vertices
            AssignMesh(spriteVertices, p_meshTrig);
        }

        public void SetDebugVaraible(List<Vector2> intersectionPoints, Vector2[] segmentOne, Vector2[] segmentTwo) {
            this.intersectionPoints = intersectionPoints;

            this.verticeSegmentOne = segmentOne;

            this.verticeSegmentTwo = segmentTwo;
        }

        private Vector2 VerticesToGeometryPos(Vector2 vertices, Sprite sprite)
        {
            Vector2 geometryPos = Vector2.zero;

            geometryPos.x = Mathf.Clamp(
                        (vertices.x - sprite.bounds.center.x -
                            (sprite.textureRectOffset.x / sprite.texture.width) + sprite.bounds.extents.x) /
                        (2.0f * sprite.bounds.extents.x) * sprite.rect.width,
                        0.0f, sprite.rect.width);

            geometryPos.y = Mathf.Clamp(
                    (vertices.y - sprite.bounds.center.y -
                        (sprite.textureRectOffset.y / sprite.texture.height) + sprite.bounds.extents.y) /
                    (2.0f * sprite.bounds.extents.y) * sprite.rect.height,
                    0.0f, sprite.rect.height);

            return geometryPos;
        }

        /// <summary>
        /// Re-apply the original sprite
        /// </summary>
        public void Restore()
        {
            if (backupSprite != null) {
                AssignMesh(backupSprite.vertices, backupSprite.triangles);
                _triangles = GenerateTriangle(backupSprite);
            }
        }

        #region Debug Tool

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


        void OnDrawGizmosSelected()
        {
            Vector2 currentPos = transform.position;
            // Draw a yellow sphere at the transform's position
            if (intersectionPoints != null)
            {
                Gizmos.color = Color.yellow;
                for (int i = 0; i < intersectionPoints.Count; i++)
                {


                    Gizmos.DrawSphere(intersectionPoints[i], 0.06f);
                }
            }

            if (verticeSegmentOne != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < verticeSegmentOne.Length; i++)
                {
                    Gizmos.DrawSphere(verticeSegmentOne[i] + currentPos, 0.04f);
                }
            }

            if (verticeSegmentTwo != null)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < verticeSegmentTwo.Length; i++)
                {
                    Gizmos.DrawSphere(verticeSegmentTwo[i] + currentPos, 0.04f);
                }
            }
        }

        #endregion


    }
}