namespace CrossCutting.Adapters
{
    public interface ITypeAdapterFactory
    {
        ITypeAdapter Create();
    }
}
