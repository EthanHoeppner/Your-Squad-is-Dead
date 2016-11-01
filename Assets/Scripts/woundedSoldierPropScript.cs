using UnityEngine;
using System.Collections;

public class woundedSoldierPropScript : MonoBehaviour {

    controller c;
    int completion;

	// Use this for initialization
	void Start () {
        c = Camera.main.gameObject.GetComponent<controller>();
        completion = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        bool inside = false;
        foreach (GameObject s in c.soldiers)
        {
            if (Vector2.Distance(transform.position, s.transform.position) < 3)
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
        Transform child = transform.GetChild(0);
        child.localScale = new Vector3(64.0f * (completion / 100.0f), 20, 1);
        child.localPosition = new Vector3(completion / 200.0f-0.5f, 0.7f, 0);
        if(completion>=100)
        {
            c.completeEvent();
        }
    }
}
