using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CristalScript : MonoBehaviour {

    
    public Material matToChange;
    public GameObject sound;


    private Vector3 pos;
    private Vector3[] Dir;
    private RaycastHit hitInfo;
    private Renderer[] rend;
    private Material[] mat;
    private bool isBreaking;
    private bool isChangingMat;
    private Material original;
    private MoveByPlayerScript move;

    void Start () {
        move = GetComponent<MoveByPlayerScript>();
        rend = gameObject.GetComponentsInChildren<Renderer>();
        mat = rend[0].materials;
        original = mat[0];
    }
	
	void Update () {
        if (!isBreaking && move.isMoved)
        {
            pos = transform.position + new Vector3(0f, 0.2f, 0f);
            Dir = new Vector3[4];
            Dir[3] = pos + new Vector3(1f, 0f, 0f);
            Dir[2] = pos + new Vector3(0f, 0f, 1f);
            Dir[1] = pos + new Vector3(-1f, 0f, 0f);
            Dir[0] = pos + new Vector3(0f, 0f, -1f);
            for (int i = 0; i < 4; i++)
            {
                bool hit;
                GetComponent<Collider>().enabled = false;
                hit = Physics.Linecast(pos, Dir[i], out hitInfo);
                GetComponent<Collider>().enabled = true;
                if (hit)
                {
                    if (hitInfo.collider.CompareTag("Crystal"))
                    {
                        hitInfo.collider.GetComponent<CristalScript>().Breack();
                        Breack();
                    }
                }
            }
        }
    }
    void Breack()
    {
        isBreaking = true;
        if (!isChangingMat)
        {
            //Debug.Log(gameObject + " is breaking");
            isChangingMat = true;
            StartCoroutine(MatChange(matToChange));
        }
    }

    IEnumerator MatChange(Material toChange)
    {
        for(int i = 0; i < 5; i++)
        {
            if(i%2 == 0)
            {
                rend[0].material= toChange;
            }
            else
            {
                rend[0].material = original;
            }
            yield return new WaitForSeconds(0.4f);
        }
        isChangingMat = false;
        Instantiate(sound);
        Destroy(gameObject);
    }


    private void OnDestroy()
    {
        mat[0] = original;
    }
}
