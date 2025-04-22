using Spectral.Autonation.MonoBehaviours;

namespace Spectral.Autonation.UI
{
    public class BuildingButton : AutonationButton
    {
        protected override void Start()
        {
            base.Start();
            OnClickAction += EnterBuildingMode;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnClickAction -= EnterBuildingMode;
        }

        private void EnterBuildingMode()
        {
            Player.Instance.SetState(Player.State.Building);
        }
    }
}