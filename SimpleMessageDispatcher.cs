using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMessageDispatcher
{
    private static readonly SimpleMessageDispatcher instance = new SimpleMessageDispatcher();

    public ConsoleMsgRecived msgRecived;

    private List<ConsoleMsg> msgPool;

    public static SimpleMessageDispatcher Instance
    {
        get { return instance; }
    }

    public SimpleMessageDispatcher()
    {
        msgPool = new List<ConsoleMsg>();
    }

    public void ConsoleMsgUpdate()
    {
        for(int i=0;i<msgPool.Count;i++)
        {
            if (msgPool[i].DispatchTime <= Time.time && msgPool[i].DispatchTime != 0) //delay到了
            {
                if (msgRecived != null)
                {
                    msgRecived(msgPool[i].MsgID, msgPool[i].ExtraInfo);
                }

                msgPool[i].DispatchTime = 0;
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
            ConsoleMsg msg = getMsg();
            msg.DispatchTime = CurrentTime;
            msg.MsgID = msgID;
            msg.ExtraInfo = ExtraInfo;

        }
    }

    public ConsoleMsg getMsg()
    {
        ConsoleMsg result = null;
        if(msgPool.Count == 0)
        {
            result = new ConsoleMsg();
            msgPool.Add(result);
            return result;
        }
        List<ConsoleMsg>.Enumerator tor = msgPool.GetEnumerator();
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
            msgPool.Add(result);
        }
        msgPool.Sort(CompareByDispatchTime);

        return result;
    }

    private int CompareByDispatchTime(ConsoleMsg x, ConsoleMsg y)
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

}

public delegate void ConsoleMsgRecived(int MsgID, params object[] ExtraInfo);

public class Telegram
{
    /// <summary> 检测两个消息包是否重复的最小延时差值 </summary>
    private const double SmallestDelay = 0.25f;

    private long m_Sender;
    /// <summary> 发送者id </summary>
    public long Sender
    {
        get { return m_Sender; }
        set { m_Sender = value; }
    }

    private long m_Receiver;
    /// <summary> 接收者id </summary>
    public long Receiver
    {
        get { return m_Receiver; }
        set { m_Receiver = value; }
    }

    private int m_Msg;
    /// <summary> 消息id </summary>
    public int Msg
    {
        get { return m_Msg; }
        set { m_Msg = value; }
    }

    private float m_DispatchTime;
    /// <summary> 消息延时 </summary>
    public float DispatchTime
    {
        get { return m_DispatchTime; }
        set { m_DispatchTime = value; }
    }

    private object[] m_ExtraInfo;
    /// <summary> 参数表 </summary>
    public object[] ExtraInfo
    {
        get { return m_ExtraInfo; }
        set { m_ExtraInfo = value; }
    }

    public Telegram()
    {
        m_DispatchTime = -1;
        m_Sender = -1;
        m_Receiver = -1;
        m_Msg = -1;
    }
    public Telegram(float time, long sender, long receiver, int msg, params object[] info)
    {
        Set(time, sender, receiver, msg, info);
    }

    public void Set(float time, long sender, long receiver, int msg, params object[] info)
    {
        m_DispatchTime = time;
        m_Sender = sender;
        m_Receiver = receiver;
        m_Msg = msg;
        m_ExtraInfo = info;
    }
    public bool Equals(Telegram obj)
    {
        bool result = false;

        if (obj != null)
        {
            float diff = m_DispatchTime - obj.DispatchTime;
            if(diff < SmallestDelay)
            {
                if(m_Sender == obj.Sender)
                {
                    if(m_Receiver == obj.m_Receiver)
                    {
                        if(m_Msg == obj.Msg)
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

public class ConsoleMsg
{
    private const double SmallestDelay = 0.25;

    private int m_MsgID;
    public int MsgID
    {
        get { return m_MsgID; }
        set { m_MsgID = value; }
    }

    private float m_DispatchTime;
    public float DispatchTime
    {
        get { return m_DispatchTime; }
        set { m_DispatchTime = value; }
    }

    private object[] m_ExtraInfo;
    public object[] ExtraInfo
    {
        get { return m_ExtraInfo; }
        set { m_ExtraInfo = value; }
    }

    public ConsoleMsg()
    {
        m_DispatchTime = -1;
        m_MsgID = -1;
    }

    public ConsoleMsg(float time, int msg, params object[] info)
    {
        Set(time, msg, info);
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
