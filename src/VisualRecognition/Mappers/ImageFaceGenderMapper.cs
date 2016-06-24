using VisualRecognition.ViewModels;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    internal static class ImageFaceGenderMapper
    {
        internal static ImageFaceGenderViewModel Map(ImageFaceGender fromModel)
        {
            if (fromModel == null)
                return null;

            var toModel = new ImageFaceGenderViewModel();
            toModel.Gender = fromModel.Gender;
            toModel.Score = fromModel.Score;
            return toModel;
        }
    }
}