namespace FirewallUtils
{
    public abstract class BasePlatform
    {
        public abstract bool HasAuthorization();

        public abstract void GrantAuthorization();

        public abstract void RemoveAuthorization();
    }
}