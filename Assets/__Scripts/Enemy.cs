using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent ( typeof(BoundsCheck))]
public class Enemy : MonoBehaviour {

    [Header("Inscribed")]
    public float speed = 10f;      // The movement speed is 10m/s
    public float fireRate = 0.3f;  // Seconds per shot (Unused in this version)
    public float health = 10;      // Damage needed to destroy this enemy
    public int score = 100;        // Points earned for destroying this
    public float powerUpDropChance = 1f;

    protected bool    calledShipDestroyed = false;

    protected BoundsCheck bndCheck;

    void Awake(){
        bndCheck = GetComponent<BoundsCheck>();
    }

    // This is a Property: A method that acts like a field
    public Vector3 pos {
        get {
            return this.transform.position;
        }
        set {
            this.transform.position = value;
        }
    }

    void Update() {
        // Call the Move method each frame
        Move();
        if (bndCheck.LocIs( BoundsCheck.eScreenLocs.offDown)){
            Destroy( gameObject);
        }
        
    }

    public virtual void Move() {
        // Move the enemy downward each frame
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    
    }
    void OnCollisionEnter( Collision coll )
    {
        GameObject otherGO = coll.gameObject;

        // Check for collisions with ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {
            // Only damage this Enemy if it's on screen
            if (bndCheck.isOnScreen)
            {
                // Get the damage amount from the Main WEAP_DICT
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if (health <= 0){
                    if (!calledShipDestroyed){
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED(this);
                    }
                    // Destroy this Enemy
                    Destroy(this.gameObject);
                }
            }
            // Destroy the ProjectileHero regardless
            Destroy(otherGO);
        }
        else
        {
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}

    // void Start() {} // Please delete the unused Start() method

