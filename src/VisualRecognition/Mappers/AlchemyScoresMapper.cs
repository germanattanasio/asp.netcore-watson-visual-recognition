using VisualRecognition.ViewModels;
using WatsonServices.Models.AlchemyVision;

namespace VisualRecognition.Mappers
{
    public class AlchemyScoresMapper
    {
        public static ClassificationScoreViewModel Map(ImageKeyword fromModel)
        {
            ClassificationScoreViewModel toModel = new ClassificationScoreViewModel();
            toModel.ClassifierId = fromModel.Text;
            toModel.ClassifierName = fromModel.Text;
            toModel.Score = fromModel.ConfidenceScore.HasValue ? fromModel.ConfidenceScore.Value.ToString("P2") : @"Not a match";
            return toModel;
        }
    }
}
