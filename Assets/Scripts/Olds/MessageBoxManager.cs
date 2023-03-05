using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBoxManager : MonoBehaviour
{
    [SerializeField] Canvas messageManager;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text message;

    void Create(Canvas messageManager, string title, string message)
    {
        this.messageManager = messageManager;
        this.title.text = title;
        this.message.text = message;
    }

    void Close()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
