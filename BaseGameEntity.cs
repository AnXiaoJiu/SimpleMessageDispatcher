using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameEntity : MonoBehaviour
{
    public int m_ID;

    public virtual void HandleMessage(object gameEntityMsg) {  }
}
