using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspectio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenAspect = screenWidth * 1.0f / screenHeight;
        float milestoneAspect = 9f / 16f; // 1080x1920

        //orthorgraphic = sizeHeight / 2
        /*
         1080 --------->milestoneAspect
         Screen.width---> X
        X = Screen.width * milestoneAspect / 1080
         */
        
        /*
         aspect = width/height
        => width = aspect * height;
        heigt = 1080/aspect
        => size Camera = height/2;
         
         */


        if (screenAspect <= milestoneAspect)
        {
            Camera.main.orthographicSize = (1080f / 100.0f) / (2 * screenAspect);
        }
        else
        {
            Camera.main.orthographicSize = Screen.height / 200f; //1560f / 200f
        }
    }

   
}
