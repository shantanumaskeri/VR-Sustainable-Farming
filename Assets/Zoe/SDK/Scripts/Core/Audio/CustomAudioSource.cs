using UnityEngine;

namespace SpatialStories
{
    public class CustomAudioSource : MonoBehaviour
    {
        public AudioSource AudioSource
        {
            get { return this.gameObject.GetComponent<AudioSource>(); }
        }

        public void Initialize()
        {
            this.gameObject.AddComponent<AudioSource>();
        }
    }
}
