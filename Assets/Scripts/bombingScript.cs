using UnityEngine;
using System.Collections;

public class bombingScript : MonoBehaviour {

	float startX;
	float bombDelay;

	public controller c;

	// Use this for initialization
	void Start () {
		startX = transform.position.x;
		bombDelay = 0.15f;
		c = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<controller> ();
	}

	public void Update()
	{
		bombDelay -= Time.deltaTime;
	}

	public void FixedUpdate()
	{
		transform.position -= new Vector3 (0.2f, 0, 0);
		if (transform.position.x <= startX - 25) {
			Destroy (gameObject);
		}
		if (bombDelay < 0) {
			c.bomb (new Vector2(transform.position.x,transform.position.y) + Random.insideUnitCircle * 4);
			bombDelay = 0.15f;
		}
	}
}
