using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrSolidParticle : MonoBehaviour
{
    // Start is called before the first frame update

    private List<Rigidbody> bindParticleRB;
    private Rigidbody rb;
    private bool transparentOn;

    void Start()
    {

        
    }

    public void QuickStart()
    {
        transparentOn = false;
        rb = GetComponent<Rigidbody>();
        bindParticleRB = new List<Rigidbody>();
    }

    public void BindToParticle(GameObject newBindParticle)
    {
        bindParticleRB.Add(newBindParticle.GetComponent<Rigidbody>());

    }

    private void Update()
    {
        transparentOn = Input.GetKey(KeyCode.F);

        if (transparentOn)
        {
            GetComponent<Transform>().localScale = new Vector3(1.632993f / 2f, 1.632993f / 2f, 1.632993f / 2f);
            GetComponent<Rigidbody>().drag = 10000000000f;
        }
        else
        {
            GetComponent<Transform>().localScale = new Vector3(1.632993f, 1.632993f, 1.632993f);
            GetComponent<Rigidbody>().freezeRotation = false;
        }

        
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!transparentOn)
        {
            foreach (Rigidbody curRB in bindParticleRB)
            {
                Vector3 bindingForce = 0.1f * (curRB.position - rb.position).normalized;

                //Vector3 offset = curRB.position - rb.position;
                //Vector3 bindingForce = (0.1f / offset.magnitude) * offset.normalized;

                rb.velocity += new Vector3(bindingForce.x, bindingForce.y, bindingForce.z);
                rb.AddForce(bindingForce, ForceMode.Impulse);
            }
        }

    }
}
