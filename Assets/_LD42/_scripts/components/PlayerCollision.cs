using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

    //Is the player colliding with something?
    public int collide = 0;
    public int lastColldierType; // 0 = Foreground Tile (Not Deadly) 1 = Deadily Tile (Deadly) 100 = Victory Tiles! (next level)
    public int newCollide = 0; // 0 when we've already collided with the object, and 1 when we first hit it!


    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(string.Format("I hit a trigger from {0}", other.name));
        collide = 1;

        lastColldierType = gamemanager.instance.GetColliderType(other.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log(string.Format("I left a trigger from {0}", other));
        collide = 0;
        newCollide = 0;
    }



}
