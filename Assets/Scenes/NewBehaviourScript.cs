using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public int health = 100;
    public float timer = 1.0f;
    public int attackpoint = 10;

    // Start is called before the first frame update
    void Start()
    {
        health += 100;

    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {

            timer = 1.0f;
            health += 20;

                



        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Characterhit(attackpoint);

        }

        checkdeath();

    }



    void Characterhit(int damage)
    {
        health -= damage;

    }
    void checkdeath()
    {

        if (health <= 0)
            Destroy(gameObject);


    }
}