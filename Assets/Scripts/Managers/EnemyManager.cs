using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance {get; private set;}

    public List<EnemyLivingBeing> enemyList  = new List<EnemyLivingBeing>();

    private void Awake() 
    {
        Instance = this;
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (var enemy in enemyList)
                enemy.BaseRevive();
        }
    }
}
