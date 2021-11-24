using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameProgress : MonoBehaviour
{
    float maxDirt;
    public List<GameObject> remainingDirt = new List<GameObject>();
    public Transform[] respawnLocation;
    public GameObject Player1, Player2, Player3, Player4; //prefabs
    WindowManager windowManager;
    WindowOpening windowOpening;

    [Header("Timer & Completion")]
    public float timer = 120;
    public float dirtCleaned;
    [Space(10)]
    [SerializeField] Text timerText;
    [SerializeField] Text percentText;

    [Header("Player Scores")]
    public Text p1Text; 
    public Text p2Text, p3Text, p4Text;
    public int p1Score, p2Score, p3Score, p4Score;
    public int scorePerClean = 10;

    [Header("Next Stage")]
    public int stageLevel = 1;
    public GameObject stagePrefab;
    public GameObject mainCamera;
    public Transform stageObject;
    public Transform stageLocation;
    public Transform cameraLocation;
    public float nextLocation = 26.62089f;
    private float privateLocation; // takes from nextlocation to make editing a bit easier
    public bool stageTransition = false;

    private void Start()
    {
        Time.timeScale = 5;
        privateLocation = nextLocation;
        GameObject newStage = Instantiate(stagePrefab, new Vector3(-5.9f, 11.01911f, -12.54786f), stagePrefab.transform.rotation);
        newStage.transform.SetParent(stageObject);
        windowManager = gameObject.GetComponent<WindowManager>();
        windowOpening = gameObject.GetComponent<WindowOpening>();
        mainCamera = GameObject.Find("Main Camera");

        setupStage();
        maxDirt = remainingDirt.Count;
    }

    private void FixedUpdate()
    {
        if (Input.GetButton("Cancel"))
        {
            Application.Quit();
        }

        if (timer > 0)
        {
            runTime();
        }
        else
        {
            timerText.text = "00:00";
            if (!stageTransition)
            {
                StartCoroutine(nextStage());
                stageTransition = true;
            }
            else
            {
                moveCamera();
            }
            
        }

    }

    public void respawn(GameObject player, string playerTag)
    {    
        Destroy(player);
        StartCoroutine(waitRespawn(playerTag));
    }

    IEnumerator waitRespawn(string playerTag)
    {
        yield return new WaitForSeconds(2);
        int random = Random.Range(0,3);
        if (playerTag == "Player")
        {
            GameObject spawnedPlayer = Instantiate(Player1, respawnLocation[random].position, respawnLocation[random].rotation);
        }
        if (playerTag == "Player2")
        {
            GameObject spawnedPlayer = Instantiate(Player2, respawnLocation[random].position, respawnLocation[random].rotation);
        }
        if (playerTag == "Player3")
        {
            GameObject spawnedPlayer = Instantiate(Player3, respawnLocation[random].position, respawnLocation[random].rotation);
        }
        if (playerTag == "Player4")
        {
            GameObject spawnedPlayer = Instantiate(Player4, respawnLocation[random].position, respawnLocation[random].rotation);
        }
    }

    void runTime()
    {
        timer = timer - Time.fixedDeltaTime;

        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        timerText.text = niceTime;
    }

    public void getPoints(int playerNumber)
    {
        if (playerNumber == 1) { p1Score += scorePerClean; }
        if (playerNumber == 2) { p2Score += scorePerClean; }
        if (playerNumber == 3) { p3Score += scorePerClean; }
        if (playerNumber == 4) { p4Score += scorePerClean; }

        int[] playerScores = { p1Score, p2Score, p3Score, p4Score };

        Text[] playerTexts = { p1Text, p2Text, p3Text, p4Text };
        playerTexts[playerNumber - 1].text = playerScores[playerNumber - 1].ToString();
    }

    IEnumerator nextStage()
    {
        windowOpening.enableOpening = false;
        Vector3 newlocation = new Vector3(stageLocation.position.x, stageLocation.position.y + privateLocation, stageLocation.position.z);
        
        GameObject newStage = Instantiate(stagePrefab, newlocation, stagePrefab.transform.rotation);
        newStage.transform.SetParent(stageObject);
        
        yield return new WaitForSeconds(4);
        privateLocation += nextLocation;
        windowManager.windows.Clear();
        windowOpening.windows.Clear();
        Destroy(stageObject.GetChild(stageLevel - 1).GetChild(0).gameObject);
        for (int i = 0; i < remainingDirt.Count; i++)
        {
            Destroy(remainingDirt[i]);
        }
        stageLevel += 1;
        timer = 45 - (5 * stageLevel - 1);
        yield return new WaitForSeconds(1);
        setupStage();
        stageTransition = false;
    }

    void setupStage()
    {
        windowManager.Setup();
        windowOpening.setupWindows();
        remainingDirt.Clear();
        foreach (GameObject dirt in GameObject.FindGameObjectsWithTag("Wiper"))
        {
            remainingDirt.Add(dirt);
        }
        windowOpening.enableOpening = true;
    }

    void moveCamera()
    {
        float speed = 2.2f;
        Vector3 newCamera = new Vector3(cameraLocation.position.x, cameraLocation.position.y + privateLocation, cameraLocation.position.z);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newCamera, speed * Time.deltaTime);
    }
}
