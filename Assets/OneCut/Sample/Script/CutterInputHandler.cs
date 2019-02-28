using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OC.Main;

namespace OC.Sample {
    public class CutterInputHandler : MonoBehaviour
    {

        private LineRenderer lineRenderer;
        public Camera camera;
        public LayerMask layermask;

        private Vector3 MouseDownPos, MouseUpPos;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        void Update()
        {

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

        void MousePosToWorld(Vector3 p_mouseDown, Vector3 p_mouseUp)
        {
            Vector3 mouseDownWorld = camera.ScreenToWorldPoint(p_mouseDown);
            Vector3 mouseUpWorld = camera.ScreenToWorldPoint(p_mouseUp);

            RaycastHit2D[] hit2ds = Physics2D.LinecastAll(mouseDownWorld, mouseUpWorld, layermask);

            foreach (var hit in hit2ds) {
                if (hit.transform != null) {

                    OneCutObject spriteCutObject = hit.collider.GetComponent<OneCutObject>();
                    OneCut.Instance.Cut(spriteCutObject, mouseDownWorld, mouseUpWorld, (OneCut.CutResult result, bool isSuccess) => {


                        if (isSuccess) {
                            //Call SpriteCutObject.ChangeSpriteMesh to apply new generated mesh
                            spriteCutObject.ChangeSpriteMesh(result.mainSprite.triangle, result.mainSprite.meshTrig, result.mainSprite.meshVert);
                        }
                    });
                }
            }

        }
    }
}