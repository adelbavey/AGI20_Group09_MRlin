using UnityEngine;

/// <summary>
/// The Vexpot.ColorTrackerDemo namespace contains all scripts used in demonstration scenes.
/// </summary>
namespace Vexpot.ColorTrackerDemo
{
    /// <summary>
    /// Used to properly orient the Unity camera according to the player's head in the VR demo scene.
    /// </summary>
    public class GyroController : MonoBehaviour
    {
        private bool gyroEnabled = false;
        private const float lowPassFilterFactor = 0.3f;

        private readonly Quaternion baseIdentity = Quaternion.Euler(90, 0, 0);

        private Quaternion cameraBase = Quaternion.identity;
        private Quaternion calibration = Quaternion.identity;
        private Quaternion baseOrientation = Quaternion.Euler(90, 0, 0);
        private Quaternion baseOrientationRotationFix = Quaternion.identity;

        private Quaternion referanceRotation = Quaternion.identity;
        private Gyroscope gyro = null;

        /// <summary>
        /// 
        /// </summary>
        protected void Start()
        {
            if (SystemInfo.supportsGyroscope)
            {
                EnableGyro();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Update()
        {
            if (!gyroEnabled)
                return;

            transform.rotation = Quaternion.Slerp(transform.rotation,
                cameraBase * (ConvertRotation(referanceRotation * gyro.attitude) * Quaternion.identity), lowPassFilterFactor);
        }

        private void EnableGyro()
        {
            gyro = Input.gyro;
            gyroEnabled = gyro.enabled = true;
            ResetBaseOrientation();
            UpdateCalibration(true);
            UpdateCameraBaseRotation(true);
            RecalculateReferenceRotation();
        }

        private void DisableGyro()
        {
            gyroEnabled = gyro.enabled = false;
        }

        private void UpdateCalibration(bool onlyHorizontal)
        {
            if (onlyHorizontal)
            {
                var fw = (gyro.attitude) * (-Vector3.forward);
                fw.z = 0;
                if (fw == Vector3.zero)
                {
                    calibration = Quaternion.identity;
                }
                else
                {
                    calibration = (Quaternion.FromToRotation(baseOrientationRotationFix * Vector3.up, fw));
                }
            }
            else
            {
                calibration = gyro.attitude;
            }
        }

        private void UpdateCameraBaseRotation(bool onlyHorizontal)
        {
            if (onlyHorizontal)
            {
                var fw = transform.forward;
                fw.y = 0;
                if (fw == Vector3.zero)
                {
                    cameraBase = Quaternion.identity;
                }
                else
                {
                    cameraBase = Quaternion.FromToRotation(Vector3.forward, fw);
                }
            }
            else
            {
                cameraBase = transform.rotation;
            }
        }

        private Quaternion ConvertRotation(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }

        private void ResetBaseOrientation()
        {
            baseOrientationRotationFix = Quaternion.identity;
            baseOrientation = baseOrientationRotationFix * baseIdentity;
        }

        private void RecalculateReferenceRotation()
        {
            referanceRotation = Quaternion.Inverse(baseOrientation) * Quaternion.Inverse(calibration);
        }

    }
}