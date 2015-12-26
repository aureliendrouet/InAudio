using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.Internal
{
    public class InCommonDataManager : MonoBehaviour
    {
        private InAudioNode AudioRoot;
        private InAudioEventNode EventRoot;
        private InAudioBankLink BankLinkRoot;
        private InMusicNode MusicRoot;
        private InInteractiveMusic InteractiveMusicRoot;

        public MonoBehaviour[] AllRoots
        {
            get
            {
                return new MonoBehaviour[] { AudioTree, EventTree, BankLinkTree, MusicTree, InteractiveMusicTree};
            }
        }

        public InAudioNode AudioTree
        {
            get { return AudioRoot; }
            set { AudioRoot = value; }
        }

        public InAudioEventNode EventTree
        {
            get { return EventRoot; }
            set { EventRoot = value; }
        }

        public InAudioBankLink BankLinkTree
        {
            get { return BankLinkRoot; }
            set { BankLinkRoot = value; }
        }

        public InMusicNode MusicTree
        {
            get { return MusicRoot; }
            set { MusicRoot = value; }
        }

        public InInteractiveMusic InteractiveMusicTree
        {
            get { return InteractiveMusicRoot; }
            set { InteractiveMusicRoot = value; }
        }

        public void Load(bool forceReload = false)
        {
            if (AudioRoot == null || BankLinkRoot == null || EventRoot == null || MusicRoot == null || InteractiveMusicRoot == null || forceReload)
            {
                Component[] audioData;
                Component[] eventData;
                Component[] bankLinkData;
                Component[] musicData;
                Component[] interactiveMusicData;

                SaveAndLoad.LoadManagerData(out audioData, out eventData, out musicData, out bankLinkData, out interactiveMusicData);
                AudioRoot = CheckData<InAudioNode>(audioData);
                EventRoot = CheckData<InAudioEventNode>(eventData);
                BankLinkTree = CheckData<InAudioBankLink>(bankLinkData);
                MusicTree = CheckData<InMusicNode>(musicData);
                InteractiveMusicTree = CheckData<InInteractiveMusic>(interactiveMusicData);
                
            }
        }

        //Does the components actually exist and does it have a root?
        private T CheckData<T>(Component[] data) where T : Object, InITreeNode<T>
        {
            if (data != null && data.Length > 0 && data[0] as T != null)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] as InITreeNode<T> != null)
                    {
                        if ((data[i] as InITreeNode<T>).IsRoot)
                        {
                            return data[i] as T;
                        }
                    }
                }
            }
            return null;
        }

        //Does the components actually exist and does it have a root?
        private T CheckData<T>(Component[] data, Func<T, bool> predicate) where T : Object, InITreeNode<T>
        {
            if (data != null && data.Length > 0 && data[0] as T != null)
            {
                return TreeWalker.FindFirst(data[0] as T, predicate);
            }
            return null;
        }

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            //Instance = this;
            Load();
        }

        public bool Loaded
        {
            get { return AudioTree != null && MusicTree != null && EventTree != null && BankLinkTree != null; }
        }


#if UNITY_EDITOR
        private bool checkVersion;

        private IEnumerator VersionCheck()
        {
            
            WWW website = new WWW("http://innersystems.net/version3.html");
            yield return website;
            if (website.error == null)
            {
                PlayerPrefs.SetString("InAudioUpdateInfo", website.text);
                PlayerPrefs.SetString("InAudioUpdateCheckTime", DateTime.Now.Date.DayOfYear.ToString(CultureInfo.InvariantCulture));
                
            }
            website.Dispose();
        }

        void Update()
        {
            if (!checkVersion && PlayerPrefs.GetString("InAudioUpdateCheckTime") != DateTime.Now.Date.DayOfYear.ToString(CultureInfo.InvariantCulture))
            {
//                Debug.Log("Run version check");
                checkVersion = true;
                StartCoroutine(VersionCheck());
            }
        }
#endif
    }

}