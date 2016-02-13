using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
	private GameObject startTime;
	private GameObject endTime;

	void Start() {
	}

	public void Movie() {
		StartCoroutine(MovieStart());
	}

    IEnumerator MovieStart()
    {
        DisplaySprite = Display.GetComponent<SpriteRenderer>();
		audioTime = 0.0f;
		maxAudioTime = 0.0f;
        audioSource = GetComponent<AudioSource>();
		shortDescription = GameObject.Find("Canvas/Footer/subtitles/Text");
		startTime = GameObject.Find("Canvas/Footer/Seekbar/Time");
		endTime = GameObject.Find("Canvas/Footer/Seekbar/EndTime");
        // JSON取得
        WWW www = new WWW(urlBase + "articles");
        yield return www;
        ArticleData[] articles = JsonMapper.ToObject<ArticleData[]>(www.text);
        foreach (var article in articles)
        {
			// 音声の取得と再生
			yield return new WaitForSeconds(audioTime);
            StartCoroutine(download(article.voicePathWav));
			yield return new WaitForSeconds(0.5f);

			www = new WWW(article.imagePath);
			yield return www;

			DisplaySprite.sprite = Sprite.Create(
				www.texture, 
				new Rect(0, 0, 400, 300), 
				new Vector2(0.5f, 0.5f)
			);

			// 要約記事テキストの表示
			if(shortDescription != null) {
				// 位置を初期化
				shortDescription.transform.localPosition = new Vector3(5500.0f, shortDescription.transform.localPosition.y, shortDescription.transform.localPosition.z);
				shortDescription.GetComponent<Text>().text = article.shortDescription;
			}

        }
    }

    IEnumerator download(string filePathUrl)
    {
        WWW www = new WWW(filePathUrl);

        while (!www.isDone)
        { // ダウンロードの進捗を表示
            print(Mathf.CeilToInt(www.progress * 100));
            yield return null;
        }

        if (!string.IsNullOrEmpty(www.error))
        { // ダウンロードでエラーが発生した
            Debug.Log("error:" + www.error);
        }
        else
        { // ダウンロードが正常に完了した
            Debug.Log("download success.");
            filePath = Application.persistentDataPath + "/" + Path.GetFileName(www.url);
            File.WriteAllBytes(filePath, www.bytes);
            Debug.Log("download file write success." + filePath);
            audioSource.PlayOneShot(www.audioClip);
			// 音声の時間を保存しておく
			maxAudioTime = www.audioClip.length;
			audioTime = maxAudioTime;
        }
    }

	void Update() {
		if(audioSource != null && audioSource.isPlaying && audioTime >= 0.0f) {
			audioTime -= Time.deltaTime;
			TimeSpan ts = TimeSpan.FromSeconds(audioTime);
			Debug.Log ("audioTime:" + audioTime + " ts:" + ts.Seconds);
			shortDescription.transform.localPosition = new Vector3(shortDescription.transform.localPosition.x - (Time.deltaTime * 250.0f), shortDescription.transform.localPosition.y, shortDescription.transform.localPosition.z);
			float nowAudioTime = maxAudioTime - audioTime;
			TimeSpan nts = TimeSpan.FromSeconds(nowAudioTime);
			startTime.GetComponent<Text>().text = nts.Seconds.ToString();
			endTime.GetComponent<Text>().text = ts.Seconds.ToString();
		}
		// 再生されていないのに音声秒数が入っているか、0を切っている場合は再生が終了している
		if((audioSource != null && !audioSource.isPlaying && audioTime > 0.0f) || audioTime < 0.0f) {
			Debug.Log ("audioTime less zero.");
		}
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
    }
}
