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


    private void OrganizeCubes()
    {
        List<Square> ourOrderedList = squares.OrderByDescending(texture => texture.Volume).ToList();
        TexSheet textureSheet = CreateTexSheet();
        bool placedBlock = false;

        Transform myTransform = new GameObject("MyRectangle").transform;
        Transform rightSpace = new GameObject("RightRectangle").transform;
        Transform topSpace = new GameObject("TopRectangle").transform;

        Rectangle RootRectangle = new Rectangle(myTransform, 1.0f, 1.0f); //public Rectangle(Transform spaceInfo, float width, float height)

        RootRectangle.position = new Vector2(0.0f, 0.0f);

        Rectangle RightRectangle = null;
        Rectangle TopRectangle = null;

        foreach (Square texture in ourOrderedList)
        {
            
            if (placedBlock == false)
            {
                texture.BottomLeftCorner = Vector2.zero;
                placedBlock = true;

                // Make the first rectangle
                RightRectangle = new Rectangle(rightSpace, RootRectangle.w - texture.Width, texture.myTransform.localScale.z);
                RightRectangle.position = new Vector2(texture.BottomRightCorner.x, texture.BottomRightCorner.y);

                // Make the second rectangle
                TopRectangle = new Rectangle(topSpace, RootRectangle.w, RootRectangle.h - texture.myTransform.localScale.z);
                TopRectangle.position = new Vector2(texture.TopLeftCorner.x, texture.TopLeftCorner.y);
                
            } 
            else
            {
                UnityEngine.Debug.Log("There is a cube already at 0,0");

                if(TopRectangle.h > texture.myTransform.localScale.z)
                {
                    UnityEngine.Debug.Log("The Hieght of Top Rectangle is" + TopRectangle.h);
                    UnityEngine.Debug.Log("The Hieght of Placed Texture is" + texture.myTransform.localScale.z);
                    texture.BottomLeftCorner = new Vector2(TopRectangle.position.x, TopRectangle.position.y);
                    TopRectangle.h = TopRectangle.h - texture.myTransform.localScale.z;
                    UnityEngine.Debug.Log("The Height of the new rectangle is" + TopRectangle.h);
                    //Set the new potion to be the TL corner of the upper texture #######################
                }
                
                else if (RightRectangle.w > texture.myTransform.localScale.x)
                {
                    UnityEngine.Debug.Log("The Width of Right Rectangle is" + RightRectangle.w);
                    texture.BottomLeftCorner = new Vector2(RightRectangle.position.x, RightRectangle.position.y);
                    RightRectangle.w = RightRectangle.w - texture.Width;
                    UnityEngine.Debug.Log("The Width of the new rectangle is" + RightRectangle.w);
                    //Set the new potion to be the BR corner of the right texture ###########################
                }
                else
                {
                    
                    UnityEngine.Debug.Log("Texture does not fit anywhere, sizing up the texture sheet");
                    ResizeSheet(textureSheet);
                    //Size up the texture sheet and then run again? #######################
                }
            }
        }
    }



    public class Rectangle //Node contains all the positional data of our open space areas
    {
        public Transform spaceInfo;
        public Vector2 position //Represents a starting point
        {
            get
            {
                return new Vector2(spaceInfo.position.x, spaceInfo.position.z);
            }
            set
            {
                spaceInfo.position = new Vector3(value.x, spaceInfo.position.y, value.y);
            }
        }
        public float w { get; set; }  //Represents the distance to the left
        public float h { get; set; }  //represents the distance to the right

        public Vector2 TopLeftCorner; // Add information about getting this position ###########################
        public Vector2 BottomRightCorner; //Add information about getting this position ########################

        public bool used { get; set; } //Represents if the node has been used
        public Rectangle(Transform spaceInfo, float width, float height)
        {
            this.spaceInfo = spaceInfo;
            w = width;
            h = height;
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
}






