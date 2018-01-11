using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public Text scoreText;
    public static GameController instance = null;
    public GameObject playerDieSound;
    public GameObject victorySound;

    [HideInInspector]
    public bool LoadLevel;
    [HideInInspector]
    public int score;
    [HideInInspector]
    public int level;


    public BoardManager boardManagerScript;
    public GameObject[] backGroundMusics;
    private GameObject backGroundMusic;

    private void Awake()
    {
        level = 1;
        int musicIndex = Random.Range(0, backGroundMusics.Length);
        backGroundMusic = Instantiate(backGroundMusics[musicIndex]);
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        boardManagerScript = GetComponent<BoardManager>();
    }
    private void Start()
    {
        scoreText.text = "Score : " + score.ToString();
        boardManagerScript.SetupScene(level);
    }
    public void LevelUp()
    {
        if(level++ >16)
        {
            Debug.Log("You Win!");
            level = 1;
        }
        LoadLevel = true;
        Instantiate(victorySound);
        Destroy(backGroundMusic);
        StartCoroutine(LevelUpCoroutin());
    }
    
    IEnumerator LevelUpCoroutin()
    {
        yield return new WaitForSeconds(3.0f);
        int musicIndex = Random.Range(0, backGroundMusics.Length);
        backGroundMusic = Instantiate(backGroundMusics[musicIndex]);
        boardManagerScript.SetupScene(level);
        LoadLevel = false;
    }
    public void GameOver()
    {
        level = 1;
        score = 0;
        scoreText.text = "Score : " + score.ToString();
        LoadLevel = true;
        Instantiate(playerDieSound);
        Destroy(backGroundMusic);
        StartCoroutine(GameOverCoroutin());
    }
    IEnumerator GameOverCoroutin()
    {   
        yield return new WaitForSeconds(3f);
        int musicIndex = Random.Range(0, backGroundMusics.Length);
        backGroundMusic = Instantiate(backGroundMusics[musicIndex]);
        boardManagerScript.SetupScene(level);
        LoadLevel = false;
    }
}
