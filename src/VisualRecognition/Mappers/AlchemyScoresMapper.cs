using VisualRecognition.ViewModels;
using WatsonServices.Models.AlchemyVision;

namespace VisualRecognition.Mappers
{
    internal class AlchemyScoresMapper
    {
        internal static ClassificationScoreViewModel Map(ImageKeyword fromModel)
        {
            ClassificationScoreViewModel toModel = new ClassificationScoreViewModel();
            toModel.ClassifierId = fromModel.Text;
            toModel.ClassifierName = fromModel.Text;
            toModel.Score = fromModel.ConfidenceScore.HasValue ? fromModel.ConfidenceScore.Value.ToString() : @"Not a match";
            return toModel;
        }
    }
}
