using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance = null;

    public static T Instance
    {
        get
        {
            m_instance = GameObject.FindObjectOfType<T>();

            if (m_instance == null)
            {
                GameObject obj = new GameObject("Singleton " + typeof(T).ToString());
                T instance = obj.AddComponent<T>();
                m_instance = instance;
            }

            return m_instance;
        }
    }
}
