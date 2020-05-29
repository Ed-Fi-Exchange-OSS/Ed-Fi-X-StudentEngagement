namespace MSDF.StudentEngagement.Web.Controllers
{
    public class CipheredActivityModel
    {
        /// <summary>
        /// The Key to use in the AES decryption.
        /// </summary>
        public string k { get; set; }

        // The message containing the JSON payload with the student learning activity event.
        public string m { get; set; }
    }
}
