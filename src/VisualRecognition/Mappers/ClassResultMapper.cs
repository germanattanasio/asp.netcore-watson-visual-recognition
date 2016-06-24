using VisualRecognition.ViewModels;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    internal static class ClassResultMapper
    {
        internal static ClassResultViewModel Map(ClassResult fromModel)
        {
            var toModel = new ClassResultViewModel();
            toModel.ClassId = fromModel?.ClassId;
            toModel.Score = fromModel?.Score ?? 0;
            toModel.TypeHierarchy = fromModel?.TypeHierarchy;
            return toModel;
        }
    }
}
