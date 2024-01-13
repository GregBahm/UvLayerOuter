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
        List<Tri> IntermediateShells = new List<Tri>(); //Make a list that holds information for shells being made

        List<UvShell> CompletedUvShells = new List<UvShell>(); //Make a list of completed UV shells that we can borrow away from AllUvShells


        for (int x = 0; x < mesh.triangles.Length; x += 3) //add all the tris to AllUvShells
        {
            Tri Tri = new Tri(mesh.triangles[x], mesh.triangles[x + 1], mesh.triangles[x + 2]);
            AllTris.Add(Tri);
        }

        for (int x = 0; x < AllTris.Count; x++)
        {

            List <Tri> LoggedTris = LogConnectingTris(AllTris[x], AllTris); // returns a list of connected Tris and removes them from AllTris

            // ################## Add logic for if (LoggedTris.Any()) then chech if those have any connections until it bring back nothing (else) #################

        }
        return AllTris;
    }
    private static List<Tri> LogConnectingTris(Tri InputTriangle, List<Tri> AllTris) // Log intersecting triangles
    {
        
        List<Tri> LoggedTris = Enumerable.Range(0, AllTris.Count)
            .Where(i => AllTris[i].GetTri().Intersect(InputTriangle.GetTri()).Any())
            .Select(i => AllTris[i])
            .ToList();

        if (LoggedTris.Any())
        {
            for (int i = 0; i < LoggedTris.Count; i++)
            {
                    AllTris.Remove(LoggedTris[i]); //Remove the LoggedTris from AllTris
            }

            return LoggedTris;

        }
        else
        {
            LoggedTris.Add(InputTriangle); //The triangle has no connections, so it must be it's own shell. 

            return LoggedTris;
        }


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
