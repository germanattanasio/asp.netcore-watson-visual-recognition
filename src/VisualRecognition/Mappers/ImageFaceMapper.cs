using VisualRecognition.ViewModels;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    internal static class ImageFaceMapper
    {
        internal static ImageFaceViewModel Map(ImageFace fromModel)
        {
            if (fromModel == null)
                return null;

            var toModel = new ImageFaceViewModel();
            toModel.Age = ImageFaceAgeMapper.Map(fromModel.Age);
            toModel.Gender = ImageFaceGenderMapper.Map(fromModel.Gender);
            toModel.Identity = ImageFaceIdentityMapper.Map(fromModel.Identity);
            toModel.Location = ImageLocationMapper.Map(fromModel.Location);
            return toModel;
        }
    }
}
