using Windows.ApplicationModel.Resources;

namespace AIPaste.common
{
    public class ResourceLoaderWrapper : IResourceLoaderWrapper
    {
        public string GetString(string key)
        {
            return ResourceLoader.GetForViewIndependentUse().GetString(key);
        }
    }

    public interface IResourceLoaderWrapper
    {
        string GetString(string key);
    }
}
