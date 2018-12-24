using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Start()
    {
        SimpleMessageDispatcher.Instance.msgRecived = ConsoleMsgRecived;

        int num = 1;
        while(num<10)
        {
            SimpleMessageDispatcher.Instance.SendMessage(num, (int)ConsoleMsgType.Common, num);
            num++;
        }
    }

  
    private void Update()
    {
        SimpleMessageDispatcher.Instance.ConsoleMsgUpdate();
    }

    public void ConsoleMsgRecived(int MsgID, params object[] ExtraInfo)
    {
        switch((ConsoleMsgType)MsgID)
        {
            case ConsoleMsgType.Common:
                int a =  (int)ExtraInfo[0];
                Debug.Log(a);
                break;
        }
    }

}
