using System.Collections.Generic;
using System.Threading.Tasks;

namespace VisualRecognition.Services
{
    public interface IFileEncoderService
    {
        Task<byte[]> DecodeFileAsync(string encodedFileString);
        Task<Dictionary<string, string>> EncodeZipFileAsync(string filePath);
        Task<string> EncodeFileAsync(byte[] fileContentByteArray);
        Task<string> EncodeFileAsync(string filePath);
    }
}
