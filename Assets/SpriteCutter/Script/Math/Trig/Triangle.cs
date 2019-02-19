using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SC.Math {
    public class Triangle
    {
        public string segmentID;
        public List<TrigPairs> pairs;


        public List<Fragment> fragments {
            get
            {
                return _fragments;
            }
        }
        private List<Fragment> _fragments;

        public List<Vector2> nodes
        {
            get
            {
                return _nodes;
            }
        }
        private List<Vector2> _nodes;

        public bool isValid {
            get {
                return (_fragments.Count <= 0);
            }
        }

        public Vector2 center {
            get {
                Debug.Log( "Center " + ((nodes[0] + nodes[1] + nodes[2]) / 3) +", node[0]" + nodes[0] + ", nodes[1]" + nodes[1] + ", nodes[2]" + nodes[2]);
                return (nodes[0] + nodes[1] + nodes[2] ) / 3 ; 
            }
        }

        public float area {
            get {
                return GetArea(_nodes);
            }
        }

        public Triangle(List<Vector2> nodes, List<TrigPairs> pairs)
        {
            this._nodes = nodes;
            this.pairs = pairs;

            this.segmentID = "trig";

            this._fragments = new List<Fragment>();
        }

        public void AddFragment(Fragment[] p_fragments)
        {
            foreach (var f in p_fragments)
                AddFragment(f);
        }

        public void AddFragment(Fragment p_fragment) {
            //Check if exist
            if (_fragments.FindIndex(x => x.node.Equals(p_fragment.node)) >= 0) {
                //Debug.LogWarning("Find repeat fragment");
                return;
            };

            _fragments.Add(p_fragment);
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

        private Triangle CreateTrig( List<Vector2> p_nodes)
        {
            if (p_nodes.Count == 3)
            {
                return new Triangle(p_nodes, new List<TrigPairs>
                {
                    new TrigPairs(p_nodes[0], p_nodes[1]),
                    new TrigPairs(p_nodes[1], p_nodes[2]),
                    new TrigPairs(p_nodes[2], p_nodes[0])
                });
            }

            return null;
        }

        public void ClearUp() {
            _fragments.Clear();
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
