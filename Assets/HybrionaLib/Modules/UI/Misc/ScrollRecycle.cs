using System;
using System.Collections;
using System.Collections.Generic;
using Hybriona;
using SimpleJSON;
using UnityEngine;

public class ScrollRecycle<T> where T : ScrollElement, new()
{
    private GameObject scrollElementPrefab;

    private GenericPool<ScrollElement> pool;
    private List<ScrollElement> activeElements = new List<ScrollElement>();
    public void Init(GameObject scrollElementPrefab)
    {
        
        if (pool == null)
        {
            this.scrollElementPrefab = scrollElementPrefab;
            this.scrollElementPrefab.SetActive(false);
            pool = new GenericPool<ScrollElement>(() =>
            {
                GameObject o = GameObject.Instantiate(this.scrollElementPrefab);
                o.transform.SetParent(scrollElementPrefab.transform.parent);
                o.transform.localScale = Vector3.one;
                T script = new T();
                script.poolContainer = pool;
                script.Init(o);
                return script;

            }, null);

            pool.PreCache(20);
        }

    }

    public void Clear()
    {
        for(int i=0;i<activeElements.Count;i++)
        {
            activeElements[i].Deactivate();
            pool.ReturnToPool(activeElements[i]);
        }
        activeElements.Clear();
    }

    public T GetNext()
    {
        var script = pool.FetchFromPool() as T;
        activeElements.Add(script);
        return script;
    }
}
public class ScrollElement 
{
    public GameObject gameObject { get; protected set; }
    public Transform transform { get; protected set; }
    public GenericPool<ScrollElement> poolContainer { get; set; }

    public void Init(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.transform = this.gameObject.transform;
        Cache();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        OnActivate();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        OnDeactivate();
    }

    public void ReturnToPool()
    {
        Deactivate();
        poolContainer.ReturnToPool(this);
    }

   

    public virtual void Cache()
    {

    }

    public virtual void OnActivate() { }
    public virtual void OnDeactivate() { }
}
