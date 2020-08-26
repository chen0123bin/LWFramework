using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using System.Threading;

public class TestUniTask : MonoBehaviour
{
    CancellationTokenSource cts;
    // Start is called before the first frame update
    void Start()
    {
        cts = new CancellationTokenSource();
        _ = TaskUpdateAsync();
       
    }

    async UniTaskVoid TaskUpdateAsync() {
        while (true) {
            await UniTask.Delay(2000, cancellationToken:cts.Token);
            string[] strArry = new string[] { "111", "222" };
            for (int i = 0; i < strArry.Length; i++)
            {
                Debug.Log(strArry[i]);
            }
           
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            cts.Cancel();
            cts.Dispose();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            cts = new CancellationTokenSource();
            _ = TaskUpdateAsync();
        }
    }

}
