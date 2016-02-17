using System;
using VisualRecognition.Models;

namespace VisualRecognition.Mappers
{
    public class ClassifierMapper
    {
        public static ClassifierViewModel Map(Classifier fromModel)
        {
            ClassifierViewModel toModel = new ClassifierViewModel();
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
