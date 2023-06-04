using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleTon<PoolManager>
{
    [SerializeField]
    private PoolDataScriptableObject _poolDataSO = null;
    private Dictionary<PoolType, Queue<PoolableObject>> _poolDic = new Dictionary<PoolType, Queue<PoolableObject>>();
    private Transform _rootTrm = null;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (_rootTrm == null)
            _rootTrm = transform;

        for (int i = 0; i< _poolDataSO.poolDatas.Count; i++)
        {
            _poolDic.Add(_poolDataSO.poolDatas[i].poolType, new Queue<PoolableObject>());

            for(int j = 0; j < _poolDataSO.poolDatas[i].spawnCount; j++)
            {
                PoolableObject poolable = Instantiate(_poolDataSO.poolDatas[i].obj, _rootTrm);
                poolable.poolType = _poolDataSO.poolDatas[i].poolType;
                poolable.StartInit();
                poolable.name =  poolable.name.Replace("(Clone)", "");
                poolable.gameObject.SetActive(false);
                _poolDic[poolable.poolType].Enqueue(poolable);
            }
        }
    }

    public void Push(PoolableObject targetObj)
    {
        targetObj.PushInit();
        targetObj.transform.SetParent(_rootTrm);
        targetObj.gameObject.SetActive(false);
        _poolDic[targetObj.poolType].Enqueue(targetObj);
    }

    public PoolableObject Pop(PoolType type)
    {
        if (_poolDic.ContainsKey(type) == false)
        {
            Debug.LogError("타입에 맞는 키 없음.");
            return null;
        }

        PoolableObject targetObj = _poolDic[type].Dequeue();
        if (_poolDic[type].Count == 0)
        {
            PoolableObject poolable = Instantiate(targetObj, _rootTrm);
            poolable.poolType = targetObj.poolType;
            poolable.StartInit();
            poolable.gameObject.SetActive(false);
            poolable.name = poolable.name.Replace("(Clone)", "");
            _poolDic[poolable.poolType].Enqueue(poolable);
        }

        targetObj.gameObject.SetActive(true);
        targetObj.PopInit();
        return targetObj;
    }

    public T Pop<T>(PoolType type)
    {
        return Pop(type).GetComponent<T>();
    }

    public PoolableObject Pop(PoolType type, Transform parent)
    {
        PoolableObject target = Pop(type);
        target.transform.SetParent(parent);
        return target;
    }
}

[System.Serializable]
public class PoolData
{
    public int spawnCount = 10;
    public PoolType poolType = PoolType.None;
    public PoolableObject obj = null;
}
