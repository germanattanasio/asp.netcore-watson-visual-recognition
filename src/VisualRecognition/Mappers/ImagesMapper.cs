using System.Collections.Generic;
using System.Linq;
using VisualRecognition.ViewModels;
using WatsonServices.Models.AlchemyVision;
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

        internal static void Map(ImageKeywordResponse fromModel, VisualRecognitionViewModel toModel, string imageName, string tempName, Dictionary<string, string> base64Images,
            int? maxScores)
        {
            if (toModel == null)
            {
                toModel = new VisualRecognitionViewModel();
            }

            toModel.ImageResults = new List<ImageViewModel>();
            toModel.ImageResults.Add(new ImageViewModel()
            {
                ImageName = imageName,
                Scores = !maxScores.HasValue ? fromModel.ImageKeywords.Select(AlchemyScoresMapper.Map).ToList() :
                    fromModel.ImageKeywords.Select(AlchemyScoresMapper.Map).Take(maxScores.Value).ToList(),
                Base64Image = base64Images.Where(m=>m.Key == tempName).Select(m=>m.Value).FirstOrDefault()
            });
        }
    }
}
