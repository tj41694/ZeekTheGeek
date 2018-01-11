using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public float movingTime = 0.1F;
    public float turnTime = 0.05f;
    public LayerMask BlockingLayer;
    public Vector3 direction;
    public AudioSource[] audios;
    
    private float moveSpeed;
    private float moveHorizontal;
    private float moveVertical;
    private float turnSpeed;
    private float remainToTurn;
    private Vector3 temp;
    private bool isMoving;
    private bool isTurnAround;
    private Rigidbody rb;
    private RaycastHit rayCasHit;
    private Animation animations;
    private int haveKey = 0;
    private DoorScript door;


    private void Start()
    {
        moveSpeed = 1 / movingTime;
        rb = GetComponent<Rigidbody>();
        temp = new Vector3(0f, 0.2f, 0f);
        animations = GetComponent<Animation>();
        turnSpeed = 90f / turnTime;
    }
    void Update()
    {
        moveHorizontal = (int)Input.GetAxisRaw("Horizontal");
        moveVertical = (int)Input.GetAxisRaw("Vertical");
        if (moveHorizontal != 0) moveVertical = 0;
        Vector3 start = transform.position;
        direction = new Vector3(moveHorizontal, 0.0f, moveVertical);
        Vector3 end = start + direction;
        if (moveHorizontal != 0 || moveVertical != 0)
        {
            if (!isTurnAround && !isMoving && !GameController.instance.LoadLevel)
            {//防止转身或移动过程中进行转身
                if (moveHorizontal != 0)
                {
                    Vector3 h = new Vector3(0f, -90f * (moveHorizontal - 2), 0f);
                    //Vector3 r = transform.rotation.eulerAngles;
                    //float f = (r - h).sqrMagnitude;
                    StartCoroutine(SmoothTrun(h));
                }
                else if (moveVertical != 0)
                {
                    Vector3 v = new Vector3(0f, 90f + -moveVertical * 90f, 0f);
                    //Vector3 r = transform.rotation.eulerAngles;
                    //float f = (r - v).sqrMagnitude;
                    StartCoroutine(SmoothTrun(v));
                }
            }
            if (!isMoving && !GameController.instance.LoadLevel)
            {//防止在移动过程中再次移动
                bool canMove = !Physics.Linecast(start + temp, end + temp, out rayCasHit, BlockingLayer);
                if (canMove)
                {
                    StartCoroutine(SmoothMove(direction));
                }
                else if (rayCasHit.collider.tag == "MoveByPlayer" || rayCasHit.collider.tag == "Apple" || rayCasHit.collider.tag == "Crystal")
                {
                    if (rayCasHit.collider.GetComponent<MoveByPlayerScript>().AttemptMove(direction))
                    {
                        StartCoroutine(SmoothMove(direction));
                    }
                }
                else if (rayCasHit.collider.tag == "Door" && haveKey > 0)
                {
                    door = rayCasHit.collider.GetComponentInParent<DoorScript>();
                    if (!door.isOpen)
                    {
                        rayCasHit.collider.GetComponentInParent<DoorScript>().OpenDoor();
                        haveKey--;
                    }
                }
                else if (rayCasHit.collider.tag == "Boom")
                {
                    if (!rayCasHit.collider.GetComponent<BoomScript>().Booming)
                        rayCasHit.collider.GetComponent<BoomScript>().Boom();
                    if (rayCasHit.collider.GetComponent<MoveByPlayerScript>().AttemptMove(direction))
                    {
                        StartCoroutine(SmoothMove(direction));
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Flower")
        {
            audios[1].Play();
            GameController.instance.score += 50;
            GameController.instance.scoreText.text = "Score : " + GameController.instance.score;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "GoldBox")
        {
            audios[3].Play();
            GameController.instance.score += 1000;
            GameController.instance.scoreText.text = "Score : " + GameController.instance.score;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Key")
        {
            haveKey++;
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Mushroom"))
        {
            other.gameObject.SetActive(false);
            LevelUp();
        }
        else if (other.CompareTag("BadMushroom"))
        {
            other.gameObject.SetActive(false);
            StartCoroutine(DestroySelf());
            GameOver();
        }
    }

    IEnumerator SmoothMove(Vector3 toward)
    {
        isMoving = true;
        audios[0].Play();
        animations.PlayQueued("PlayerMove");
        Vector3 targetPositon = transform.position + toward;
        //Debug.Log("is moving to " + lastStep + toward);
        float distanceRemain = (transform.position - targetPositon).sqrMagnitude;
        while (distanceRemain > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPositon, Time.deltaTime * moveSpeed);
            rb.MovePosition(newPosition);
            distanceRemain = (newPosition - targetPositon).sqrMagnitude;
            yield return null;
        }
        //rb.MovePosition(new Vector3((int)(transform.position.x + 0.1), transform.position.y, (int)(transform.position.z + 0.1)));
        //animations.Stop("PlayerMove");
        isMoving = false;
        if(GameController.instance.LoadLevel)
        {
            StartCoroutine(SmoothTrun(new Vector3(0f, 180f, 0f)));
        }
    }

    IEnumerator SmoothTrun(Vector3 euler)
    {
        isTurnAround = true;
        Quaternion start = transform.rotation;
        Quaternion end = Quaternion.Euler(euler);
        remainToTurn = (start.eulerAngles - euler).sqrMagnitude;
        while (remainToTurn > float.Epsilon)
        {
            float step = turnSpeed * Time.deltaTime;
            Quaternion newAngle = Quaternion.RotateTowards(start, end, step);
            start = newAngle;
            transform.rotation = newAngle;
            remainToTurn = (start.eulerAngles - euler).sqrMagnitude;
            //Debug.Log("remainToTurn : " + remainToTurn);
            //Debug.Log("float.Epsilon : " + float.Epsilon);
            yield return null;
        }
        isTurnAround = false;
    }
    void GameOver()
    {
        GameController.instance.GameOver();
    }
    void LevelUp()
    {
        GameController.instance.LevelUp();
        StartCoroutine(DestroySelf());
    }
    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(3.0F);
        Destroy(gameObject);
    }
}
