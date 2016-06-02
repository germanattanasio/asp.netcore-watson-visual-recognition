using System.Collections.Generic;
using System.Linq;
using VisualRecognition.ViewModels;
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
    }
}
