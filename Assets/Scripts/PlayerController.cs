using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private int count;

    public float speed;
    public Text countText;
    public Text winText;
    public Text timerText;

    public float currentTimer;
    public bool isLevelComplete;

    public int countToComplete;
    int currentLevel;

    void Start()
    {

        //InitializeFirebaseSDK();
            
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://ufogame-bcdca.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseApp app = FirebaseApp.DefaultInstance;
        Debug.Log("reference.ToString() = " + reference.ToString());

        //Debug.Log("PlayerStats = " + PlayerStats.test);
        
        reference.Child("HighScores").SetRawJsonValueAsync("2"); //updates Firebase with new value
        reference.Child("Jim").SetValueAsync("Top Player");


 
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Scene = " + currentLevel);

        rb2d = GetComponent<Rigidbody2D>();
        count = 0;
        StartCoroutine(SetCountText());
        winText.text = "";
        timerText.text = "Timer: ";
        isLevelComplete = false;


        FirebaseDatabase.DefaultInstance.GetReference("Jim").GetValueAsync().ContinueWith(task => 
        {
          if (task.IsFaulted)
            {
                    // Handle the error...
                    Debug.Log("isFaulted");
            }
          else if (task.IsCompleted)
            {
              DataSnapshot snapshot = task.Result;
                    // Do something with snapshot...
                    Debug.Log("snapshot string = " + snapshot.ToString());
           
            }
        });
    }
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
       
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rb2d.AddForce(movement * speed);

        StartCoroutine(SetCountText());

        if (!isLevelComplete)
        {
            currentTimer = currentTimer + Time.deltaTime;
            timerText.text = "Timer: " + currentTimer.ToString("00.00");
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
        }

    }
    void InitializeFirebaseSDK()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp, i.e.
                //   app = Firebase.FirebaseApp.DefaultInstance;
                // where app is a Firebase.FirebaseApp property of your application class.

                // Set a flag here indicating that Firebase is ready to use by your
                // application.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
    IEnumerator SetCountText ()
    {
        //Updates the counter and checks if the level is complete.
        //If it's the last level then game ends.
 
        countText.text = "Count = " + count.ToString();
        if (count >= countToComplete)
        {
       
            isLevelComplete = true;
            winText.text = "You Win!";

            yield return new WaitForSeconds(2); //time before the next scene is loaded

            if (currentLevel < SceneManager.sceneCountInBuildSettings - 1) //build index starts at 0
            {
                UpdateLevelTime(currentLevel);
                SceneManager.LoadScene(currentLevel + 1);
            }
            else
            {
               //Game is over
                Application.Quit();
            }

            PlayerStats.test = PlayerStats.test + count; //code to just test persistent data
         
        }
    }
    void UpdateLevelTime (int level)
    {
        Debug.Log("UpdateLeve() currentLevel = " + level);
        FirebaseDatabase.DefaultInstance.GetReference("Jim").SetValueAsync("Another Player");

    }

}