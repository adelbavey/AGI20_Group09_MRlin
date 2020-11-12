using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeviceRotation
{
    private static bool gyroInitialized = false;
    private static Quaternion initRot;

    public static bool HasGyroscope
    {
        get
        {
            return SystemInfo.supportsGyroscope;
        }
    }

    public static Quaternion Get()
    {
        if (!gyroInitialized)
        {
            InitGyro();
        }

        return HasGyroscope
            ? ReadGyroscopeRotation()
            : Quaternion.identity;
    }

    private static void InitGyro()
    {
        if (HasGyroscope)
        {
            Input.gyro.enabled = true;                // enable the gyroscope
            Input.gyro.updateInterval = 0.0167f;    // set the update interval to it's highest value (60 Hz)
        }
        gyroInitialized = true;
        initRot = Input.gyro.attitude;
    }

    private static Quaternion ReadGyroscopeRotation()
    {
        return Input.gyro.attitude * Quaternion.Inverse(initRot) /**  new Quaternion(0, 0, 1, 0) */;
    }
}
