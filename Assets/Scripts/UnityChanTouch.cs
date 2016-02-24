using UnityEngine;
using System.Collections;
using System.Linq;

public class UnityChanTouch : TapBehaviour {

	Animator anim;
	SkinnedMeshRenderer skinMesh;
	float mouseTimer = 0.0f;
	bool mouseClose = true;
	public bool useLip = false;
	bool nowAnimation = false;

	void Start() {
		anim = GetComponent<Animator>();
		skinMesh = this.GetComponentsInChildren<SkinnedMeshRenderer>().First(s => s.name == "MTH_DEF");
//		useLip = true; //デバッグ用
	}

	// タッチしたときに呼ばれる。
	public override void TapDown(ref RaycastHit hit){
		anim.Rebind();
		nowAnimation = true;
		int animIndex = Random.Range(0, 5); // intの場合max値は含まない
		switch (animIndex) {
		case 0:
			anim.Play("WIN00");
			break;
		case 1:
			anim.Play("WAIT01");
			break;
		case 2:
			anim.Play("WAIT02");
			break;
		case 3:
			anim.Play("WAIT03");
			break;
		case 4:
			anim.Play("WAIT04");
			break;
		default:
			break;
		}
	}
	
	void Update() {
		if(anim != null) {
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("FIRST_WAIT")) {
				// 初めのポーズを取るため一瞬動かしてからアニメーションをストップさせ口パクに譲る
				if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f) {
					anim.Stop();
				}
			} else if(anim.GetCurrentAnimatorStateInfo(0).IsName("WAIT00")) {
				if(nowAnimation) {
					anim.Stop();
					nowAnimation = false;
				}
			}
		}
		if(useLip) {
			// 口パク
			mouseTimer += Time.deltaTime;
			if (mouseTimer > 0.1f) {
				paku();
				mouseTimer = 0;
			}
		}
	}

	void paku() {
		if (mouseClose) {
			skinMesh.SetBlendShapeWeight(0, 0.0f);
		} else {
			skinMesh.SetBlendShapeWeight(0, 50.0f);
		}
		mouseClose = !mouseClose;
	}

}
