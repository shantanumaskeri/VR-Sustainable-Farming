using System.Collections.Generic;
using UnityEngine;


namespace SpatialStories
{
    [ExecuteInEditMode]
    public class S_InteractiveObjectVisuals : MonoBehaviour
    {
        private List<Renderer> m_AllRenderers = new List<Renderer>();
        public List<Renderer> GetAllRenderers()
        {
            UpdateAllRenderers();
            return m_AllRenderers;
        }

        void OnEnable()
        {
            UpdateAllRenderers();
        }

        void OnTransformChildrenChanged()
        {
            UpdateAllRenderers();
        }

        /// <summary>
        /// Updates the list of all renderers attached to this gameObject or his children.
        /// </summary>
        public void UpdateAllRenderers()
        {
            m_AllRenderers.Clear();
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
                m_AllRenderers.Add(renderer);
        }


        internal void AlterAllVisuals(bool enable)
        {
            ParticleSystem[] allParticles = GetComponentsInChildren<ParticleSystem>();
            foreach (Renderer renderer in m_AllRenderers)
                renderer.enabled = enable;

            foreach (ParticleSystem particle in allParticles)
            {
                if (enable)
                    particle.Play();
                else
                    particle.Stop();
            }
        }

        internal void AlterSelectedVisuals(bool enable, List<int> selectedRenderers)
        {
            //TODO: @ancestral (Maybe the renderers are not prepared)
            foreach (int r in selectedRenderers)
            {
                m_AllRenderers[r].enabled = enable;
                ParticleSystem ps = m_AllRenderers[r].gameObject.GetComponent<ParticleSystem>();
                if (ps)
                {
                    if (enable)
                        ps.Play();
                    else
                        ps.Stop();
                }

            }
        }
    }

}