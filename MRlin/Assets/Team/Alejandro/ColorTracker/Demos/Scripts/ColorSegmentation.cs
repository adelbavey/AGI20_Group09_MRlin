using UnityEngine;
using Vexpot.Integration;
using Vexpot;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class ColorSegmentation : AugmentedRealityRenderer {

    /// <summary>
    /// 
    /// </summary>
    public bool showColorMap = true;

    private Rect _colorMapViewPosition = new Rect(0, 0, 266, 200);
    private ColorTracker _tracker;

    /// <summary>
    /// 
    /// </summary>
    void OnGUI()
    {
        if (colorTrackerPanel && showColorMap)
        {
            _tracker = colorTrackerPanel.GetColorTracker();
            GUI.DrawTexture(_colorMapViewPosition, _tracker.GetColorMap().texture2D);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    override public void OnPostRender()
    {
        ColorTracker tracker = colorTrackerPanel.GetColorTracker();
        if (tracker == null) return;

        tracker.Compute();

        List<ColorPoint> colorPointsResults = tracker.GetColorMap().points;

        CreateObjectsIfNeeded(colorPointsResults.Count);

        foreach (var item in _targets)
        {
            item.SetActive(false);
        }

        for (var t = 0; t < colorPointsResults.Count; t++)
        {
            ColorPoint r = colorPointsResults[t];
            CoordinateMapper.ConvertInputToWorld(perspectiveCamera,tracker.input, r.position, ref _reusablePosition, 3.8f);
            _targets[t].transform.position = _reusablePosition;
            _targets[t].SetActive(true);
        }
    }
}
