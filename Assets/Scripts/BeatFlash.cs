using UnityEngine;
using System.Collections;

public class BeatFlash : MonoBehaviour {
    private MicrophoneSource mMicSource;

    public float baseScale = 0.5f;
    public float scale = 5.0f;
    public float damping = 0.95f;

    private float mBeat;

    private Color mDefColor;

	// Use this for initialization
	void Start () {
        mMicSource = GameObject.Find("MicrophoneSourceObject").GetComponent<MicrophoneSource>();
        Renderer rend = GetComponent<Renderer>();
        mDefColor = rend.material.GetColor("_Color");
	}
	
	// Update is called once per frame
	void Update () {
        float vol = mMicSource.Beat() * scale;
        mBeat = Mathf.Max(mBeat * damping, vol);

        Renderer rend = GetComponent<Renderer>();
        rend.material.SetColor("_Color", Color.Lerp(mDefColor, Color.white, mBeat));
	}
}
