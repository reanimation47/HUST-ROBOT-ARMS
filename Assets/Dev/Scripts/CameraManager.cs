using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public List<GameObject> Cameras;
    int currentIndex = 0;
    void Start()
    {
        Cameras[currentIndex].SetActive(true);
    }

    public void NextCamera()
    {
        DisableAllCameras();
        currentIndex = currentIndex >= Cameras.Count -1? 0 : currentIndex+ 1;
        Cameras[currentIndex].SetActive(true);


    }

    private void DisableAllCameras()
    {
        foreach(var cam in Cameras)
        {
            cam.SetActive(false);
        }
    }
}
