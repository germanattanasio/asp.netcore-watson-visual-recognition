using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisualRecognition.Services
{
    public interface IFileEncoderService
    {
        Task<Dictionary<string, string>> EncodeZipFileAsync(string filePath);
        Task<string> EncodeFileAsync(byte[] fileContentByteArray);
        Task<string> EncodeFileAsync(string filePath);
    }
}
