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

		imageUrl = "amd.c.yimg.jp/amd/20160213-00010005-tennisd-000-1-view.jpg";

		WWW www = new WWW(imageUrl);
		yield return www;

		DisplaySprite.sprite = Sprite.Create (
			www.texture, 
			new Rect (0, 0, 400, 300), 
			new Vector2(0.5f, 0.5f)
		);
	}
}
