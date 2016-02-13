using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MySceneManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SceneManager.LoadScene("spaceship", LoadSceneMode.Additive);
		SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
