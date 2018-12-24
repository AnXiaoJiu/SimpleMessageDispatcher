using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制中心
/// </summary>
public class ConsoleCenter : MonoBehaviour
{
    private void Awake()
    {
        SimpleMessageDispatcher.Instance.msgRecived = ConsoleMsgRecived;
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
