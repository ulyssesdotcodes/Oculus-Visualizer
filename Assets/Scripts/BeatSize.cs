using UnityEngine;
using System.Collections;

public class BeatSize : MonoBehaviour {
    private MicrophoneSource mMicSource;

    public float baseScale = 0.5f;
    public float scale = 5.0f;
    public float damping = 0.95f;

    private float mBeat;

	// Use this for initialization
	void Start () {
        mMicSource = GameObject.Find("MicrophoneSourceObject").GetComponent<MicrophoneSource>();
	}
	
	// Update is called once per frame
	void Update () {
        float vol = mMicSource.Beat() * scale;
        mBeat = Mathf.Max(mBeat * damping, vol);

        float transformScale = mBeat + baseScale;

        transform.localScale = new Vector3(transformScale, transformScale, transformScale);
	}
}
