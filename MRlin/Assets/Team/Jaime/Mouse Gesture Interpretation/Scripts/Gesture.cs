
using System.Collections.Generic;

using UnityEngine;




public class Gesture
{
    public List<Vector3> mouseData = new List<Vector3>();
    private bool gesturing;
    public int widthText = 3;

    // NOTHING TO BE MODIFIED WHEN ADDING A NEW PATTERN
    // NOR WHEN PUTTING EVERYTHING TOGETHER

    public Texture2D MapPattern()
    {
        Debug.Log($"{this.mouseData.Count}");
        if (this.mouseData.Count < 10)
            return null;
        Bounds bounds = new Bounds(this.mouseData[0], Vector3.zero);
        for (int index = 1; index < this.mouseData.Count; ++index)
        {
            bounds.min = (Vector3.Min(bounds.min, this.mouseData[index]));
            bounds.max = (Vector3.Max(bounds.max, this.mouseData[index]));
        }
        Texture2D texture2D = new Texture2D(32, 32);
        Color[] pixels = texture2D.GetPixels();
        for (int index = 0; index < pixels.Length; ++index)
        pixels[index] = Color.white;
        Vector3 size = bounds.size;
        if (size.magnitude < 20.0)
        {
            Debug.Log($"{size}");
            Debug.Log($"{size.magnitude}");
            return null;
        }
        for (int index1 = 0; index1 < this.mouseData.Count - 1; ++index1)
        {
        int num1 = (int) Mathf.Clamp((float) ((this.mouseData[index1].x - bounds.min.x) / bounds.size.x * 32.0), 0.0f, 31f);
        int num2 = (int) Mathf.Clamp((float) ((this.mouseData[index1].y - bounds.min.y) / bounds.size.y * 32.0), 0.0f, 31f);
        int num3 = (int) Mathf.Clamp((float) ((this.mouseData[index1 + 1].x - bounds.min.x) / bounds.size.x * 32.0), 0.0f, 31f);
        int num4 = (int) Mathf.Clamp((float) ((this.mouseData[index1 + 1].y - bounds.min.y) / bounds.size.y * 32.0), 0.0f, 31f);
        float num5 = Mathf.Sqrt(Mathf.Pow((float) (num3 - num1), 2f) + Mathf.Pow((float) (num4 - num2), 2f));
        for (int index2 = 0; index2 <= 20; ++index2)
        {
            float num6 = (float) index2 * 0.05f;
            int num7 = (int) ((double) num1 + (double) (num3 - num1) * (double) num6);
            int num8 = (int) ((double) num2 + (double) (num4 - num2) * (double) num6);
            pixels[num7 + (num8 * 32)] = Color.black;
            for (int index3 = 1; index3 < widthText; ++index3)
            {
            int num9 = (int) ((double) num7 + (double) ((num4 - num2) * index3) / (double) num5);
            int num10 = (int) ((double) num8 - (double) ((num3 - num1) * index3) / (double) num5);
            int num11 = (int) ((double) num7 - (double) ((num4 - num2) * index3) / (double) num5);
            int num12 = (int) ((double) num8 + (double) ((num3 - num1) * index3) / (double) num5);
            if (num9 >= 0 && num9 < 32 && (num10 >= 0 && num10 < 32))
                pixels[num9 + num10 * 32] = Color.black;
            if (num11 >= 0 && num11 < 32 && (num12 >= 0 && num12 < 32))
                pixels[num11 + num12 * 32] = Color.black;
            }
        }
        }
        texture2D.SetPixels(pixels);
        texture2D.Apply();
        return texture2D;
    }

    public float TestPattern(Texture2D fromTexture, Texture2D toTexture)
    {
        if (Object.Equals(toTexture, null))
        {
        Debug.LogError((object) "<b>Mouse Gesture Interpretation:</b> texture pattern for comparison is not set.");
        return 0.0f;
        }
        Color[] pixels1 = fromTexture.GetPixels();
        Color[] pixels2 = toTexture.GetPixels();
        float num1 = 0.0f;
        float num2 = 0.0f;
        float num3 = 0.0f;
        for (int index = 0; index < pixels2.Length; ++index)
        {
        if (Equals(pixels2[index], Color.black))
            ++num1;
        }
        for (int index = 0; index < pixels1.Length; ++index)
        {
        if (Equals(pixels1[index], Color.black))
        {
            ++num2;
            if (Equals(pixels2[index], Color.black))
            ++num3;
        }
        }
        float num4 = num2 - num3;
        float num5 = num3 / num1;
        return num4 <  num3 ? num5 : 0.0f;
    }

    public void setIsGesturing(bool b)
    {
        gesturing = b;
    }

    public bool getIsGesturing()
    {
        return gesturing;
    }

    public void setTextWidth(int w)
    {
        widthText = w;
    }
}

