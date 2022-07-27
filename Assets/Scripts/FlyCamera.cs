using UnityEngine;
using System.Collections;
 
public class FlyCamera : MonoBehaviour {
 
    /*
    Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
    Converted to C# 27-02-13 - no credit wanted.
    Simple flycam I made, since I couldn't find any others made public.  
    Made simple to use (drag and drop, done) for regular keyboard layout  
    wasd : basic movement
    shift : Makes camera accelerate
    space : Moves camera on X and Z axi ddadadada dads only.  So camera doesn't gain any height*/
     
     
    float mainSpeed = 25.0f; //regular speed
    float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
    float maxShift = 1000.0f; //Maximum speed when holdin gshift
    // float camSens = 0.25f; //How sensitive it with mouse
    float minFov = 2f;
    float maxFov = 300f;
    float sensitivity = -1f;
    // private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun= 1.0f;
     
    void Update () {
       
        //Keyboard commands
        // float f = 0.0f;
        Vector3 p = GetBaseInput();
        if (p.sqrMagnitude > 0){ // only move while a direction key is pressed
          if (Input.GetKey (KeyCode.LeftShift)){
              totalRun += Time.deltaTime;
              p  = p * totalRun * shiftAdd;
              p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
              p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
          } else {
              totalRun = Mathf.Clamp(totalRun * 0.5f, totalRun * 0.5f, 1f);
              p = p * mainSpeed;
          }
         
          p = p * Time.deltaTime;
          Vector3 newPosition = transform.position;
          if (Input.GetKey(KeyCode.Space)){ //If player wants to move on X and Z axis only
              transform.Translate(p);
              newPosition.x = transform.position.x;
              newPosition.y = transform.position.y;
              transform.position = newPosition;
          } else {
              transform.Translate(p);
          }
        }

        float fov = Camera.main.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;


        //  float ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");            //This little peece of code is written by JelleWho https://github.com/jellewie
        //  if (ScrollWheelChange != 0){                                            //If the scrollwheel has changed
        //      float R = ScrollWheelChange * 15;                                   //The radius from current camera
        //      float PosX = Camera.main.transform.eulerAngles.x + 90;              //Get up and down
        //      float PosY = -1 * (Camera.main.transform.eulerAngles.y - 90);       //Get left to right
        //      PosX = PosX / 180 * Mathf.PI;                                       //Convert from degrees to radians
        //      PosY = PosY / 180 * Mathf.PI;                                       //^
        //      float X = R * Mathf.Sin(PosX) * Mathf.Cos(PosY);                    //Calculate new coords
        //      float Z = R * Mathf.Sin(PosX) * Mathf.Sin(PosY);                    //^
        //      float Y = R * Mathf.Cos(PosX);                                      //^
        //      float CamX = Camera.main.transform.position.x;                      //Get current camera postition for the offset
        //      float CamY = Camera.main.transform.position.y;                      //^
        //      float CamZ = Camera.main.transform.position.z;                      //^
        //      Camera.main.transform.position = new Vector3(CamX + X, CamY + Y, CamZ + Z);//Move the main camera
        //  }


    }
     
    private Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey (KeyCode.W)){
            p_Velocity += new Vector3(0, 1 , 0);
        }
        if (Input.GetKey (KeyCode.S)){
            p_Velocity += new Vector3(0, -1, 0);
        }
        if (Input.GetKey (KeyCode.A)){
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey (KeyCode.D)){
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }
}