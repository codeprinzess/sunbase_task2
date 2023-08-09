using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CircleManager : MonoBehaviour
{
    public GameObject circlePrefab;
    public GameObject linePrefab;
    public Button restartButton;

    private List<GameObject> circles = new List<GameObject>();
    private GameObject currentLine;
    private List<GameObject> intersectedCircles = new List<GameObject>();

    private Vector3 lineStartPos;

    private bool drawingLine = false;

    [Header("UI Setup")]
    public Canvas canvas; // Reference to your Canvas object

    private void Start()
    {
        restartButton.onClick.AddListener(Restart);
        Restart();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            drawingLine = true;
            lineStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineStartPos.z = 0;
            currentLine = Instantiate(linePrefab, lineStartPos, Quaternion.identity, canvas.transform); // Attach to canvas
            currentLine.GetComponent<LineRenderer>().positionCount = 2; // Set up LineRenderer
        }
        else if (Input.GetMouseButtonUp(0))
        {
            drawingLine = false;
            Destroy(currentLine);

            foreach (var circle in circles)
            {
                if (IntersectsLine(circle.transform.position, lineStartPos, Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                {
                    intersectedCircles.Add(circle);
                }
            }

            foreach (var circle in intersectedCircles)
            {
                circles.Remove(circle);
                circle.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => Destroy(circle));
            }

            intersectedCircles.Clear();
        }

        if (drawingLine && currentLine != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            currentLine.GetComponent<LineRenderer>().SetPosition(1, mousePos);
        }
    }

    private bool IntersectsLine(Vector3 circleCenter, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = (lineEnd - lineStart).normalized;
        float distance = Vector3.Dot(circleCenter - lineStart, lineDirection);
        Vector3 nearestPoint = lineStart + distance * lineDirection;
        float distanceToNearest = Vector3.Distance(circleCenter, nearestPoint);

        return distanceToNearest < circlePrefab.transform.localScale.x / 2;
    }

    private void SpawnCircles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 3f), 0);
            GameObject circle = Instantiate(circlePrefab, spawnPos, Quaternion.identity);
            circles.Add(circle);
        }
    }

    public void Restart()
    {
        foreach (var circle in circles)
        {
            Destroy(circle);
        }

        circles.Clear();
        SpawnCircles(Random.Range(5, 11));
    }
}
