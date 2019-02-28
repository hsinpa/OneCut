using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using OC.Math;
using OC.Trig;

#if !UNITY_WEBGL	
    using System.Threading;
#endif
namespace OC.Main {

    public class OneCut : MonoBehaviour
    {
        #region Parameter

        //Singleton
        private static OneCut s_Instance;
        public static OneCut Instance
        {
            get
            {
                if (s_Instance != null)
                {
                    return s_Instance;
                }

                s_Instance = FindObjectOfType<OneCut>();
                if (s_Instance != null)
                {
                    return s_Instance;
                }

                return null;
            }
        }
        public bool debugMode = false;

        private OneCutAlgorithm mainAlgorithm;
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
        #endregion

        void Start()
        {
            s_Instance = this;
            mainAlgorithm = new OneCutAlgorithm();
        }

        /// <summary>
        /// Cut object with line given
        /// </summary>
        /// <param name="p_spriteObj">Object to cut</param>
        /// <param name="p_point1">Line header</param>
        /// <param name="p_point2">Line footer</param>
        /// <param name="p_callback">Callback after calculation</param>
        public void Cut(OneCutObject p_spriteObj, Vector2 p_point1, Vector2 p_point2, System.Action<CutResult, bool> p_callback)
        {
            Vector2 objectPos = p_spriteObj.transform.position;
#if !UNITY_WEBGL
            RunCutAyns(delegate { RunCut(p_spriteObj, objectPos, p_point1, p_point2, p_callback); });
#else
            RunCut(p_spriteObj, p_point1, p_point2, p_callback);
#endif
        }

        /// <summary>
        /// Actual cutting mehtod
        /// </summary>
        /// <param name="p_spriteObj"></param>
        /// <param name="objectPos">Transform.positon couldn;t be obtain inside Threading, so give it individually here</param>
        /// <param name="p_point1"></param>
        /// <param name="p_point2"></param>
        /// <param name="p_callback"></param>
        private void RunCut(OneCutObject p_spriteObj, Vector2 objectPos, Vector2 p_point1, Vector2 p_point2, System.Action<CutResult, bool> p_callback) {
            List<Triangle> originalTrig = p_spriteObj.triangles;
            CutResult cutResult = mainAlgorithm.CutSpriteToMesh(p_spriteObj, objectPos, p_point1, p_point2);

            var newArea = System.Math.Round(cutResult.mainSprite.area + cutResult.subSprite.area, 2);
            var oriArea = System.Math.Round(cutResult.originSprite.area, 2);
            bool isSuccess = (newArea >= oriArea);

            lock (results)
            {
                results.Enqueue(new TaskResult(cutResult, isSuccess, p_callback));
            }

            if (debugMode)
            {

                p_spriteObj.SetDebugVaraible(cutResult.intersectionPoints, cutResult.mainSprite.meshVert, cutResult.subSprite.meshVert);
            }

        }

#if !UNITY_WEBGL
        private void RunCutAyns(System.Action p_task) {
            Thread t = new Thread(new ThreadStart(p_task));
            t.Start();
        }
#endif

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

    }
}
