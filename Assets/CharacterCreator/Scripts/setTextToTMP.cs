using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class setTextToTMP : MonoBehaviour
{
    public GameObject headingGameObject;
    public TMP_Text textGameObject;
    
    // Start is called before the first frame update
    void Start()
    {
        textGameObject.text = headingGameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
