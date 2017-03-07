using UnityEngine;
using System.Collections;

public interface IXUITexture : IXUIObject
{
    Color Color
    {
        get;
        set;
    }
    void SetTexture(string strTextureFile);
    void SetTexture(Texture texture);
}
