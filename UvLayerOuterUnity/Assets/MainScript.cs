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
        SquareBreakdown currentBreakdown;
        squares = CreateRandomCubes();

        Square textureSquare = new Square(new GameObject("Texture Sheet").transform);
        textureSquare.BottomLeftCorner = Vector2.zero;
        textureSquare.Visualize();

        List<Square> containingSquares = new List<Square>(); //Make a list of containing squares

        List<Square> ourOrderedList = squares.OrderByDescending(texture => texture.Volume).ToList();

        List<Square> allSquaresToVisualize = new List<Square>();


        for (int i = 0; i < ourOrderedList.Count; i++)
        {
            Square unplacedSquare = ourOrderedList[i];
            //If its the first square this will always get placed at vector2.zero
            if  (i == 0)
            {
                currentBreakdown = textureSquare.GetBreakdown(unplacedSquare);
                unplacedSquare.BottomLeftCorner = Vector2.zero;

                allSquaresToVisualize.Add(currentBreakdown.TopRectangle);
                allSquaresToVisualize.Add(currentBreakdown.RightRectangle);

                containingSquares.Add(currentBreakdown.TopRectangle);
                containingSquares.Add(currentBreakdown.RightRectangle);
            }

            else
            {
                foreach (Square square in containingSquares)
                {
                    if (square.Used == true) //check if square is used.
                    {
                        Debug.Log("Not doing the thing");
                        continue;
                    }
                    else
                    {
                        //Add logic for figuring out which square to put the object in. 
                        
                    }
                    
                }

                //Square SquareToUse = square;
                //currentBreakdown = SquareToUse.GetBreakdown(unplacedSquare);

                //containingSquares.Add(currentBreakdown.TopRectangle);
                //containingSquares.Add(currentBreakdown.RightRectangle);

                //allSquaresToVisualize.Add(currentBreakdown.TopRectangle);
                //allSquaresToVisualize.Add(currentBreakdown.RightRectangle);
            }

        }
        VisualizeSquares(allSquaresToVisualize);
    }


    private void VisualizeSquares(List<Square> theSquareSpaces)
    {
        foreach (Square square in theSquareSpaces)
        {
            square.Visualize();
        }
    }

    private void ResizeSheet(TexSheet textureSheet)
    {
        textureSheet.RealTexSize = new Vector2(.2f, .2f);//REPLACE with something that intakes the current value and adds it by 1)
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

    private List<Square> CreateRandomCubes()
    {
        List<Square> ret = new List<Square>();
        for (int i = 0; i < cubesCount; i++)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "Random Cube " + i.ToString();
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

    private bool used;

    public bool Used
    {
        get { return used; }
        set { used = value; }
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

    private Vector2 GetCornerFromCenter(bool isCornerTop, bool isCornerLeft) //false false
    {
        float yOffset = isCornerTop ? Size.y : -Size.y;
        float xOffset = isCornerLeft ? -Size.x : Size.x;
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

    internal void Visualize()
    {
        Transform cubeTransform = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        cubeTransform.gameObject.name = "visualization of " + myTransform.name;
        cubeTransform.position = myTransform.position;
        cubeTransform.localScale = myTransform.localScale * .99f;
    }

    public SquareBreakdown GetBreakdown(Square unplacedSquare)
    {
        return new SquareBreakdown(this, unplacedSquare);
    }
}

public class SquareBreakdown
{
    private Square sourceSquare;
    public Square SourceSquare { get => sourceSquare; }

    private Square topRectangle;
    public Square TopRectangle
    {
        get
        {
            return topRectangle;
        }
        set
        {
            topRectangle = TopRectangle;
        }
    }
    private Square rightRectangle;
    public Square RightRectangle { get => rightRectangle; }



    public SquareBreakdown(Square containingSquare, Square sourceSquare)
    {      
        
        this.sourceSquare = sourceSquare;

        //Get the differences of hieght and width of the new spaces
        float diffWidth = containingSquare.Width - sourceSquare.Width;
        float diffHeight = containingSquare.Height - sourceSquare.Height;

        if (diffWidth > diffHeight) //If the difference in width is greater than the difference of height split the space vertically. If not, split horizontally
        {
            rightRectangle = new Square(new GameObject("Right empty space square").transform);
            rightRectangle.Size = new Vector2(containingSquare.Width - sourceSquare.Width, containingSquare.Height);
            rightRectangle.BottomLeftCorner = new Vector2(sourceSquare.Width, 0);
            rightRectangle.Used = false;
            

            topRectangle = new Square(new GameObject("Top empty space square").transform);
            topRectangle.Size = new Vector2(SourceSquare.Width, containingSquare.Height - sourceSquare.Height);
            topRectangle.BottomLeftCorner = new Vector2(0, sourceSquare.Height);
            topRectangle.Used = false;
        }


        else
        {
            rightRectangle = new Square(new GameObject("Right empty space square").transform);
            rightRectangle.Size = new Vector2(containingSquare.Width - sourceSquare.Width, SourceSquare.Height);
            rightRectangle.BottomLeftCorner = new Vector2(sourceSquare.Width, 0);
            rightRectangle.Used = false;

            topRectangle = new Square(new GameObject("Top empty space square").transform);
            topRectangle.Size = new Vector2(containingSquare.Width, containingSquare.Height - sourceSquare.Height);
            topRectangle.BottomLeftCorner = new Vector2(0, sourceSquare.Height);
            topRectangle.Used = false;
        }
    }
}