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
        List<UvShell> avatarShells = GetUvShells(Avatar);
        Debug.Log($"size of avatarShells is {avatarShells.Count}"); //Looking for 86
        
        
    }

    public static List<UvShell> GetUvShells(Mesh mesh)
    {
        List<UvShell> AllUvShells = new List<UvShell>(); //Define a list that contains the entire shell array
        List<UvShell> IntermediateUvShells = new List<UvShell>(); //Make a list that holds information for shells being made
        List<UvShell> CompletedUvShells = new List<UvShell>(); //Make a list of completed UV shells that we can borrow away from AllUvShells
        

        for (int x = 0; x < mesh.triangles.Length; x+= 3) //add all the tris (17,910)
        {
            UvShell Tri = new UvShell(mesh.triangles[x], mesh.triangles[x+1], mesh.triangles[x+2]);
            AllUvShells.Add(Tri);
        }

        

        //UvShell testUv = AllUvShells[1]; You can call specific UV Tris by calling the index
        UvShell firstShell = AllUvShells[0]; //Add the first tri by indexing from all the shells
        IntermediateUvShells.Add(firstShell);
            

        List<UvShell> shellsConnectedToPoint = GetShellsConnectedToPoint(IntermediateUvShells, mesh).ToList();


        return AllUvShells;
    }

    //private static UvShell GetMergedShell(List<UvShell> shellsConnectedToPoint)
    //{

    //}

    private static List<UvShell> GetShellsConnectedToPoint(List<UvShell> inputShellList, Mesh mesh)
    {
        int triPointA = mesh.triangles[0];
        int triPointB = mesh.triangles[1];
        int triPointC = mesh.triangles[2];
    }
}

public class UvShell
{
    private readonly HashSet<int> uvs;

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
