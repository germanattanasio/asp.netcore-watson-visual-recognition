using VisualRecognition.ViewModels;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    internal static class ImageFaceAgeMapper
    {
        internal static ImageFaceAgeViewModel Map(ImageFaceAge fromModel)
        {
            if (fromModel == null)
                return null;

            var toModel = new ImageFaceAgeViewModel();
            toModel.MaximumAge = fromModel.MaximumAge;
            toModel.MinimumAge = fromModel.MinimumAge;
            toModel.Score = fromModel.Score;
            return toModel;
        }
    }
}