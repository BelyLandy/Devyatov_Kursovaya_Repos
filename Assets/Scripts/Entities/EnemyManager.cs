using UnityEngine;
using System.Collections.Generic;

public static class EnemyManager
{
    public static List<GameObject> enemyList = new List<GameObject>();

    public static void RemoveEnemyFromList(GameObject enemy)
    {
        enemyList.Remove(enemy);
    }

    public static void AddEnemyToList(GameObject enemy)
    {
        if (!enemyList.Contains(enemy)) enemyList.Add(enemy);
    }

    public static void DisableAllEnemyAI()
    {
        foreach (GameObject enemy in enemyList)
        {
            enemy.GetComponent<EnemyBehaviour>().AI_Active = false;
            enemy.GetComponent<StateMachine>()?.SetState(new EnemyIdle());
        }
    }

    public static int GetEnemyAttackerCount()
    {
        int attackerCount = 0;
        foreach (GameObject enemy in enemyList)
        {
            if (!enemy) continue;
            StateMachine stateMachine = enemy.GetComponent<StateMachine>();
            if (stateMachine == null) continue;
            else if (stateMachine.GetCurrentState() is EnemyAttack ||
                     stateMachine.GetCurrentState() is EnemyMoveToTargetAndAttack) attackerCount++;
        }

        return attackerCount;
    }

    public static GameObject GetRandomEnemy()
    {
        if (enemyList.Count == 0) return null;
        int i = Random.Range(0, enemyList.Count);
        return enemyList[i];
    }

    public static int GetTotalEnemyCount()
    {
        var enemies = Object.FindObjectsByType<HealthController>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        int count = 0;
        foreach (var h in enemies)
        {
            if (h.IsEnemy && h.CurrentHealth > 0) count++;
        }

        return count;
    }

    public static int GetCurrentEnemyCount()
    {
        var enemies = Object.FindObjectsByType<HealthController>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None);

        int count = 0;
        foreach (var h in enemies)
        {
            if (h.IsEnemy && h.CurrentHealth > 0) count++;
        }

        return count;
    }
}