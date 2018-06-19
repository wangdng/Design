using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SkillCD : MonoBehaviour
{
    public float cdTime = 3.0f;

    private float cdDelta;

    Image image;

    bool isStartCD = false;

	void Start ()
    {
        image = GetComponent<Image>();
        this.gameObject.SetActive(false);
    }

    public void StartCD()
    {
        isStartCD = true;
        this.gameObject.SetActive(true);

        cdDelta = cdTime;
    }

    public bool isInCD
    {
        get
        {
            return this.gameObject.activeSelf;
        }
    }
	
	void Update ()
    {
        if (isStartCD)
        {
            cdDelta -= Time.deltaTime;

            image.fillAmount = cdDelta / cdTime;

            if (image.fillAmount <=0)
            {
                this.gameObject.SetActive(false);

                isStartCD = false;

                cdDelta = cdTime;
            }
        }
    }
}
