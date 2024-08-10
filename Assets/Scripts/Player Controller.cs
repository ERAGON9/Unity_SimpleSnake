using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameManager m_gameManager;
    [SerializeField] private GameObject m_sankePartPrefab;
    [SerializeField] private float m_StepTimeOut = 0.4f;

    private float _stepTimer = 0;
    private Vector3 _resentPlayerMovement;
    private List<GameObject> _snakeParts = new ();
    
    private void Awake()
    {
        m_gameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var initialSnakePart = Instantiate(m_sankePartPrefab, Vector3.zero, quaternion.identity);
        _snakeParts.Add(initialSnakePart);
    }

    // Update is called once per frame
    void Update()
    {
        var playerInput = GetInputOnUpdate();
        if (playerInput != Vector3.zero)
        {
            _resentPlayerMovement = playerInput;
        }
        
        _stepTimer += Time.deltaTime;
        if (_stepTimer >= m_StepTimeOut)
        {
            // TODO: refactor.
            if (_resentPlayerMovement == Vector3.zero)
                return;
            
            var newSnakeHeadPosition = _snakeParts.Last().transform.position + _resentPlayerMovement;
            var newSnakeHead = Instantiate(m_sankePartPrefab, newSnakeHeadPosition, quaternion.identity);
            _snakeParts.Add(newSnakeHead);
            
            var foundFood = m_gameManager.IsThereFoodInPosition(newSnakeHead.transform.position);
            if (foundFood)
            {
                m_gameManager.EatFood();
            }

            if (!foundFood)
            {
                var snakeTail = _snakeParts.First();
                _snakeParts.Remove(snakeTail);
                Destroy(snakeTail);
            }
            
            _stepTimer = 0;
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
}
