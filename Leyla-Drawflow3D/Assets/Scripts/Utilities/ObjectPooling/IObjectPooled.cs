using UnityEngine;

public interface IObjectPooled
{
    void OnSpawnObjectPooled();
    bool OnPoolingObject();
}
