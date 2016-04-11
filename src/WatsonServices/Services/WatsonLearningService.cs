namespace WatsonServices.Services
{
    public class WatsonLearningService : IWatsonLearningService
    {
        protected bool learningOptOut;

        // Specifies whether or not to share data with Watson for learning purposes
        public bool ShareData
        {
            get
            {
                return !learningOptOut;
            }
            set
            {
                // if ShareData is set true, don't opt out of sharing
                learningOptOut = !value;
            }
        }
    }
}
