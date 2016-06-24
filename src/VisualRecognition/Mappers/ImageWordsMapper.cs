using VisualRecognition.ViewModels;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    internal static class ImageWordsMapper
    {
        internal static ImageWordViewModel Map(ImageWord fromModel)
        {
            if (fromModel == null)
                return null;

            var toModel = new ImageWordViewModel();
            toModel.ConfidenceScore = fromModel.ConfidenceScore;
            toModel.LineNumber = fromModel.LineNumber;
            toModel.Location = ImageLocationMapper.Map(fromModel.Location);
            toModel.Word = fromModel.Word;
            return toModel;
        }
    }
}
