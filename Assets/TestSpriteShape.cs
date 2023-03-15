using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class TestSpriteShape : MonoBehaviour
{

    //TODO - Write it to a general PolygonDrawer

    public SpriteShapeController spriteShapeController;
    public SpriteShape spriteShape;

    public Vector3[] vector3s;

    // Start is called before the first frame update
    void Start()
    {

        //spriteShapeController.spline.InsertPointAt(4, new Vector3(5, 5, 5));
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(spriteShapeController.spline.GetPointCount());
        spriteShapeController.spline.SetPosition(0, vector3s[0]);
        spriteShapeController.spline.SetPosition(1, vector3s[1]);
        spriteShapeController.spline.SetPosition(2, vector3s[2]);
        spriteShapeController.spline.SetPosition(3, vector3s[3]);
        
    }
}
