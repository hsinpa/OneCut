using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OC.Trig;
using System.Linq;
using OC.Math;

namespace OC.Main
{

    public class OneCutAlgorithm
    {
        #region Core Logic Flow

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_spriteObj"></param>
        /// <param name="p_sprite"></param>
        /// <param name="p_point1"></param>
        /// <param name="p_point2"></param>
        /// <returns></returns>
        public OneCut.CutResult CutSpriteToMesh(OneCutObject p_spriteObj, Vector2 selfPos, Vector2 p_point1, Vector2 p_point2)
        {
            List<Triangle> exp_trig = new List<Triangle>(p_spriteObj.triangles);
            List<Vector2> intersectionPoints = new List<Vector2>();

            Sprite sprite = p_spriteObj.sprite;
            Vector2[] vertices = p_spriteObj.meshVert;
            VerticesSegmentor verticesSegmentor = new VerticesSegmentor(p_point1, p_point2);

            for (int i = 0; i < exp_trig.Count; i++)
            {
                if (exp_trig[i].pairs.Count == 3)
                {
                    HandleTrigIntersection(intersectionPoints, exp_trig[i], verticesSegmentor, selfPos, p_point1, p_point2);
                }
            }

            List<Triangle> newTrigCol = TrigBuilder.Build(exp_trig);

            //always contain 2 trig result
            var segmentResult = ResegmentTriangle(newTrigCol, verticesSegmentor, selfPos);

            return new OneCut.CutResult(
                GetSprite(segmentResult[0]),
                GetSprite(segmentResult[1]),
                GetSprite(exp_trig),

                intersectionPoints
                
            );
                
            //ResegmentVertices(vertices, verticesSegmentor);
        }

        private OneCut.Sprite GetSprite(List<Triangle> p_triangles) {
            MeshBuilder meshBuilder = new MeshBuilder(p_triangles);

            return new OneCut.Sprite(p_triangles, meshBuilder.meshTrig, meshBuilder.meshVertices);
        }

        private void HandleTrigIntersection(List<Vector2> intersectList, Triangle triangle, VerticesSegmentor verticesSegmentor, Vector2 selfPos, Vector2 p_pointA, Vector2 p_pointB)
        {
            int pairNum = triangle.pairs.Count;

            for (int i = pairNum - 1; i >= 0; i--)
            {
                TrigPairs pair = triangle.pairs[i];

                Vector2 point = AddIntersectPoint(
                    pair.nodeA + selfPos,
                    pair.nodeB + selfPos,
                    p_pointA, p_pointB
                );

                //Intersection has occur
                if (!point.Equals(Vector2.positiveInfinity))
                {
                    Triangle.Fragment fragmentA = FindVerticesSegment(verticesSegmentor, pair.nodeA, pair.nodeA + selfPos);
                    Triangle.Fragment fragmentB = FindVerticesSegment(verticesSegmentor, pair.nodeB, pair.nodeB + selfPos);
                    Triangle.Fragment fragmentC = new Triangle.Fragment(point - selfPos, "", Triangle.Fragment.Type.Cutted);

                    triangle.AddFragment(new Triangle.Fragment[] { fragmentA, fragmentB, fragmentC });
                    intersectList.Add(point);
                }
            }
        }

        private Triangle.Fragment FindVerticesSegment(VerticesSegmentor verticesSegmentor, Vector2 vertices, Vector2 worldVert)
        {
            //Triangle.Fragment fragment = new Triangle.Fragment(vertices,);
            string segmentID = (verticesSegmentor.CompareInputWithAverageLine(worldVert)) ? "A" : "B";
            Triangle.Fragment fragment = new Triangle.Fragment(vertices, segmentID, Triangle.Fragment.Type.Original);

            return fragment;
        }

        private Vector2 AddIntersectPoint(Vector2 p_line1A, Vector2 p_line1B, Vector2 p_line2A, Vector2 p_line2B)
        {
            bool isIntersect = Line.DoIntersect(p_line1A, p_line1B, p_line2A, p_line2B);

            if (isIntersect)
            {
                Vector2 intersectPoint = Line.GetLineIntersection(p_line1A, p_line1B, p_line2A, p_line2B);
                if (!intersectPoint.Equals(Vector2.positiveInfinity))
                {
                    return intersectPoint;
                }
            }

            return Vector2.positiveInfinity;
        }
        #endregion

        #region Segmenting Functions
        /// <summary>
        /// Segment raw triangles into two according to cutting line
        /// </summary>
        /// <param name="p_triangles"></param>
        /// <param name="verticesSegmentor"></param>
        /// <returns></returns>
        private List<Triangle>[] ResegmentTriangle(List<Triangle> p_triangles, VerticesSegmentor verticesSegmentor, Vector2 selfPos)
        {
            List<Triangle> segmentOne = new List<Triangle>();
            List<Triangle> segmentTwo = new List<Triangle>();

            float segmentA_area = 0, segmentB_area = 0;


            for (int i = 0; i < p_triangles.Count; i++)
            {
                if (verticesSegmentor.CompareInputWithAverageLine(p_triangles[i].center + selfPos))
                {
                    p_triangles[i].segmentID = "A";
                    segmentA_area += p_triangles[i].area;
                    segmentOne.Add(p_triangles[i]);
                }
                else
                {
                    p_triangles[i].segmentID = "B";
                    segmentB_area += p_triangles[i].area;
                    segmentTwo.Add(p_triangles[i]);
                }
            }

            List<Triangle> mainTrig = (segmentA_area > segmentB_area) ? segmentOne : segmentTwo;
            List<Triangle> subTrig = (mainTrig == segmentOne) ? segmentTwo : segmentOne;

            return new List<Triangle>[] { mainTrig, subTrig };
        }

        private List<Vector2> SortIntersectionPoint(List<Vector2> unsortPoints, Vector2 cut_direction)
        {
            cut_direction = cut_direction * cut_direction;
            bool isSortByX = (cut_direction.x > cut_direction.y);

            return unsortPoints.OrderBy(x => (isSortByX) ? x.x : x.y).ToList();
        }
        #endregion
    }
}