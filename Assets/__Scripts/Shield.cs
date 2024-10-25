using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {
    // This field controls the rotation speed of the shield
    public float rotationsPerSecond = 0.1f;

    [Header("Dynamic")]
    public int levelShown = 0; // This is set between lines c & d

    // This non-public variable will not appear in the Inspector
    Material mat;

    void Start() {
        // Get the material component of the Renderer
        mat = GetComponent<Renderer>().material;
    }

    void Update() {
        // Read the current shield level from the Hero Singleton
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        
        // If this is different from the level shown...
        if (levelShown != currLevel) {
            levelShown = currLevel;

            // Adjust the texture offset to show different shield levels
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }

        // Rotate the shield a bit every frame in a time-based way
        float rZ = -(rotationsPerSecond * Time.time * 360) % 360;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
