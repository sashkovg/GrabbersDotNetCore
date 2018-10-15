using System.Runtime.Serialization;

namespace GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Meta
{
    /// <summary>
    /// Meta section
    /// </summary>
    [DataContract]
    public class Meta
    {
        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "executionTime")]
        public string ExecutionTime { get; set; }
    }
}
