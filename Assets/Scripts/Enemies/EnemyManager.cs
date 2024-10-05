using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class EnemyDefinition
{
    public string name;
    public Enemy prefab;
}

class EnemyDefinitionWithIndex : EnemyDefinition
{
    public readonly int index;

    public EnemyDefinitionWithIndex(int index, EnemyDefinition definition)
    {
        name = definition.name;
        prefab = definition.prefab;
        this.index = index;
    }
}

public class EnemyManager : MonoBehaviour
{
    #region Variables

    [Header("Input")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private EnemyDefinition[] inputDefinitions = Array.Empty<EnemyDefinition>();
    [SerializeField][Range(0, 500)] private int bufferSize = 100;

    private Dictionary<string, int> definitionIndices = new();
    private EnemyDefinitionWithIndex[] definitions = Array.Empty<EnemyDefinitionWithIndex>();

    private Enemy[][] enemies = Array.Empty<Enemy[]>();
    private System.Random random = new System.Random();

    #endregion

    public void Init()
    {
        Clear();

        enemies = new Enemy[inputDefinitions.Length][];
        definitions = new EnemyDefinitionWithIndex[inputDefinitions.Length];
        for (int i = 0; i < inputDefinitions.Length; i++)
        {
            EnemyDefinitionWithIndex definition = new EnemyDefinitionWithIndex(i, inputDefinitions[i]);
            definitions[i] = definition;
            enemies[definition.index] = new Enemy[bufferSize];
            for (int j = 0; j < bufferSize; j++)
            {
                Enemy enemy = Instantiate(definition.prefab, transform);
                enemy.name = $"{definition.name} [{j}]";
                enemies[i][j] = enemy;
            }
        }
    }

    public void SpawnOne()
    {
        Spawn(0, 10);
    }

    // Spawns some enemies of a given definition
    public void Spawn(int definitionIndex, int quantity)
    {
        if (definitionIndex >= definitions.Length)
        {
            return;
        }

        int remaining = quantity;
        int index = 0;
        var definitionEnemies = enemies[definitionIndex];
        Vector3 position = playerTransform.position + (Vector3)GetRandomPosition(4);
        while (remaining > 0 && index < definitionEnemies.Length)
        {
            var enemy = definitionEnemies[index];
            if (!enemy.IsActive)
            {
                enemy.transform.position = position + new Vector3((float)random.NextDouble() * 1f, (float)random.NextDouble() * 1f, 0f);
                remaining--;
                enemy.IsActive = true;
            }

            index++;
        }
    }

    public void Clear()
    {
        foreach (var definitionEnemies in enemies)
        {
            foreach (var enemy in definitionEnemies)
            {
                Destroy(enemy.gameObject);
            }
        }

        enemies = Array.Empty<Enemy[]>();
    }

    private Vector2 GetRandomPosition(float distance)
    {
        float angle = (float)(random.NextDouble() * 2.0 * Math.PI);

        return new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
    }

    #region Game loop

    public void Update()
    {
        foreach (var definitionEnemies in enemies)
        {
            foreach (var enemy in definitionEnemies)
            {
                // Check if active
                if (enemy.IsActive)
                {
                    enemy.MoveTowards(playerTransform.position);
                }
            }
        }
    }

    #endregion
}
