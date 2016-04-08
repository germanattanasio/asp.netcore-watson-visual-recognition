using System.Collections.Generic;
using System.Linq;
using VisualRecognition.ViewModels;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    public class ImagesMapper
    {
        public static void Map(ClassifyResponse fromModel, VisualRecognitionViewModel toModel, Dictionary<string,string> base64Images,
            int? maxScores)
        {
            if (toModel == null)
            {
                toModel = new VisualRecognitionViewModel();
            }

            toModel.ImageResults = new List<ImageViewModel>();
            foreach (var image in fromModel.Images)
            {
                toModel.ImageResults.Add(new ImageViewModel()
                {
                    ImageName = image.ImageName,
                    Scores = !maxScores.HasValue ? image.Scores.Select(ScoresMapper.Map).ToList() :
                        image.Scores.Select(ScoresMapper.Map).Take(maxScores.Value).ToList(),
                    Base64Image = base64Images.Where(m => m.Key == image.ImageName).Select(m => m.Value).FirstOrDefault()
                });
            }
        }
    }
}
