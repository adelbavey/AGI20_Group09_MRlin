using System.Collections.Generic;
using UnityEngine;
using Vexpot.Integration;

/// <summary>
/// The Vexpot.ColorTrackerDemo namespace contains all scripts used in demonstration scenes.
/// </summary>
namespace Vexpot.ColorTrackerDemo
{
    /// <summary>
    /// Overrides the <see cref="AugmentedRealityRenderer"/> behavior to achieve a custom result.
    /// Used in the game to draw trails when you move the virtual pointer.
    /// </summary>
    public class FruitTrailRenderer : AugmentedRealityRenderer
    {
        public Vector3 targetPos = new Vector3();

        public override void OnPostRender()
        {
            if (colorTrackerPanel != null && colorTrackerPanel.GetColorTracker() != null)
            {
                ColorTracker tracker = colorTrackerPanel.GetColorTracker();
                if (tracker == null) return;

                List<TrackerResult> colorTrackerResults = tracker.GetLatestResult();

                CreateObjectsIfNeeded(colorTrackerResults.Count);

                for (var t = 0; t < colorTrackerResults.Count; t++)
                {
                    TrackerResult r = colorTrackerResults[t];

                    if (r.state == TrackingState.Tracked)
                    {
                        CoordinateMapper.ConvertInputToScreen(tracker.input, r.center, ref _reusablePosition);
                        _reusablePosition.z = perspectiveCamera.nearClipPlane + 2.3f;
                        _targets[t].transform.position = perspectiveCamera.ScreenToWorldPoint(_reusablePosition);
                        targetPos = _targets[t].transform.position;
                        _targets[t].SetActive(true);
                    }
                    else
                    {
                        _targets[t].SetActive(false);
                    }

                }
            }

        }

        public Vector3 getTargetPos()
        {
            return targetPos;
        }
    }

}