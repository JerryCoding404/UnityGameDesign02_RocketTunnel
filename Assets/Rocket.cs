using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip mainEngine;

    public Rigidbody rigidBody;
    public AudioSource audioSource;
    public enum State { Alive, Dying, Transcending };
    public State state = State.Alive;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive){ return; } //如果dead就不管collosion這件事了

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("OK");
                break;
            case "Fuel":
                print("Fuel");
                break;
            case "finish":
                state = State.Transcending;
                Invoke("LoadNextScene", 1f); //建立新功能後記得將數值參數化、方便控制。
                break;
            default:
                state = State.Dying;
                Invoke("LoadfirstScene", 1f);
                break;
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(1);
        print("you are here!!");
    }

    void LoadfirstScene()
    {
        SceneManager.LoadScene(0);
        print("Dead");
    }

    void Thrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
            if (audioSource.isPlaying == false)
            {
                audioSource.PlayOneShot(mainEngine);
            }
            // print("Thrusting!!");
        }  
        else
        {
            audioSource.Stop();
        }
    }

    void Rotate()
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
