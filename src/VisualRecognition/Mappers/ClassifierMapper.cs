using System;
using VisualRecognition.ViewModels;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    internal class ClassifierMapper
    {
        internal static ClassifierViewModel Map(Classifier fromModel)
        {
            ClassifierViewModel toModel = new ClassifierViewModel();

            // return the model now if the fromModel is null
            if (fromModel == null)
                return toModel;

            toModel.ClassifierId = fromModel.ClassifierId;
            DateTime createdTime;
            if (DateTime.TryParse(fromModel.CreatedTime, out createdTime))
            {
                toModel.CreatedTime = createdTime;
            }
            toModel.Name = fromModel.Name;
            toModel.Owner = fromModel.Owner;

            return toModel;
        }
    }
}
