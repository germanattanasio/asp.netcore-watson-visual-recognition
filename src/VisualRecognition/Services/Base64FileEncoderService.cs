using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VisualRecognition.Services
{
    public class Base64FileEncoderService : IFileEncoderService
    {
        private readonly HashSet<string> _fileTypes;

        public Base64FileEncoderService()
        {
            // set file types to encode when extracting files from zip files
            _fileTypes = new HashSet<string>()
            {
                ".jpg",
                ".jpeg",
                ".png"
            };
        }

        public async Task<byte[]> DecodeFileAsync(string encodedFileString)
        {
            return await Task.Run(() =>
            {
                return Convert.FromBase64String(encodedFileString);
            });
        }

        public async Task<string> EncodeFileAsync(byte[] fileContentByteArray)
        {
            return await Task.Run(() =>
            {
                return Convert.ToBase64String(fileContentByteArray);
            });
        }

        public async Task<string> EncodeFileAsync(string filePath)
        {
            return await EncodeFileAsync(File.ReadAllBytes(filePath));
        }

        public async Task<Dictionary<string, string>> EncodeZipFileAsync(string filePath)
        {
            // create a dictionary to store the original filenames and the base64 encoded image files (for displaying images)
            var base64Images = new Dictionary<string, string>();

            // get the filename of the image file, without the path
            var filename = Path.GetFileName(filePath);

            // if the file is not a zip file, add the filename and it's base64 encoded form to base64Images and return
            if (Path.GetExtension(filePath).ToLowerInvariant() != ".zip")
            {
                base64Images.Add(filename, Convert.ToBase64String(File.ReadAllBytes(filePath)));
                return base64Images;
            }

            await Task.Run(() =>
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var folder = Path.GetDirectoryName(filePath);
                var newTempFolder = Path.Combine(folder, fileName);
                System.IO.Compression.ZipFile.ExtractToDirectory(filePath, newTempFolder);

                // add each of the extracted images to the base64Images dictionary
                foreach (var file in Directory.GetFiles(newTempFolder)
                    .Where(m => _fileTypes.Contains(Path.GetExtension(m).ToLowerInvariant())).ToList())
                {
                    base64Images.Add(Path.GetFileName(file), Convert.ToBase64String(File.ReadAllBytes(file)));
                }

                // cleanup the extracted files as they are no longer needed
                foreach (var file in Directory.GetFiles(newTempFolder))
                {
                    File.Delete(file);
                }
                Directory.Delete(newTempFolder);
            });

            return base64Images;
        }
    }
}
