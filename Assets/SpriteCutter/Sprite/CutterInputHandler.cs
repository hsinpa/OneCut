using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutterInputHandler : MonoBehaviour {

    private LineRenderer lineRenderer;
    public Camera camera;

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
            Debug.Log("Mouse Down");
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse Up");
        }
    }


}
