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


        for (int x = 0; x < mesh.triangles.Length; x+= 3) //add all the tris to AllUvShells
        {
            UvShell Tri = new UvShell(mesh.triangles[x], mesh.triangles[x+1], mesh.triangles[x+2]);
            AllUvShells.Add(Tri);
        }
            
        for (int x = 0; x < AllUvShells.Count; x += 3) //For every 3 points on triangle, check to see if they overlap with the first triangle
        {
            UvShell firstShell = new UvShell(mesh.triangles[0], mesh.triangles[1], mesh.triangles[2]); //define the starting triangle

            IntermediateUvShells.Add(firstShell); //Add the starting shell to the list

            List<UvShell> shellsConnectedToPoint = GetShellsConnectedToPoint(IntermediateUvShells, AllUvShells).ToList();

            CompletedUvShells.AddRange(shellsConnectedToPoint);

        }


        
        return AllUvShells;
    }

    //private static UvShell GetMergedShell(List<UvShell> shellsConnectedToPoint)
    //{

    //}



    private static List<UvShell> GetShellsConnectedToPoint(List<UvShell> IntermediateUvShells, List<UvShell> AllUvShells)
    {
        List<UvShell> shellsToAdd = new List<UvShell>();

        foreach (UvShell shell in IntermediateUvShells) //for each point check to see if any other triangle shares the same point
        {
            bool shellsAddedInIteration = false;

            foreach (int uvIndex in shell.UvIndices)
            {
                List<UvShell> connectedShells = AllUvShells.Where(otherShell => otherShell.ContainsUv(uvIndex)).ToList(); // Find other shells that share the same UV point, and add them to temp list

                shellsToAdd.AddRange(connectedShells); // Add connected shells to the list

                AllUvShells.RemoveAll(otherShell => connectedShells.Contains(otherShell)); // Remove the connected shells from allUvShells

                shellsToAdd.AddRange(GetShellsConnectedToPoint(connectedShells, AllUvShells)); // Recursively find more connected shells

                shellsAddedInIteration |= connectedShells.Count > 0;
            }

            if (!shellsAddedInIteration)
            {
                break;
            }
        }
        return shellsToAdd;
    }
}

public class UvShell
{
    private readonly HashSet<int> uvs;

    public IEnumerable<int> UvIndices => uvs;

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
