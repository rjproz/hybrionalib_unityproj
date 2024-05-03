using System.Collections.Generic;
using UnityEngine;
namespace Hybriona
{
    public class ScrollRecycle<T> where T : ScrollElement, new()
    {
        //private GameObject scrollElementPrefab;

        private GenericPool<ScrollElement> [] pool;
        private List<ScrollElement> activeElements = new List<ScrollElement>();
        public void Init(GameObject scrollElementPrefab, uint precacheCount = 10)
        {
            Init(new GameObject[1] { scrollElementPrefab }, precacheCount);
        }

        public void Init(GameObject[] scrollElementsPrefab, uint precacheCount = 10)
        {

            if (pool == null)
            {
                
                
                pool = new GenericPool<ScrollElement>[scrollElementsPrefab.Length];

                for (int i = 0; i < pool.Length; i++)
                {
                    int poolIndex = i;
                    scrollElementsPrefab[poolIndex].SetActive(false);
                    
                    pool[poolIndex] = new GenericPool<ScrollElement>(() =>
                    {
                        GameObject o = GameObject.Instantiate(scrollElementsPrefab[poolIndex]);
                        o.transform.SetParent(scrollElementsPrefab[poolIndex].transform.parent);
                        o.transform.localScale = Vector3.one;
                        T script = new T();
                        script.poolContainer = pool[poolIndex];
                        script.Init(poolIndex, o);
                        return script;

                    }, null);

                    pool[i].PreCache(precacheCount);
                }


            }

        }

        public void Clear()
        {
            for (int i = 0; i < activeElements.Count; i++)
            {
                activeElements[i].Deactivate();
                pool[activeElements[i].poolIndex].ReturnToPool(activeElements[i]);
            }
            activeElements.Clear();
        }

        public T FillNext(int poolIndex, System.Action<T> fillAction)
        {
            var script = pool[poolIndex].FetchFromPool() as T;
            activeElements.Add(script);
            fillAction(script);
            script.Activate();
            return script;
        }
    }
    public class ScrollElement
    {
        public int poolIndex { get; protected set; }
        public GameObject gameObject { get; protected set; }
        public Transform transform { get; protected set; }
        public GenericPool<ScrollElement> poolContainer { get; set; }

        public void Init(int index, GameObject gameObject)
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
}