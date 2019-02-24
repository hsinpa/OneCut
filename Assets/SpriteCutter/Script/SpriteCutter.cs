using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SC.Math;
using SC.Trig;

#if !UNITY_WEBGL	
    using System.Threading;
#endif
namespace SC.Main {

    public class SpriteCutter : MonoBehaviour
    {
        #region Parameter

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

        private SpriteCutAlgorithm mainAlgorithm;
        private Queue<TaskResult> results = new Queue<TaskResult>();

        ///// <summary>
        ///// System will auto increase the length of cut if not long enough
        ///// </summary>
        //public bool autoCompensation;

        ///// <summary>
        ///// Ratio to decide whether want to cut image or not according to sprite bounding box
        ///// </summary>
        //[Range(0, 1)]
        //public float acceptedRatio = 1;

        //For debug purpose
        private List<Vector2> intersectionPoints = new List<Vector2>();
        private List<Vector2> verticeSegmentOne;
        private List<Vector2> verticeSegmentTwo;
        private MeshBuilder meshBuilder;
        #endregion

        void Start()
        {
            s_Instance = this;
            mainAlgorithm = new SpriteCutAlgorithm();
        }


        public void Cut(SpriteCutObject p_spriteObj, Vector2 p_point1, Vector2 p_point2, System.Action<CutResult, bool> p_callback)
        {
            //Debug.Log("Call Cut");
            var sprite = p_spriteObj.sr.sprite;

            Vector2 direction = (p_point2 - p_point1).normalized;
            //var lossPercent = Line.CalculateLossPercent(p_spriteObj.sr.bounds.size.magnitude, (p_point2 - p_point1).magnitude);

            //if (autoCompensation && (acceptedRatio <= lossPercent))
            //{
            //    //Debug.Log("Loss Percent " + lossPercent + ", Direciton " + direction);
            //    //Debug.Log("Point1 " + p_point1 + ", Point2 " + p_point2);
            //    var slopeFormula = Line.CalculateLinearRegressionY(p_point1, p_point2);

            //    var positionOneX = p_spriteObj.transform.position.x - p_spriteObj.sr.bounds.size.x;
            //    var positionTwoX = p_spriteObj.transform.position.x + p_spriteObj.sr.bounds.size.x;

            //    p_point1 = new Vector2(positionOneX, slopeFormula(positionOneX));
            //    p_point2 = new Vector2(positionTwoX, slopeFormula(positionTwoX));
            //}
            //else {
            //    p_callback(default(CutResult), false);
            //    return;
            //}


#if !UNITY_WEBGL	
            Thread t = new Thread(new ThreadStart(delegate
            {
#endif
                List<Triangle> originalTrig = p_spriteObj.triangles;
                CutResult cutResult = mainAlgorithm.CutSpriteToMesh(p_spriteObj, p_point1, p_point2);

                var newArea = System.Math.Round(cutResult.mainSprite.area + cutResult.subSprite.area, 2);
                var oriArea = System.Math.Round(cutResult.originSprite.area, 2);
                bool isSuccess = (newArea >= oriArea);

                lock (results)
                {
                    results.Enqueue(new TaskResult(cutResult, isSuccess, p_callback));
                }

#if !UNITY_WEBGL

            }));

            t.Start();
#endif
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
                        result.callback(result.cutResult, result.isSuccess);
                    }
                }
            }
        }


        #region Data Structure
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

        public struct CutResult
        {
            public Sprite mainSprite;
            public Sprite subSprite;
            public Sprite originSprite;
            public List<Vector2> intersectionPoints;

            public CutResult(Sprite mainSprite, Sprite subSprite, Sprite originSprite, List<Vector2> intersectionPoints)
            {
                this.mainSprite = mainSprite;
                this.subSprite = subSprite;
                this.originSprite = originSprite;

                this.intersectionPoints = intersectionPoints;
            }
        }

        public struct Sprite
        {
            public List<Triangle> triangle;
            public ushort[] meshTrig;
            public Vector2[] meshVert;
            public float area;

            public Sprite(List<Triangle> triangle, ushort[] meshTrig, Vector2[] meshVert)
            {
                this.triangle = triangle;
                this.meshTrig = meshTrig;
                this.meshVert = meshVert;

                this.area = 0;
                this.area = GetArea(triangle);
            }

            private float GetArea(List<Triangle> p_triangle)
            {
                float a = 0;
                foreach (Triangle trig in p_triangle)
                {
                    a += trig.area;
                }
                return a;
            }
        }
        #endregion

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

    }


}
