using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.VisualScripting;
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
        List<LayoutRect> recs = cubes.Select(cube => LayoutRect.FromCube(cube)).ToList();
        List<LayoutRect> laidOut = Skyliner.GetLaidOut(recs);
        foreach (LayoutRect item in laidOut)
        {
            item.Place();
        }
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

public class LayoutRect
{
    public float X;
    public float Y;
    public float Width;
    public float Height;

    public float Left
    {
        get
        {
            return X + Width * .5f;
        }
        set
        {
            X = value - Width * .5f;
        }
    }
    public float Right
    {
        get
        {
            return X - Width * .5f;
        }
        set
        {
            X = value + Width * .5f;
        }
    }
    public float Top
    {
        get
        {
            return Y + Height * .5f;
        }
        set
        {
            Y = value - Height * .5f;
        }
    }
    public float Bottom
    {
        get
        {
            return Y - Height * .5f;
        }
        set
        {
            Y = value + Height * .5f;
        }
    }

    private Transform source;

    public LayoutRect(float x, float y, float width, float height, Transform source)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        this.source = source;
    }

    public void Place()
    {
        source.position = new Vector3(X, 0, Y);
    }

    public static LayoutRect FromCube(Transform cube)
    {
        return new LayoutRect(cube.position.x, cube.position.z, cube.localScale.x, cube.localScale.z, cube);
    }
}

public static class Skyliner
{
    public static List<LayoutRect> GetLaidOut(List<LayoutRect> sourceRecs)
    {
        List<LayoutRect> orderedByHeight = sourceRecs.OrderBy(item => item.Height).ToList();
        LayoutRect first = orderedByHeight[0];
        float currentHeight = 0;
        first.Left = 0;
        first.Bottom = currentHeight;

        for (int i = 1; i < orderedByHeight.Count; i++)
        {
            LayoutRect previous = orderedByHeight[i - 1];
            LayoutRect current = orderedByHeight[i];
            current.Left = previous.Right - .1f;
            current.Bottom = currentHeight;
        }
        return orderedByHeight;
    }
}