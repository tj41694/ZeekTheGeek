using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piranha : MonoBehaviour
{

    public bool isActive;
    public float colorChangeTime = 2f;
    public float turnTime = 0.5f;

    private Vector3 pos;
    private Vector3[] Dir;
    private RaycastHit hitInfo;
    private Renderer[] rend;
    private Material[] mat;
    private Color purple;
    private float colorChangeSpeed;
    private float remainAngleToturn;
    private bool isTurnAround = false;
    private bool isChangingColore = false;
    private bool playingAniamtion = false;
    private Animation animations;
    private AudioSource eat;


    private void Start()
    {
        eat = GetComponent<AudioSource>();
        pos = transform.position + new Vector3(0f, 0.2f, 0f);
        Dir = new Vector3[4];
        Dir[3] = pos + new Vector3(1f, 0f, 0f);
        Dir[2] = pos + new Vector3(0f, 0f, 1f);
        Dir[1] = pos + new Vector3(-1f, 0f, 0f);
        Dir[0] = pos + new Vector3(0f, 0f, -1f);
        rend = gameObject.GetComponentsInChildren<Renderer>();
        mat = rend[0].materials;
        purple = mat[0].color;
        if (!isActive)
        {
            mat[0].color = Color.gray;
        }
        else
        {
            turnTime = 0.2f;
        }
        colorChangeSpeed = 1.0f / colorChangeTime;
        animations = GetComponent<Animation>();
    }
    private void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (Physics.Linecast(pos, Dir[i], out hitInfo))
            {
                if (isActive)
                {
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        Vector3 eulerAngleToturn = new Vector3(0f, i * 90f, 0f);
                        remainAngleToturn = Mathf.Sqrt((transform.rotation.eulerAngles - eulerAngleToturn).sqrMagnitude);
                        if (!isTurnAround && !playingAniamtion)
                            StartCoroutine(SmoothTrun(eulerAngleToturn));
                        if (!isTurnAround && !playingAniamtion)
                        {
                            eat.Play();
                            animations.CrossFade("PiranhaEat");
                            Destroy(hitInfo.collider.gameObject);
                            GameController.instance.GameOver();
                            Invoke("Deactivate", 10f);
                        }
                    }
                    else if(hitInfo.collider.CompareTag("Apple"))
                    {
                        Vector3 eulerAngleToturn = new Vector3(0f, i * 90f, 0f);
                        remainAngleToturn = Mathf.Sqrt((transform.rotation.eulerAngles - eulerAngleToturn).sqrMagnitude);
                        if (!isTurnAround && !playingAniamtion)
                            StartCoroutine(SmoothTrun(eulerAngleToturn));
                        if (!isTurnAround && !playingAniamtion)
                        {
                            eat.Play();
                            animations.CrossFade("PiranhaEat");
                            Destroy(hitInfo.collider.gameObject);
                            Invoke("Deactivate", 10f);
                        }
                    }
                }
                else if (hitInfo.collider.CompareTag("Player"))
                {
                    if (!isChangingColore)
                        StartCoroutine(SmoothColorChange(mat[0].color, purple));
                    Vector3 eulerAngleToturn = new Vector3(0f, i * 90f, 0f);
                    remainAngleToturn = Mathf.Sqrt((transform.rotation.eulerAngles - eulerAngleToturn).sqrMagnitude);
                    Invoke("Acctive", 0.5f);
                    if ((transform.rotation.eulerAngles - eulerAngleToturn).sqrMagnitude > float.Epsilon)
                    {
                        StartCoroutine(SmoothTrun(eulerAngleToturn));
                    }
                }
            }
        }
        playingAniamtion = animations.IsPlaying("PiranhaEat");
    }

    void Acctive()
    {
        isActive = true;
    }
    void Deactivate()
    {
        isActive = false;
        //Debug.Log("isActive: " + isActive);
        StartCoroutine(SmoothColorChange(mat[0].color, Color.gray));
    }

    IEnumerator SmoothColorChange(Color start, Color end)
    {
        Vector3 colorStart = new Vector3(start.r, start.g, start.b);
        Vector3 colorEnd = new Vector3(end.r, end.g, end.b);
        float remainToChange = (colorStart - colorEnd).sqrMagnitude;
        while (remainToChange > float.Epsilon)
        {
            isChangingColore = true;
            float step = Time.deltaTime * colorChangeSpeed;
            Vector3 newColor = Vector3.MoveTowards(colorStart, colorEnd, step);
            colorStart = newColor;
            mat[0].color = new Color(newColor.x, newColor.y, newColor.z);
            remainToChange = (newColor - colorEnd).sqrMagnitude;
            yield return null;
        }
        isChangingColore = false;
    }
    private void OnDestroy()
    {
        mat[0].color = purple;
    }
    IEnumerator SmoothTrun(Vector3 euler)
    {
        isTurnAround = true;
        Quaternion start = transform.rotation;
        Quaternion end = Quaternion.Euler(euler);
        float turnSpeed = remainAngleToturn / turnTime;
        turnTime = 0.2f;
        while ((start.eulerAngles - euler).sqrMagnitude > float.Epsilon)
        {
            float step = turnSpeed * Time.deltaTime;
            //Debug.Log("remainAngleToturn: " + remainAngleToturn);
            //Debug.Log("turnTime: " + turnTime);
            //Debug.Log("Time.deltaTime: " + Time.deltaTime);
            //Debug.Log("turnSpeed: " + turnSpeed);
            //Debug.Log("step: " + step);
            Quaternion newAngle = Quaternion.RotateTowards(start, end, step);
            start = newAngle;
            transform.rotation = newAngle;
            yield return null;
        }
        isTurnAround = false;
    }
}
