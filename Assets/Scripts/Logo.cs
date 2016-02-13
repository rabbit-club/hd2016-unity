using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Logo : MonoBehaviour, IPointerClickHandler {
	GameObject MainObject;
	MainController MainController;

	void Start() {
	}

	public void OnPointerClick (PointerEventData eventData){
		MainController = GameObject.Find("Camera").GetComponent<MainController>();;
		MainController.Movie();
		Debug.Log ( "OnPointerClick :" + eventData );
		GameObject.Destroy(this.gameObject);
	}
}
