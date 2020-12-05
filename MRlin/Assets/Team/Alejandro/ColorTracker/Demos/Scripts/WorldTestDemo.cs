using UnityEngine;
using Vexpot.Integration;

/// <summary>
/// The Vexpot.ColorTrackerDemo namespace contains all scripts used in demonstration scenes.
/// </summary>
namespace Vexpot.ColorTrackerDemo
{
    /// <summary>
    /// Basic interface controls for World Test demo.
    /// </summary>
    public class WorldTestDemo : MonoBehaviour
    {
        public ColorTrackerPanel panel;

        void OnGUI()
        {
            GUI.Label(new Rect(30, 30, 200, 50), "ColorTracker Simple Demo");

            if (panel)
            {
                panel.useKalmanFilter = GUI.Toggle(new Rect(30, 60, 200, 50), panel.useKalmanFilter, "Use Kalman filter");
            }
        }
    }
}
