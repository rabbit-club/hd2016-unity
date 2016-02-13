using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainController : MonoBehaviour {
	public GameObject Display;
	SpriteRenderer DisplaySprite;
	string imageUrl;

	IEnumerator Start ()
	{
		DisplaySprite = Display.GetComponent<SpriteRenderer>();

		imageUrl = "http://www.footballchannel.jp/wordpress/assets/2013/03/20130329_ni.jpg";

		WWW www = new WWW(imageUrl);
		yield return www;

		DisplaySprite.sprite = Sprite.Create (
			www.texture, 
			new Rect (0, 0, 400, 300), 
			new Vector2(0.5f, 0.5f)
		);
	}
}
