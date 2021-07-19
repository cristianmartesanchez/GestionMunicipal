using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppActividades
{
    public interface IGMMultiImagePicker
    {
        Task<List<string>> PickMultiImage();
        Task<List<string>> PickMultiImage(bool needsHighQuality);
        void ClearFileDirectory();
    }
}
