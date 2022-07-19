using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float seconds;
    public GameObject curveobj;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > seconds)
            Destroy(gameObject);
        if (curveobj != null)
        {
            var curve = curveobj.GetComponent<ActionTab>().disappearing;
            gameObject.UISetOpacity(curve.Evaluate(timer));
        }
    }
}
