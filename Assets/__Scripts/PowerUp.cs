using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Use TextMeshPro

[RequireComponent(typeof(BoundsCheck))]
public class PowerUp : MonoBehaviour {

    [Header("Inscribed")]
    // This is an unusual but handy use of Vector2s.
    [Tooltip ("x holds a min value and y a max value for a Random.Range() call.")]
    public Vector2 rotMinMax = new Vector2(15, 90);

    [Tooltip("x holds a min value and y a max value for a Random.Range() call.")]
    public Vector2 driftMinMax = new Vector2(.25f, 2);

    public float lifeTime = 10;  // PowerUp will exist for # seconds
    public float fadeTime = 4;   // Then it fades over # seconds

    [Header("Dynamic")]
    public GameObject cube;      // Reference to the PowerCube child
    public TextMeshPro letter;   // Reference to the TextMeshPro (was TextMesh)
    public Vector3 rotPerSecond; // Euler rotation speed for PowerCube
    public float birthTime;      // The Time.time this was instantiated

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Material cubeMat;

    private eWeaponType _type;  // Backing field for the type property

    void Awake() {
        // Find the Cube reference (there’s only a single child)
        cube = transform.GetChild(0).gameObject;

        // Find the TextMeshPro and other components
        letter = cube.GetComponentInChildren<TextMeshPro>();  // Updated to TextMeshPro
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeMat = cube.GetComponent<Renderer>().material;

        // Set a random velocity
        Vector3 vel = Random.onUnitSphere; // Get random XYZ velocity
        vel.z = 0; // Flatten the vel to the XY plane
        vel.Normalize(); // Normalize the Vector3 to length 1m
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        // Set the rotation of the PowerUp GameObject to R:[0,0,0]
        transform.rotation = Quaternion.identity;

        // Quaternion.identity is equal to no rotation.
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y));

        birthTime = Time.time;
    }

    void Update() {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        // Fade out the PowerUp over time
        // Given the default values, a PowerUp will exist for 10 seconds and then fade out over 4 seconds.
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;

        // If u >= 1, destroy this PowerUp
        if (u >= 1) {
            Destroy(this.gameObject);
            return;
        }

        // If u > 0, decrease the opacity (i.e., alpha) of the PowerCube & Letter
        if (u > 0) {
            Color c = cubeMat.color;
            c.a = 1f - u;
            cubeMat.color = c;

            // Fade the Letter too, just not as much
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }

        if (!bndCheck.isOnScreen) {
            Destroy(gameObject);  // If PowerUp drifts off screen, destroy it
        }
    }

    public eWeaponType type { 
        get { return _type; } 
        set { SetType(value); }
    }

    public void SetType(eWeaponType wt) {
        // Grab the WeaponDefinition from Main
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(wt);

        // Set the color of PowerCube
        cubeMat.color = def.powerUpColor;

        // Set the letter that is shown on the PowerCube
        letter.text = def.letter;

        _type = wt;  // Finally, actually set the type
    }

    public void AbsorbedBy(GameObject target) {
        // This function is called by the Hero class when a PowerUp is collected
        Destroy(this.gameObject);
    }
}
