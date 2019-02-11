using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.Math {
    public class Triangle
    {
        private List<Vector2> nodes;
        private List<TrigPairs> pairs;

        public float area {
            get {
                return GetArea(nodes);
            }
        }

        public Triangle(List<Vector2> nodes, List<TrigPairs> pairs)
        {
            this.nodes = nodes;
            this.pairs = pairs;
        }

        private float GetArea(List<Vector2> nodes)
        {
            if (nodes != null && nodes.Count >= 3) {
                Vector2 A = nodes[0], B = nodes[1], C = nodes[2];
                float a = Mathf.Abs((A - B).magnitude), b = Mathf.Abs((B - C).magnitude), c = Mathf.Abs((C - A).magnitude);
                float p = (a + b + c) / 2;

                return Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));
            }
            return 0;
        }

    }
}
