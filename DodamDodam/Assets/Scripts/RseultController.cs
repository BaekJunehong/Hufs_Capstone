using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultController : MonoBehaviour
{
    public TextMeshProUGUI score_text;
    public TextMeshProUGUI error_text;
    // Start is called before the first frame update
    void Start()
    {
        score_text.text = Timer.T.ToString("N2");
        error_text.text = ErrorCount.errorCount.ToString("0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}