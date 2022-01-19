using SpatialStories;
using UnityEngine;
using UnityEngine.SceneManagement;
#if PHOTON_INSTALLED
using Photon.Pun;
#endif

namespace SpatialStories
{
    /// <summary>
    /// Custom action that allows to make the controllers vibrate
    /// N.B. This behaviour will be improved on as it is now specific to
    /// Oculus
    /// </summary>
    [AddComponentMenu("Zoe/SA_LoadScene")]
    public class SA_LoadScene : S_AbstractAction
    {
        public string SceneName;
#if PHOTON_INSTALLED
        public bool Networked;
#endif

        protected override void ActionLogic()
        {
#if PHOTON_INSTALLED
            if (Networked)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.AutomaticallySyncScene = true;
                    PhotonNetwork.LoadLevel(SceneName);
                }
            }
            else
            {
#endif
                SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
#if PHOTON_INSTALLED
            }
#endif
        }

        public override void SetupUsingApi(GameObject _interaction)
        {

        }
    }

    public static partial class APIExtensions
    {

    }
}
