using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    [SerializeField]
    private int cubesCount = 20;

    private Transform[] cubes;

    private void Start()
    {
        cubes = CreateCubes();
        OrganizeCubes();
    }

    private void OrganizeCubes()
    {
        throw new NotImplementedException();
    }

    private Transform[] CreateCubes()
    {
        Transform[] ret = new Transform[cubesCount];
        for (int i = 0; i < cubesCount; i++)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            float randomX = UnityEngine.Random.value;
            float randomZ = UnityEngine.Random.value;
            obj.transform.localScale = new Vector3(randomX, 1, randomZ);
            ret[i] = obj.transform;
        }
        return ret;
    }
}
