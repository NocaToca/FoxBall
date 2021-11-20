using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GroundCheck : MonoBehaviour
{
    BoxCollider2D bc;
    List<GameObject> ignoredObjects = new List<GameObject>();
    public bool grounded;

    

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        return;
    }

    void OnTriggerEnter2D(Collider2D collision){
        //Debug.Log("Enter");
        foreach(GameObject go in ignoredObjects){
            if(collision.gameObject == go){
                return;
            }
        }
        grounded = true;
    }

    void OnTriggerStay2D(Collider2D collision){

        foreach(GameObject go in ignoredObjects){
            if(collision.gameObject == go){
                return;
            }
        }
        grounded = true;

    }

    void OnTriggerExit2D(Collider2D collision){
        foreach(GameObject go in ignoredObjects){
            if(collision.gameObject == go){
                return;
            }
        }
        grounded = false;
    }

    public void AddIgnoredObject(GameObject gameObject){
        ignoredObjects.Add(gameObject);
    }
}
