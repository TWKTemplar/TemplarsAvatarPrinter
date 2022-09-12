using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ScreenshotManager : MonoBehaviour
{
    //This script only exports a screenshot of what ever is in the camera, the script 'Video.cs' is what grabs the screen and displays it as a render texture

    public string FilePath;
    public int PictureNumber;
    
    [ContextMenu("Open File Path")]
    public void ShowExplorer()
    {
        var itemPath = FilePath;
        itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
    }
    public void TakeScreenShot()
    {

        PictureNumber++;
        var itemPath = FilePath;
        itemPath = itemPath.Replace(@"\", @"/"); //Must save with forward shashes
        string ScreenShotPathNName = itemPath + "/" + PictureNumber + ".png";
        ScreenCapture.CaptureScreenshot(ScreenShotPathNName);//Captures only what is being rendered in the camera
        Debug.Log("ScreenCapture: " + ScreenShotPathNName);

    }

}
