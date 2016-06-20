using VisualRecognition.ViewModels;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    internal static class ImageFaceIdentityMapper
    {
        internal static ImageFaceIdentityViewModel Map(ImageFaceIdentity fromModel)
        {
            if (fromModel == null)
                return null;

            var toModel = new ImageFaceIdentityViewModel();
            toModel.ConfidenceScore = fromModel.ConfidenceScore;
            toModel.Name = fromModel.Name;
            toModel.TypeHierarchy = fromModel.TypeHierarchy;
            return toModel;
        }
    }
}