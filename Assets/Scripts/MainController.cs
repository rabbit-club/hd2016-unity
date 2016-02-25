using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityChan;
using System;
using System.IO;
using System.Collections;
using LitJson;
using System.Linq;

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
	GameObject UnityChan;
	UnityChanTouch unityChanTouch;
	bool isOffLine = false;
	// 後ろのボードx
	int backBoardX = 400;
	// 後ろのボードy
	int backBoardY = 300;

	public void Movie ()
	{
		StartCoroutine (MovieStart ());
	}

	IEnumerator MovieStart ()
	{
		UnityChan = GameObject.Find ("unitychan");
		unityChanTouch = UnityChan.GetComponent<UnityChanTouch>();
		DisplaySprite = Display.GetComponent<SpriteRenderer> ();
		audioTime = 0.0f;
		maxAudioTime = 0.0f;
		audioSource = GetComponent<AudioSource> ();
		shortDescription = GameObject.Find ("Canvas/Footer/subtitles/Text");
		title = GameObject.Find ("Content/Info/Text");
		startTime = GameObject.Find ("Canvas/Footer/Seekbar/Time");
		endTime = GameObject.Find ("Canvas/Footer/Seekbar/EndTime");
		circle = GameObject.Find ("Canvas/Footer/Seekbar/circle");

		AudioClip seStart = Resources.Load ("SE/start", typeof(AudioClip)) as AudioClip;
		audioSource.PlayOneShot(seStart);
		yield return new WaitForSeconds(seStart.length);

		AudioClip hello = Resources.Load("Voices/ohiru", typeof(AudioClip)) as AudioClip;
		audioSource.PlayOneShot(hello);
		audioTime += hello.length; // 挨拶音声の時間を初回のウェイトにする
		audioTime += 0.5f;         // ワンテンポの間

		// JSON取得
		WWW www = new WWW (urlBase + "articles?mode=y");
		yield return www;
		ArticleData[] articles = JsonMapper.ToObject<ArticleData[]> (www.text);

		// ローカルキャッシュを作成する
//		createLocalCache(articles);
		if(isOffLine) {
			return false;
		}
		foreach (var article in articles) {
			// 音声の取得と再生
			yield return new WaitForSeconds (audioTime);
			StartCoroutine (download (article.voicePathMp3));
			yield return new WaitForSeconds (1.0f);

			// 画像を取得する
			www = new WWW (article.imagePath);
			yield return www;

			Texture2D tex = www.texture;
			// 画像をリサイズする
			reseizeTexture(tex);

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
			filePath = Application.persistentDataPath + "/" + Path.GetFileName (www.url);
			File.WriteAllBytes (filePath, www.bytes);
			Debug.Log ("download file write success." + filePath);
			audioSource.clip = www.audioClip;
			audioSource.Play();
			// 音声の時間を保存しておく
			maxAudioTime = www.audioClip.length;
			audioTime = maxAudioTime;
		}
	}

	void Update ()
	{
		if (audioSource != null && audioSource.isPlaying && audioTime >= 0.0f) {
			// 口パク
			unityChanTouch.useLip = true;

			audioTime -= Time.deltaTime;
			shortDescription.transform.localPosition = new Vector3 (shortDescription.transform.localPosition.x - (Time.deltaTime * 240.0f), shortDescription.transform.localPosition.y, shortDescription.transform.localPosition.z);
			float nowAudioTime = maxAudioTime - audioTime;
			TimeSpan nts = TimeSpan.FromSeconds (nowAudioTime);
			startTime.GetComponent<Text>().text = nts.Seconds.ToString();
		}
	}

	// 画像のテクスチャをリサイズする
	void reseizeTexture(Texture2D tex) {
		Debug.Log("texsize height:" + tex.height + " width:" + tex.width);
		float fixedHeight = tex.height;
		float fixedWidth = tex.width;
		// 画像サイズがパネルサイズより小さければリサイズせずに適用する。どちらかが越えていればリサイズを行う。
		if (tex.width > backBoardX || tex.height > backBoardY) {
			// 式：リサイズ対象の元となる辺ではない方の辺から引く長さ＝（リサイズ対象の元となる辺ー変更後サイズ）の絶対値 * (リサイズ対象の元となる辺ではない方の辺 / リサイズ対象の元となる辺)
			if ((tex.width - backBoardX) >= (tex.height - backBoardY)) {
				float resizeBaseX = tex.width;
				float resizeBaseY = tex.height - (Mathf.Abs (resizeBaseX - backBoardX) * (tex.height / resizeBaseX));
				Debug.Log("resizeBaseX:" + resizeBaseX + " backBoardX:" + backBoardX + " resizeBaseY:" + resizeBaseY);
				TextureScale.Bilinear(tex, backBoardX, (int)resizeBaseY);
				fixedHeight = resizeBaseY;
			} else {
				float resizeBaseY = tex.height;
				float resizeBaseX = tex.width - (Mathf.Abs (resizeBaseY - backBoardY) * (tex.width / resizeBaseY));
				TextureScale.Bilinear(tex, (int)resizeBaseX, backBoardY);
				fixedWidth = resizeBaseX;
			}
		}
		Debug.Log("fixedWidth:" + fixedWidth + " fixedHeight:" + fixedHeight);
		DisplaySprite.sprite = Sprite.Create (
			tex, 
			new Rect (0, 0, tex.width, tex.height), 
			new Vector2 (0.5f, 0.5f)
		);
	}

	void createLocalCache(ArticleData[] articles) {
		string json = LitJson.JsonMapper.ToJson(articles);
		PlayerPrefs.SetString("ONEWS_ARTICLES", json);
	}

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
