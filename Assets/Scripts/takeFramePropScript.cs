using UnityEngine;
using System.Collections;

public class takeFramePropScript : MonoBehaviour {

    int completion;
    controller c;
	// Use this for initialization
	void Start () {
        completion = 0;
        c = Camera.main.gameObject.GetComponent<controller>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        bool inside = false;
        foreach (GameObject s in c.soldiers)
        {
            float xDiff = s.transform.position.x - transform.position.x;
            float yDiff = s.transform.position.y - transform.position.y;
            if (xDiff>=-4.1f && xDiff<= 4.1f && yDiff>=-4.1f && yDiff<= 4.1f)
            {
                inside = true;
            }
        }
        if(inside)
        {
            completion++;
        }
        else
        {
            if(completion>0)
            {
                completion--;
            }
        }
        Transform health = transform.GetChild(0);
        health.localScale = new Vector3(512.0f * (completion / 300.0f), 40, 1);
        health.localPosition = new Vector3(4.0f*(completion / 300.0f)-4.0f, 4.5f, 0);
        if (completion >= 300)
        {
            c.completeEvent();
        }
    }
}
