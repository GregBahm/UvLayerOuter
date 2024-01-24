using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UvShellCreator : MonoBehaviour
{
    
    public Mesh Avatar;

    private void Start()
    {
        List<Tri> avatarShells = GetUvShells(Avatar);
        Debug.Log($"Tri point is {avatarShells.Count}"); //Looking for 86


    }

    public static List<Tri> GetUvShells(Mesh mesh)
    {
        List<Tri> AllTris = new List<Tri>(); //Define a list that contains the entire shell array
        List<Tri> IntermediateTris = new List<Tri>(); //Make a list that holds information for shells being made

        List<UvShell> CompletedUvShells = new List<UvShell>(); //Make a list of completed UV shells that we can borrow away from AllUvShells


        for (int x = 0; x < mesh.triangles.Length; x += 3) //add all the tris to AllUvShells
        {
            Tri Tri = new Tri(mesh.triangles[x], mesh.triangles[x + 1], mesh.triangles[x + 2]);
            AllTris.Add(Tri);
        }

        IntermediateTris.Add(AllTris[2]); //Add the first triangle to start the process. 


        List<Tri> LoggedTris = LogConnectingTris(IntermediateTris, AllTris); // returns a list of connected Tris and removes them from AllTris



        return LoggedTris;
    }
    private static List<Tri> LogConnectingTris(List<Tri> InputTriangle, List<Tri> AllTris) // Log intersecting triangles
    {
        List<Tri> LoggedTris = new List<Tri>();
        List<int> trianglesToRemove = new List<int>();

        for (int x = 0; x < InputTriangle.Count; x++)
        {
            LoggedTris = Enumerable.Range(0, AllTris.Count)
            .Where(i =>
            {
                bool intersects = AllTris[i].GetTri().Intersect(InputTriangle[x].GetTri()).Any();
                if (intersects)
                {
                    // If intersected, mark the triangle index for removal
                    trianglesToRemove.Add(i);
                }
                return intersects;
            })
            .Select(i => AllTris[i])
            .ToList();

                // Remove intersecting triangles from AllTris
                foreach (int indexToRemove in trianglesToRemove.OrderByDescending(i => i))
                {
                    AllTris.RemoveAt(indexToRemove);
                }
            }

        return LoggedTris;


    }
}




public class Tri
{
    private readonly List<int> Uvs;

    public Tri(int triPointA, int triPointB, int triPointC)
    {
        Uvs = new List<int> { triPointA, triPointB, triPointC };
    }
    public List<int> GetTri()
    {
        return Uvs;
    }
    public bool ContainsInt(int value)
    {
        return Uvs.Contains(value);
    }
}


public class UvShell
{
    private readonly List<Tri> Shell;

    public UvShell(List<Tri> tri)
    {
        Shell = new List<Tri>(tri);
    }
}
