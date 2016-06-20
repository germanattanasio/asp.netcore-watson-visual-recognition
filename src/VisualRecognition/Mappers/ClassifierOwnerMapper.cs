using System;
using System.Collections.Generic;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.Mappers
{
    public class ClassifierOwnerMapper
    {
        internal static ICollection<ClassifierOwner> Map(ICollection<string> owners)
        {
            if (owners == null)
            {
                return null;
            }

            ICollection<ClassifierOwner> result = new List<ClassifierOwner>();
            foreach (var ownerString in owners)
            {
                ClassifierOwner owner;
                if (Enum.TryParse(ownerString, true, out owner))
                {
                    result.Add(owner);
                }
            }
            return result;
        }
    }
}
