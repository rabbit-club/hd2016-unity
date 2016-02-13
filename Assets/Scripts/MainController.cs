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

    IEnumerator Start()
    {
        DisplaySprite = Display.GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        // JSON取得
        WWW www = new WWW(urlBase + "articles");
        yield return www;
        ArticleData[] articles = JsonMapper.ToObject<ArticleData[]>(www.text);
        foreach (var article in articles)
        {
            imageUrl = "http://www.footballchannel.jp/wordpress/assets/2013/03/20130329_ni.jpg";
//            www = new WWW(article.imagePath);
            www = new WWW(imageUrl);
            yield return www;

            DisplaySprite.sprite = Sprite.Create(
                www.texture, 
                new Rect(0, 0, 400, 300), 
                new Vector2(0.5f, 0.5f)
            );

            StartCoroutine(download(article.voicePath));
        }

		audioTime = 0.0f;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(download("test01.wav"));
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
        public string voicePath;
    }
}
