using UnityEngine;

namespace SC.Math
{
    public struct TrigPairs
    {
        Vector2 nodeA;
        Vector2 nodeB;

        public TrigPairs(Vector2 nodeA, Vector2 nodeB) {
            this.nodeA = nodeA;
            this.nodeB = nodeB;
        }
    }
}