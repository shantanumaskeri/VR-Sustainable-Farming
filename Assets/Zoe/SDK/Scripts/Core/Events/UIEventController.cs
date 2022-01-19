using UnityEngine;
using System.Collections.Generic;

namespace SpatialStories
{

	public delegate void UIEventHandler(string _nameEvent, params object[] _list);

	/******************************************
	 * 
	 * UIEventController
	 * 
	 * Class used to dispatch events related to the UI
	 * 
	 * @author Esteban Gallardo
	 */
	public class UIEventController : MonoBehaviour
	{
		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------	
		// ScreenController
		public const string EVENT_SCREENMANAGER_OPEN_GENERIC_SCREEN		= "EVENT_SCREENMANAGER_OPEN_GENERIC_SCREEN";
        public const string EVENT_SCREENMANAGER_OPEN_LAYER_GENERIC_SCREEN = "EVENT_SCREENMANAGER_OPEN_LAYER_GENERIC_SCREEN";
        public const string EVENT_SCREENMANAGER_VR_OPEN_GENERIC_SCREEN  = "EVENT_SCREENMANAGER_VR_OPEN_GENERIC_SCREEN";        
        public const string EVENT_SCREENMANAGER_OPEN_INFORMATION_SCREEN = "EVENT_SCREENMANAGER_OPEN_INFORMATION_SCREEN";
        public const string EVENT_SCREENMANAGER_OPEN_LAYER_INFORMATION_SCREEN = "EVENT_SCREENMANAGER_OPEN_LAYER_INFORMATION_SCREEN";
        public const string EVENT_SCREENMANAGER_VR_OPEN_INFORMATION_SCREEN = "EVENT_SCREENMANAGER_VR_OPEN_INFORMATION_SCREEN";
        public const string EVENT_SCREENMANAGER_DESTROY_SCREEN			= "EVENT_SCREENMANAGER_DESTROY_SCREEN";
		public const string EVENT_SCREENMANAGER_DESTROY_SCREEN_BY_NAME	= "EVENT_SCREENMANAGER_DESTROY_SCREEN_BY_NAME";
		public const string EVENT_SCREENMANAGER_DESTROY_ALL_SCREEN		= "EVENT_SCREENMANAGER_DESTROY_ALL_SCREEN";
        public const string EVENT_SCREENMANAGER_DESTROY_SUBEVENT_SCREEN = "EVENT_SCREENMANAGER_DESTROY_SUBEVENT_SCREEN";        
        public const string EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON		= "EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON";
		public const string EVENT_GENERIC_MESSAGE_INFO_OK_BUTTON		= "EVENT_GENERIC_MESSAGE_INFO_OK_BUTTON";
		public const string EVENT_SCREENMANAGER_OVERLAY_DESTROY_LAST	= "EVENT_SCREENMANAGER_OVERLAY_DESTROY_LAST";
		public const string EVENT_SCREENMANAGER_POOL_DESTROY_LAST		= "EVENT_SCREENMANAGER_POOL_DESTROY_LAST";
        public const string EVENT_SCREENMANAGER_RELOAD_SCREEN_DATA      = "EVENT_SCREENMANAGER_RELOAD_SCREEN_DATA";
        public const string EVENT_SCREENMANAGER_CLEAR_SELECTOR_DATA     = "EVENT_SCREENMANAGER_CLEAR_SELECTOR_DATA";        
        public const string EVENT_SCREENMANAGER_LOAD_NEW_SCENE          = "EVENT_SCREENMANAGER_LOAD_NEW_SCENE";
        public const string EVENT_SCREENMANAGER_ENABLE_LAYERS_BELOW_ME  = "EVENT_SCREENMANAGER_ENABLE_LAYERS_BELOW_ME";
        public const string EVENT_SCREENMANAGER_ENABLE_ONE_LAYER_BELOW_ME = "EVENT_SCREENMANAGER_ENABLE_ONE_LAYER_BELOW_ME";
        public const string EVENT_SCREENMANAGER_MOVE_SCREEN_TO_LAYER    = "EVENT_SCREENMANAGER_MOVE_SCREEN_TO_LAYER";
        public const string EVENT_SCREENMANAGER_DESTROY_SCREENS_LAYERS_ABOVE    = "EVENT_SCREENMANAGER_DESTROY_SCREENS_LAYERS_ABOVE";
        public const string EVENT_SCREENMANAGER_DESTROY_SCREENS_LAYERS_BELOW = "EVENT_SCREENMANAGER_DESTROY_SCREENS_LAYERS_BELOW";

        // ScreenMainCommandCenterView 
        public const string EVENT_SCREENMAINCOMMANDCENTER_REGISTER_LOG				= "EVENT_SCREENMAINCOMMANDCENTER_REGISTER_LOG";
		public const string EVENT_SCREENMAINCOMMANDCENTER_REQUEST_LIST_USERS		= "EVENT_SCREENMAINCOMMANDCENTER_REQUEST_LIST_USERS";
		public const string EVENT_SCREENMAINCOMMANDCENTER_LIST_USERS				= "EVENT_SCREENMAINCOMMANDCENTER_LIST_USERS";
		public const string EVENT_SCREENMAINCOMMANDCENTER_ACTIVATION_VISUAL_INTERFACE = "EVENT_SCREENMAINCOMMANDCENTER_ACTIVATION_VISUAL_INTERFACE";
		public const string EVENT_SCREENMAINCOMMANDCENTER_TEXTURE_REMOTE_STREAMING_DATA = "EVENT_SCREENMAINCOMMANDCENTER_TEXTURE_REMOTE_STREAMING_DATA";

		// MISCELANEA
		public const string EVENT_BASICEVENT_DELAYED_CALL			= "EVENT_BASICEVENT_DELAYED_CALL";
		public const string EVENT_SCREENMANAGER_REPORT_DESTROYED	= "EVENT_SCREENMANAGER_REPORT_DESTROYED";
        public const string EVENT_SCREENMANAGER_CREATE_FADE_SCREEN  = "EVENT_SCREENMANAGER_CREATE_FADE_SCREEN";
        public const string EVENT_SCREENMANAGER_ANIMATION_SCREEN    = "EVENT_SCREENMANAGER_ANIMATION_SCREEN";

        // ----------------------------------------------
        // EVENTS
        // ----------------------------------------------	
        public const string SCREEN_LOADING = "SCREEN_LOADING";
		public const string SCREEN_INFORMATION = "SCREEN_INFORMATION";

		public event UIEventHandler UIEvent;

		// ----------------------------------------------
		// SINGLETON
		// ----------------------------------------------	
		private static UIEventController instance;

		public static UIEventController Instance
		{
			get
			{
				if (!instance)
				{
					instance = GameObject.FindObjectOfType(typeof(UIEventController)) as UIEventController;
					if (!instance)
					{
						GameObject container = new GameObject();
#if DONT_DESTROY_ON_LOAD
						DontDestroyOnLoad(container);
#endif
						container.name = "UIEventController";
						instance = container.AddComponent(typeof(UIEventController)) as UIEventController;
					}
				}
				return instance;
			}
		}

        // ----------------------------------------------
        // PUBLIC MEMBERS
        // ----------------------------------------------    
        public string URLAssetBundle = "";
        public int VersionAssetBundle = -1;
        public float ActivateAutoDestruction = -1;

        // ----------------------------------------------
        // PRIVATE MEMBERS
        // ----------------------------------------------
        private List<AppEventData> listEvents = new List<AppEventData>();
        private float m_blockEvents = 0;
        private List<string> m_blockedEvents = new List<string>();

        // -------------------------------------------
        /* 
		 * Constructor
		 */
        private UIEventController()
		{
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		void OnDestroy()
		{
			Destroy();
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			if (instance != null)
			{
				Destroy(instance.gameObject);
				instance = null;
			}
		}

        // -------------------------------------------
        /* 
		 * BlockUIEvents
		 */
        public void BlockUIEvents(float _time, params string[] _list)
        {
            m_blockEvents = _time;
            if (m_blockEvents > 0)
            {
                for (int i = 0; i < _list.Length; i++)
                {
                    m_blockedEvents.Add(_list[i]);
                }
            }
            else
            {
                m_blockedEvents.Clear();
            }
        }

        // -------------------------------------------
        /* 
		 * Will check if we need to block an event
		 */
        private bool CheckBlockEvent(string _nameEvent)
        {
            if (m_blockEvents > 0)
            {
                for (int i = 0; i < m_blockedEvents.Count; i++)
                {
                    if (m_blockedEvents[i].Equals(_nameEvent))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // -------------------------------------------
        /* 
		 * Will dispatch a UI event
		 */
        public void DispatchUIEvent(string _nameEvent, params object[] _list)
		{
            // Debug.Log("[UI]_nameEvent=" + _nameEvent);
            if (!CheckBlockEvent(_nameEvent))
            {
                if (UIEvent != null) UIEvent(_nameEvent, _list);
            }            
		}

		// -------------------------------------------
		/* 
		 * Will dispatch a delayed UI event
		 */
		public void DelayUIEvent(string _nameEvent, float _time, params object[] _list)
		{
            if (!CheckBlockEvent(_nameEvent))
            {
                listEvents.Add(new AppEventData(_nameEvent, -1, true, -1, _time, _list));
            }
		}

        // -------------------------------------------
        /* 
		 * Will dispatch a delayed UI event
		 */
        public void ClearUIEvents(string _nameEvent = "")
        {
            if (_nameEvent.Length == 0)
            {
                for (int i = 0; i < listEvents.Count; i++)
                {
                    listEvents[i].Time = -1000;
                }
            }
            else
            {
                for (int i = 0; i < listEvents.Count; i++)
                {
                    AppEventData eventData = listEvents[i];
                    if (eventData.NameEvent == _nameEvent)
                    {
                        eventData.Time = -1000;
                    }
                }
            }            
        }

        // -------------------------------------------
        /* 
		 * Will process the queue of delayed events 
		 */
        void Update()
		{
            // BLOCKED EVENTS
            if (m_blockEvents > 0)
            {
                m_blockEvents -= Time.deltaTime;
                if (m_blockEvents <= 0)
                {
                    m_blockedEvents.Clear();
                }
            }

            // DELAYED EVENTS
            for (int i = 0; i < listEvents.Count; i++)
			{
				AppEventData eventData = listEvents[i];				
                if (eventData.Time == -1000)
                {
                    eventData.Destroy();
                    listEvents.RemoveAt(i);
                    break;
                }
                else
                {
                    eventData.Time -= Time.deltaTime;
                    if (eventData.Time <= 0)
                    {
                        UIEvent(eventData.NameEvent, eventData.ListParameters);
                        eventData.Destroy();
                        listEvents.RemoveAt(i);
                        break;
                    }
                }
            }
		}
	}
}
