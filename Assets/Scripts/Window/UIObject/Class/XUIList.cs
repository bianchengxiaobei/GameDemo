using UnityEngine;
using System.Collections.Generic;
using System;
using Game.Common;
[AddComponentMenu("XUI/XUIList")]
public class XUIList : XUIObject, IXUIObject,IXUIList
{
    public GameObject m_prefabListItem;
    public bool m_bMultiSelect;
    private bool m_bHasAddItem;
    private List<XUIListItem> m_listXUIListItem = new List<XUIListItem>();
    private List<XUIListItem> m_listXUIListItemSelected = new List<XUIListItem>();
    private ListSelectEventHandler m_eventhandlerOnSelect;
    private ListSelectEventHandler m_eventhandlerOnUnSelect;
    private ListOnClickEventHandler m_eventhandlerOnClick;
    private UIGrid m_uiGrid;
    private UITable m_uiTable;
    public int Count 
    {
        get
        {
            if (null == this.m_listXUIListItem)
            {
                return 0;
            }
            return this.m_listXUIListItem.Count;
        }
    }

    public override void Init()
    {
        base.Init();
        this.m_listXUIListItem.Clear();
        this.m_listXUIListItemSelected.Clear();
        for (int i = 0; i < base.transform.childCount; i++)
        {
            Transform child = base.transform.GetChild(i);
            XUIListItem component = child.GetComponent<XUIListItem>();
            if (component == null)
            {
                Debug.LogError("null == uiListitem");
            }
            else 
            {
                component.Parent = this;
                this.m_listXUIListItem.Add(component);
            }
            this.m_listXUIListItem.Sort(new Comparison<XUIListItem>(XUIList.SortByName));
            int num = 0;
            foreach (XUIListItem current in this.m_listXUIListItem)
            {
                current.name = string.Format("{0:0000}", num);
                current.Index = num;
                current.Id = num;
                if (!current.IsInited)
                {
                    current.Init();
                }
                UIToggle component2 = current.GetComponent<UIToggle>();
                if (current.IsSelected)
                {
                    this.m_listXUIListItemSelected.Add(current);
                }
                num++;
            }
            this.m_uiGrid = base.GetComponent<UIGrid>();
            if (null != this.m_uiGrid)
            {
                this.m_uiGrid.Reposition();
            }
            this.m_uiTable = base.GetComponent<UITable>();
            if (null != this.m_uiTable)
            {
                this.m_uiTable.Reposition();
            }
        }
    }
    public int GetSelectedIndex()
    {
        if (this.m_listXUIListItemSelected.Count > 0)
        {
            return this.m_listXUIListItemSelected[0].Index;
        }
        return -1;
    }
    public void SetSelectedIndex(int nIndex)
    {
        XUIListItem selectedItem = this.GetItemByIndex(nIndex) as XUIListItem;
        this.SetSelectedItem(selectedItem);
        //this.FocusIndex(nIndex);
    }
    public void SetSelectedItemByIndex(List<int> listItemIndex)
    {
        if (this.m_bMultiSelect)
        {
            this.m_listXUIListItemSelected.Clear();
            foreach (XUIListItem current in this.m_listXUIListItem)
            {
                if (listItemIndex != null && listItemIndex.Contains(current.Index))
                {
                    current.SetSelected(true);
                    this.m_listXUIListItemSelected.Add(current);
                }
                else
                {
                    current.SetSelected(false);
                }
            }
        }
        else
        {
            Debug.LogError("false == m_bMultiSelect");
        }
    }
    public void SetSelectedItemById(int unId)
    {
        XUIListItem selectedItem = this.GetItemById(unId) as XUIListItem;
        this.SetSelectedItem(selectedItem);
    }
    public void SetSelectedItemById(List<int> listItemId)
    {
        if (this.m_bMultiSelect)
        {
            this.m_listXUIListItemSelected.Clear();
            foreach (XUIListItem current in this.m_listXUIListItem)
            {
                if (listItemId != null && listItemId.Contains(current.Id))
                {
                    current.SetSelected(true);
                    this.m_listXUIListItemSelected.Add(current);
                }
                else
                {
                    current.SetSelected(false);
                }
            }
        }
        else
        {
            Debug.LogError("false == m_bMultiSelect");
        }
    }
    public void SetSelectedItem(XUIListItem uiListItem)
    {
        foreach (XUIListItem current in this.m_listXUIListItemSelected)
        {
            current.SetSelected(false);
        }
        this.m_listXUIListItemSelected.Clear();
        if (null != uiListItem)
        {
            uiListItem.SetSelected(true);
            this.m_listXUIListItemSelected.Add(uiListItem);
        }
    }
    public IXUIListItem GetItemByIndex(int nIndex)
    {
        if (this.m_listXUIListItem == null)
        {
            return null;
        }
        if (0 > nIndex || nIndex >= this.m_listXUIListItem.Count)
        {
            return null;
        }
        return this.m_listXUIListItem[nIndex];
    }
    public IXUIListItem GetSelectedItem()
    {
        if (this.m_listXUIListItemSelected.Count > 0)
        {
            return this.m_listXUIListItemSelected[0];
        }
        return null;
    }
    public IXUIListItem[] GetSelectedItems()
    {
        return this.m_listXUIListItemSelected.ToArray();
    }
    public IXUIListItem GetItemById(int unId)
    {
        foreach (XUIListItem current in this.m_listXUIListItem)
        {
            if (unId == current.Id)
            {
                return current;
            }
        }
        return null;
    }
    public IXUIListItem GetItemById(int unId, bool bVisible)
    {
        foreach (XUIListItem current in this.m_listXUIListItem)
        {
            if (unId == current.Id && current.IsVisible() == bVisible)
            {
                return current;
            }
        }
        return null;
    }
    public IXUIListItem GetItemByGUID(long ulId)
    {
        foreach (XUIListItem current in this.m_listXUIListItem)
        {
            if (ulId == current.GUID)
            {
                return current;
            }
        }
        return null;
    }
    public IXUIListItem[] GetAllItems()
    {
        return this.m_listXUIListItem.ToArray();
    }
    public IXUIListItem AddListItem(GameObject obj)
    {
        if (null == obj)
        {
            return null;
        }
        GameObject gameObject = UnityEngine.Object.Instantiate(obj) as GameObject;
        gameObject.name = string.Format("{0:0000}", this.Count);
        gameObject.transform.parent = base.transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.localRotation = Quaternion.identity;
        NGUITools.SetLayer(gameObject, this.CacheGameObject.layer);
        UIToggle component = gameObject.GetComponent<UIToggle>();
        XUIListItem xUIListItem = gameObject.GetComponent<XUIListItem>();
        if (null == xUIListItem)
        {
            //Debug.LogError("null == uiListItem");
            xUIListItem = gameObject.AddComponent<XUIListItem>();
        }
        xUIListItem.Index = this.Count;
        xUIListItem.Id = xUIListItem.Index;
        xUIListItem.Parent = this;
        if (!xUIListItem.IsInited)
        {
            xUIListItem.Init();
        }
        this.m_listXUIListItem.Add(xUIListItem);
        this.m_bHasAddItem = true;
        this.Refresh();
        return xUIListItem;
    }
    public IXUIListItem AddListItem()
    {
        if (null != this.m_prefabListItem)
        {
            return this.AddListItem(this.m_prefabListItem);
        }
        return null;
    }
    public bool DelItemById(int unId)
    {
        IXUIListItem itemById = this.GetItemById(unId);
        return this.DelItem(itemById);
    }
    public bool DelItemByIndex(int nIndex)
    {
        IXUIListItem itemByIndex = this.GetItemByIndex(nIndex);
        return this.DelItem(itemByIndex);
    }
    public bool DelItem(IXUIListItem iUIListItem)
    {
        XUIListItem xUIListItem = iUIListItem as XUIListItem;
        if (null == xUIListItem)
        {
            return false;
        }
        this.m_listXUIListItemSelected.Remove(xUIListItem);
        int index = xUIListItem.Index;
        for (int i = index + 1; i < this.Count; i++)
        {
            this.m_listXUIListItem[i].name = string.Format("{0:0000}", i - 1);
            this.m_listXUIListItem[i].Index = i - 1;
        }
        this.m_listXUIListItem.Remove(xUIListItem);
        xUIListItem.gameObject.transform.parent = null;
        UnityEngine.Object.Destroy(xUIListItem.gameObject);
        this.Refresh();
        return true;
    }
    public void Clear()
    {
        if (this.m_listXUIListItem == null)
        {
            return;
        }
        foreach (XUIListItem current in this.m_listXUIListItem)
        {
            current.gameObject.transform.parent = null;
            UnityEngine.Object.Destroy(current.gameObject);
        }
        this.m_listXUIListItemSelected.Clear();
        this.m_listXUIListItem.Clear();
        this.Refresh();
    }
    public void SetEnable(bool bEnable)
    {
        foreach (XUIListItem current in this.m_listXUIListItem)
        {
            current.SetEnable(bEnable);
        }
    }
    public void SetEnableSelect(bool bEnable)
    {
        foreach (XUIListItem current in this.m_listXUIListItem)
        {
            current.SetEnableSelect(bEnable);
        }
        if (!bEnable)
        {
            this.m_listXUIListItemSelected.Clear();
        }
    }
    public void SetEnableSelect(List<int> listIds)
    {
        if (listIds == null)
        {
            return;
        }
        foreach (XUIListItem current in this.m_listXUIListItem)
        {
            if (listIds.Contains(current.Id))
            {
                current.SetEnableSelect(true);
            }
            else
            {
                current.SetEnableSelect(false);
            }
        }
        this.m_listXUIListItemSelected.RemoveAll((XUIListItem x) => !listIds.Contains(x.Id));
    }
    public override void Highlight(bool bTrue)
    {
        foreach (XUIListItem current in this.m_listXUIListItem)
        {
            current.Highlight(bTrue);
        }
    }
    public void Highlight(List<int> listIds)
    {
        this.Highlight(false);
        if (listIds == null)
        {
            return;
        }
        foreach (XUIListItem current in this.m_listXUIListItem)
        {
            if (listIds.Contains(current.Id))
            {
                current.Highlight(true);
            }
        }
    }
    public void Refresh()
    {
        if (null != this.m_uiGrid)
        {
            this.m_uiGrid.repositionNow = true;
        }
        if (null != this.m_uiTable)
        {
            this.m_uiTable.repositionNow = true;
        }
        this.RefreshAllItemStatus();
    }
    public void OnSelectItem(XUIListItem listItem)
    {
        this.SelectItem(listItem, true);
    }
    public void SelectItem(XUIListItem listItem, bool bTrigerEvent)
    {
        if (null == listItem)
        {
            return;
        }
        if (this.m_listXUIListItemSelected.Contains(listItem))
        {
            return;
        }
        if (!this.m_bMultiSelect)
        {
            this.m_listXUIListItemSelected.Clear();
        }
        this.m_listXUIListItemSelected.Add(listItem);
        //触发选择委托
        if (bTrigerEvent && this.m_eventhandlerOnSelect != null)
        {
            this.m_eventhandlerOnSelect(listItem);
        }
    }
    public void OnUnSelectItem(XUIListItem listItem)
    {
        this.UnSelectItem(listItem, true);
    }
    public void UnSelectItem(XUIListItem listItem, bool bTrigerEvent)
    {
        if (null == listItem)
        {
            return;
        }
        if (!this.m_listXUIListItemSelected.Contains(listItem))
        {
            return;
        }
        this.m_listXUIListItemSelected.Remove(listItem);
        //不选择委托
        if (bTrigerEvent && this.m_eventhandlerOnUnSelect != null)
        {
            this.m_eventhandlerOnUnSelect(listItem);
        }
    }
    public void RegisterListOnSelectEventHandler(ListSelectEventHandler handler)
    {
        this.m_eventhandlerOnSelect = handler;
    }
    public void RegisterListOnUnSelectEventHandler(ListSelectEventHandler handler)
    {
        this.m_eventhandlerOnUnSelect = handler;
    }
    public void RegisterListOnClickEventHandler(ListOnClickEventHandler handler)
    {
        this.m_eventhandlerOnClick = handler;
    }
    public void _OnClick(XUIListItem item)
    {
        if (this.GetSelectedIndex() != item.Index)
        {
            this.SelectItem(item, false);
        }
        if (this.m_eventhandlerOnClick != null)
        {
            this.m_eventhandlerOnClick(item);
        }
    }
    public static int SortByName(XUIListItem a, XUIListItem b)
    {
        return string.Compare(a.name, b.name);
    }
    private void RefreshAllItemStatus()
    {
        int num = this.GetSelectedIndex();
        if (num < 0)
        {
            this.SetSelectedIndex(0);
        }
    }
}
