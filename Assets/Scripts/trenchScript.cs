using UnityEngine;
using System.Collections;

public class trenchScript : MonoBehaviour {

    public int health=4;

	// Use this for initialization
	void Start ()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.5f+health / 8.0f, 0.5f+health / 8.0f, 0.5f + health / 8.0f);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void damage(int d)
    {
        health-=d;
        GetComponent<SpriteRenderer>().color = new Color(0.5f + health / 8.0f, 0.5f + health / 8.0f, 0.5f + health / 8.0f);
        if (health<=0)
        {
            GameObject.Destroy(gameObject);
            int x = (int)transform.position.x;
            int y = (int)transform.position.y;
            Camera.main.gameObject.GetComponent<controller>().trenches[x, y] = null;
            Camera.main.gameObject.GetComponent<controller>().terrain[x,y] = false;
            Camera.main.gameObject.GetComponent<controller>().enemyTerrain[x, y] = false;
        }
    }
    public void repair()
    {
        health++;
        GetComponent<SpriteRenderer>().color = new Color(0.5f + health / 8.0f, 0.5f + health / 8.0f, 0.5f + health / 8.0f);
    }
}
