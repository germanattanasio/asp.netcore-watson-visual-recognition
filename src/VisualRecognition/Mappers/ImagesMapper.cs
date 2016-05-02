using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisualRecognition.ViewModels;
using WatsonServices.Models.AlchemyVision;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    internal class ImagesMapper
    {
        internal static VisualRecognitionViewModel Map(ClassifyResponse fromModel)
        {
            VisualRecognitionViewModel toModel = new VisualRecognitionViewModel();

            toModel.Images = new List<ImageViewModel>();
            foreach (var image in fromModel.Images)
            {
                toModel.Images.Add(new ImageViewModel()
                {
                    ImageName = image.ImageName,
                    Scores = image?.Scores?.Select(ScoresMapper.Map).ToArray()
                });
            }

            if (fromModel.Images.Count() == 0)
            {
                toModel.Images.Add(new ImageViewModel());
            }

            return toModel;
        }

        internal static VisualRecognitionViewModel Map(ImageKeywordResponse fromModel)
        {
            VisualRecognitionViewModel toModel = new VisualRecognitionViewModel();

            toModel.Images = new List<ImageViewModel>();
            toModel.Images.Add(new ImageViewModel()
            {
                ImageName = !string.IsNullOrEmpty(fromModel?.Url) ? Path.GetFileName(fromModel.Url) : "",
                Scores = fromModel?.ImageKeywords?.Select(AlchemyScoresMapper.Map).ToArray()
            });

            return toModel;
        }
    }
}
