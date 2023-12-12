using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.SearchService;
using UnityEngine.UIElements;

public class MainScript : MonoBehaviour
{
    [SerializeField]
    private int cubesCount = 20;

    private List<Square> squares;


    private void Start()
    {
        squares = CreateCubes();
        OrganizeCubes();
    }

    private Node root; //Adding a root node that contains the default positions 0,0,0

    public MainScript (int w, int h)
    {
        this.root = new Node { x = 0, y = 0, w = w, h = h };
    }
    

    private void OrganizeCubes()
    {
        List<Square> ourOrderedList = squares.OrderByDescending(texture => texture.Volume).ToList(); //Using squares defined above which is a list of ret. Organize by Volume

        TexSheet textureSheet = CreateTexSheet(); //Create the Texturesheet object

        bool placedBlock = false;
        Rect remainingRect = new Rect(0, 0, 1, 1); //Initialize with the entire sheet

        foreach (Square texture in ourOrderedList)
        {
            if (!placedBlock)
            {
                texture.BottomLeftCorner = Vector2.zero;
                placedBlock = true;

                //Split the remaining area into two rectangles
                Rect rect1, rect2;

                if (remainingRect.width > remainingRect.height)
                {
                    // Split horizontally
                    rect1 = new Rect(remainingRect.x, remainingRect.y, texture.Width, remainingRect.height);
                    rect2 = new Rect(remainingRect.x + texture.Width, remainingRect.y, remainingRect.width - texture.Width, remainingRect.height);
                }
                else
                {
                    // Split vertically
                    rect1 = new Rect(remainingRect.x, remainingRect.y, remainingRect.width, texture.Height);
                    rect2 = new Rect(remainingRect.x, remainingRect.y + texture.Height, remainingRect.width, remainingRect.height - texture.Height);
                }
                // Now, check if the next cube can fit into either of the two rectangles
                // Note: You need to implement the logic for checking if a cube fits into a rectangle
                remainingRect = rect1;
            }
            else
            {
                UnityEngine.Debug.Log("There is a cube already at 0,0");
                texture.BottomLeftCorner = new Vector2(1, 1);
            }

            ////Get the distance between the sheetEnd and the edge of the new block on top and bottom;
            ////somehow store that information;

            ////If the cube is too large to be placed, size the sheet up one segment;

            ////If there is no placed block, then default to putting the cube at space 0,0;
            //if (placedBlock == false)
            //{
            //    texture.BottomLeftCorner = Vector2.zero;
            //    placedBlock = true;
            //    //divide the space into 2 and 
            //}
            //else
            //{
            //    UnityEngine.Debug.Log("There is a cube already at 0,0");
            //    texture.BottomLeftCorner = new Vector2(1, 1); //Replace this with call function that checks the size between the new cube and the old one and then saves those values
            //    UnityEngine.Debug.Log(placedBlock);

            //}
            ////ResizeSheet(textureSheet); //Size up the texture sheet
            ////UnityEngine.Debug.Log(texture);
            //UnityEngine.Debug.Log(textureSheet);
        }
    }



    public class Node //Node contains all the positional data of our open space areas
    {
        public int x;
        public int y;
        public int w;
        public int h;
        public bool used;
        public Node up;
        public Node right;

    }

    private void ResizeSheet(TexSheet textureSheet)
    {
        textureSheet.RealTexSize = new Vector2(.1f, .1f);//REPLACE with something that intakes the current value and adds it by 1)
        textureSheet.TexBottomLeftCorner = Vector2.zero; //Always place the sheet back to corner (0,0,0)
    }

    private TexSheet CreateTexSheet()
    {   
        //Generate Texture Sheet
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        TexSheet textureSheet = new TexSheet(obj.transform);
        textureSheet.RealTexSize = new Vector2(.1f, .1f);
        textureSheet.TexBottomLeftCorner = Vector2.zero;
        return textureSheet;
    }

    private List<Square> CreateCubes()
    {
        List<Square> ret = new List<Square>();
        for (int i = 0; i < cubesCount; i++)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            float randomX = UnityEngine.Random.value;
            float randomZ = UnityEngine.Random.value;
            obj.transform.localScale = new Vector3(randomX, 1, randomZ);
            ret.Add(new Square(obj.transform));
        }
        
        return ret;
        
    }
   
}

public class TexSheet
{
    public Transform texInfo;


    public Vector2 TexCenter
    {
        get
        {
            return new Vector2(texInfo.position.x, texInfo.position.z);
        }
        set
        {
            texInfo.position = new Vector3(value.x, 0, value.y);
        }
    }
    public Vector2 TexSize
    {
        get
        {
            return new Vector2(texInfo.localScale.x * 10, texInfo.localScale.z * 10);
        }
        set
        {
            texInfo.localScale = new Vector3(value.x * 10, 0, value.y * 10);
        }
    }
    public float TexLength
    {
        get { return TexBottomRightCorner.x; }
    }
    public float TexHieght
    {
        get { return TexTopLeftCorner.y; }
    }
    public Vector2 RealTexSize
    {
        get
        {
            return new Vector2(texInfo.localScale.x, texInfo.localScale.z);
        }
        set
        {
            texInfo.localScale = new Vector3(value.x, .01f, value.y);
        }
    }
    public Vector2 TexTopLeftCorner
    {
        get
        {
            return GetCornerFromCenter(true, true);
        }
        set
        {
            TexCenter = ConvertCornerToCenter(value, true, true);
        }
    }
    public Vector2 TexTopRightCorner
    {
        get
        {
            return GetCornerFromCenter(true, false);
        }
        set
        {
            TexCenter = ConvertCornerToCenter(value, true, false);
        }
    }
    public Vector2 TexBottomRightCorner
    {
        get
        {
            return GetCornerFromCenter(false, false);
        }
        set
        {
            TexCenter = ConvertCornerToCenter(value, false, false);
        }
    }
    public Vector2 TexBottomLeftCorner
    {
        get
        {
            return GetCornerFromCenter(false, true);
        }
        set
        {
            TexCenter = ConvertCornerToCenter(value, false, true);
        }
    }




    public TexSheet(Transform info)
    {
        texInfo = info;
        TexCenter = new Vector2(info.position.x, info.position.z);
        RealTexSize = new Vector2(texInfo.localScale.x, texInfo.localScale.z);
        TexSize = new Vector2(texInfo.localScale.x * 10, texInfo.localScale.z *10);

    }
    private Vector2 GetCornerFromCenter(bool isCornerTop, bool isCornerLeft) //
    {
        float yOffset = isCornerTop ? TexSize.y : -TexSize.y;
        float xOffset = isCornerLeft ? -TexSize.x : TexSize.x;
        return TexCenter + new Vector2(xOffset, yOffset) * .5f;
    }
    private Vector2 ConvertCornerToCenter(Vector2 cornerValue, bool isCornerTop, bool isCornerLeft) //
    {
        float yOffset = isCornerTop ? -RealTexSize.y : RealTexSize.y;
        float xOffset = isCornerLeft ? RealTexSize.x : -RealTexSize.x;
        return cornerValue + new Vector2(xOffset, yOffset) * 5;
    }
    public override string ToString()
    {
        return "Texture Sheet Length is " + (TexLength) + ", The hieght is " + (TexHieght);
    }
}


public class Square
{
    public Transform myTransform;
    public Vector2 Center
    {
        get
        {
            return new Vector2(myTransform.position.x, myTransform.position.z);
        }

        set
        {
            myTransform.position = new Vector3(value.x, 0, value.y);
        }
    }
    public Vector2 Size 
    {
        get
        {
            return new Vector2(myTransform.localScale.x, myTransform.localScale.z);
        }
        
        set
        {
            myTransform.localScale = new Vector3(value.x, 1, value.y);
        }
    }
    public Vector2 TopLeftCorner
    {
        get
        {
            return GetCornerFromCenter(true, true);
        }
        set
        {
            Center = ConvertCornerToCenter(value, true, true);
        }
    }
    public Vector2 TopRightCorner
    {
        get
        {
            return GetCornerFromCenter(true, false);
        }
        set
        {
            Center = ConvertCornerToCenter(value, true, false);
        }
    }
    public Vector2 BottomRightCorner
    {
        get
        {
            return GetCornerFromCenter(false, false);
        }
        set
        {
            Center = ConvertCornerToCenter(value, false, false);
        }
    }
    public Vector2 BottomLeftCorner
    {
        get
        {
            return GetCornerFromCenter(false, true);
        }
        set
        {
            Center = ConvertCornerToCenter(value, false, true);
        }
    }

    public float Volume
    {
        get
        {
            return Size.x * Size.y;
        }
    }
    public float Width
    {
        get{ return myTransform.localScale.x;}
    }
    public float Height
    {
        get { return myTransform.localScale.z; }
    }

    public bool FitsIn(Rectangle rect)
    {
        return Width <= rect.Width && Height <= rect.Height;
    }
    public bool FitsPerfectlyIn(Rectangle rect)
    {
        return Width == rect.Width && Height == rect.Height;
    }




    public Square(Transform t)
    {
        if(t.localScale.x < 0)
        {
            throw new Exception("Tried to make an inverse box");
        }
        myTransform = t;
        Center = new Vector2(t.position.x, t.position.z);
        Size = new Vector2(t.localScale.x, t.localScale.z);
    }

    private Vector2 GetCornerFromCenter(bool isCornerTop, bool isCornerLeft)
    {
        float yOffset = isCornerTop ? Size.y : -Size.y;
        float xOffset = isCornerLeft ? Size.x : -Size.x;
        return Center + new Vector2(xOffset, yOffset) * .5f;
    }

    private Vector2 ConvertCornerToCenter(Vector2 cornerValue, bool isCornerTop, bool isCornerLeft)
    {
        float xOffset = isCornerLeft ? Size.x : -Size.x;
        float yOffset = isCornerTop ? -Size.y : Size.y;
        return cornerValue + new Vector2(xOffset, yOffset) * .5f;
    }

    public override string ToString()
    {
        return "Square of size " + Size.x + "," + Size.y + " at " + Center.x + ", " + Center.y;
    }
}






