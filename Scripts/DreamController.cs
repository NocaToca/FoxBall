using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamController : MonoBehaviour
{
    public float waitTime;
    Animator anime;
    GameObject player;
    // Start is called before the first frame update

    float deltaTime;
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        this.transform.position = new Vector3(player.transform.position.x + .7f, player.transform.position.y + .75f, 0.0f);
        anime = GetComponent<Animator>();

        int ran = Random.Range(0, 100);
        ran %= 2;
        anime.SetInteger("Thought", ran);
        anime.speed = .5f;
        anime.Play("Entry");
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += Time.deltaTime;
    }

    public void Pop(){
        anime.SetBool("Pop", true);
    }

    public void Play(){
        if (anime.GetCurrentAnimatorStateInfo(0).IsName("Blank") && deltaTime > waitTime){
            this.transform.position = new Vector3(player.transform.position.x + .7f, player.transform.position.y + .75f, 0.0f);
            int ran = Random.Range(0, 100);
            ran %= 2;
            anime.SetInteger("Thought", ran);
            anime.Play("Entry");
            deltaTime = 0.0f;
        }
    }
}
