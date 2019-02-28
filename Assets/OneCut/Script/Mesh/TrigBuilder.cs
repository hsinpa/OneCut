using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OC.Trig;
using OC.Math;
using System.Linq;

namespace OC.Main
{
    public class TrigBuilder
    {

        public static List<Triangle> Build(List<Triangle> p_triangles)
        {
            List<Triangle> newTrigCollect = new List<Triangle>();

            for (int i = 0; i < p_triangles.Count; i++)
            {

                if (p_triangles[i].isValid)
                {

                    newTrigCollect.Add(p_triangles[i]);
                    continue;
                }
                else
                {
                    newTrigCollect.AddRange(Split(p_triangles[i]));
                }
            }

            return newTrigCollect;
        }

        private static List<Triangle> Split(Triangle p_trig)
        {

            List<Triangle.Fragment> cuttedNode = p_trig.fragments.FindAll(x => x.type == Triangle.Fragment.Type.Cutted);
            List<Triangle> generatedTrig = new List<Triangle>();

            if (!p_trig.isValid && cuttedNode.Count == 2)
            {
                List<Triangle.Fragment> segmentA = p_trig.fragments.FindAll(x => x.segment_name == "A");
                List<Triangle.Fragment> segmentB = p_trig.fragments.FindAll(x => x.segment_name == "B");

                if (segmentA.Count > 0 && segmentB.Count > 0)
                {
                    Triangle[] trigA = CreateTrigFromFragment(cuttedNode, segmentA);
                    Triangle[] trigB = CreateTrigFromFragment(cuttedNode, segmentB);

                    if (trigA != null)
                        generatedTrig.AddRange(trigA);

                    if (trigB != null)
                        generatedTrig.AddRange(trigB);
                }
            }

            p_trig.ClearUp();

            return generatedTrig;
        }


        private static Triangle[] CreateTrigFromFragment(List<Triangle.Fragment> p_cuttedFrag, List<Triangle.Fragment> p_originalFrag)
        {
            if (p_cuttedFrag.Count != 2) return null;

            if (p_originalFrag.Count == 1)
            {
                return new Triangle[] { CreateTrig(SortVertices(new List<Vector2>() { p_cuttedFrag[0].node, p_cuttedFrag[1].node, p_originalFrag[0].node })) };
            }
            else if (p_originalFrag.Count == 2)
            {
                Triangle[] mTriangles = new Triangle[2];
                //First trig
                List<Vector2> sortedVertices = SortVertices(new List<Vector2>() { p_cuttedFrag[0].node, p_cuttedFrag[1].node, p_originalFrag[0].node });

                mTriangles[0] = CreateTrig(sortedVertices);

                //Second trig
                for (int c = 0; c < p_cuttedFrag.Count; c++)
                {
                    bool intersectPointA = Line.DoIntersect(p_originalFrag[1].node, p_cuttedFrag[c].node,
                                                                    p_originalFrag[0].node, p_cuttedFrag[0].node);

                    bool intersectPointB = Line.DoIntersect(p_originalFrag[1].node, p_cuttedFrag[c].node,
                                                    p_originalFrag[0].node, p_cuttedFrag[1].node);

                    int int_count = (intersectPointA) ? 1 : 0;
                    int_count += (intersectPointB) ? 1 : 0;

                    if (int_count == 1)
                    {
                        mTriangles[1] = CreateTrig(SortVertices(new List<Vector2>() { p_originalFrag[0].node, p_originalFrag[1].node, p_cuttedFrag[c].node }));

                        return mTriangles;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sort to Clock-Counter
        /// </summary>
        /// <returns></returns>
        private static List<Vector2> SortVertices(List<Vector2> p_vertices)
        {
            return p_vertices.OrderByDescending(v => v.x).ThenBy(v => v.y).ToList();
        }



        private static Triangle CreateTrig(List<Vector2> p_nodes)
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
    }
}
