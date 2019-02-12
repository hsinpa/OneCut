using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.Math {
    public class Triangle
    {
        private List<Vector2> nodes;
        public List<TrigPairs> pairs;

        public bool isValid {
            get {
                return (pairs.Count == 3);
            }
        }

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

        public Triangle[] Split() {


            return null;
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

        public struct Fragment {
            public Vector2 node;
            public string segment_name;

            public enum Type {
                Original,
                Cutted
            }

            public Type type;

            public Fragment(Vector2 node, string segment_name, Type type) {
                this.node = node;
                this.segment_name = segment_name;
                this.type = type;
            }
        }

    }
}
