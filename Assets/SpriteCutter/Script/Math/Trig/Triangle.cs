using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SC.Math {
    public class Triangle
    {
        public string segmentID;
        public List<TrigPairs> pairs;

        private List<Fragment> fragments;
        private List<Vector2> nodes;


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

            this.segmentID = "trig";

            this.fragments = new List<Fragment>();
        }

        public Triangle[] Split() {
            if (!isValid) {
                List<Fragment> segmentA = fragments.FindAll(x => x.segment_name == "A");
                List<Fragment> segmentB = fragments.FindAll(x => x.segment_name == "B");

                if (segmentA.Count > 0 && segmentB.Count > 0)
                {
                    //Start with segmentA
                    if (segmentA.Count == 1) {

                    }


                }
            }

            return null;
        }

        public void AddFragment(Fragment[] p_fragments)
        {
            foreach (var f in p_fragments)
                AddFragment(f);
        }

        public void AddFragment(Fragment p_fragment) {
            //Check if exist
            if (fragments.FindIndex(x => x.node.Equals(p_fragment.node)) > 0) {
                Debug.LogWarning("Find repeat fragment");
                return;
            };

            fragments.Add(p_fragment);
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

        private CreateTrig()
        {

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
