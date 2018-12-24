using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMain : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {

        BaseGameEntity entity =  FindObjectOfType<Player>();
        SimpleMessageDispatcher.Instance.RegisterEntity(entity);

        int num = 1;
        while (num < 10)
        {
            SimpleMessageDispatcher.Instance.SendMessage(num, (int)ConsoleMsgType.Common, num);
            SimpleMessageDispatcher.Instance.SendMessage(num, entity.m_ID, entity.m_ID,(int)GameEntityMsgType.EntityEx, num);
            num++;
        }
    }
	
    private void Update()
    {
        SimpleMessageDispatcher.Instance.ConsoleMsgUpdate();

        SimpleMessageDispatcher.Instance.GameEntityMsgUpdate();
    }
}
