namespace SimpleFrame
{
    public class SimpleArchitecture : AbstractArchitecture<SimpleArchitecture>
    {
        protected override void OnInit()
        {
            Utility();
        }

        private void Utility()
        {
            this.RegisterUtility<IAddressablesUtility>(new AddressablesUtility());
        }
    }
}