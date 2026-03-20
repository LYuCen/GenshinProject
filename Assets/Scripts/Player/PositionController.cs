using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionController : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 vec;
    void Start()
    {
        vec = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = vec;
    }
}
