using System;
using System.Threading;
using UnityEngine;
namespace Game
{
    public class Singleton<T> where T : new()
    {
        private static T s_singleton = default(T);
        private static object s_objectLock = new object();
        public static T singleton
        {
            get
            {
                if (null == Singleton<T>.s_singleton)
                {
                    object obj;
                    Monitor.Enter(obj = Singleton<T>.s_objectLock);
                    try
                    {
                        if (null == Singleton<T>.s_singleton)
                        {
                            Singleton<T>.s_singleton = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
                        }
                    }
                    finally
                    {
                        Monitor.Exit(obj);
                    }
                }
                return Singleton<T>.s_singleton;
            }
        }
        protected Singleton()
        {
        }
    }
    public class UnitySingleton<T> : MonoBehaviour
       where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;//如果激活的物体上找到这个脚本
                    //没有找到这个脚本，就自己创建一个物体，附上这个脚本
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        //obj.hide Flags = HideFlags.DontSave;
                        obj.hideFlags = HideFlags.HideAndDontSave;//设置物体不显示
                        _instance = (T)obj.AddComponent(typeof(T));
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// 加载另外一个场景的时候不要销毁这个物体
        /// </summary>
        public virtual void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
