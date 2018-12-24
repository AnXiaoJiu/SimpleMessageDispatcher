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
                    Debug.Log("��չ��Ϣ:" + (int)msg.ExtraInfo[0]);
                }
                break;
            default:
                Debug.Log("Ĭ����Ϣ //��߿��������������,������״̬�����������");
                break;
        }
    }
}
