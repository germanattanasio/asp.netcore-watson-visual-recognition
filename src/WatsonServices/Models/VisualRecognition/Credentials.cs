namespace WatsonServices.Models.VisualRecognition
{
    public class Credentials : IValidated
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }

        public bool IsValid
        {
            get
            {
                return !(string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password)
                    || string.IsNullOrEmpty(Url));
            }
        }
    }
}
