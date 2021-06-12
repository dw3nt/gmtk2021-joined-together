using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchWallController : MonoBehaviour
{
    public GameObject match1;
    public GameObject match2;
    public float lineWidth = 0.2f;
    public Color lineColor = Color.magenta;

    private EdgeCollider2D edge;
    private LineRenderer line;

    void Start()
    {
        edge = GetComponent<EdgeCollider2D>();
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector2 pos1 = match1.transform.position;
        Vector2 pos2 = match2.transform.position;

        List<Vector2> points = new List<Vector2>();
        points.Add(pos1);
        points.Add(pos2);

        edge.SetPoints(points);

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.startColor = lineColor;
        line.endColor = lineColor;
        line.SetPosition(0, (Vector3)pos1);
        line.SetPosition(1, (Vector3)pos2);
    }
}
