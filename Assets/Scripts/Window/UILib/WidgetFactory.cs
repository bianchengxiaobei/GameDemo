using UnityEngine;
using System.Collections.Generic;
namespace UILib
{
    public class WidgetFactory
    {
        /// <summary>
        /// 查找节点下所有的UI组件，缓存到字典里面
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="parent"></param>
        /// <param name="dicAllUIObjects"></param>
        public static void FindAllUIObjects(Transform trans, IXUIObject parent, ref Dictionary<string, XUIObjectBase> dicAllUIObjects)
        {
            int i = 0;
            while (i < trans.childCount)
            {
                Transform child = trans.GetChild(i);
                XUIObjectBase component = child.GetComponent<XUIObjectBase>();
                if (!(null != component))
                {
                    goto IL_85;
                }
                if (component.GetType().GetInterface("IXUIListItem") == null)
                {
                    if (dicAllUIObjects.ContainsKey(component.name))
                    {
                        Debug.Log(component.name);
                        Debug.LogError("m_dicId2UIObject.ContainsKey:" + WidgetFactory.GetUIObjectId(component));
                    }
                    dicAllUIObjects[component.name] = component;
                    component.Parent = parent;
                    goto IL_85;
                }
                else 
                {
                    Debug.Log("fdsfd");
                }
            IL_8F:
                i++;
                continue;
            IL_85:
                WidgetFactory.FindAllUIObjects(child, parent, ref dicAllUIObjects);
                goto IL_8F;
            }
        }
        /// <summary>
        /// 取得该组件所在的id（包含父亲节点的id）
        /// </summary>
        /// <param name="uiObject"></param>
        /// <returns></returns>
        public static string GetUIObjectId(IXUIObject uiObject)
        {
            string result;
            if (null == uiObject)
            {
                result = string.Empty;
            }
            else
            {
                string text = uiObject.CacheGameObject.name;
                IXUIListItem iXUIListItem = uiObject as IXUIListItem;
                if (iXUIListItem != null)
                {
                    text = iXUIListItem.Id.ToString();
                }
                while (null != uiObject.Parent)
                {
                    uiObject = uiObject.Parent;
                    string arg = uiObject.CacheGameObject.name;
                    iXUIListItem = (uiObject as IXUIListItem);
                    if (null != iXUIListItem)
                    {
                        arg = iXUIListItem.Id.ToString();
                    }
                    text = string.Format("{0}#{1}", arg, text);
                }
                result = text;
            }
            return result;
        }
    }
}
