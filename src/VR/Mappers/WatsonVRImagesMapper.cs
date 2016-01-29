using System.Collections.Generic;
using System.Linq;
using VR.Models;

namespace VR.Mappers
{
    public class WatsonVRImagesMapper
    {
        public static void Map(WatsonVRResponse fromModel, WatsonVRViewModel toModel, Dictionary<string,string> base64Images,
            int? maxScores)
        {
            toModel.ImageResults = new List<WatsonVRImageViewModel>();
            foreach (var image in fromModel.Images)
            {
                toModel.ImageResults.Add(new WatsonVRImageViewModel()
                {
                    ImageName = image.ImageName,
                    Scores = !maxScores.HasValue ? image.Scores.Select(WatsonVRScoreMapper.Map).ToList() :
                        image.Scores.Select(WatsonVRScoreMapper.Map).Take(maxScores.Value).ToList(),
                    Base64Image = base64Images.Where(m => m.Key == image.ImageName).Select(m => m.Value).FirstOrDefault()
                });
            }
        }
    }
}
