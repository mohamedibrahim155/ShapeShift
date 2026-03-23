using UnityEngine;
using Zenject;

namespace Scripts.SkyService
{
    public class SkyboxView : MonoBehaviour
    {
        private SkyboxConfig config;

        private ISkyService skyService;

        [Inject]
        public void Contruct(ISkyService skyService)
        {
            this.skyService = skyService;
        }

        private void Start()
        {
            skyService.Initialize();
        }

    }
}
