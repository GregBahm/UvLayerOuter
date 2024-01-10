using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        List<UvShell> uvShells = new List<UvShell>();
        List<UvShell> CompletedUvShells = new List<UvShell>(); //Make a list of completed UV shells that we can borrow away from AllUvShells
        List<UvShell> AllUvShells = new List<UvShell>(); //Define a list that contains the entire shell array

        for (int x = 0; x < mesh.triangles.Length; x+= 3) //add all the tris (17,910)
        {
            UvShell uvShell = new UvShell(mesh.triangles[x], mesh.triangles[x+1], mesh.triangles[x+2]);
            AllUvShells.Add(uvShell);
        }
        //Debug.Log($"The size of AllUvShells is {AllUvShells.Count}");

        //UvShell testUv = AllUvShells[1]; You can call specific UV Tris by calling the index
        UvShell firstShell = AllUvShells[0]; //Add the first tri by indexing from all the shells

        uvShells.Add(firstShell);

        for (int i = 3; i < mesh.triangles.Length; i += 3)
        {
            int triPointA = mesh.triangles[i];
            int triPointB = mesh.triangles[i + 1];
            int triPointC = mesh.triangles[i + 2];

            List<UvShell> shellsConnectedToPoint = GetShellsConnectedToPoint(AllUvShells, triPointA, triPointB, triPointC).ToList(); //Comes back with an inversed of removed shells from the list

            if(shellsConnectedToPoint.Count == 0) // Create new shell
            {
                UvShell newShell = new UvShell(triPointA, triPointB, triPointC);
                CompletedUvShells.Add(newShell); //Add the finished shell to this list
                //AllUvShells.Remove(newShell);
            }
            else if(shellsConnectedToPoint.Count == 1) // Add it to one shell
            {
                shellsConnectedToPoint[0].AddTriangle(triPointA, triPointB, triPointC);
                //then what does this do?
            }
            else // It is connected to multiple shells. Merge the shells and add triangle.
            {
                UvShell newShell = GetMergedShell(shellsConnectedToPoint);
                foreach (UvShell shell1 in shellsConnectedToPoint)
                {
                    uvShells.Remove(shell1);
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

    private static IEnumerable<UvShell> GetShellsConnectedToPoint(List<UvShell> inputShellList, int triPointA, int triPointB, int triPointC)
    {
        foreach (UvShell shell in inputShellList)
        {
            if (shell.ContainsUv(triPointA)|| shell.ContainsUv(triPointB)|| shell.ContainsUv(triPointC))
            {
                inputShellList.Remove(shell);
                yield return shell; //need to create a system that removes the used shells and only checks against shells in AllShells list
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
