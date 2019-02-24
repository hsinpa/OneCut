using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SC.Main;

namespace SC.Sample {
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

            RaycastHit2D[] hit2d = Physics2D.LinecastAll(mouseDownWorld, mouseUpWorld, layermask);

            if (hit2d.Length > 0)
            {
                SpriteCutObject spriteCutObject = hit2d[0].collider.GetComponent<SpriteCutObject>();

                SpriteCutter.Instance.Cut(spriteCutObject, mouseDownWorld, mouseUpWorld, (SpriteCutter.CutResult result, bool isSuccess) => {

                    spriteCutObject.ChangeSpriteMesh(result.mainSprite.triangle, result.mainSprite.meshTrig, result.mainSprite.meshVert);

                });
            }
        }
    }
}