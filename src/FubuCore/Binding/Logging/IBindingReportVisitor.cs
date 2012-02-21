namespace FubuCore.Binding.Logging
{
    public interface IBindingReportVisitor
    {
        void Report(BindingReport report);
        void Property(PropertyBindingReport report);
        void Element(ElementBinding binding);

        void EndReport();
        void EndProperty();
        void EndElement();
    }
}