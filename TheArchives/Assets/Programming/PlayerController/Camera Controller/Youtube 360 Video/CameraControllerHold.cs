using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraController.Scripts.HoldButton
{
    public class CameraControllerHold : MonoBehaviour
    {
        public float sensitivityX = 15;
        public float sensitivityY = 15;
        public float minimumX = -360;
        public float maximumX = 360;
        public float minimumY = -90;
        public float maximumY = 90;
        public float dampFactor = 1f;

        private float rotationX;
        private float rotationY;
        private Quaternion originalRotation;

        private Vector2 rotationSpeed;

        private void Start()
        {
            originalRotation = transform.localRotation;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                rotationSpeed = new Vector2(mouseX, mouseY);
            }
            else
            {
                rotationSpeed = Vector2.Lerp(rotationSpeed, Vector2.zero, Time.deltaTime * dampFactor);
            }

            rotationX -= rotationSpeed.x * sensitivityX;
            rotationY -= rotationSpeed.y * sensitivityY;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            while (angle < -360)
            {
                angle += 360;
            }
            while (angle > 360)
            {
                angle -= 360;
            }
            return Mathf.Clamp(angle, min, max);
        }
    }
}
