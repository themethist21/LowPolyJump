using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;


public enum StageStates
{
    Despawned,
    Spawning,
    Spawned,
    Despawn,
    Cleanup,
    End
}

public class GameController : MonoBehaviour
{

    // Constant values
    private const float STAGESPAWNTIME = 1.0f;
    private const float OBSSPAWNTIME = 1.4f;
    private const float STAGEDESPAWNTIME = 1.4f;
    private const float OBSDESPAWNTIME = 0.4f;
    private const float LEVEL1TOTALTIME = 40.0f;

    private const float LEVEL2TOTALTIME = 42.0f;

    public int score { get; private set;}

    private bool godMode = false;
    private bool gamePaused = false;
    private bool levelStart = false;

    //Events
    public UnityEvent<bool> terrainSpawn;
    public UnityEvent<bool> obstacleSpawn;
    public UnityEvent<bool> playerRun;
    public UnityEvent levelFinish;
    public UnityEvent<bool> pauseGame;

    //Timers
    private float stageSpawnTimer;
    private float obstacleSpawnTimer;
    private float levelTime = 0;

    public int stageCount = 1;
    private List<GameObject> stages;
    private int currentStage;
    private StageStates currentStageState;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        currentStage = 0;
        stageSpawnTimer = STAGESPAWNTIME;
        currentStageState = StageStates.Despawned;

        List<GameObject> terrain = new List<GameObject>();
        GameObject.FindGameObjectsWithTag("Terrain", terrain);
        foreach (GameObject obj in terrain)
        {
            terrainSpawn.AddListener(obj.GetComponent<Terrain>().SetVisible);
        }
        List<GameObject> obstacles = new List<GameObject>();
        GameObject.FindGameObjectsWithTag("Spikes", obstacles);
        foreach (GameObject obj in obstacles)
        {
            obstacleSpawn.AddListener(obj.GetComponent<Obstacles>().SetVisible);
        }

        List<GameObject> coins = new List<GameObject>();
        GameObject.FindGameObjectsWithTag("Coin", coins);
        foreach (GameObject obj in coins)
        {
            obstacleSpawn.AddListener(obj.GetComponent<Coin>().SetVisible);
        }

        List<GameObject> decor = new List<GameObject>();
        GameObject.FindGameObjectsWithTag("Decoration", decor);
        foreach (GameObject obj in decor)
        {
            obstacleSpawn.AddListener(obj.GetComponent<Decoration>().SetVisible);
        }

        levelFinish.AddListener(GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>().ShowWinMenu);
        pauseGame.AddListener(GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>().PauseGameMenu);
        pauseGame.AddListener(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementHorizontal>().SetGamePaused);

        getStages();
        for (int i = 0; i < stageCount; i++)
        {
            ShowStage(i,false);
        }
    }

    private void Update()
    {
        stageSpawnTimer -= Time.deltaTime;
        obstacleSpawnTimer -= Time.deltaTime;

        if (levelStart) levelTime += Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.G))
        {
            godMode = !godMode;
            foreach (var obj in GameObject.FindGameObjectsWithTag("JumpTrigger"))
            {
                obj.GetComponent<BoxCollider>().enabled = !obj.GetComponent<BoxCollider>().enabled;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();        
        }


        switch (currentStageState)
        {
            case StageStates.Despawned:
                if (stageSpawnTimer < 0)
                {
                    ShowStage(currentStage, true);
                    currentStageState = StageStates.Spawning;
                    terrainSpawn.Invoke(true);
                    obstacleSpawnTimer = OBSSPAWNTIME;
                }
                break;
            case StageStates.Spawning:
                if (obstacleSpawnTimer < 0)
                {
                    obstacleSpawn.Invoke(true);
                    currentStageState = StageStates.Spawned;
                    playerRun.Invoke(true);
                    if (!levelStart) levelStart = true;
                }
                break;
            case StageStates.Spawned:
         
                break;
            case StageStates.Despawn:
                if (obstacleSpawnTimer < 0)
                {
                    terrainSpawn.Invoke(false);
                    stageSpawnTimer = STAGEDESPAWNTIME;
                    currentStageState = StageStates.Cleanup;
                }
                break;
            case StageStates.Cleanup:
                if (stageSpawnTimer < 0)
                {
                    ShowStage(currentStage, false);
                    currentStage++;
                    if (currentStage == stageCount)
                    {
                        PlayerWin();
                        currentStageState = StageStates.End;
                        levelStart = false;
                    }
                    else currentStageState = StageStates.Despawned;
                }
                break;
            default: break;
        }

    }

    private void getStages()
    {
        stages = new List<GameObject>();
        for (int i = 1; i < stageCount +1 ; i++)
        {
            stages.Add(GameObject.FindGameObjectWithTag("Stage" + i.ToString()));
        }
    }

    private void ShowStage(int level, bool b)
    {
        stages[level].SetActive(b);
        if (godMode)
        {
            foreach (var obj in GameObject.FindGameObjectsWithTag("JumpTrigger"))
            {
                obj.GetComponent<BoxCollider>().enabled = b;
            }
        }
    }

    public void IncrementScore()
    {
        ++score;
    }

    public void PlayerEndStage()
    {
        playerRun.Invoke(false);
        currentStageState = StageStates.Despawn;
        obstacleSpawn.Invoke(false);
        obstacleSpawnTimer = OBSDESPAWNTIME;
    }

    public void PlayerLose()
    {
        SetBestPercentage();
        levelStart = false;
        playerRun.Invoke(false);
    }

    private void PlayerWin()
    {
        SetBestPercentage();
        levelFinish.Invoke(); 
        Debug.Log(levelTime);
    }

    public void PauseGame()
    {
        if (!gamePaused)
        {
            pauseGame.Invoke(true);
            PauseTimeScale(true); //Pausa el tiempo
            gamePaused = true;
            SoundManager.Instance.PauseAllSounds();
        }
        else
        {
            gamePaused = false;
            pauseGame.Invoke(false);
        }
    }

    public void PauseTimeScale (bool b)
    {
        if (b) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public float GetLevelPercentage()
    {
        int lvl = PlayerPrefs.GetInt("level");
        float perc;
        
        switch (lvl)
        {
            case 1:
                perc = (levelTime / LEVEL1TOTALTIME) * 100;
                break;

            case 2:
                perc = (levelTime / LEVEL2TOTALTIME) * 100;
                break;

            default:
                perc = 0;
                break;
        }

        if (perc > 100) return 100;
        else return perc;
    }

    private void SetBestPercentage()
    {
        int lvl = PlayerPrefs.GetInt("level");
        string level = "level" + lvl.ToString() + "Best";
        PlayerPrefs.SetFloat(level, Mathf.Max(GetLevelPercentage(), PlayerPrefs.GetFloat(level)));
    }
}
