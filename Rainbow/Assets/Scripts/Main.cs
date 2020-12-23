using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance;
    App app;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        app = new App();
        app.Init();
        app.GameStart();
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
