namespace FunctionDurableAppTest.Models
{
    public class OrchestrationResponse
    {
        public bool CloseParent { get; set; }
        public AccountDetails AccountDetails { get; set; }

        public dynamic OtherData { get; set; }
    }
}
