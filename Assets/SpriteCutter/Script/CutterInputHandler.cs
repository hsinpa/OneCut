using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutterInputHandler : MonoBehaviour {

    private LineRenderer lineRenderer;
    public Camera camera;
    public LayerMask layermask;

    private Vector3 MouseDownPos, MouseUpPos;



    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }


    void Update () {
 //        var v3 = Input.mousePosition;
 //v3.z = 10.0;
 //v3 = Camera.main.ScreenToWorldPoint(v3);

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Mouse Down");
            MouseDownPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("Mouse Up");
            MouseUpPos = Input.mousePosition;
            MousePosToWorld(MouseDownPos, MouseUpPos);
        }
    }

    void MousePosToWorld(Vector3 p_mouseDown, Vector3 p_mouseUp) {
        Vector3 mouseDownWorld = camera.ScreenToWorldPoint(p_mouseDown);
        Vector3 mouseUpWorld = camera.ScreenToWorldPoint(p_mouseUp);

        //Debug.Log(mouseDownWorld);
        //Debug.Log(mouseUpWorld);

        RaycastHit2D[] hit2d = Physics2D.LinecastAll(mouseDownWorld, mouseUpWorld, layermask);

        if (hit2d.Length > 0) {
            SpriteCutter spriteCutter = hit2d[0].collider.GetComponent<SpriteCutter>();
            spriteCutter.FindIntersection(mouseDownWorld, mouseUpWorld);

            
            
        }

        //for (int i = 0; i < hit2d.Length; i++)
        //{
        //}


    }

}
