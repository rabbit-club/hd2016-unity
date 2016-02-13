using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Logo : MonoBehaviour, IPointerClickHandler {
	GameObject MainObject;
	MainController MainController;

	public GameObject UnityChan;
	Animator UnityChanAnim;

	void Start() {
	}

	public void OnPointerClick (PointerEventData eventData){
		UnityChan = GameObject.Find ("unitychan");
		UnityChanAnim = UnityChan.GetComponent<Animator>();
		UnityChanAnim.SetBool ("Next", true);

		MainController = GameObject.Find("Camera").GetComponent<MainController>();;
		MainController.Movie();
		Debug.Log ( "OnPointerClick :" + eventData );
		GameObject.Destroy(this.gameObject);
	}
}
