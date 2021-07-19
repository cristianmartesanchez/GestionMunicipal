using System.IO;
using System.Threading.Tasks;

namespace AppActividades.Services
{
    public interface IGaleriaImagenes
    {
        Task<Stream> GetFotoAsync();
    }
}