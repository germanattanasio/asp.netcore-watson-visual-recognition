using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VR.Models
{
    public class VrModel
    {
        public string VersionReleaseDate { get; private set; }
        public string ImagesFile { get; private set; }
        public IEnumerable<string> ClassifierIds { get; private set; }

        public VrModel(string versionReleaseDate, FileStream imagesFile, params string[] classifierIds)
        {
            byte[] buffer = new byte[imagesFile.Length];
            imagesFile.ReadAsync(buffer, 0, (int)imagesFile.Length);
            VersionReleaseDate = versionReleaseDate;
            ImagesFile = Convert.ToBase64String(buffer);
            ClassifierIds = classifierIds.ToList();
        }
    }
}