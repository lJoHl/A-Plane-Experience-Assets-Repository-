using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameObject Propeller;
    public GameObject MainCamera;
    public GameObject Goal3D;

    public float speed;
    public float rotationSpeed;
    public float propellerSpeed;

    private Vector3 offset;
    private Vector3 offsetRotate;
    private int changeCameraId = 1;

    private float verticalInput;
    private float horizontalInput;

    public AudioSource motorSound;
    public AudioSource deathSound;

    public AudioSource InBGM;
    public AudioSource playBGM;
    public AudioSource OutBGM;


    // Start is called before the first frame update.....................................................................
    void Start()
    {
        // freeze time
        Time.timeScale = 0;

        // avoid deathSounds at the start of the game 
        deathSound.Stop();
        OutBGM.Stop();

        // sounds InBGM at the start of the game
        InBGM.Play();
    }


    // Update is called once per frame...................................................................................
    void Update()
    {
        //_____________________________start and end____________________________________
        // defrost time and avoid continuing to play after crashing
        if (Input.GetKeyDown(KeyCode.Space) & !OutBGM.isPlaying)
            Time.timeScale = 1;

        // stops the player at the end of the game
        if (transform.position.z >= 620)
            speed = 0;

        // restart the game
        if (Input.GetKeyDown(KeyCode.R) & (Time.timeScale == 0 | speed == 0))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // change the way you view the goal, depending on your point of view
        if (changeCameraId == 1)
            Goal3D.SetActive(false);
        else if (changeCameraId == 2)
            Goal3D.SetActive(true);

        //________________________________movement______________________________________
        // get the user's horizontal and vertical input 
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        transform.rotation = Quaternion.Euler(45f * verticalInput, 0, 90f * horizontalInput);

        // move the plane forward at a constant rate
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        Propeller.transform.Rotate(Vector3.forward * Time.deltaTime * propellerSpeed);


        //_________________________________sound________________________________________
        // makes the propeller sound
        if (Time.timeScale == 0)
            motorSound.Stop();
        else if (!motorSound.isPlaying)
            motorSound.PlayScheduled(.0001);

        // makes playBGM sound when starting the game
        if (Time.timeScale == 0)
            playBGM.Stop();
        else if (!playBGM.isPlaying & speed > 0)
            playBGM.Play();

        // makes InBGM stop when starting the game
        if (playBGM.isPlaying | OutBGM.isPlaying)
            InBGM.Stop();
    }

    // LateUpdate method.................................................................................................
    void LateUpdate()
    {
        // call changeCamera method
        changeCamera();

        // change camera position
        switch (changeCameraId)
        {
            case 1: //changeCameraId = 1 --> side view
                offset.x = 32; 
                offset.y = 7;
                offset.z = 20;

                offsetRotate.x = 3.957f;
                offsetRotate.y = -84.669f;
                offsetRotate.z = 0;

                break;

            case 2: //changeCameraId = 2 --> third person view
                offset.x = 0;
                offset.y = 4.68f;
                offset.z = -11.25f;

                offsetRotate.x = 3;
                offsetRotate.y = 0;
                offsetRotate.z = 0;

                break;

            default:
                break;
        }


        // makes the camera follow the player
        MainCamera.transform.position = transform.position + offset;
        MainCamera.transform.rotation = Quaternion.Euler(offsetRotate);
    }


    // Change the camera view by pressing the "Q" key....................................................................
    void changeCamera()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            if (changeCameraId == 2)
                changeCameraId = 1;
            else
                changeCameraId++;
    }


    // Makes collision the player........................................................................................
    void OnCollisionEnter()
    {
        Time.timeScale = 0;
        deathSound.PlayScheduled(.0001);
        OutBGM.Play();
    }


    // shows the texts on the screen
    void OnGUI()
    {
        //_________________________________Button Instructions Texts________________________________________
        // button instructions in side view 
        if (Time.timeScale == 0 & InBGM.isPlaying & changeCameraId == 1)
        {
            GUI.Label(new Rect(350, 100, 150, 100), "Move: WASD / ↑←↓→");
            GUI.Label(new Rect(350, 115, 100, 100), "Change View: Q");
            GUI.Label(new Rect(356, 150, 124, 100), "Press space to Start");
        }

        // button instructions in third person view
        if (Time.timeScale == 0 & InBGM.isPlaying & changeCameraId == 2)
        {
            GUI.Label(new Rect(600, 50, 150, 100), "Move: WASD / ↑←↓→");
            GUI.Label(new Rect(600, 65, 100, 100), "Change View: Q");
            GUI.Label(new Rect(606, 90, 124, 100), "Press space to Start");
        }


        //______________________________________Game Title Text_____________________________________________
        // game title in side view 
        if (transform.position.z >= -17 & transform.position.z <= 140 & changeCameraId == 1 & playBGM.isPlaying)
            GUI.Label(new Rect(360, 115, 200, 100), "A PLANE EXPERIENCE");

        // game title in third person view
        if (transform.position.z >= -17 & transform.position.z <= 140 & changeCameraId == 2 & playBGM.isPlaying)
            GUI.Label(new Rect(170, 110, 200, 100), "A PLANE EXPERIENCE");


        //_____________________________________Death Restart Text___________________________________________
        // restart message in side view 
        if (OutBGM.isPlaying & changeCameraId == 1)
            GUI.Label(new Rect(290, 160, 100, 100), "R to Restart");

        // restart message in third person view
        if (OutBGM.isPlaying & changeCameraId == 2)
            GUI.Label(new Rect(625, 60, 100, 100), "R to Restart");


        //________________________________________Thanks Text_______________________________________________
        // thanks and restart message in side view 
        if (speed == 0 & changeCameraId == 1)
        {
            GUI.Label(new Rect(500, 100, 150, 100), "THANKS FOR PLAYING");
            GUI.Label(new Rect(535, 140, 100, 100), "R to Restart");
        }

        // thanks and restart message in third person view
        if (speed == 0 & changeCameraId == 2)
        {
            GUI.Label(new Rect(100, 140, 150, 100), "THANKS FOR PLAYING");
            GUI.Label(new Rect(535, 140, 100, 100), "R to Restart");
        }
    }
}
