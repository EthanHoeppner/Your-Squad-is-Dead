using UnityEngine;
using System.Collections;

public class artilleryExplosionEffectScript : MonoBehaviour {

	float life;
	public float size=3;

	// Use this for initialization
	void Start () {
		life = 0.6f;
	}
	
	// Update is called once per frame
	void Update () {
		life -= Time.deltaTime;
		if (life <= 0) {
			Destroy (gameObject);
		}
	}
	void FixedUpdate()
	{
		transform.localScale = new Vector3 (Random.Range (2*size-0.4f,2*size+0.4f), Random.Range (2*size-0.4f,2*size+0.4f), 1);
		GetComponent<SpriteRenderer> ().color = new Color (Random.Range (0.77f, 0.89f), Random.Range (0.29f, 0.41f), Random.Range (0, 0.08f),0.5f);
	}
}
