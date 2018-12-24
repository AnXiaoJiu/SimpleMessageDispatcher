using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseGameEntity
{

    public override void HandleMessage(object gameEntityMsg)
    {
        base.HandleMessage(gameEntityMsg);


        GameEntityMsg msg = gameEntityMsg as GameEntityMsg;

        switch( (GameEntityMsgType)msg.MsgID)
        {
            case GameEntityMsgType.EntityEx:
                {
                    Debug.Log("扩展消息:" + (int)msg.ExtraInfo[0]);
                }
                break;
            default:
                Debug.Log("默认消息 //这边可以用来管理对象,比如用状态机来管理对象");
                break;
        }
    }
}
