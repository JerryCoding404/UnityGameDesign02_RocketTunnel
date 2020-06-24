using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float LevelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip Death;
    [SerializeField] AudioClip Win;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem DeathParticles;
    [SerializeField] ParticleSystem WinParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;
    public enum State { Alive, Dying, Transcending };
    public State state = State.Alive;
    private Vector3 thrustThisFrame;

    public bool collisiondisabled = false;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        TestingLoadSceneKey();
    }

    void TestingLoadSceneKey()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKey(KeyCode.C))
        {
            collisiondisabled = !collisiondisabled;
            //disable collisions
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisiondisabled) { return; } //如果dead就不管collosion這件事了

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //print("OK");
                break;
            case "Fuel":
                print("Fuel");
                break;
            case "finish":
                StartWinSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }
    void StartWinSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(Win);
        WinParticles.Stop();
        mainEngineParticles.Stop();
        WinParticles.Play();
        state = State.Transcending;
        Invoke("LoadNextScene", LevelLoadDelay); //建立新功能後記得將數值參數0化、方便控制。
    }

    void StartDeathSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(Death);
        DeathParticles.Stop();
        mainEngineParticles.Stop();
        DeathParticles.Play();
        state = State.Dying;
        Invoke("LoadfirstScene", LevelLoadDelay);
    }

    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        print(nextSceneIndex);

        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }

        //另一種作法
        //switch (currentSceneIndex)
        //{
        //    case 0:
        //        nextSceneIndex = 1;
        //        break;
        //    case 1:
        //        nextSceneIndex = 1;
        //        break;
        //    case 2:
        //        nextSceneIndex = 1;
        //        break;
        //    case 3:
        //        nextSceneIndex = 1;
        //        break;
        //    case 4:
        //        nextSceneIndex = 1;
        //        break;
        //    case 5:
        //        nextSceneIndex = 1;
        //        break;
        //    case 6:
        //        nextSceneIndex = -6; //回到Level1 (index = 0)
        //        break;
        // }
        //currentSceneIndex = currentSceneIndex + nextSceneIndex;

        SceneManager.LoadScene(nextSceneIndex);
        print(nextSceneIndex);
    }

    void LoadfirstScene()
    {
        SceneManager.LoadScene(0);
        print("Dead");
    }

    void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    void ApplyThrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (audioSource.isPlaying == false)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        // print("Thrusting!!");
        mainEngineParticles.Play();
    }

    void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; //控制物件旋轉後不會亂動。
        float rotateThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotateThisFrame);
            // print("Rotate Left");
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotateThisFrame);
            // print("Rotate Right");
        }

        rigidBody.freezeRotation = false; //在按完按鍵後將物體的旋轉關掉，不能控制旋轉，回到原本的狀態。
    }
}
