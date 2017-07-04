using UnityEngine;
using System.Collections;

public class Screenshot : MonoBehaviour
{
    public int superSize = 2;
    public string filename = "screenshot";

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Application.CaptureScreenshot(filename + ".png", superSize);
            print("screenshot captured");
        }
    }
}