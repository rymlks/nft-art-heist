using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float period;
    [Range(0.0f, 1.0f)]
    public float amplitude;
    [Range(0.0f, 10.0f)]
    public float phase;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float scale = 1 + Mathf.Sin((Time.time + phase) * period) * amplitude;
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }
}
