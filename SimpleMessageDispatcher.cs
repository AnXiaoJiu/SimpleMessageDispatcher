using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMessageDispatcher
{
    private static readonly SimpleMessageDispatcher instance = new SimpleMessageDispatcher();

    public ConsoleMsgRecived msgRecived;

    /// <summary>
    /// 消息池
    /// </summary>
    private List<ConsoleMsg> consoleMsgPool;

    /// <summary>
    /// 对象集合
    /// </summary>
    private Dictionary<long, BaseGameEntity> m_dictEntities;

    private List<GameEntityMsg> gameEntityMsgPool;

    public static SimpleMessageDispatcher Instance
    {
        get { return instance; }
    }

    public SimpleMessageDispatcher()
    {
        consoleMsgPool = new List<ConsoleMsg>();

        gameEntityMsgPool = new List<GameEntityMsg>();

        m_dictEntities = new Dictionary<long, BaseGameEntity>();
    }

    #region 控制台消息

    public void ConsoleMsgUpdate()
    {
        for(int i=0;i<consoleMsgPool.Count;i++)
        {
            if (consoleMsgPool[i].DispatchTime <= Time.time && consoleMsgPool[i].DispatchTime != 0) //delay到了
            {
                if (msgRecived != null)
                {
                    msgRecived(consoleMsgPool[i].MsgID, consoleMsgPool[i].ExtraInfo);
                }

                consoleMsgPool[i].DispatchTime = 0;
            }
        }
    }

    public void SendMessage(float delay, int msgID, params object[] ExtraInfo)
    {
        if(msgRecived == null)
        {
            return;
        }

        if(delay <= 0)
        {
            msgRecived(msgID, ExtraInfo);
        }
        else
        {
            float CurrentTime = Time.time;
            CurrentTime += delay;
            ConsoleMsg msg = getConsoleMsg();    //ConsoleMsg msg = new ConsoleMsg();
            msg.Set(CurrentTime, msgID, ExtraInfo);
        }
    }

    private ConsoleMsg getConsoleMsg()
    {
        ConsoleMsg result = null;
        if(consoleMsgPool.Count == 0)
        {
            result = new ConsoleMsg();
            consoleMsgPool.Add(result);
            return result;
        }
        List<ConsoleMsg>.Enumerator tor = consoleMsgPool.GetEnumerator();
        while(tor.MoveNext())
        {
           if( tor.Current.DispatchTime ==0 )
            {
                result = tor.Current;
                break;
            }
        }
        if(result == null)
        {
            result = new ConsoleMsg();
            consoleMsgPool.Add(result);
        }
        consoleMsgPool.Sort(CompareByDispatchTime);

        return result;
    }


    #endregion

    #region 对象实体消息

    public void SendMessage(float delay,int senderID, int receiverID,int msgID, params object[] ExtraInfo)
    {
        BaseGameEntity receiver = getRegisteredEntityByID(receiverID);

        GameEntityMsg msg = getGameEntityMsg();
        if (delay <= 0.0f)
        {
            receiver.HandleMessage(msg);
        }
        else
        {
            float CurrentTime = Time.time;
            CurrentTime += delay;
            msg.DispatchTime = CurrentTime;
        }

        msg.Set(delay, senderID, receiverID, msgID, ExtraInfo);
    }

    private GameEntityMsg getGameEntityMsg()
    {
        GameEntityMsg result = null;

        if(gameEntityMsgPool.Count == 0)
        {
            result = new GameEntityMsg();
            gameEntityMsgPool.Add(result);
            return result;
        }

        List<GameEntityMsg>.Enumerator tor = gameEntityMsgPool.GetEnumerator();
        while (tor.MoveNext())
        {
            if (tor.Current.DispatchTime == 0)
            {
                result = tor.Current;
                break;
            }
        }

        if (result == null)
        {
            result = new GameEntityMsg();
            gameEntityMsgPool.Add(result);
        }
        gameEntityMsgPool.Sort(CompareByDispatchTime);

        return result;
    }

    public BaseGameEntity getRegisteredEntityByID(long id)
    {
        try
        {
            return m_dictEntities[id];
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError(String.Format("Key of id {0} not registered!", id));
        }

        return null;
    }


    public void GameEntityMsgUpdate()
    {
        for (int i = 0; i < gameEntityMsgPool.Count; i++)
        {
            if (gameEntityMsgPool[i].DispatchTime <= Time.time && gameEntityMsgPool[i].DispatchTime != 0) //delay到了
            {
                BaseGameEntity receiver = getRegisteredEntityByID(gameEntityMsgPool[i].receiverID);
                receiver.HandleMessage(gameEntityMsgPool[i]);
                gameEntityMsgPool[i].DispatchTime = 0;
            }
        }
    }


    #endregion

    private int CompareByDispatchTime(Msg x, Msg y)
    {
        if (x == y)
        {
            return 0;
        }
        else if (x.DispatchTime > y.DispatchTime)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public void RegisterEntity(BaseGameEntity NewEntity)
    {
        m_dictEntities[NewEntity.m_ID] = NewEntity;
    }

}

public delegate void ConsoleMsgRecived(int MsgID, params object[] ExtraInfo);


public abstract class Msg
{
    /// <summary> 检测两个消息包是否重复的最小延时差值 </summary>
    protected const double SmallestDelay = 0.25;

    protected int m_MsgID;
    public int MsgID
    {
        get { return m_MsgID; }
        set { m_MsgID = value; }
    }

    protected float m_DispatchTime;
    public float DispatchTime
    {
        get { return m_DispatchTime; }
        set { m_DispatchTime = value; }
    }

    protected object[] m_ExtraInfo;
    public object[] ExtraInfo
    {
        get { return m_ExtraInfo; }
        set { m_ExtraInfo = value; }
    }

    public Msg()
    {
        DispatchTime = -1;
        MsgID = -1;
    }


}

public class GameEntityMsg : Msg
{
   
    private int m_Sender;
    /// <summary> 发送者id </summary>
    public int senderID
    {
        get { return m_Sender; }
        set { m_Sender = value; }
    }

    private int m_Receiver;
    /// <summary> 接收者id </summary>
    public int receiverID
    {
        get { return m_Receiver; }
        set { m_Receiver = value; }
    }



    public GameEntityMsg()
    {
        m_Sender = -1;
        m_Receiver = -1;
    }

    public void Set(float time, int sender, int receiver, int msg, params object[] info)
    {
        m_DispatchTime = time;
        m_Sender = sender;
        m_Receiver = receiver;
        m_MsgID = msg;
        m_ExtraInfo = info;
    }
    public bool Equals(GameEntityMsg obj)
    {
        bool result = false;

        if (obj != null)
        {
            float diff = DispatchTime - obj.DispatchTime;
            if(diff < SmallestDelay)
            {
                if(m_Sender == obj.senderID)
                {
                    if(m_Receiver == obj.m_Receiver)
                    {
                        if(MsgID == obj.MsgID)
                        {
                            result = true;
                        }
                    }
                }
            }

        }

        return result;
    }

}

public class ConsoleMsg : Msg
{
    public ConsoleMsg():base()
    {

    }

    public void Set(float time, int msgID, params object[] info)
    {
        m_DispatchTime = time;
        m_MsgID = msgID;
        m_ExtraInfo = info;
    }

}

public enum ConsoleMsgType:int
{
    Common
}

public enum GameEntityMsgType:int
{
    EntityEx,
}
