using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    public GameObject cubePrefab;
    public GameObject robotArm;
    public Transform cubePos;

    private void Awake() 
    { 
    // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 

    }
    void Start()
    {
        Application.targetFrameRate = 30;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        //Screen.SetResolution(1920, 1080, true);

        Time.timeScale = 2.0f;
    }
    void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            respawnBox();
        }
        
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void respawnBox()
    {
        Instantiate(cubePrefab, cubePos.position, Quaternion.Euler(0,0,0));
    }

    public void respawnRobot(GameObject r)
    {
        var pos = r.transform.position;
        DestroyImmediate(r);
        Instantiate(robotArm, pos, Quaternion.Euler(0,0,0));

    }
}
