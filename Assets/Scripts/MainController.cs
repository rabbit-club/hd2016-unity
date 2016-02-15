using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityChan;
using System;
using System.IO;
using System.Collections;
using LitJson;

public class MainController : MonoBehaviour
{
	public GameObject Display;
	SpriteRenderer DisplaySprite;
	string imageUrl;
	string urlBase = "http://210.140.161.190:3000/";
	public string filePath = "";
	private AudioSource audioSource;
	private float maxAudioTime;
	private float audioTime;
	private GameObject shortDescription;
	private GameObject title;
	private GameObject startTime;
	private GameObject endTime;
	private GameObject circle;
	public FaceUpdate faceUpdate;
	Animator anim;
	//    float mouseTimer;
	//    bool mouseClose;

	public void Movie ()
	{
		StartCoroutine (MovieStart ());
	}

	IEnumerator MovieStart ()
	{
//        mouseTimer = 0;
//        mouseClose = true;
		anim = faceUpdate.anim;
		anim.CrossFade (faceUpdate.animations [12].name, 0);
		DisplaySprite = Display.GetComponent<SpriteRenderer> ();
		audioTime = 0.0f;
		maxAudioTime = 0.0f;
		audioSource = GetComponent<AudioSource> ();
		shortDescription = GameObject.Find ("Canvas/Footer/subtitles/Text");
		title = GameObject.Find ("Content/Info/Text");
		startTime = GameObject.Find ("Canvas/Footer/Seekbar/Time");
		endTime = GameObject.Find ("Canvas/Footer/Seekbar/EndTime");
		circle = GameObject.Find ("Canvas/Footer/Seekbar/circle");

		audioSource.PlayOneShot (Resources.Load ("SE/start", typeof(AudioClip)) as AudioClip);
		yield return new WaitForSeconds (0.5f);

		AudioClip hello = Resources.Load ("Voices/ohiru", typeof(AudioClip)) as AudioClip;
		audioSource.PlayOneShot (hello);
		yield return new WaitForSeconds (2.5f);

		// JSON取得
		WWW www = new WWW (urlBase + "articles?mode=y");
		yield return www;
		ArticleData[] articles = JsonMapper.ToObject<ArticleData[]> (www.text);
		foreach (var article in articles) {
			// 音声の取得と再生
			yield return new WaitForSeconds (audioTime);
			StartCoroutine (download (article.voicePathMp3));
			yield return new WaitForSeconds (1.0f);

			www = new WWW (article.imagePath);
			yield return www;

			Texture2D tex = www.texture;
			int x = 400;
			int y = 300;
			if (tex.width >= x && tex.height >= y) {
				DisplaySprite.sprite = Sprite.Create (
					tex, 
					new Rect (0, 0, x, y), 
					new Vector2 (0.5f, 0.5f)
				);
			} else if (tex.width >= x || tex.height < y) {
				double tmpWidth = tex.width;
				double tmpHeight = tex.height;
				double yy = y;
				double d = (double)(tmpWidth * (yy / tmpHeight));
				double e = (double)(tmpHeight * (yy / tmpHeight));
				int a = (int)Math.Ceiling (d);
				int b = (int)Math.Ceiling (e);
				TextureScale.Bilinear (tex, a, b);
				DisplaySprite.sprite = Sprite.Create (
					tex, 
					new Rect (0, 0, x, y), 
					new Vector2 (0.5f, 0.5f)
				);
			} else if (tex.width < x || tex.height >= y) {
				double tmpWidth = tex.width;
				double tmpHeight = tex.height;
				double xx = x;
				double d = (double)(tmpWidth * (xx / tmpWidth));
				double e = (double)(tmpHeight * (xx / tmpWidth));
				int a = (int)Math.Ceiling (d);
				int b = (int)Math.Ceiling (e);
				TextureScale.Bilinear (tex, a, b);
				DisplaySprite.sprite = Sprite.Create (
					tex, 
					new Rect (0, 0, x, y), 
					new Vector2 (0.5f, 0.5f)
				);
			} else {
				DisplaySprite.sprite = Sprite.Create (
					tex, 
					new Rect (0, 0, tex.width, tex.height), 
					new Vector2 (0.5f, 0.5f)
				);
			}

			// 要約記事テキストの表示
			if (shortDescription != null) {
				// 位置を初期化
				shortDescription.transform.localPosition = new Vector3 (5500.0f, shortDescription.transform.localPosition.y, shortDescription.transform.localPosition.z);
				shortDescription.GetComponent<Text> ().text = article.shortDescription;
			}

			// 記事タイトルの表示
			if (title != null) {
				title.GetComponent<Text> ().text = article.title;
			}

			// シークバーを動かす
			if (circle != null) {
				circle.transform.position = new Vector3 (-204, circle.transform.position.y, circle.transform.position.z);
				iTween.MoveTo (circle, iTween.Hash ("position", new Vector3 (373, circle.transform.position.y, 0), "time", maxAudioTime - 1, "easeType", "linear"));
			}

			// 音声時間maxの表示
			TimeSpan maxTs = TimeSpan.FromSeconds (maxAudioTime);
			endTime.GetComponent<Text> ().text = maxTs.Seconds.ToString ();
		}
	}

	IEnumerator download (string filePathUrl)
	{
		WWW www = new WWW (filePathUrl);

		while (!www.isDone) { // ダウンロードの進捗を表示
			print (Mathf.CeilToInt (www.progress * 100));
			yield return null;
		}

		if (!string.IsNullOrEmpty (www.error)) { // ダウンロードでエラーが発生した
			Debug.Log ("error:" + www.error);
		} else { // ダウンロードが正常に完了した
			Debug.Log ("download success.");
			filePath = Application.persistentDataPath + "/" + Path.GetFileName (www.url);
			File.WriteAllBytes (filePath, www.bytes);
			Debug.Log ("download file write success." + filePath);
//            audioSource.PlayOneShot(www.audioClip);
			audioSource.clip = www.audioClip;
			audioSource.Play ();
			// 音声の時間を保存しておく
			maxAudioTime = www.audioClip.length;
			audioTime = maxAudioTime;
		}
	}

	void Update ()
	{
		if (audioSource != null && audioSource.isPlaying && audioTime >= 0.0f) {
//            mouseTimer += Time.deltaTime;
//            if (mouseTimer > 0.2f)
//            {
//                paku();
//                mouseTimer = 0;
//                mouseClose = !mouseClose;
//            }
			audioTime -= Time.deltaTime;
			shortDescription.transform.localPosition = new Vector3 (shortDescription.transform.localPosition.x - (Time.deltaTime * 240.0f), shortDescription.transform.localPosition.y, shortDescription.transform.localPosition.z);
			float nowAudioTime = maxAudioTime - audioTime;
			TimeSpan nts = TimeSpan.FromSeconds (nowAudioTime);
			startTime.GetComponent<Text> ().text = nts.Seconds.ToString ();
		}
	}
	/*
    void paku()
    {
        if (mouseClose)
        {
            faceUpdate.OnCallChangeFace(faceUpdate.animations[0].name);
        }
        else
        {
            faceUpdate.OnCallChangeFace(faceUpdate.animations[12].name);
        }
    }
*/
	[System.Serializable]
	public class ArticleData
	{
		public string url;
		public string title;
		public string shortDescription;
		public string imagePath;
		public string voicePathOgg;
		public string voicePathWav;
		public string voicePathMp3;
	}
}
