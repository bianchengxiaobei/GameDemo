using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using Game.Common;
public class ResourceUnit : IDisposable
{
    private string mPath;//资源路径
    private Object mAsset;//资源
    private ResourceType mResourceType;//资源类型
    private List<ResourceUnit> mNextLevelAssets;//用到的所有资源，ab加载时有用到
    private AssetBundle mAssetBundle;//资源的ab文件
    private int mAssetBundleSize;//ab文件的大小
    private int mReferenceCount;//被引用的次数
    internal ResourceUnit(AssetBundle assetBundle, int assetBundleSize, Object asset, string path, ResourceType resourceType/*, int allDependencesAssetSize*/)
    {
        mPath = path;
        mAsset = asset;
        mResourceType = resourceType;
        mNextLevelAssets = new List<ResourceUnit>();
        mAssetBundle = assetBundle;
        mAssetBundleSize = assetBundleSize;
        mReferenceCount = 0;
    }

    public Object Asset
    {
        get
        {
            return mAsset;
        }

        internal set
        {
            mAsset = value;
        }
    }

    public ResourceType resourceType
    {
        get
        {
            return mResourceType;
        }
    }

    public List<ResourceUnit> NextLevelAssets
    {
        get
        {
            return mNextLevelAssets;
        }

        internal set
        {
            foreach (ResourceUnit asset in value)
            {
                mNextLevelAssets.Add(asset);
            }
        }
    }

    public AssetBundle Assetbundle
    {
        get
        {
            return mAssetBundle;
        }
        set
        {
            mAssetBundle = value;
        }
    }

    public int AssetBundleSize
    {
        get
        {
            return mAssetBundleSize;
        }
    }

    public int ReferenceCount
    {
        get
        {
            return mReferenceCount;
        }
    }
    public void dumpNextLevel()
    {
        string info = mPath + " the mReferenceCount : " + mReferenceCount + "\n";
        foreach (ResourceUnit ru in mNextLevelAssets)
        {
            ru.dumpNextLevel();
            info += ru.mPath + "\n";
        }
        Debug.Log(info);
    }

    public void addReferenceCount()
    {
        ++mReferenceCount;
        foreach (ResourceUnit asset in mNextLevelAssets)
        {
            asset.addReferenceCount();
        }
    }

    public void reduceReferenceCount()
    {
        --mReferenceCount;

        foreach (ResourceUnit asset in mNextLevelAssets)
        {
            asset.reduceReferenceCount();
        }
        if (isCanDestory())
        {
            //ResourcesManager.Instance.mLoadedResourceUnit.Remove(ResourceCommon.getFileName(mPath, true));
            Dispose();
        }
    }

    public bool isCanDestory() { return (0 == mReferenceCount); }

    public void Dispose()
    {
        Debug.Log("Destory " + mPath);

        if (null != mAssetBundle)
        {
            mAssetBundle = null;
        }
        mNextLevelAssets.Clear();
        mAsset = null;
    }
    
}
