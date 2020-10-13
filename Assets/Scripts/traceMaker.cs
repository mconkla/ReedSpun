using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class traceMaker : MonoBehaviour
{
    GameObject Player;
    SpriteRenderer playerSpriteRenderer;
    SpriteRenderer traceSpriteRenderer;

    bool deathTimer = false;
    // Start is called before the first frame update
    private void Awake()
    {
        Player = FindObjectOfType<playerMovement>().gameObject;
        playerSpriteRenderer = Player.GetComponent<SpriteRenderer>();
        traceSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        traceSpriteRenderer.sprite = playerSpriteRenderer.sprite;
        this.gameObject.transform.position = Player.transform.position;
        this.transform.parent = null;
        deathTimer = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (deathTimer)
            startDeathTimer();
    }

 
 

    private void startDeathTimer()
    {
        Debug.Log(traceSpriteRenderer.color.a);
        float temp = traceSpriteRenderer.color.a;
        if (temp > 0)
        {
            
            traceSpriteRenderer.color = new Color(traceSpriteRenderer.color.r, traceSpriteRenderer.color.g, traceSpriteRenderer.color.b,( temp - .05f)); ;

        }
        else
        {
            Destroy(this);
        }
    }
}
