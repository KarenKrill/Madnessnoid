namespace Madnessnoid
{
    public class BrickBehaviour : DamagableBehaviour
    {
        protected override void OnDied()
        {
            gameObject.SetActive(false);
        }
    }
}
