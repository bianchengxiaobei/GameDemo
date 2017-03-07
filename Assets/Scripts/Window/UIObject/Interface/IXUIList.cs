using UnityEngine;
using System.Collections.Generic;
using Game.Common;
public interface IXUIList : IXUIObject
{
    int Count
    {
        get;
    }
    int GetSelectedIndex();
    void SetSelectedIndex(int nIndex);
    void SetSelectedItemByIndex(List<int> listItemIndex);
    void SetSelectedItemById(int unId);
    void SetSelectedItemById(List<int> listItemId);
    IXUIListItem GetSelectedItem();
    IXUIListItem[] GetSelectedItems();
    IXUIListItem GetItemById(int unId, bool bVisible);
    IXUIListItem GetItemByGUID(long ulId);
    IXUIListItem[] GetAllItems();
    IXUIListItem GetItemByIndex(int nIndex);
    IXUIListItem AddListItem(GameObject obj);
    IXUIListItem AddListItem();
    bool DelItemById(int unId);
    bool DelItemByIndex(int nIndex);
    bool DelItem(IXUIListItem iUIListItem);
    void SetEnable(bool bEnable);
    void SetEnableSelect(bool bEnable);
    void SetEnableSelect(List<int> listIds);
    void Highlight(List<int> listIds);
    void Refresh();
    void OnSelectItem(XUIListItem listItem);
    void SelectItem(XUIListItem listItem, bool bTrigerEvent);
    void OnUnSelectItem(XUIListItem listItem);
    void UnSelectItem(XUIListItem listItem, bool bTrigerEvent);
    void RegisterListOnSelectEventHandler(ListSelectEventHandler handler);
    void RegisterListOnUnSelectEventHandler(ListSelectEventHandler handler);
    void RegisterListOnClickEventHandler(ListOnClickEventHandler handler);
    void Clear();
}
