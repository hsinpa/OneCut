using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtility;

public class VerticesSegmentor {
    private bool isHorizontal;
    private System.Func<float, float> segmentEquation;

    private System.Func<float, float> segmentEquationX;
    private System.Func<float, float> segmentEquationY;


    public VerticesSegmentor(Vector2 point1, Vector2 point2)
    {
        float slope = Line.CalculateSlopeM(point1, point2);
        isHorizontal = ((slope) < 0.5f);

        Debug.Log("IsHorizontal " + isHorizontal +", Slope " + slope);

        segmentEquationX = MathUtility.Line.CalculateLinearRegressionX(point1, point2);
        segmentEquationY = MathUtility.Line.CalculateLinearRegressionY(point1, point2);
        segmentEquation = FindSegmentEquation(point1, point2, isHorizontal);
    }

    private System.Func<float, float> FindSegmentEquation(Vector2 p_point1, Vector2 p_point2, bool isHorizontal)
    {
        System.Func<float, float> slopeFormula = null;

        float slope = MathUtility.Line.CalculateSlopeM(p_point1, p_point2);
        if (!isHorizontal)
            slopeFormula = MathUtility.Line.CalculateLinearRegressionX(p_point1, p_point2);
        else
            slopeFormula = MathUtility.Line.CalculateLinearRegressionY(p_point1, p_point2);

        return slopeFormula;
    }

    public bool CompareInputWithAverageLine(Vector2 p_point) {
        float v = (isHorizontal) ? segmentEquation(p_point.x) : segmentEquation(p_point.y);
        Debug.Log("v " + v + ", Point x " + p_point.x + ", Point y " + p_point.y);
        if (isHorizontal)
        {
            //var v = segmentEquationY(p_point.y);

            //if (v == p_point.y)
            //{
            //    return segmentEquationX(p_point.x) < p_point.x;
            //}
            //else
            //{
            //    return v > p_point.y;
            //}
            return (v > p_point.y);
        }
        else
        {
            //var yValue = segmentEquationX(p_point.x);

            //if (v == p_point.x)
            //{
            //    return segmentEquationY(p_point.y) < p_point.y;
            //}
            //else
            //{
            //    return v > p_point.x;
            //}

            return (v > p_point.x);
        }
    }
}
