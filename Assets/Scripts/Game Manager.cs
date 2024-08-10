using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_food;
    

    private void Awake()
    {
        m_food = GameObject.Find("Food");
        if (m_food != null)
        {
            Debug.Log("Found Food!");
        }
        else
        {
            Debug.LogError("Did not find Food!");
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsThereFoodInPosition(Vector3 i_SnakePosition)
    {
        if (m_food == null)
            return false;
        
        return m_food.transform.position == i_SnakePosition;
    }

    public void EatFood()
    {
        Destroy(m_food);
        // TODO: spawn food in another space
    }
}
