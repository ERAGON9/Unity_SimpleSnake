using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Properties")] 
    [SerializeField] private int m_Width = 20;
    [SerializeField] private int m_Hight = 12;
    [SerializeField] private GameObject m_WallPrefab;
    [SerializeField] private GameObject m_FoodPrefab;
    [SerializeField] private TextMeshProUGUI m_ScoreText;
    
    
    private GameObject m_Food;
    private int m_CurrentScore = 0;

    private bool m_IsGameRunning = true;
    public bool IsGameRunning => m_IsGameRunning;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    public void StartGameSession()
    {
        UpdateScoreText();
        createWalls();
        spawnFood();
    }

    private void UpdateScoreText()
    {
        m_ScoreText.text = m_CurrentScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_IsGameRunning)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name,LoadSceneMode.Single);
            }
        }
    }
    
    private int m_RightBorder => (m_Width / 2) + 1;
    private int m_LeftBoarder => -1 * ((m_Width / 2) + 1);
    private int m_TopBoarder => (m_Hight / 2) + 1;
    private int m_BottonBoarder => -1 * ((m_Hight / 2) + 1);
    
    private void createWalls()
    {
        createWall("RightWall", m_RightBorder, 0, 1, m_Hight + 3);
        createWall("LeftWall", m_LeftBoarder, 0, 1, m_Hight + 3);
        createWall("TopWall", 0, m_TopBoarder, m_Width + 1, 1);
        createWall("BottomWall", 0, m_BottonBoarder, m_Width + 1, 1);
    }

    private void createWall(string i_Name, int i_X, int i_Y, int i_Width, int i_Hight)
    {
        var wall = Instantiate(m_WallPrefab);
        wall.name = i_Name;
        wall.transform.position = new Vector3(i_X, i_Y, 0);
        wall.transform.localScale = new Vector3(i_Width, i_Hight, 0);
    }

    public bool IsThereFoodInPosition(Vector3 i_Position)
    {
        if (m_Food == null)
            return false;
        
        return m_Food.transform.position == i_Position;
    }

    public void EatFood()
    {
        Destroy(m_Food);
        m_CurrentScore++;
        UpdateScoreText();
        spawnFood();
    }
    
    private void spawnFood()
    {
        var foodPosition = getNextFoodPosition();
        m_Food = Instantiate(m_FoodPrefab, foodPosition, quaternion.identity);
    }
    
    private Vector3 getNextFoodPosition()
    {
        var newPosition = new Vector3();
        var positionValid = false;
        
        // Can't win the Snake game, fix at home.
        while (!positionValid)
        {
            var x = Random.Range(m_LeftBoarder, m_RightBorder);
            var y = Random.Range(m_BottonBoarder, m_TopBoarder);
            newPosition = new Vector3(x, y, 0);
            positionValid = !IsPositionOnWall(newPosition) && !PlayerController.Instance.IsPositionOnSnake(newPosition);
        }

        return newPosition;
    }
    
    public bool IsPositionOnWall(Vector3 i_Position)
    {
        return (i_Position.x <= m_LeftBoarder ||
                i_Position.x >= m_RightBorder ||
                i_Position.y >= m_TopBoarder ||
                i_Position.y <= m_BottonBoarder);
    }

    public void GameOver()
    {
        m_IsGameRunning = false;
    }
}
