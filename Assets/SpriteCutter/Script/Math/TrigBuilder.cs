using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SC.Math;

public class TrigBuilder {

    public List<Triangle> Build(List<Triangle> p_triangles) {
        List<Triangle> newTrigCollect = new List<Triangle>();

        for (int i = 0; i < p_triangles.Count; i++) {

            if (p_triangles[i].isValid) {
                newTrigCollect.Add(p_triangles[i]);
                continue;
            }


            

        }

        return null;
    }

    private List<Triangle> Split(Triangle p_trig) {

        List<Triangle.Fragment> cuttedNode = p_trig.fragments.FindAll(x => x.type == Triangle.Fragment.Type.Cutted);

        if (!p_trig.isValid && cuttedNode.Count == 2)
        {
            List<Triangle.Fragment> segmentA = p_trig.fragments.FindAll(x => x.segment_name == "A");
            List<Triangle.Fragment> segmentB = p_trig.fragments.FindAll(x => x.segment_name == "B");

            if (segmentA.Count > 0 && segmentB.Count > 0)
            {
                //Start with segmentA
                if (segmentA.Count == 1)
                {

                }



            }
        }


        return null;
    }


    private Triangle[] CreateTrigFromFragment(List<Triangle.Fragment> p_cuttedFrag, List<Triangle.Fragment> p_originalFrag)
    {
        if (p_cuttedFrag.Count != 2) return null;

        if (p_originalFrag.Count == 1)
        {
            return new Triangle[] { CreateTrig(new List<Vector2>() { p_cuttedFrag[0].node, p_cuttedFrag[1].node, p_originalFrag[0].node }) };
        }
        else if (p_originalFrag.Count == 2)
        {
            Triangle[] mTriangles = new Triangle[2];
            //First trig
            mTriangles[0] = CreateTrig(new List<Vector2>() { p_cuttedFrag[0].node, p_cuttedFrag[1].node, p_originalFrag[0].node });

            //Second trig
            for (int p = 0; p < mTriangles[0].pairs.Count; p++)
            {
                for (int c = 0; c < p_cuttedFrag.Count; c++)
                {

                    Vector2 intersectPoint = MathUtility.Line.lineLineIntersection(p_originalFrag[1].node, p_cuttedFrag[c].node,
                                                                                    mTriangles[0].pairs[p].nodeA, mTriangles[0].pairs[p].nodeB);
                    if (intersectPoint != Vector2.positiveInfinity)
                    {
                        mTriangles[1] = CreateTrig(new List<Vector2>() { p_originalFrag[1].node, p_originalFrag[0].node, p_cuttedFrag[c].node });

                        return mTriangles;
                    }

                }
            }
        }

        return null;
    }

    private Triangle CreateTrig(List<Vector2> p_nodes)
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


    private void Reset() {

    }
}
