using VisualRecognition.ViewModels;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    internal static class ImageLocationMapper
    {
        internal static ImageLocationViewModel Map(ImageLocation fromModel)
        {
            if (fromModel == null)
                return null;

            var toModel = new ImageLocationViewModel();
            toModel.Height = fromModel.Height;
            toModel.LeftOffset = fromModel.LeftOffset;
            toModel.TopOffset = fromModel.TopOffset;
            toModel.Width = fromModel.Width;
            return toModel;
        }
    }
}