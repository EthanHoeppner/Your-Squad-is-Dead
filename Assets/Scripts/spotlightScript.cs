using UnityEngine;
using System.Collections;

public class spotlightScript : MonoBehaviour {

    public float a;
    public float va;
    public float life;
	public bool player;

	// Use this for initialization
	void Start () {
        a = 0;
        va =0.25f;
        life = 120;
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Euler(0, 0, a-(player?0:180));
        life -= Time.deltaTime;
        if(life<=0)
        {
            Destroy(gameObject);
        }
        Transform health = transform.GetChild(0);
        health.localScale = new Vector3(64*life / 120, 8, 1);
        health.localPosition = new Vector3(life/240-0.5f, -0.515f, -0.5f);
    }
    void FixedUpdate()
    {
        a += va;
        if(a>=40)
        {
            va = -0.25f;
        }
        if(a<=-40)
        {
            va = 0.25f;
        }

    }
}
