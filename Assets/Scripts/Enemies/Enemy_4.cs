using UnityEngine;
using System.Collections;


//Part is another serializable data storage class just like WeaponDefinition
[System.Serializable]
public class Part
{
     //These three fields need to be defined in the Inspector pane
     public string name;                //The name of this part
     public float health;               //The amount of health this part has
     public string[] protectedBy;       //The other parts that protect this

     //These two fields are set automatically in Start()
     //Caching like this makes it faster and easier to find these later
     public GameObject go;              //The GameObject of this part
     public Material mat;               //The material to show damage

}//public class Part


public class Enemy_4 : Enemy
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     //Enemy_4 will start offscreen and then pick a random point on screen to move to
     //Once it has arrived, it will pick another random point and continue until the
     //player has shot it down

     public Vector3[] points;      //Store the p0 and p1 for interpolation
     public float timeStart;       //Birth time for this Enemy_4
     public float duration = 4f;   //Duration of the movement

     public Part[] parts;          //The array of ship Parts


     /*************************************************************************************************
     *** Start
     *************************************************************************************************/
     void Start()
     {
          //There is already an initial position chosen by Main.SpawnEnemy()
          points = new Vector3[2];
          points[0] = pos;
          points[1] = pos;

          InitMovement();

          //Cache GameObject & Material of each Part in parts
          Transform t;
          foreach (Part part in parts)
          {
               t = transform.Find(part.name);
               if (t != null)
               {
                    part.go = t.gameObject;
                    part.mat = part.go.GetComponent<Renderer>().material;

               }//if

          }//foreach

     }//void Start


     /*************************************************************************************************
     *** Update
     *************************************************************************************************/
     //void Update ()
     //{


     //}//void Update


     /*************************************************************************************************
     *** OnCollisionEnter
     *************************************************************************************************/
     /* This will override the OnCollisionEnter that is part of Enemy
      * Because of the way that MonoBehaviour declares common Unity functions like onCollisionEnter()
      * the override keyword is not necessary */
     void OnCollisionEnter(Collision col)
     {
          GameObject other = col.gameObject;

          switch (other.tag)
          {
               case "Projectile_Hero":
                    Projectile p = other.GetComponent<Projectile>();

                    //Enemies dont take damage unless they are on screen
                    //This stops the player from shooting them before they are visible
                    bounds.center = gameObject.transform.position + boundsCenterOffset;

                    if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero)
                    {
                         Destroy(other);
                         break;

                    }//if

                    /* The collision col has contacts[], an array of ContactPoints
                     * Because there was a collision, we are guaranteed that there is at
                     * least a contacts[0], and ContactPoints have a reference to this collider,
                     * which will be the collider for the part of the Enemy_4 that was hit */

                    //Hurt this enemy
                    //Find the GameObject that was hit
                    GameObject goHit = col.contacts[0].thisCollider.gameObject;
                    Part partHit = FindPart(goHit);

                    /* If partHit wasnt found, then its usually because, very rarely, this collider on
                     * contacts[0] will be the Projectile_Hero instead of the ship part. If so, just
                     * look for other collider instead */
                    if (partHit == null)
                    {
                         goHit = col.contacts[0].otherCollider.gameObject;
                         partHit = FindPart(goHit);

                    }//if

                    //Check whether this part is still protected
                    if (partHit.protectedBy != null)
                    {
                         foreach (string str in partHit.protectedBy)
                              if (!Destroyed(str))          //If one of the protecting parts hasnt been destroyed
                              {                             //Then dont damage tihs part yet
                                   Destroy(other);          //Destroy the Projectile_Hero
                                   return;                  //Return before causing damage

                              }//if

                    }//if

                    //Its not protected, so make it take damage
                    //Get the damage amount from the Projectile.type & Main.W_DEFS
                    partHit.health -= Main.W_DEFS[p.type].damageOnHit;

                    //Show damage on the part
                    ShowLocalizedDamage(partHit.mat);

                    //Instead of Destroying this enemy, disable the damaged part
                    if (partHit.health <= 0)
                         partHit.go.SetActive(false);

                    //Check to see if the whole ship is destroyed
                    bool allDestroyed = true;     //Asume it is destroyed

                    foreach (Part part in parts)
                    {
                         if (!Destroyed(part))
                         {
                              //If a part still exists, change allDestroyed
                              allDestroyed = false;
                              break;

                         }//if

                    }//foreach

                    if (allDestroyed)
                    {
                         //If it IS completely destroyed, tell the Main singleton
                         Main.S.ShipDestroyed(this);

                         //Destroy this Enemy
                         Destroy(gameObject);

                    }//if

                    Destroy(other);     //Destroy the Projectile_Hero
                    break;

          }//switch

     }//void OnCollisionEnter


     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     void InitMovement()
     {
          //Pick a new point to move to that is on screen
          Vector3 p1 = Vector3.zero;
          float esp = Main.S.enemySpawnPadding;
          Bounds cBounds = Utils.camBounds;

          p1.x = Random.Range(cBounds.min.x + esp, cBounds.max.x - esp);
          p1.y = Random.Range(cBounds.min.y + esp, cBounds.max.x - esp);

          points[0] = points[1];   //Shift points[1] to points[0]
          points[1] = p1;          //Add p1 as points[1]

          //Reset the time
          timeStart = Time.time;

     }//void InitMovement


     public override void Move()
     {
          //This completely overrides Enemy.Move() with a linear interpolation
          float u = (Time.time - timeStart) / duration;

          if (u >= 1)
          {                        //If u >= 1
               InitMovement();     //Then initialize the movement to a new point
               u = 0;

          }//if

          //Apply Ease Out easing to u
          u = 1 - Mathf.Pow(1f - u, 2f);

          //Simple linear interpolation
          pos = (1 - u) * points[0] + u * points[1];

     }//public override void Move


     //These two functions find a Part in this.parts by name or GameObject
     Part FindPart(string n)
     {
          foreach (Part part in parts)
               if (part.name == n)
                    return (part);

          return (null);

     }//Part FindPart

     Part FindPart(GameObject go)
     {
          foreach (Part part in parts)
               if (part.go == go)
                    return (part);

          return (null);

     }//Part FindPart


     //These functions return true if the Part has been destroyed
     bool Destroyed(GameObject go)
     {
          return (Destroyed(FindPart(go)));

     }//bool Destroyed

     bool Destroyed(string n)
     {
          return (Destroyed(FindPart(n)));

     }//bool Destroyed

     bool Destroyed (Part part)
     {
          if (part == null)        //If no real Part was passed in
               return (true);      //Return true (meaning yes, it was destroyed)

          //Returns the result of the comparison: part.health <= 0
          //If part.health is 0 or less, returns true (yes, it was destroyed)
          return (part.health <= 0);

     }//bool Destroyed


     //This changes the color of just one Part to red instead of the whole ship
     void ShowLocalizedDamage(Material m)
     {
          m.color = Color.red;
          remainingDamageFrames = showDamageForFrames;

     }//void ShowLocalizedDamage


}//public class Enemy_4
