using System.Collections;
using System.Collections.Generic;
using Germinator;
using UnityEngine;

public class EnemySpecie : MonoBehaviour
{
    [SerializeField]
    private EnemyBuilder builder;

    public void Start()
    {
        // var enemy = Instantiate(builder.data, transform.position, Quaternion.identity);
    }
}
