using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SC.Math;
using SC.Trig;
using System.Threading;

namespace SC.Main {

    public class SpriteCutter : MonoBehaviour
    {
        //Singleton
        private static SpriteCutter s_Instance;
        public static SpriteCutter Instance
        {
            get
            {
                if (s_Instance != null)
                {
                    return s_Instance;
                }

                s_Instance = FindObjectOfType<SpriteCutter>();
                if (s_Instance != null)
                {
                    return s_Instance;
                }

                return null;
            }
        }

        //For debug purpose
        private List<Vector2> intersectionPoints = new List<Vector2>();
        private List<Vector2> verticeSegmentOne;
        private List<Vector2> verticeSegmentTwo;
        private MeshBuilder meshBuilder;

        private SpriteCutAlgorithm mainAlgorithm;
        private Queue<TaskResult> results = new Queue<TaskResult>();

        public delegate void MathAction(CutResult cutResult);

        void Start()
        {
            s_Instance = this;
            mainAlgorithm = new SpriteCutAlgorithm();
        }


        public void Cut(SpriteCutObject p_spriteObj, Vector2 p_point1, Vector2 p_point2, System.Action<CutResult, bool> p_callback)
        {
            //Debug.Log("Call Cut");

            Thread t = new Thread(new ThreadStart(delegate
            {

                CutResult cutResult = mainAlgorithm.CutSpriteToMesh(p_spriteObj, p_point1, p_point2);
                bool isSuccess = (cutResult.intersectionPoints.Count > 0);
                Debug.Log("Success " + isSuccess);


                lock (results)
                {
                    results.Enqueue(new TaskResult(cutResult, isSuccess, p_callback));
                }

            }));

            t.Start();
        }

        public void Update()
        {
            if (results.Count > 0)
            {
                int itemsInQueue = results.Count;
                lock (results)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        TaskResult result = results.Dequeue();
                        //result.callback(result.path, result.success);
                    }
                }
            }
        }

        public struct TaskRequest
        {
            public SpriteCutObject target_sprite;
            public Vector2 pathStart;
            public Vector2 pathEnd;
            public System.Action<Vector3[], bool> callback;

            public TaskRequest(SpriteCutObject p_targetSprite, Vector2 _start, Vector2 _end, System.Action<Vector3[], bool> _callback)
            {
                target_sprite = p_targetSprite;
                pathStart = _start;
                pathEnd = _end;
                callback = _callback;
            }
        }

        public struct TaskResult
        {
            public CutResult cutResult;
            public bool isSuccess;
            public System.Action<CutResult, bool> callback;

            public TaskResult(CutResult p_cutResult, bool p_isSuccess, System.Action<CutResult, bool> p_callback)
            {
                cutResult = p_cutResult;
                isSuccess = p_isSuccess;
                callback = p_callback;
            }
        }

        #region Debug Functions
        void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position
            if (intersectionPoints != null)
            {
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

            if (meshBuilder != null)
            {
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
        #endregion


        public struct CutResult {
            public Sprite mainSprite;
            public Sprite subSprite;
            public List<Vector2> intersectionPoints;

            public CutResult(Sprite mainSprite, Sprite subSprite, List<Vector2> intersectionPoints)
            {
                this.mainSprite = mainSprite;
                this.subSprite = subSprite;
                this.intersectionPoints = intersectionPoints;
            }
        }

        public struct Sprite {
            public List<Triangle> triangle;
            public ushort[] meshTrig;
            public Vector2[] meshVert;

            public Sprite(List<Triangle> triangle, ushort[] meshTrig, Vector2[] meshVert)
            {
                this.triangle = triangle;
                this.meshTrig = meshTrig;
                this.meshVert = meshVert;
            }
        }

    }


}
