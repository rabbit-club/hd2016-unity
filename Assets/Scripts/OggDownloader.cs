using UnityEngine;
using System.IO;
using System.Collections;

public class OggDownloader : MonoBehaviour {

	string urlBase = "http://210.140.161.190:3000/";
	public string filePath = "";

	void Start () {
	}

	public void downloadButton() {
		StartCoroutine(download("test01.ogg"));
	}

	IEnumerator download(string fileName) {
		WWW www = new WWW(urlBase + "/" + fileName);

		while (!www.isDone) { // ダウンロードの進捗を表示
			print(Mathf.CeilToInt(www.progress*100));
			yield return null;
		}

		if (!string.IsNullOrEmpty(www.error)) { // ダウンロードでエラーが発生した
			Debug.Log("error:" + www.error);
		} else { // ダウンロードが正常に完了した
			Debug.Log("download success.");
			filePath = Application.persistentDataPath + "/" + Path.GetFileName (www.url);
			File.WriteAllBytes(filePath, www.bytes);
			Debug.Log("download file write success." + filePath);
		}
	}
}
