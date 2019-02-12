using UnityEngine;

namespace SC.Math
{
    public struct TrigPairs
    {
        public Vector2 nodeA;
        public Vector2 nodeB;

        public TrigPairs(Vector2 nodeA, Vector2 nodeB) {
            this.nodeA = nodeA;
            this.nodeB = nodeB;
        }
    }
}