using UnityEngine;
using System.Collections;

public class gatlingScript : MonoBehaviour {

    float life;
	public bool player;

	// Use this for initialization
	void Start () {
        life = 120;
		if (!player) {
			transform.rotation = Quaternion.Euler (0, 0, 180);
		}
	}
	
	// Update is called once per frame
	void Update () {
        life -= Time.deltaTime;
        if(life<=0)
        {
            Destroy(gameObject);
        }

        Transform health = transform.GetChild(0);
        health.localScale = new Vector3((64 * life / 120), 8, 1);
        health.localPosition = new Vector3(life / 240 - 0.5f, -0.515f, -0.5f);
    }
}
