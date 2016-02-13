using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Logo : MonoBehaviour, IPointerClickHandler {

	public void OnPointerClick (PointerEventData eventData){
		Debug.Log ( "OnPointerClick :" + eventData );
		GameObject.Destroy(this.gameObject);
	}
}
