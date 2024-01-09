using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UvShellCreator : MonoBehaviour
{
    public Mesh OneUvShell;
    public Mesh TwoUvShell;
    public Mesh Avatar;

    private void Start()
    {
        List<UvShell> avatarShells = GetUvShells(Avatar);
    }


    public static List<UvShell> GetUvShells(Mesh mesh)
    {
        List<UvShell> uvShells = new List<UvShell>();
        UvShell firstShell = new UvShell(mesh.triangles[0], mesh.triangles[1], mesh.triangles[2]);
        uvShells.Add(firstShell);

        for (int i = 3; i < mesh.triangles.Length; i += 3)
        {
            int triPointA = mesh.triangles[i];
            int triPointB = mesh.triangles[i + 1];
            int triPointC = mesh.triangles[i + 2];

            List<UvShell> shellsConnectedToPoint = GetShellsConnectedToPoint(uvShells, triPointA, triPointB, triPointC).ToList();

            if(shellsConnectedToPoint.Count == 0) // Create new shell
            {
                UvShell newShell = new UvShell(triPointA, triPointB, triPointC);
                uvShells.Add(newShell);
            }
            else if(shellsConnectedToPoint.Count == 1) // Add it to one shell
            {
                shellsConnectedToPoint[0].AddTriangle(triPointA, triPointB, triPointC);
            }
            else // It is connected to multiple shells. Merge the shells and add triangle.
            {
                UvShell newShell = GetMergedShell(shellsConnectedToPoint);
                foreach (UvShell shell1 in shellsConnectedToPoint)
                {
                    uvShells.Remove(newShell);
                }
                newShell.AddTriangle(triPointA, triPointB, triPointC);
                uvShells.Add(newShell);
            }
        }
        return uvShells;
    }

    private static UvShell GetMergedShell(List<UvShell> shellsConnectedToPoint)
    {
        List<int> indicies = new List<int>();
        foreach (UvShell item in shellsConnectedToPoint)
        {
            indicies.AddRange(item.GetIndices());
        }
        return new UvShell(indicies.ToArray());
    }

    private static IEnumerable<UvShell> GetShellsConnectedToPoint(List<UvShell> shells, int triPointA, int triPointB, int triPointC)
    {
        foreach (UvShell shell in shells)
        {
            if (shell.ContainsUv(triPointA)
                || shell.ContainsUv(triPointB)
                || shell.ContainsUv(triPointC))
            {
                yield return shell;
            }
        }
    }
}

public class UvShell
{
    private readonly HashSet<int> uvs;

    public IEnumerable<int> GetIndices()
    {
        return uvs;
    }

    public UvShell(params int[] indicies)
    {
        uvs = new HashSet<int>(indicies);
    }

    public void AddTriangle(int triPointA, int triPointB, int triPointC)
    {
        uvs.Add(triPointA);
        uvs.Add(triPointB);
        uvs.Add(triPointC);
    }

    public bool ContainsUv(int uv)
    {
        return uvs.Contains(uv);
    }
}
