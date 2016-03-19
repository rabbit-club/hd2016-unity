using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class UnityAdsScript : MonoBehaviour {

	[SerializeField] string iosGameId;
	[SerializeField] string androidGameId;
	[SerializeField] bool enableTestMode;

	public void ShowAd () {
		if (Advertisement.IsReady ()) {
			Advertisement.Show ();
		}

//		if (Advertisement.IsReady ()) {
//			Debug.Log("ShowAds");
//			Advertisement.Show("rewardedVideoZone", new ShowOptions {
//				pause = true,
//				resultCallback = result =>
//				{
//					switch (result)
//					{
//					case ShowResult.Finished:
//						Debug.Log("The ad was successfully shown.");
//						break;
//					case ShowResult.Skipped:
//						Debug.Log("The ad was skipped before reaching the end.");
//						break;
//					case ShowResult.Failed:
//						Debug.LogError("The ad failed to be shown.");
//						break;
//					}
//				}
//			} );
//		}
	}
}