using UnityEngine;
using UnityEngine.SceneManagement;
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
	private float audioTime;

	void Start() {
	}

	public void Movie() {
		StartCoroutine(MovieStart());
	}

    IEnumerator MovieStart()
    {
        DisplaySprite = Display.GetComponent<SpriteRenderer>();
		audioTime = 0.0f;
        audioSource = GetComponent<AudioSource>();
        // JSON取得
        WWW www = new WWW(urlBase + "articles");
        yield return www;
        ArticleData[] articles = JsonMapper.ToObject<ArticleData[]>(www.text);
        foreach (var article in articles)
        {
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
			audioTime = www.audioClip.length;
        }
    }

	void Update() {
		if(audioSource != null && audioSource.isPlaying && audioTime >= 0.0f) {
			audioTime -= Time.deltaTime;
			Debug.Log ("audioTime:" + audioTime);
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
