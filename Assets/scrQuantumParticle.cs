using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrQuantumParticle : MonoBehaviour
{
    private int timeLeft;
    private Rigidbody rb;

    private const int xLength = 60;
    private const int yLength = 60;
    private const int zLength = 60;



    // Start is called before the first frame update
    void Start()
    {}

    public void QuickStart(int newTimeAlive)
    {
        timeLeft = newTimeAlive;
        rb = GetComponent<Rigidbody>();
        rb.velocity = 30f * (new Vector3(Random.value * 2f - 1f, Random.value * 2f - 1f, Random.value * 2f - 1f)).normalized;
    }

    private void FixedUpdate()
    {
        timeLeft--;



        if (timeLeft <= 0)
        {
            Destroy(gameObject);
        }

        //if    ((rb.position.x < -20f) || (rb.position.x > xLength * 1.6329931618554520654648560498039f + 20f)
        //    || (rb.position.y < -20f) || (rb.position.y > yLength * 1.333333333333333333333333333333f + 20f)
        //    || (rb.position.z < -20f) || (rb.position.z > zLength * 1.4142135623730950488016887242097f + 20f))
        //{
        //    Destroy(gameObject);

        //}
    }



}
