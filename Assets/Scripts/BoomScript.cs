using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomScript : MonoBehaviour
{

    public GameObject boomText;
    public GameObject sound;
    public GameObject boomExplosion;
    public GameObject pointLight;
    [HideInInspector]
    public bool Booming;

    private Vector3 pos;
    private Vector3[] Dir;
    private RaycastHit hitInfo;
    private TextMesh boomTime;
    private int time = 10;

    void Start()
    {
        boomTime = boomText.GetComponent<TextMesh>();
        boomTime.text = "";
        Dir = new Vector3[4];
        Dir[3] = new Vector3(1f, 0f, 0f);
        Dir[2] = new Vector3(0f, 0f, 1f);
        Dir[1] = new Vector3(-1f, 0f, 0f);
        Dir[0] = new Vector3(0f, 0f, -1f);

    }

    public void Boom()
    {
        Booming = true;
        InvokeRepeating("StartBoom", 0.2f, 1.0f);
    }
    void StartBoom()
    {
        boomTime.text = time.ToString();
        time--;
        if(time == 1)
        {
            Instantiate(boomExplosion, transform.position, Quaternion.identity);
        }
        else if (time == 0)
        {
            Instantiate(sound);
            Instantiate(pointLight, transform.position, Quaternion.identity);
            for (int i = 0; i < 4; i++)
            {
                pos = transform.position + new Vector3(0f, 0.2f, 0f);
                Dir[i] += pos;
                bool hit;
                GetComponent<Collider>().enabled = false;
                hit = Physics.Linecast(pos, Dir[i], out hitInfo);
                GetComponent<Collider>().enabled = true;
                if (hit)
                {
                    if(hitInfo.collider.CompareTag("Player"))
                    {
                        Destroy(hitInfo.collider.gameObject);
                        GameController.instance.GameOver();
                    }
                    else if (hitInfo.collider.CompareTag("Flower") ||
                        hitInfo.collider.CompareTag("Apple") ||
                        hitInfo.collider.CompareTag("Mushroom") ||
                        hitInfo.collider.CompareTag("BadMushroom") ||
                        hitInfo.collider.CompareTag("GoldBox") ||
                        hitInfo.collider.CompareTag("Key") ||
                        hitInfo.collider.CompareTag("Piranha"))
                    {
                        hitInfo.collider.gameObject.SetActive(false);
                    }
                    else if (hitInfo.collider.CompareTag("Boom"))
                    {
                        if (!hitInfo.collider.GetComponent<BoomScript>().Booming)
                            hitInfo.collider.GetComponent<BoomScript>().Boom();
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
