using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using SC.Trig;

namespace SC.Main {
    public class TaskProcessor
    {
        Queue<CutResult> results = new Queue<CutResult>();

        public void AssignTask() {

        }

        public void OnUpdate() {
            if (results.Count > 0)
            {
                int itemsInQueue = results.Count;
                lock (results)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        CutResult result = results.Dequeue();
                        //result.callback(result.path, result.success);
                    }
                }
            }
        }

        public struct CutRequest
        {
            public Vector2 pathStart;
            public Vector2 pathEnd;
            public System.Action<Vector3[], bool> callback;

            public CutRequest(Vector2 _start, Vector2 _end, System.Action<Vector3[], bool> _callback)
            {
                pathStart = _start;
                pathEnd = _end;
                callback = _callback;
            }
        }

        public struct CutResult
        {
            public List<Triangle> mainTrig;
            public List<Triangle> subTrig;
            public bool isSuccess;
            public System.Action<List<Triangle>, List<Triangle>, bool> callback;

            public CutResult(List<Triangle> _mainTrig, List<Triangle> _subTrig, System.Action<List<Triangle>, List<Triangle>, bool> _callback)
            {
                mainTrig = _mainTrig;
                subTrig = _subTrig;
                callback = _callback;
                isSuccess = (_mainTrig.Count > 0);
            }
        }

    }

}

