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
        Debug.Log($"The amount of shells is {avatarShells.Count}"); //Looking for 86


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

        int descentValue = 1;
        for (int Tri = AllTris.Count -1; Tri >= 0; Tri-= descentValue) //### Doesn't check the number again ###
        {
            IntermediateTris.Add(AllTris[Tri]); //Add the first triangle to start the process. 

            List<Tri> LoggedTris = LogConnectingTris(IntermediateTris, AllTris); // returns a list of connected Tris and removes them from AllTris

            UvShell shell = new UvShell(LoggedTris);//Once all connectingTris are found put the LoggedTris into a Hash list called Shell

            IntermediateTris.Clear();

            CompletedUvShells.Add(shell);//Add completed shells to a list
            descentValue = LoggedTris.Count;
        }

        return AllTris; //return total count
    }

    private IEnumerable<int> GetTriIndices(List<Tri> allTris, Tri inputTriangle)
    {
        List<int> inputTriangleValues = inputTriangle.GetTri();
        for (int i = 0; i < allTris.Count; i++)
        {
            Tri theTriInQuestion = allTris[i];
            List<int> theValues = theTriInQuestion.GetTri();
            IEnumerable<int> theIntersection = theValues.Intersect(inputTriangleValues);
            bool areThereAny = theIntersection.Any();
            if (areThereAny)
            {
                yield return i;
            }
        }
    }

    private static List<Tri> LogConnectingTris(List<Tri> InputTriangle, List<Tri> AllTris) // Log intersecting triangles
    {
        List<Tri> LoggedTris = new List<Tri>();
        HashSet<int> trianglesToRemove = new HashSet<int>();
        List<Tri> ExtraTris = new List<Tri>();

        int previousLength = InputTriangle.Count;



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

             
        }
        List<int> trianglesToRemoveList = trianglesToRemove.ToList();
        // Remove intersecting triangles from AllTris
        foreach (int indexToRemove in trianglesToRemoveList.OrderByDescending(i => i)) //If you remove the index, then the items will slot down and youll remove the wrong one
        {
            try
            {
                AllTris.RemoveAt(indexToRemove);
            }
            catch
            {
                Debug.Log("Cant remove");
                continue;
            }

        }

        if (LoggedTris.Count > previousLength) //if the count of logged tris is more than the original tri do the thing again.
        {
            ExtraTris = LogConnectingTris(LoggedTris, AllTris);
        }

        LoggedTris.AddRange(ExtraTris);

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
    public HashSet<int> GetPoints()
    {
        HashSet<int> Vertex = new HashSet<int>();

        for (int x = 0; x < Shell.Count; x += 3)
        {
            List <int> TriSet = Shell[x].GetTri();

            HashSet<int> Interim = new HashSet<int>(TriSet);

            Interim.UnionWith(Vertex);
        }

        return Vertex;
    }
}
