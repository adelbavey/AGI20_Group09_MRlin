using UnityEngine;
using System.Collections;

/// <summary>
/// The Vexpot.ColorTrackerDemo namespace contains all scripts used in demonstration scenes.
/// </summary>
namespace Vexpot.ColorTrackerDemo
{
    /// <summary>
    /// Used to control the Unity camera in World Test demo.
    /// </summary>
    public class MouseController : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public enum RotationAxes {
            /// <summary>
            /// 
            /// </summary>
            MouseXAndY ,
            /// <summary>
            /// 
            /// </summary>
            MouseX ,
            /// <summary>
            /// 
            /// </summary>
            MouseY
        }
        /// <summary>
        /// 
        /// </summary>
        public RotationAxes axes = RotationAxes.MouseXAndY;
        /// <summary>
        /// 
        /// </summary>
        public float sensitivityX = 15f;
        /// <summary>
        /// 
        /// </summary>
        public float sensitivityY = 15f;
        /// <summary>
        /// 
        /// </summary>
        public float minimumX = -360f;
        /// <summary>
        /// 
        /// </summary>
        public float maximumX = 360f;
        /// <summary>
        /// 
        /// </summary>
        public float minimumY = -60f;
        /// <summary>
        /// 
        /// </summary>
        public float maximumY = 60F;

        private float rotationY = 0F;
        private bool mouse_on = true;

        /// <summary>
        /// Updates camera position according to the mouse position.
        /// </summary>
        void Update()
        {
            if (mouse_on)
            {

                if (axes == RotationAxes.MouseXAndY)
                {
                    float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                    transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
                }
                else if (axes == RotationAxes.MouseX)
                {
                    transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
                }
                else
                {
                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                    transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
                }
            }
        }

        void Start()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                mouse_on = false;
        }
    }
}