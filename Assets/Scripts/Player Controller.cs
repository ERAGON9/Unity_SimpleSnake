using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    
    [FormerlySerializedAs("m_gameManager")]
    [Header("Properties")]
    [SerializeField] private GameObject m_SankePartPrefab;
    [SerializeField] private float m_StepTimeOut = 0.2f;

    private float m_StepTimer = 0;
    private Vector3 m_ResentPlayerMovement;
    
    private List<GameObject> m_SnakeParts = new();
    
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
        var initialSnakePart = Instantiate(m_SankePartPrefab, Vector3.zero, quaternion.identity);
        m_SnakeParts.Add(initialSnakePart);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGameRunning)
        {
            return;
        }
        
        var playerInput = GetInputOnUpdate();
        if (playerInput != Vector3.zero)
        {
            m_ResentPlayerMovement = playerInput;
        }
        
        m_StepTimer += Time.deltaTime;
        if (m_StepTimer >= m_StepTimeOut)
        {
            // TODO: refactor. (Only at the beginning of the game.)
            if (m_ResentPlayerMovement == Vector3.zero)
                return;
            
            var newSnakeHeadPosition = m_SnakeParts.Last().transform.position + m_ResentPlayerMovement;

            var contactedWall = GameManager.Instance.IsPositionOnWall(newSnakeHeadPosition);
            if (contactedWall)
            {
                GameManager.Instance.GameOver();
                return;
            }
            
            var newSnakeHead = Instantiate(m_SankePartPrefab, newSnakeHeadPosition, quaternion.identity);
            m_SnakeParts.Add(newSnakeHead);
            
            var foundFood = GameManager.Instance.IsThereFoodInPosition(newSnakeHead.transform.position);
            if (foundFood)
            {
                GameManager.Instance.EatFood();
            }
            else  // (!foundFood)
            {
                var snakeTail = m_SnakeParts.First();
                m_SnakeParts.Remove(snakeTail);
                Destroy(snakeTail);
            }
            
            m_StepTimer = 0;
        }
    }

    private Vector3 GetInputOnUpdate()
    {
        var movementDirection = new Vector3();
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            movementDirection = Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movementDirection = Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            movementDirection = Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            movementDirection = Vector3.down;
        }

        return movementDirection;
    }
    
    public bool IsPositionOnSnake(Vector3 i_Position)
    {
        return m_SnakeParts.Any(snakePart =>
            Mathf.Approximately(snakePart.transform.position.x, i_Position.x) &&
            Mathf.Approximately(snakePart.transform.position.y, i_Position.y));
    }
}
