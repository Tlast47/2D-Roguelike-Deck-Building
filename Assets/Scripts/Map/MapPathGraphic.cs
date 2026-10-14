using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MapPathGraphic : MonoBehaviour
{
    private const float Thickness = 3f;
    private const float DashLength = 9f;
    private const float GapLength = 6f;
    private const int SampleCount = 64;
    private readonly Vector2[] points = new Vector2[SampleCount + 1];
    private readonly float[] lengths = new float[SampleCount + 1];
    private readonly List<Image> dashes = new List<Image>();
    private Color lineColor = Color.white;
    private int activeDashCount;

    public Color color
    {
        get { return lineColor; }
        set
        {
            if (lineColor == value) return;
            lineColor = value;
            foreach (Image dash in dashes)
                if (dash != null) dash.color = value;
        }
    }

    public void SetVisible(bool visible)
    {
        for (int i = 0; i < dashes.Count; i++)
        {
            bool active = visible && i < activeDashCount;
            if (dashes[i] != null && dashes[i].gameObject.activeSelf != active)
                dashes[i].gameObject.SetActive(active);
        }
    }

    public void SetPath(Vector2 start, Vector2 end)
    {
        // Reuse sample buffers and Image objects when the endpoints move.
        Vector2 controlA = start + Vector2.up * (end.y - start.y) * 0.42f;
        Vector2 controlB = end - Vector2.up * (end.y - start.y) * 0.42f;
        points[0] = start;
        lengths[0] = 0f;
        for (int i = 1; i <= SampleCount; i++)
        {
            float t = i / (float)SampleCount;
            float u = 1f - t;
            points[i] = u * u * u * start + 3f * u * u * t * controlA +
                3f * u * t * t * controlB + t * t * t * end;
            lengths[i] = lengths[i - 1] + Vector2.Distance(points[i - 1], points[i]);
        }

        int index = 0;
        float totalLength = lengths[SampleCount];
        float lastDashEnd = 0f;
        for (float distance = 0f; distance < totalLength; distance += DashLength + GapLength)
        {
            float dashEnd = Mathf.Min(distance + DashLength, totalLength);
            Vector2 a = PointAtDistance(distance);
            Vector2 b = PointAtDistance(dashEnd);
            if (Vector2.Distance(a, b) < 0.01f) continue;
            SetDash(index++, a, b);
            lastDashEnd = dashEnd;
        }
        // Finish at the target border even when the dash pattern ends in a gap.
        if (totalLength - lastDashEnd > 0.01f)
            SetDash(index++, PointAtDistance(Mathf.Max(lastDashEnd, totalLength - DashLength)), end);
        activeDashCount = index;
        SetVisible(true);
    }

    private void SetDash(int index, Vector2 a, Vector2 b)
    {
        Image dash;
        if (index < dashes.Count) dash = dashes[index];
        else
        {
            GameObject piece = new GameObject("Path Dash",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            piece.layer = gameObject.layer;
            piece.transform.SetParent(transform, false);
            dash = piece.GetComponent<Image>();
            dash.raycastTarget = false;
            dashes.Add(dash);
        }
        dash.enabled = true;
        dash.color = lineColor;
        RectTransform rect = dash.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.localScale = Vector3.one;
        // Convert local coordinates to the center anchor's coordinates.
        rect.anchoredPosition = a - ((RectTransform)transform).rect.center;
        rect.sizeDelta = new Vector2(Vector2.Distance(a, b), Thickness);
        Vector2 direction = b - a;
        rect.localRotation = Quaternion.Euler(0f, 0f,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    private Vector2 PointAtDistance(float distance)
    {
        for (int i = 1; i < lengths.Length; i++)
        {
            if (distance > lengths[i]) continue;
            float segmentLength = lengths[i] - lengths[i - 1];
            if (segmentLength < 0.0001f) return points[i];
            return Vector2.Lerp(points[i - 1], points[i],
                (distance - lengths[i - 1]) / segmentLength);
        }
        return points[points.Length - 1];
    }
}
