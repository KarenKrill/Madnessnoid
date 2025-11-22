using System.Threading;
using System.Threading.Tasks;

namespace KarenKrill.ContentLoading.Abstractions
{
    public interface ISceneLoader
    {
        Task LoadAsync(string sceneName, SceneLoadParameters? loadParameters = null, CancellationToken cancellationToken = default);
        Task LoadAsync(int sceneBuildIndex, SceneLoadParameters? loadParameters = null, CancellationToken cancellationToken = default);
    }
}
