using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyRot : MonoBehaviour {
    
	public float rtSpeed = 90f;
    private Vector3 rt;
    private void Start()
    {
        rt = new Vector3(0f, 0f, 0f);
    }
    void Update () {
        rt += new Vector3(0f, 1f, 0f) * rtSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rt);
	}
}
