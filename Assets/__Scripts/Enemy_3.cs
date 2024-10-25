using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy {  // Enemy_3 also extends the Enemy class
    [Header("Enemy_3 Inscribed Fields")]
    public float lifeTime = 5;
    public Vector2 midpointYRange = new Vector2(1.5f, 3);
    [Tooltip("If true, the Bezier points & path are drawn in the Scene pane")]
    public bool drawDebugInfo = true;

    [Header("Enemy_3 Private Fields")]
    [SerializeField] private Vector3[] points;   // The three points for the Bezier curve
    [SerializeField] private float birthTime;

    void Start() {
        // Initialize points
        points = new Vector3[3];

        // The start position has already been set by Main.SpawnEnemy()
        points[0] = pos;

        // Set xMin and xMax the same way that Main.SpawnEnemy() does
        float xMin = -bndCheck.camWidth + bndCheck.radius;
        float xMax = bndCheck.camWidth - bndCheck.radius;

        // Pick a random middle position in the bottom half of the screen
        Vector3 midPt = Vector3.zero;
        midPt.x = Random.Range(xMin, xMax);
        midPt.y = Random.Range(midpointYRange[0], midpointYRange[1]);
        points[1] = midPt;

        // Pick a random final position above the top of the screen
        points[2] = Vector3.zero;
        points[2].x = Random.Range(xMin, xMax);
        points[2].y = bndCheck.camHeight + bndCheck.radius;

        // Set the birthTime to the current time
        birthTime = Time.time;

        if (drawDebugInfo) DrawDebug();
    }

    public override void Move() {
        // Bezier curves work based on a u value between 0 & 1
        float u = (Time.time - birthTime) / lifeTime;

        if (u > 1) {
            // This Enemy_3 has finished its life
            Destroy(this.gameObject);
            return;
        }

        // Adjust the ship's rotation based on u
        transform.rotation = Quaternion.Euler(u * 180, 0, 0);
        u = u - 0.1f * Mathf.Sin(u * Mathf.PI * 2);

        // Interpolate the three Bezier curve points
        pos = Utils.Bezier(u, points);
    }

    void DrawDebug() {
        // Draw the three points
        Debug.DrawLine(points[0], points[1], Color.cyan, lifeTime);
        Debug.DrawLine(points[1], points[2], Color.yellow, lifeTime);

        // Draw the Bezier curve
        float numSections = 20;
        Vector3 prevPoint = points[0];
        Color col;

        for (int i = 1; i <= numSections; i++) {
            float t = i / numSections;
            Vector3 pt = Utils.Bezier(t, points);
            col = Color.Lerp(Color.cyan, Color.yellow, t);
            Debug.DrawLine(prevPoint, pt, col, lifeTime);
            prevPoint = pt;
        }
    }
}
